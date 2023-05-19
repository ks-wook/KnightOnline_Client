using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : CreatureController
{
    [SerializeField]
    private Transform Player;

    Animator animator;

    protected override void Init()
    {
        base.Init();
        animator = Player.GetComponentInChildren<Animator>();
    }


    // --------------- Position --------------------
    // Ŭ���̾�Ʈ ���� ��ġ ����
    public override PositionInfo PosInfo
    {
        get { return _positionInfo; }
        set 
        {
            if (_positionInfo.Equals(value))
                return;

            _positionInfo = value;
            DirInfo = value.DirInfo;
            STATE = (CharacterState)value.State;
        }
    }

    // ��Ŷ�� ���� ó��
    public override void MovePosition()
    {
        Vector3 _moveDir = DestPosition - transform.position;

        if (_moveDir.magnitude < 0.0001f) // ������ ����
        {
            STATE = CharacterState.Idle;
        }
        else
        {
            transform.forward = ForwardDir;
            transform.position += ForwardDir * Time.deltaTime * Stat.Speed;

            if(_moveDir.magnitude > 0.5f) // ���� ����
            {
                transform.position = DestPosition;
            }
        }

    }

    // --------------------------------------------------



    // ------------------- State ------------------------
    public override CharacterState STATE
    {
        get { return _state; }
        set
        {
            if (STATE == (CharacterState)PosInfo.State)
                return;

            _state = value;

            // �ִϸ��̼� ���� ������Ʈ
            switch (_state)
            {
                case CharacterState.Idle:
                    animator.SetFloat("speed", 0);
                    break;
                case CharacterState.Walk:
                    animator.SetFloat("speed", 1);
                    break;
                case CharacterState.Sprint:
                    animator.SetFloat("speed", 2);
                    break;
                case CharacterState.Attack:
                    StartCoroutine(CoAttack());
                    break;
                case CharacterState.Dead:
                    Debug.Log($"iD : {this.Id} is dead");
                    animator.Play("Death");
                    break;
                case CharacterState.KnockBack:
                    // StartCoroutine(CoKnockBack());
                    break;
            }


        }
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
            case CharacterState.Attack:
                // UpdateAttack();
                break;
            case CharacterState.KnockBack:
                // UpdateKnockBack();
                break;
            case CharacterState.Dead:
                UpdateDead();
                break;
            default:
                UpdateIdle();
                break;
        }
    }

    protected override void UpdateIdle()
    {
        // CheckAttack();
    }

    protected override void UpdateWalk()
    {
        CheckAttack();

        // speed �� ���� �� -> ��Ŷ�� �߰��� ����
        MovePosition();
    }

    protected override void UpdateSprint()
    {

        if(STATE == CharacterState.Idle || STATE == CharacterState.Walk || STATE == CharacterState.Sprint)
        {
            STATE = CharacterState.Sprint;
        }

        CheckAttack();

        MovePosition();

    }

    protected override void UpdateDead()
    {
        // TODO
    }

    // ------------------------------------------------




    // ------------------- Control --------------------
    void CheckAttack()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    STATE = CharacterState.Attack;
        //    if (Input.GetMouseButtonDown(0))
        //        STATE = CharacterState.Attack;

        //}
    }
    
    public void UseSkill(int skillId)
    {
        if(skillId == 1) // �⺻ ����
        {
            STATE = CharacterState.Attack;
            // StartCoroutine(CoAttack()); 
        }
        else if(skillId == 2) // ��Ÿ ��ų��...
        {
            // TODO
        }

    }

    IEnumerator CoAttack()
    {
        animator.SetFloat("normalAttack", 1);

        yield return new WaitForSeconds(0.9f);

        animator.SetFloat("normalAttack", 0);
        STATE = CharacterState.Idle;
    }

    IEnumerator CoKnockBack()
    {

        // �ִϸ��̼� ��� �ð����� �������� ���Ѵ�
        yield return new WaitForSeconds(1.05f);

        STATE = CharacterState.Idle;

    }

    public override void OnDead()
    {
        STATE = CharacterState.Dead;
    }

    // -------------------------------------------------

    void Start()
    {
        Init();
    }

    void Update()
    {
        // �����κ��� ���� ��Ŷ�� ���� ���� ������Ʈ
        UpdateState();
    }

}
