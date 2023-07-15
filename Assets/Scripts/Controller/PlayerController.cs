using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * ��Ƽ �÷��̽� �ٸ� �÷��̾��� ĳ���͸� �����ϴ� ��Ʈ�ѷ� ��ũ��Ʈ
 * 
 * MyPlayerController�� ���� �����ϸ� �Է°� ���õ� ��ũ��Ʈ �� ��Ʈ��ũ ������Ʈ�� ���õ�
 * ��� ��ũ��Ʈ�� ������ ��Ʈ�ѷ��̴�.
 * 
 * �������� ���� ��ɿ� ���� �̵��ϰų� �ִϸ��̼��� ����Ͽ� ���� �ٸ� �÷��̾���
 * �ൿ�� �����ϴ� ������ �Ѵ�.
 */


public class PlayerController : CreatureController
{
    [SerializeField]
    private Transform Player;

    // ----------------------- State Update ���� ����------------------------

    // �ִϸ��̼� �� particleSystem
    public Animator PlayerAnimator; // �÷��̾� �ִϸ�����

    [HideInInspector]
    public ParticleSystem SlashEffectController; // �˱� ����Ʈ

    [HideInInspector]
    public ParticleSystem DashEffectController; // �뽬 ����Ʈ

    [HideInInspector]
    public ParticleSystem BattleSkillEffectController; // ���� ��ų ����Ʈ

    [HideInInspector]
    public ParticleSystem UltimateSkillEffectController; // �ñر� ����Ʈ

    Transform _handGrip = null; // �տ� �� ���� ������
    Transform _backGrip = null; // � �� ���� ������

    Transform _lastWeapon = null; // �������� �����ϰ� �ִ� ����

    // �ִϸ��̼� ����ð����ȿ� ���� �ִϸ��̼� ��� ��Ÿ��
    WaitForSeconds nomalAttackCool = new WaitForSeconds(0.3f);
    WaitForSeconds battleSkillCool = new WaitForSeconds(1.1f);
    WaitForSeconds UltimateCool = new WaitForSeconds(6f);

    // ----------------------------------------------------------------------



    // ------------------------- �÷��̾� �ʱ�ȭ ----------------------------
    protected override void Init()
    {
        base.Init();
        InitAnimAndParticlesys();
    }

    private void InitAnimAndParticlesys() // �ִϸ����� �� ����Ʈ�� ���� ��ƼŬ �ý��� ȹ��
    {
        _handGrip = transform.GetChild(0)
            .Find("Root/Hips/Spine_01/Spine_02/Spine_03/Clavicle_R/Shoulder_R/Elbow_R/Hand_R/HandGrip");

        // ����Ʈ ������ �� ���� �˻��Ͽ� ȹ��
        if (SlashEffectController == null) // �˱� ����Ʈ ȹ��
            SlashEffectController = _handGrip.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();

        // ���Ⱑ � �����Ǵ� �׸�
        _backGrip = transform.GetChild(0)
            .Find("Root/Hips/Spine_01/Spine_02/BackGrip");


        // �뽬 ����Ʈ ȹ��
        if (DashEffectController == null)
        {
            Transform dashParticle = transform.Find("DashParticle");
            DashEffectController = dashParticle.GetComponent<ParticleSystem>();
        }

        // ���� ��ų �� �ñر� ����Ʈ ȹ��
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
    // Ŭ���̾�Ʈ ���� ��ġ ����
    public override PositionInfo PosInfo
    {
        get { return _positionInfo; }
        set 
        {
            if (_positionInfo.Equals(value))
                return;

            // �������� ��ġ�� ���� Ŭ���̾�Ʈ������ ��ġ ����
            _positionInfo = value;

            // ���� ������ ���ÿ� ����
            DirInfo = value.DirInfo;

            // ���¸� �����ϸ鼭 ��ų �ִϸ��̼� ����� �ڵ����� �̷������.
            STATE = (CharacterState)value.State;
        }
    }

    // ��Ŷ�� ���� ó��
    public void MovePosition()
    {
        Vector3 _moveDir = VectorPosInfo - transform.position;

        if (_moveDir.magnitude < 0.001f) // ������ ����
        {
            STATE = CharacterState.Idle;
        }
        else
        {
            transform.forward = ForwardDir;
            transform.position += ForwardDir * Time.deltaTime * STAT.Speed;

            if(_moveDir.magnitude > 1f) // ���� ����
            {
                transform.position = VectorPosInfo;
            }
        }

    }

    // ----------------------------------------------------------------------



    // ----------------------------- State ----------------------------------
    // �ִϸ��̼� ���� ������Ʈ
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
                    DashEffectController.Stop(); // ��� ����Ʈ ��� ����
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

    // ���� �ִϸ��̼��� ����ϴ� �ڷ�ƾ
    // �ٸ� �÷��̾��� �÷��̾� ��ü�� �ִϸ��̼Ǹ� ����ϸ� ���� �����̳� �Է°��� ���� �ʴ´�.
    IEnumerator CoAttack(int skillId)
    {
        if (skillId == 1) // �Ϲ� ����
        {
            Debug.Log("�Ϲ� ���� ����");

            SlashEffectController.Play();
            PlayerAnimator.SetFloat("normalAttack", 1);

            yield return nomalAttackCool;

            SlashEffectController.Stop();
            PlayerAnimator.SetFloat("normalAttack", 0);
            STATE = CharacterState.Idle;
        }
        else if (skillId == 2) // ���� ��ų
        {
            Debug.Log("���� ��ų ����");

            PlayerAnimator.Play("BattleSkill");
            BattleSkillEffectController.gameObject.SetActive(true);

            yield return battleSkillCool;

            STATE = CharacterState.Idle;
        }
        else if (skillId == 3) // �ñر�
        {
            Debug.Log("�ñر� ����");

            PlayerAnimator.Play("Ultimate");

            yield return UltimateCool;

            STATE = CharacterState.Idle;
        }
    }


    // �˹� �� ó��
    IEnumerator CoKnockBack()
    {
        // �ִϸ��̼� ��� �ð����� �������� ���Ѵ�
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
        if (!DashEffectController.isPlaying) // �뽬 ����Ʈ ���
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
