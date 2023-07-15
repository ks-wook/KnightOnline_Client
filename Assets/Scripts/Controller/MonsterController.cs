using Assets.Scripts.Controller;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/*
 * ������ ��Ʈ�ѷ� ��ũ��Ʈ
 */

public class MonsterController : CreatureController
{



    [Header("Prefab Info")]

    [SerializeField]
    [Tooltip("���� �� ������")]
    GameObject ModelPrefab;

    [SerializeField]
    [Tooltip("������ ��� ���� �ɶ��� ����Ʈ")]
    public ParticleSystem StartEffect;

    [SerializeField]
    [Tooltip("���� �ǰ� �� ��ƼŬ")]
    public ParticleSystem HitEffectController;

    [SerializeField]
    [Tooltip("���� ��� �� ��ƼŬ")]
    public ParticleSystem DeadEffectController;

    [HideInInspector]
    public Gimmick RelatedGimmic;








    [Header("Status")]

    [SerializeField]
    [Tooltip("�������� ���� �������� ����, ���� ������ ��� ����� �������� Ŭ���� ó��")]
    bool isStageBoss = false;

    [SerializeField]
    [Tooltip("���̵� ���� �������� ����, ���� ������ ��� ����� �������� Ŭ���� ó��")]
    bool isRaidBoss = false;

    [SerializeField]
    [Tooltip("�ش� ���� óġ�� Ȱ��ȭ ��ų ������Ʈ")]
    GameObject RelatedObject;

    [SerializeField]
    [Tooltip("���� ���� �� (������ ���� Stat �����Ϳ��� �ɷ�ġ ȹ��)")]
    public int Level = 1;

    [SerializeField]
    [Tooltip("���� �þ� ���� ���� ��")]
    public float VisualRange = 10.0f;

    [SerializeField]
    [Tooltip("���Ϳ��� �Ÿ��� ������ ������ �����ϴ� �Ÿ�")]
    public float ChaseRange = 2.0f;

    [SerializeField]
    [Tooltip("���� �������� �������� �� �� �ִ� �ִ� �Ÿ�")]
    public float ReturnBaseRange = 5.0f;

    [SerializeField]
    [Tooltip("���ݹ޾��� �� �˹��� �Ǵ� �� ����")]
    public bool HasSuerArmor = false;

    [SerializeField]
    [Tooltip("hp �� ��ġ")]
    public float HpBarHeight = 1.0f;

    [SerializeField]
    [Tooltip("�̵��ӵ� ���� ��")]
    public float SpeedFactor = -4;








    [Header("Skill Info")]

    [SerializeField]
    [Tooltip("���� ��ų �� ����")]
    public MonsterSkillSet[] MonsterSkillSets;

    [SerializeField]
    [Tooltip("�� Ƚ�� ��ŭ ���� ���� �� �ñر� ��� (�ñرⰡ �ִ� ���͸�)")]
    public int UltimateCount = 5;


    [System.Serializable]
    public class MonsterSkillSet // ������ ��ų�� ���õ� ���� ��
    {
        [SerializeField]
        [Tooltip("���� ����")]
        public float SkillRange = 1.0f;

        [SerializeField]
        [Tooltip("���� ������ (���ݽ��� ���ۺ��� �������� ���ϴ� �ð�)")]
        public float SkillDelay = 1.2f;

        [SerializeField]
        [Tooltip("��ų ����Ʈ")]
        public ParticleSystem SkillEffect;

    }

    // �ڷ�ƾ ��ȯ �� ĳ��
    private List<WaitForSeconds> _skillDelay = new List<WaitForSeconds>();
    private WaitForSeconds CoolTime;



    // -------------------- Animation ���� ���� -------------------------
    [HideInInspector]
    public Animator Animator;

    // ------------------------------------------------------------------





    // ------------------------- UI ���� ���� ---------------------------
    GameObject _damageText;

    // ------------------------------------------------------------------




    // ---------------------- Target ���� ���� --------------------------
    private MyPlayerController _targetPlayer;
    private MonsterAI _monsterAI;
    public Vector3 BasePosition = new Vector3(0, 0, 0);

    LayerMask _hittalbeMask;

    NavMeshAgent monsterNma;
    NavMeshPath pathToTarget;
    Vector3 pathVector;

    // ------------------------------------------------------------------





    // ----------------------------- Init--------------------------------

    // ���� ���·� ����
    bool _updated = false; // 1. STATE�� �ٲ�ų� 2. ��ġ�� �ٲ�ų�
    public bool Inputable { get; set; } = true; // ���� �ൿ���� ���� (�÷��̾� ��Ʈ�ѷ��� ������ ����)

    protected override void Init()
    {
        base.Init();

        AnimInit();
        NavMeshInit();
        InitStat();

        Initnitialized = true; // ���� �ʱ�ȭ �Ϸ�

        // TEMP : ������Ʈ Ǯ�� �׽�Ʈ
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

    // NavMesh ���� �ʱ�ȭ
    void NavMeshInit()
    {
        if(!isRaidBoss) // ���̵� ������ AI�� ������ �����ϹǷ� �ʿ� ����
        {
            // NavMeshAgent ���� ���� �ʱ�ȭ
            monsterNma = transform.GetComponent<NavMeshAgent>();
            pathToTarget = new NavMeshPath();

            // �⺻������ �ڽ��� ��ġ�� Base (��׷� Ǯ�� �� ���ƿ��� ��ġ) �� ����
            BasePosition = transform.position;

            // AI ������Ʈ ȹ��
            _monsterAI = transform.GetComponentInChildren<MonsterAI>();
        }
    }

    // �ִϸ��̼� ���� �ʱ�ȭ
    void AnimInit()
    {
        // �ִϸ����� ȹ��
        Animator = transform.GetComponent<Animator>();

        // ������ �� �ִ� ��� ����ũ
        _hittalbeMask = LayerMask.GetMask("Player");

        // ��ų ������ �� �̿��Ͽ� Waitforseconds ��ü �̸� ����(ĳ��)
        foreach(MonsterSkillSet ms in MonsterSkillSets)
        {
            _skillDelay.Add(new WaitForSeconds(ms.SkillDelay));
        }
    }

    // ���� �����ͷκ��� ������ ���� �ɷ�ġ ȹ��
    void InitStat()
    {
        StatInfo stat;
        Managers.Data.StatDict.TryGetValue(Level, out stat);

        Debug.Log(stat);

        if (stat != null) // �ɷ�ġ ����
        {
            STAT = stat;
            SetHpBar(HpBarHeight); // �ɷ�ġ �ο��� HP �� ����
        }

    }

    // -------------------------------------------------------------------------------------










    // ------------------------------------- State -----------------------------------------
    // �ִϸ��̼� ���� ������Ʈ
    public override CharacterState STATE
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            _state = value;

            // �ִϸ��̼� ���� ������Ʈ
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

    

    // ���� ���¿� ���� ������Ʈ EX) �̵�
    void UpdateState()
    {
        switch (STATE)
        {
            case CharacterState.Walk:
                MovePosition();
                break;
        }

    }

    // �̵� ó�� �Լ�
    void MovePosition()
    {
        if (Animator.GetFloat("speed") == 0) // �ִϸ����Ϳ� �̵� ���� ����ȭ (�̵� �ִϸ��̼� ������� �ƴ� ���� �̵� ����)
            return;

        if (_monsterAI.Target == null) // Ÿ�� ���� ��� ����(���̽�) �������� �̵�
            pathVector = BasePosition;
        else // Ÿ���� �ִ� ��� Ÿ���� ��ġ�� ����
            pathVector = _monsterAI.Target.transform.position;
        

        if (STAT.Speed + SpeedFactor > 0)
            monsterNma.speed = STAT.Speed + SpeedFactor; // �⺻ ���� + ���ǵ� ���� ��
        else
            monsterNma.speed = 0;

        monsterNma.CalculatePath(pathVector, pathToTarget);
        monsterNma.SetPath(pathToTarget); // ���� ��η� �̵�

        
        if (!monsterNma.pathPending && monsterNma.remainingDistance < 0.1f) // �������� ������ ���
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

        if (!HasSuerArmor) // �˹� ������ ����̸� �˹�
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

    // ���� �õ� �� ������ ���鿡 ���� ������ ���
    public void HandleDamage(int skillId, Collider[] hitColliders)
    {
        // �⺻ ���� ������ ó��
        if (skillId == 1)
        {
            foreach (Collider monsterCollider in hitColliders)
            {
                if (monsterCollider.TryGetComponent<MyPlayerController>(out _targetPlayer))
                {
                    // ������ = 0.8 ~ 1.2 (������) * ���ݷ� * ��ų ���
                    int damage = Mathf.RoundToInt(Random.Range(0.8f, 1.2f) * STAT.Attack * 1f);
                    _targetPlayer.GetDamage(damage, true);
                }
            }
        }
        else if (skillId == 2) // �⺻������ 2, 3��° ��ų�� �˹� ������ ������ �÷��̾ �˹� ��Ų��
        {
            foreach (Collider monsterCollider in hitColliders)
            {
                if (monsterCollider.TryGetComponent<MyPlayerController>(out _targetPlayer))
                {
                    // ������ = 0.8 ~ 1.2 (������) * ���ݷ� * ��ų ���
                    int damage = Mathf.RoundToInt(Random.Range(0.8f, 1.2f) * STAT.Attack * 1.5f);
                    _targetPlayer.GetDamage(damage, true);

                }
            }
        }
        else if (skillId == 3) // �⺻������ 2, 3��° ��ų�� �˹� ������ ������ �÷��̾ �˹� ��Ų��
        {
            // ���Ͱ� ����ϴ� ��ų�� �Ϲ������� ������ ����° ��ų�� ������


            foreach (Collider monsterCollider in hitColliders)
            {
                if (monsterCollider.TryGetComponent<MyPlayerController>(out _targetPlayer))
                {
                    // ������ = 0.8 ~ 1.2 (������) * ���ݷ� * ��ų ���
                    int damage = Mathf.RoundToInt(Random.Range(0.8f, 1.2f) * STAT.Attack * 2.0f);
                    _targetPlayer.GetDamage(damage, true);
                }
            }
        }
    }


    // ���� �õ� �ڷ�ƾ
    IEnumerator CoAttack(int skillId)
    {
        CoolTime = _skillDelay[skillId - 1];
;
        // ��ų ����Ʈ ���
        if (MonsterSkillSets[skillId - 1].SkillEffect != null)
        {
            MonsterSkillSets[skillId - 1].SkillEffect.gameObject.SetActive(true);
            MonsterSkillSets[skillId - 1].SkillEffect.Stop();
            MonsterSkillSets[skillId - 1].SkillEffect.Play();
        }


        // ��ų �ִϸ��̼� ���
        if (skillId == 1)
        {
            Animator.Play("Attack1");

            yield return CoolTime; // ��ų ������ ���� �������� ���Ѵ�
        }
        else if(skillId == 2)
        {
            Animator.Play("BattleSkill");

            yield return CoolTime; // ��ų ������ ���� �������� ���Ѵ�
        }
        else if(skillId == 3)
        {
            Animator.Play("Ultimate");

            yield return CoolTime; // ��ų ������ ���� �������� ���Ѵ�
        }
        
        STATE = CharacterState.Idle;

    }

    // �������� �޾��� ���� ó��
    public void GetDamage(int damage)
    {
        // ������ �ؽ�Ʈ ��� (������Ʈ Ǯ�� ����)
        _damageText = Managers.Resource.Instantiate("UI/WorldSpace/DamageTextEffect", transform);

        HP = HP - damage; // HP ����, HP Bar�� ü�� ���� ��Ȳ ����

        if (_damageText != null) // ���� ������ ���
        {
            _damageText.GetComponent<DamageTextEffect>().SetDamageText(damage);
        }

        if (HitEffectController != null) // �ǰ� ����Ʈ ���
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

        yield return new WaitForSeconds(0.2f); // �ִϸ��̼� ��� �ð����� �������� ���Ѵ�

        Inputable = true;
    }

    
    IEnumerator CoDead()
    {
        Animator.Play("Dead");
        gameObject.layer = 2; // ���̾ �����Ͽ� �÷��̾ ������ �� ���� ������� ����

        yield return new WaitForSeconds(2f);


        if (DeadEffectController != null) // ��� ����Ʈ ���
        {
            DeadEffectController.gameObject.SetActive(true);
            DeadEffectController.Stop();
            DeadEffectController.Play();
        }

        ModelPrefab.SetActive(false);

        if (RelatedGimmic != null) // ���� ����� �ִٸ� ���� ī��Ʈ ���
            RelatedGimmic.CountUp();


        if(isStageBoss) // ���� ���Ͷ�� �������� Ŭ���� ó���� �ʿ�
        {
            // �������� Ŭ���� UI ���
            Managers.UI.ShowPopupUI<UI_StageClearPopup>();

            // �������� Ŭ���� ��û ��Ŷ ����
            C_StageClear stageClear = new C_StageClear()
            {
                StageName = Managers.Scene.GetCurrentSceneName()
            };

            Managers.Network.Send(stageClear);

            // ���� ���������� �����Ͽ� ����Ʈ�� �ִٸ� Ŭ���� ó��
            Managers.Quest.CheckClearTypeQuest(stageClear.StageName);

        }

        if (RelatedObject != null)
            RelatedObject.SetActive(true);


        Destroy(gameObject, 4f); // ������Ʈ destory
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









    // ============================= ���� ���� Ȯ�ο� �ڵ� =================================

    // ��ų 1 2 3 �����ϱ� ���� ����
    Color[] colors = new Color[] { 
        new Color(1, 0, 0, 0.2f),
        new Color(0, 1, 0, 0.2f),
        new Color(0, 0, 1, 0.2f), };


    // ���� ���� Ȯ��
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
