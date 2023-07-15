using Assets.Scripts.Controller;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/*
 * 몬스터의 컨트롤러 스크립트
 */

public class MonsterController : CreatureController
{



    [Header("Prefab Info")]

    [SerializeField]
    [Tooltip("몬스터 모델 프리팹")]
    GameObject ModelPrefab;

    [SerializeField]
    [Tooltip("보스의 경우 줌인 될때의 이펙트")]
    public ParticleSystem StartEffect;

    [SerializeField]
    [Tooltip("몬스터 피격 시 파티클")]
    public ParticleSystem HitEffectController;

    [SerializeField]
    [Tooltip("몬스터 사망 시 파티클")]
    public ParticleSystem DeadEffectController;

    [HideInInspector]
    public Gimmick RelatedGimmic;








    [Header("Status")]

    [SerializeField]
    [Tooltip("스테이지 보스 몬스터인지 여부, 보스 몬스터의 경우 사망시 스테이지 클리어 처리")]
    bool isStageBoss = false;

    [SerializeField]
    [Tooltip("레이드 보스 몬스터인지 여부, 보스 몬스터의 경우 사망시 스테이지 클리어 처리")]
    bool isRaidBoss = false;

    [SerializeField]
    [Tooltip("해당 몬스터 처치시 활성화 시킬 오브젝트")]
    GameObject RelatedObject;

    [SerializeField]
    [Tooltip("레벨 설정 값 (레벨을 통해 Stat 데이터에서 능력치 획득)")]
    public int Level = 1;

    [SerializeField]
    [Tooltip("몬스터 시야 범위 설정 값")]
    public float VisualRange = 10.0f;

    [SerializeField]
    [Tooltip("몬스터와의 거리가 벌어져 추적을 시작하는 거리")]
    public float ChaseRange = 2.0f;

    [SerializeField]
    [Tooltip("스폰 지점으로 떨어져서 갈 수 있는 최대 거리")]
    public float ReturnBaseRange = 5.0f;

    [SerializeField]
    [Tooltip("공격받았을 때 넉백이 되는 지 여부")]
    public bool HasSuerArmor = false;

    [SerializeField]
    [Tooltip("hp 바 위치")]
    public float HpBarHeight = 1.0f;

    [SerializeField]
    [Tooltip("이동속도 보정 값")]
    public float SpeedFactor = -4;








    [Header("Skill Info")]

    [SerializeField]
    [Tooltip("몬스터 스킬 셋 설정")]
    public MonsterSkillSet[] MonsterSkillSets;

    [SerializeField]
    [Tooltip("이 횟수 만큼 패턴 진행 후 궁극기 사용 (궁극기가 있는 몬스터만)")]
    public int UltimateCount = 5;


    [System.Serializable]
    public class MonsterSkillSet // 몬스터의 스킬과 관련된 설정 값
    {
        [SerializeField]
        [Tooltip("공격 범위")]
        public float SkillRange = 1.0f;

        [SerializeField]
        [Tooltip("공격 딜레이 (공격시전 시작부터 움직이지 못하는 시간)")]
        public float SkillDelay = 1.2f;

        [SerializeField]
        [Tooltip("스킬 이펙트")]
        public ParticleSystem SkillEffect;

    }

    // 코루틴 반환 값 캐싱
    private List<WaitForSeconds> _skillDelay = new List<WaitForSeconds>();
    private WaitForSeconds CoolTime;



    // -------------------- Animation 관련 변수 -------------------------
    [HideInInspector]
    public Animator Animator;

    // ------------------------------------------------------------------





    // ------------------------- UI 관련 변수 ---------------------------
    GameObject _damageText;

    // ------------------------------------------------------------------




    // ---------------------- Target 관련 변수 --------------------------
    private MyPlayerController _targetPlayer;
    private MonsterAI _monsterAI;
    public Vector3 BasePosition = new Vector3(0, 0, 0);

    LayerMask _hittalbeMask;

    NavMeshAgent monsterNma;
    NavMeshPath pathToTarget;
    Vector3 pathVector;

    // ------------------------------------------------------------------





    // ----------------------------- Init--------------------------------

    // 전부 상태로 관리
    bool _updated = false; // 1. STATE가 바뀌거나 2. 위치가 바뀌거나
    public bool Inputable { get; set; } = true; // 다음 행동가능 여부 (플레이어 컨트롤러와 변수명 통일)

    protected override void Init()
    {
        base.Init();

        AnimInit();
        NavMeshInit();
        InitStat();

        Initnitialized = true; // 몬스터 초기화 완료

        // TEMP : 오브젝트 풀링 테스트
        /*List<GameObject> list = new List<GameObject>();
        for(int i = 0; i < 10; i++)
        {
            list.Add(Managers.Resource.Instantiate("PoolingTest"));
        }

        foreach(GameObject obj in list)
        {
            Managers.Resource.Destroy(obj);
        }

        Managers.Resource.Instantiate("PoolingTest");
        Managers.Resource.Instantiate("PoolingTest");*/
    }

    // NavMesh 관련 초기화
    void NavMeshInit()
    {
        if(!isRaidBoss) // 레이드 보스의 AI는 서버가 관리하므로 필요 없음
        {
            // NavMeshAgent 관련 변수 초기화
            monsterNma = transform.GetComponent<NavMeshAgent>();
            pathToTarget = new NavMeshPath();

            // 기본적으로 자신의 위치를 Base (어그로 풀릴 시 돌아오는 위치) 로 지정
            BasePosition = transform.position;

            // AI 컴포넌트 획득
            _monsterAI = transform.GetComponentInChildren<MonsterAI>();
        }
    }

    // 애니메이션 관련 초기화
    void AnimInit()
    {
        // 애니메이터 획득
        Animator = transform.GetComponent<Animator>();

        // 공격할 수 있는 대상 마스크
        _hittalbeMask = LayerMask.GetMask("Player");

        // 스킬 딜레이 값 이용하여 Waitforseconds 객체 미리 생성(캐싱)
        foreach(MonsterSkillSet ms in MonsterSkillSets)
        {
            _skillDelay.Add(new WaitForSeconds(ms.SkillDelay));
        }
    }

    // 스탯 데이터로부터 레벨에 따른 능력치 획득
    void InitStat()
    {
        StatInfo stat;
        Managers.Data.StatDict.TryGetValue(Level, out stat);

        Debug.Log(stat);

        if (stat != null) // 능력치 세팅
        {
            STAT = stat;
            SetHpBar(HpBarHeight); // 능력치 부여후 HP 바 세팅
        }

    }

    // -------------------------------------------------------------------------------------










    // ------------------------------------- State -----------------------------------------
    // 애니메이션 관련 업데이트
    public override CharacterState STATE
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            _state = value;

            // 애니메이션 관련 업데이트
            switch (_state)
            {
                case CharacterState.Idle:
                    OnIdle();
                    break;
                case CharacterState.Walk:
                    OnWalk();
                    break; 
                case CharacterState.NomalAttack:
                    OnAttack(1);
                    break;
                case CharacterState.BattleSkill:
                    OnAttack(2);
                    break;
                case CharacterState.Ultimate:
                    OnAttack(3);
                    break;
                case CharacterState.Dead:
                    OnDead();
                    break;
                case CharacterState.AggroLost:
                    OnWalk();
                    break;
                case CharacterState.KnockBack:
                    OnKnockback();
                    break;
            }

        }
    }

    

    // 현재 상태에 따른 업데이트 EX) 이동
    void UpdateState()
    {
        switch (STATE)
        {
            case CharacterState.Walk:
                MovePosition();
                break;
        }

    }

    // 이동 처리 함수
    void MovePosition()
    {
        if (Animator.GetFloat("speed") == 0) // 애니메이터와 이동 상태 동기화 (이동 애니메이션 재생중이 아닐 때는 이동 금지)
            return;

        if (_monsterAI.Target == null) // 타겟 없는 경우 스폰(베이스) 지점으로 이동
            pathVector = BasePosition;
        else // 타켓이 있는 경우 타겟의 위치를 추적
            pathVector = _monsterAI.Target.transform.position;
        

        if (STAT.Speed + SpeedFactor > 0)
            monsterNma.speed = STAT.Speed + SpeedFactor; // 기본 스탯 + 스피드 보정 값
        else
            monsterNma.speed = 0;

        monsterNma.CalculatePath(pathVector, pathToTarget);
        monsterNma.SetPath(pathToTarget); // 계산된 경로로 이동

        
        if (!monsterNma.pathPending && monsterNma.remainingDistance < 0.1f) // 목적지에 도착한 경우
        {
            STATE = CharacterState.Idle;
            return;
        }

    }

    // -------------------------------------------------------------------------------------







    // ----------------------------------- Control -----------------------------------------

    public override void OnIdle()
    {
        if(!isRaidBoss)
        {
            Animator.SetFloat("speed", 0);
            monsterNma.isStopped = true;
            monsterNma.updatePosition = false;
            monsterNma.updateRotation = false;
            monsterNma.velocity = Vector3.zero;
        }
    }

    public override void OnWalk()
    {
        if(!isRaidBoss)
        {
            Animator.SetFloat("speed", 1);
            monsterNma.isStopped = false;
            monsterNma.updatePosition = true;
            monsterNma.updateRotation = true;
        }
    }

    public override void OnAttack(int skillId)
    {
        if (!isRaidBoss)
        {
            Animator.SetFloat("speed", 0);
            monsterNma.isStopped = true;
            monsterNma.updatePosition = false;
            monsterNma.updateRotation = false;
            monsterNma.velocity = Vector3.zero;
        }

        StartCoroutine("CoAttack", skillId);
    }

    public override void OnKnockback()
    {
        if(HP <= 0)
        {
            STATE = CharacterState.Dead;
            return;
        }

        if (!HasSuerArmor) // 넉백 가능한 대상이면 넉백
            StartCoroutine("CoKnockBack");
    }

    public override void OnDead()
    {
        Inputable = false;

        if(_monsterAI != null)
            _monsterAI.gameObject.SetActive(false);

        if(!isRaidBoss)
        {
            Animator.SetFloat("speed", 0);
            monsterNma.isStopped = true;
            monsterNma.updatePosition = false;
            monsterNma.updateRotation = false;
            monsterNma.velocity = Vector3.zero;
        }
        
        StartCoroutine("CoDead");
    }

    // 공격 시도 후 명중한 대상들에 대한 데미지 계산
    public void HandleDamage(int skillId, Collider[] hitColliders)
    {
        // 기본 공격 데미지 처리
        if (skillId == 1)
        {
            foreach (Collider monsterCollider in hitColliders)
            {
                if (monsterCollider.TryGetComponent<MyPlayerController>(out _targetPlayer))
                {
                    // 데미지 = 0.8 ~ 1.2 (랜덤값) * 공격력 * 스킬 계수
                    int damage = Mathf.RoundToInt(Random.Range(0.8f, 1.2f) * STAT.Attack * 1f);
                    _targetPlayer.GetDamage(damage, true);
                }
            }
        }
        else if (skillId == 2) // 기본적으로 2, 3번째 스킬은 넉백 판정을 가지며 플레이어를 넉백 시킨다
        {
            foreach (Collider monsterCollider in hitColliders)
            {
                if (monsterCollider.TryGetComponent<MyPlayerController>(out _targetPlayer))
                {
                    // 데미지 = 0.8 ~ 1.2 (랜덤값) * 공격력 * 스킬 계수
                    int damage = Mathf.RoundToInt(Random.Range(0.8f, 1.2f) * STAT.Attack * 1.5f);
                    _targetPlayer.GetDamage(damage, true);

                }
            }
        }
        else if (skillId == 3) // 기본적으로 2, 3번째 스킬은 넉백 판정을 가지며 플레이어를 넉백 시킨다
        {
            // 몬스터가 사용하는 스킬로 일반적으로 보스만 세번째 스킬을 가진다


            foreach (Collider monsterCollider in hitColliders)
            {
                if (monsterCollider.TryGetComponent<MyPlayerController>(out _targetPlayer))
                {
                    // 데미지 = 0.8 ~ 1.2 (랜덤값) * 공격력 * 스킬 계수
                    int damage = Mathf.RoundToInt(Random.Range(0.8f, 1.2f) * STAT.Attack * 2.0f);
                    _targetPlayer.GetDamage(damage, true);
                }
            }
        }
    }


    // 공격 시도 코루틴
    IEnumerator CoAttack(int skillId)
    {
        CoolTime = _skillDelay[skillId - 1];
;
        // 스킬 이펙트 출력
        if (MonsterSkillSets[skillId - 1].SkillEffect != null)
        {
            MonsterSkillSets[skillId - 1].SkillEffect.gameObject.SetActive(true);
            MonsterSkillSets[skillId - 1].SkillEffect.Stop();
            MonsterSkillSets[skillId - 1].SkillEffect.Play();
        }


        // 스킬 애니메이션 재생
        if (skillId == 1)
        {
            Animator.Play("Attack1");

            yield return CoolTime; // 스킬 딜레이 동안 움직이지 못한다
        }
        else if(skillId == 2)
        {
            Animator.Play("BattleSkill");

            yield return CoolTime; // 스킬 딜레이 동안 움직이지 못한다
        }
        else if(skillId == 3)
        {
            Animator.Play("Ultimate");

            yield return CoolTime; // 스킬 딜레이 동안 움직이지 못한다
        }
        
        STATE = CharacterState.Idle;

    }

    // 데미지를 받았을 때의 처리
    public void GetDamage(int damage)
    {
        // 데미지 텍스트 출력 (오브젝트 풀링 적용)
        _damageText = Managers.Resource.Instantiate("UI/WorldSpace/DamageTextEffect", transform);

        HP = HP - damage; // HP 감소, HP Bar의 체력 보유 상황 갱신

        if (_damageText != null) // 받은 데미지 출력
        {
            _damageText.GetComponent<DamageTextEffect>().SetDamageText(damage);
        }

        if (HitEffectController != null) // 피격 이펙트 재생
        {
            HitEffectController.gameObject.SetActive(true);
            HitEffectController.Stop();
            HitEffectController.Play();
        }
    }

    IEnumerator CoKnockBack()
    {
        Inputable = false;
        Animator.Play("Damage");

        yield return new WaitForSeconds(0.2f); // 애니메이션 재생 시간동안 움직이지 못한다

        Inputable = true;
    }

    
    IEnumerator CoDead()
    {
        Animator.Play("Dead");
        gameObject.layer = 2; // 레이어를 변경하여 플레이어가 공격할 수 없는 대상으로 지정

        yield return new WaitForSeconds(2f);


        if (DeadEffectController != null) // 사망 이펙트 재생
        {
            DeadEffectController.gameObject.SetActive(true);
            DeadEffectController.Stop();
            DeadEffectController.Play();
        }

        ModelPrefab.SetActive(false);

        if (RelatedGimmic != null) // 관련 기믹이 있다면 조건 카운트 상승
            RelatedGimmic.CountUp();


        if(isStageBoss) // 보스 몬스터라면 스테이지 클리어 처리가 필요
        {
            // 스테이지 클리어 UI 출력
            Managers.UI.ShowPopupUI<UI_StageClearPopup>();

            // 스테이지 클리어 요청 패킷 전송
            C_StageClear stageClear = new C_StageClear()
            {
                StageName = Managers.Scene.GetCurrentSceneName()
            };

            Managers.Network.Send(stageClear);

            // 현재 스테이지와 관련하여 퀘스트가 있다면 클리어 처리
            Managers.Quest.CheckClearTypeQuest(stageClear.StageName);

        }

        if (RelatedObject != null)
            RelatedObject.SetActive(true);


        Destroy(gameObject, 4f); // 오브젝트 destory
    }

    // -----------------------------------------------------------------------------------









    // ------------------------------- Start & Update ------------------------------------


    void Start()
    {
        Init();
    }

    void Update()
    {
        UpdateState();
    }
    // -------------------------------------------------------------------------------------









    // ============================= 공격 범위 확인용 코드 =================================

    // 스킬 1 2 3 구분하기 위한 색상
    Color[] colors = new Color[] { 
        new Color(1, 0, 0, 0.2f),
        new Color(0, 1, 0, 0.2f),
        new Color(0, 0, 1, 0.2f), };


    // 공격 범위 확인
    void OnDrawGizmos()
    {
        if(MonsterSkillSets != null)
        {
            for (int i = 0; i < MonsterSkillSets.Length; i++)
            {
                Gizmos.color = colors[i];
                Gizmos.DrawSphere(transform.position + (transform.forward * 1),
                    MonsterSkillSets[i].SkillRange);
            }
        }     
    }

    // =====================================================================================


}
