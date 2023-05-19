using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : CreatureController
{
    private Animator animator;

    private Transform _targetUnit; // target unit (player)



    // 전부 상태로 관리
    bool _updated = false; // 1. STATE가 바뀌거나 2. 위치가 바뀌거나

    protected override void Init()
    {
        base.Init();
        animator = transform.GetComponentInChildren<Animator>();

    }


    // 몬스터의 현재 위치
    public override PositionInfo PosInfo
    {
        get { return _positionInfo; }
        set
        {
            if (_positionInfo.Equals(value))
                return;

            _positionInfo = value;
            // 이동 처리
            // TODO
        }
    }


    // -------------------- State ------------------------
    // 애니메이션 관련 업데이트
    public override CharacterState STATE
    {
        get { return _state; }
        set
        {
            //if (STATE == (CharacterState)PosInfo.State)
            //    return;

            _state = value;

            // 애니메이션 관련 업데이트
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


    // 탐지 거리가 긴 몬스터는 플레이어가 멀리에 있는 것을 발견 시 일정 거리까지 달려온다.
    protected override void UpdateSprint()
    {
        // TODO
        
    }

    protected override void UpdateAttack()
    {
        // 공격 모드 전환
    }

    void MovePosition(float moveSpeed)
    {
        // 플레이어 발견 시
        if (_targetUnit != null)
        {
            _updated = true;

            transform.LookAt(_targetUnit);
            float dist = (transform.position - _targetUnit.position).magnitude;

            // Debug.DrawRay(transform.position, transform.position - destination, Color.red);
            if(dist <= 1.0f) // 플레이어 도착
            {
                STATE = CharacterState.Attack;
            }
            else // 플레이어가 멀어지면 다시 따라감
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
            else // 다시 제 위치로 돌아감
            {
                // TODO
                STATE = CharacterState.Idle;
                _targetUnit = null;
            }
            


            yield return new WaitForSeconds(1.0f); // 1초마다 플레이어 탐색

        }
    }

    // 넉백 상태
    IEnumerator CoKnockBack()
    {

        // 애니메이션 재생 시간동안 움직이지 못한다
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
