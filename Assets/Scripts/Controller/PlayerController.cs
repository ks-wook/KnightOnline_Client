using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * 멀티 플레이시 다른 플레이어의 캐릭터를 조종하는 컨트롤러 스크립트
 * 
 * MyPlayerController와 거의 유사하며 입력과 관련된 스크립트 및 네트워크 업데이트와 관련된
 * 모든 스크립트를 제거한 컨트롤러이다.
 * 
 * 서버에서 보낸 명령에 따라서 이동하거나 애니메이션을 재생하여 실제 다른 플레이어의
 * 행동을 재현하는 역할을 한다.
 */


public class PlayerController : CreatureController
{
    [SerializeField]
    private Transform Player;

    // ----------------------- State Update 관련 변수------------------------

    // 애니메이션 및 particleSystem
    public Animator PlayerAnimator; // 플레이어 애니메이터

    [HideInInspector]
    public ParticleSystem SlashEffectController; // 검기 이펙트

    [HideInInspector]
    public ParticleSystem DashEffectController; // 대쉬 이펙트

    [HideInInspector]
    public ParticleSystem BattleSkillEffectController; // 전투 스킬 이펙트

    [HideInInspector]
    public ParticleSystem UltimateSkillEffectController; // 궁극기 이펙트

    Transform _handGrip = null; // 손에 쥔 무기 프리팹
    Transform _backGrip = null; // 등에 멘 무기 프리팹

    Transform _lastWeapon = null; // 마지막에 장착하고 있던 무기

    // 애니메이션 재생시간동안에 의한 애니메이션 재생 쿨타임
    WaitForSeconds nomalAttackCool = new WaitForSeconds(0.3f);
    WaitForSeconds battleSkillCool = new WaitForSeconds(1.1f);
    WaitForSeconds UltimateCool = new WaitForSeconds(6f);

    // ----------------------------------------------------------------------



    // ------------------------- 플레이어 초기화 ----------------------------
    protected override void Init()
    {
        base.Init();
        InitAnimAndParticlesys();
    }

    private void InitAnimAndParticlesys() // 애니메이터 및 이펙트를 위한 파티클 시스템 획득
    {
        _handGrip = transform.GetChild(0)
            .Find("Root/Hips/Spine_01/Spine_02/Spine_03/Clavicle_R/Shoulder_R/Elbow_R/Hand_R/HandGrip");

        // 이펙트 미지정 시 직접 검색하여 획득
        if (SlashEffectController == null) // 검기 이펙트 획득
            SlashEffectController = _handGrip.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();

        // 무기가 등에 장착되는 그립
        _backGrip = transform.GetChild(0)
            .Find("Root/Hips/Spine_01/Spine_02/BackGrip");


        // 대쉬 이펙트 획득
        if (DashEffectController == null)
        {
            Transform dashParticle = transform.Find("DashParticle");
            DashEffectController = dashParticle.GetComponent<ParticleSystem>();
        }

        // 전투 스킬 및 궁극기 이펙트 획득
        if (BattleSkillEffectController == null)
        {
            BattleSkillEffectController =
                transform.Find("BattleSkillEffect").GetComponent<ParticleSystem>();
        }

        if (UltimateSkillEffectController == null)
        {
            UltimateSkillEffectController =
                transform.Find("UltimateSkillEffect").GetComponent<ParticleSystem>();
        }
    }

    // ----------------------------------------------------------------------


    // ---------------------------- Position --------------------------------
    // 클라이언트 현재 위치 정보
    public override PositionInfo PosInfo
    {
        get { return _positionInfo; }
        set 
        {
            if (_positionInfo.Equals(value))
                return;

            // 서버상의 위치를 통해 클라이언트에서의 위치 갱신
            _positionInfo = value;

            // 방향 정보도 동시에 갱신
            DirInfo = value.DirInfo;

            // 상태를 갱신하면서 스킬 애니메이션 재생도 자동으로 이루어진다.
            STATE = (CharacterState)value.State;
        }
    }

    // 패킷에 의해 처리
    public void MovePosition()
    {
        Vector3 _moveDir = VectorPosInfo - transform.position;

        if (_moveDir.magnitude < 0.001f) // 목적지 도착
        {
            STATE = CharacterState.Idle;
        }
        else
        {
            transform.forward = ForwardDir;
            transform.position += ForwardDir * Time.deltaTime * STAT.Speed;

            if(_moveDir.magnitude > 1f) // 오차 보정
            {
                transform.position = VectorPosInfo;
            }
        }

    }

    // ----------------------------------------------------------------------



    // ----------------------------- State ----------------------------------
    // 애니메이션 관련 업데이트
    public override CharacterState STATE
    {
        get { return _state; }
        set
        {
            if (STATE == value)
                return;

            if (_state == CharacterState.Sprint)
            {
                if (DashEffectController.isPlaying)
                    DashEffectController.Stop(); // 대시 이펙트 재생 중지
            }

            _state = value;

            switch (_state)
            {
                case CharacterState.Idle:
                    OnIdle();
                    break;
                case CharacterState.Walk:
                    OnWalk();
                    break;
                case CharacterState.Sprint:
                    OnSprint();
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
                case CharacterState.KnockBack:
                    OnKnockback();
                    break;
                case CharacterState.Dead:
                    OnDead();
                    break;
            }
        }
    }

    public override void OnIdle()
    {
        PlayerAnimator.SetFloat("speed", 0);
    }

    public override void OnWalk()
    {
        PlayerAnimator.SetFloat("speed", 1);
    }

    public override void OnSprint()
    {
        PlayerAnimator.SetFloat("speed", 2);
    }

    public override void OnAttack(int skillId)
    {
        if (skillId > 0)
        {
            StartCoroutine("CoAttack", skillId);
        }
    }

    public override void OnKnockback()
    {
        StartCoroutine("CoKnockBack");
    }

    public override void OnDead()
    {
        PlayerAnimator.Play("Death");
    }

    // 공격 애니메이션을 재생하는 코루틴
    // 다른 플레이어의 플레이어 객체는 애니메이션만 재생하며 실제 연산이나 입력값을 받지 않는다.
    IEnumerator CoAttack(int skillId)
    {
        if (skillId == 1) // 일반 공격
        {
            Debug.Log("일반 공격 실행");

            SlashEffectController.Play();
            PlayerAnimator.SetFloat("normalAttack", 1);

            yield return nomalAttackCool;

            SlashEffectController.Stop();
            PlayerAnimator.SetFloat("normalAttack", 0);
            STATE = CharacterState.Idle;
        }
        else if (skillId == 2) // 전투 스킬
        {
            Debug.Log("전투 스킬 실행");

            PlayerAnimator.Play("BattleSkill");
            BattleSkillEffectController.gameObject.SetActive(true);

            yield return battleSkillCool;

            STATE = CharacterState.Idle;
        }
        else if (skillId == 3) // 궁극기
        {
            Debug.Log("궁극기 실행");

            PlayerAnimator.Play("Ultimate");

            yield return UltimateCool;

            STATE = CharacterState.Idle;
        }
    }


    // 넉백 시 처리
    IEnumerator CoKnockBack()
    {
        // 애니메이션 재생 시간동안 움직이지 못한다
        yield return new WaitForSeconds(1.05f);

        STATE = CharacterState.Idle;
    }

    void UpdateState()
    {
        switch (STATE)
        {
            case CharacterState.Idle:
                UpdateIdle();
                break;
            case CharacterState.Walk:
                UpdateWalk();
                break;
            case CharacterState.Sprint:
                UpdateSprint();
                break;
            default:
                UpdateIdle();
                break;
        }

        PosInfo.State = (CreateureState)STATE;
    }

    protected override void UpdateIdle()
    {
        PosInfo.State = CreateureState.Idle;
    }

    protected override void UpdateWalk()
    {
        MovePosition();
        PosInfo.State = CreateureState.Walk;
    }

    protected override void UpdateSprint()
    {
        if (!DashEffectController.isPlaying) // 대쉬 이펙트 재생
            DashEffectController.Play();

        MovePosition();
        PosInfo.State = CreateureState.Sprint;

    }

    // ----------------------------------------------------------------------







    // -------------------------- Start & Update ---------------------------
    void Start()
    {
        Init();
    }

    void Update()
    {
        UpdateState();
    }

    // ----------------------------------------------------------------------

}
