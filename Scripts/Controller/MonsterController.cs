using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : CreatureController
{
    private Animator animator;

    private Transform _targetUnit; // target unit (player)



    // ���� ���·� ����
    bool _updated = false; // 1. STATE�� �ٲ�ų� 2. ��ġ�� �ٲ�ų�

    protected override void Init()
    {
        base.Init();
        animator = transform.GetComponentInChildren<Animator>();

    }


    // ������ ���� ��ġ
    public override PositionInfo PosInfo
    {
        get { return _positionInfo; }
        set
        {
            if (_positionInfo.Equals(value))
                return;

            _positionInfo = value;
            // �̵� ó��
            // TODO
        }
    }


    // -------------------- State ------------------------
    // �ִϸ��̼� ���� ������Ʈ
    public override CharacterState STATE
    {
        get { return _state; }
        set
        {
            //if (STATE == (CharacterState)PosInfo.State)
            //    return;

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
                case CharacterState.Attack:
                    StartCoroutine(CoAttack());
                    break;
                case CharacterState.Dead:
                    Debug.Log($"iD : {this.Id} is dead");
                    animator.Play("Death");
                    break;
                case CharacterState.KnockBack:
                    StartCoroutine(CoKnockBack());
                    break;
            }

            _updated = true;

            


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

        PosInfo.State = (CreateureState)STATE;
        CheckUpdatedFlag();

    }

    protected override void UpdateIdle()
    {

    }

    protected override void UpdateWalk()
    {
        MovePosition(2.0f);
    }


    // Ž�� �Ÿ��� �� ���ʹ� �÷��̾ �ָ��� �ִ� ���� �߰� �� ���� �Ÿ����� �޷��´�.
    protected override void UpdateSprint()
    {
        // TODO
        
    }

    protected override void UpdateAttack()
    {
        // ���� ��� ��ȯ
    }

    void MovePosition(float moveSpeed)
    {
        // �÷��̾� �߰� ��
        if (_targetUnit != null)
        {
            _updated = true;

            transform.LookAt(_targetUnit);
            float dist = (transform.position - _targetUnit.position).magnitude;

            // Debug.DrawRay(transform.position, transform.position - destination, Color.red);
            if(dist <= 1.0f) // �÷��̾� ����
            {
                STATE = CharacterState.Attack;
            }
            else // �÷��̾ �־����� �ٽ� ����
            {
                transform.position = Vector3.MoveTowards(transform.position, _targetUnit.position, Time.deltaTime * moveSpeed);
            }

        }
        else
        {
            STATE = CharacterState.Idle;
        }
    }


    // ------------------- Control --------------------
    void CheckAttack()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    isAttak = true;
        //    STATE = MonsterState.Attack;
        //    if (Input.GetMouseButtonDown(0))
        //        STATE = MonsterState.Attack;

        //}
    }

    void CheckUpdatedFlag()
    {
        if (_updated)
        {
            C_Move movePacket = new C_Move();
            movePacket.PosInfo = PosInfo;
            movePacket.PosInfo.DirInfo = DirInfo;
            Managers.Network.Send(movePacket);
            _updated = false;
        }
    }

    IEnumerator CoAttack()
    {

        animator.SetFloat("normalAttack", 1);

        yield return new WaitForSeconds(1.0f);


        animator.SetFloat("normalAttack", 0);
        STATE = CharacterState.Idle;

    }

    IEnumerator CoSearchPlayer()
    {
        while(true)
        {
            
            Collider[] overlaps = Physics.OverlapSphere(transform.position, 2.0f, LayerMask.GetMask("Player"));
            if (overlaps.Length != 0)
            {
                Collider overlap = overlaps[0];
                if (overlap != null)
                {

                    _targetUnit = overlap.gameObject.transform;
                    if (STATE != CharacterState.Attack)
                        STATE = CharacterState.Walk;


                }


            }
            else // �ٽ� �� ��ġ�� ���ư�
            {
                // TODO
                STATE = CharacterState.Idle;
                _targetUnit = null;
            }
            


            yield return new WaitForSeconds(1.0f); // 1�ʸ��� �÷��̾� Ž��

        }
    }

    // �˹� ����
    IEnumerator CoKnockBack()
    {

        // �ִϸ��̼� ��� �ð����� �������� ���Ѵ�
        yield return new WaitForSeconds(0.2f);

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
        // StartCoroutine("CoSearchPlayer");
    }

    void Update()
    {
        
        // UpdateState();
    }
}
