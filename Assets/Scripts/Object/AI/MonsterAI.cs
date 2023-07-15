using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 기본 몬스터의 시야 범위를 설정하고, 일정 범위내에 Player 태그를 달고있는 오브젝트가 접근 시 
 * 타겟을 설정하고 해당 타겟을 추적하는 기능을 하는 스크립트
 * 
 * 플레이어나 몬스터처럼 FSM 모델을 적용하여 트리거의 상태를 STATE라는 프로퍼티로 나타냄
*/

public class MonsterAI : AIStateMachine
{
    [HideInInspector]
    public GameObject Target = null;

    [HideInInspector]
    MonsterController _controller;


    // -------------------------------- 스킬 관련 변수 ------------------------------------
    // 필요한 모든 정보들은 MonsterController 스크립트로부터 자동으로 얻어온다.

    float ChaseRange; // 플레이어와의 거리가 벌어져 재추적에 들어가는 거리
    float ReturnBaseRange; // BasePosition으로 부터 최대로 떨어질 수 있는 거리
    int UltimateCount; // UltimateStack 값이 이 값에 도달 시 궁극기 사용


    // ------------------------------------------------------------------------------------

    void Init()
    {
        // 컨트롤러 획득 후 설정된 값들을 얻어온다.
        _controller = transform.GetComponentInParent<MonsterController>();
        if (_controller == null)
        {
            Debug.Log("VisualRangeTrigger 초기화 실패 : 몬스터 컨트롤러 스크립트 검색 불가능");
        }
        else
        {
            ChaseRange = _controller.ChaseRange;
            ReturnBaseRange = _controller.ReturnBaseRange;
            UltimateCount = _controller.UltimateCount;
        }
    }

    // -------------------------------- EventHandler ------------------------------------
    // 트리거 범위안에 플레이어가 들어온 경우
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.tag == "Player")
        {
            Debug.Log("플레이어 추적 시작");

            Target = other.gameObject;

            AI_State = AISTATE.Enter;
        }
    }

    // 트리거 범위 밖으로 목표가 나간 경우
    override protected void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);


        if (other.tag == "Player")
        {
            if (Target != null)
                Target = null;

            Debug.Log("플레이어 추적 중지");

            AI_State = AISTATE.Exit;
        }
    }

    // -----------------------------------------------------------------------------------





    // ------------------------------------ STATE ----------------------------------------
    public override void UpdateAIState()
    {
        switch(AI_State)
        {
            case AISTATE.Enter: // 플레이어가 시야 범위 내로 들어온 상태
                if(Target != null)
                {
                    if(_controller.Inputable) // 움직일 수 있는 상태라면 추적 시작
                        _controller.STATE = CreatureController.CharacterState.Walk;

                    if((Target.transform.position - transform.position).magnitude < ChaseRange) // 일정 거리 이내로 타겟이 들어온 경우
                    {
                        AI_State = AISTATE.Battle; // 트리거는 다시 배틀 상태로 돌아간다
                    }
                }
                break; 
            case AISTATE.Battle: // 플레이어가 공격 범위내로 들어와서 공격을 시도할 수 있는 상태

                if (Target != null)
                {
                    if ((Target.transform.position - transform.position).magnitude > ChaseRange) // 목표와의 거리가 일정 이상 멀어진 경우 재추적
                    {
                        AI_State = AISTATE.Enter;
                    }

                    if (((Target.transform.position - transform.position).magnitude < ChaseRange) && _controller.Inputable) // 목표가 일정거리 이내로 들어 왔을 때
                    {

                        // 다음 행동 결정
                        int behavior = UnityEngine.Random.Range(1, 10);

                        if (behavior < 3) // 20% 확률로 가만히 서있거나
                        {
                            _controller.STATE = CreatureController.CharacterState.Idle;
                            StartCoroutine("CoNextBehaviorCoolTime", 2.0f);

                        }
                        else // 공격을 시도한다
                        {
                            Debug.Log(behavior);

                            if (_controller.MonsterSkillSets.Length == 3) // 보스와 같이 3가지 기술을 가진 경우
                            {

                                Debug.Log("궁극기 스택 : " + _controller._ultimateStack + "/" + UltimateCount);
                                if (_controller._ultimateStack >= UltimateCount)
                                {
                                    StartCoroutine("CoNextBehaviorCoolTime", _controller.MonsterSkillSets[2].SkillDelay);
                                    _controller.STATE = CreatureController.CharacterState.Ultimate;
                                    _controller._ultimateStack = 0;
                                
                                }
                                else if ((behavior > 2) && (behavior < 6)) // 기본 공격 시도
                                {
                                    StartCoroutine("CoNextBehaviorCoolTime", _controller.MonsterSkillSets[0].SkillDelay);
                                    _controller.STATE = CreatureController.CharacterState.NomalAttack;                                    
                                }
                                else if (behavior > 5 && (behavior < 10)) // 스킬 공격 시도
                                {
                                    StartCoroutine("CoNextBehaviorCoolTime", _controller.MonsterSkillSets[1].SkillDelay);
                                    _controller.STATE = CreatureController.CharacterState.BattleSkill;
                                }
                            }
                            else if (_controller.MonsterSkillSets.Length == 1) // 기본 공격만 존재하는 경우
                            {
                                // 무조건 기본 공격만 시도
                                StartCoroutine("CoNextBehaviorCoolTime", _controller.MonsterSkillSets[0].SkillDelay);
                                _controller.STATE = CreatureController.CharacterState.NomalAttack;
                            }

                        }

                        // 베이스 포지션으로 부터 일정 거리이상 멀어진 경우 AggroLost 상태로 전환
                        if ((_controller.BasePosition - transform.position).magnitude > ReturnBaseRange)
                        {
                            AI_State = AISTATE.AggroLost;
                        }

                    }

                }
                break;
            case AISTATE.AggroLost: // 베이스 포지션으로 부터 지정거리만큼 멀어진 상태

                // 어그로를 잃고 제자리로 돌아간다

                Target = null; // 타겟 초기화
                _controller.STATE = CreatureController.CharacterState.Walk; // 타겟이 null인 상태에서 이동 명령시 베이스 포지션으로 복귀

                if ((_controller.BasePosition - transform.position).magnitude < 0.5f) // 베이스 포지션에 도착했다면
                {
                    // Enter 상태로 들어가고 다시 처음 상태로 복귀
                    AI_State = AISTATE.Enter;
                    _controller.STATE = CreatureController.CharacterState.Idle;
                } 
                break;
            case AISTATE.Exit: // 플레이어가 시야 범위 밖으로 나간 상태

                if (Target != null) // 타겟 초기화
                    Target = null;

                _controller.STATE = CreatureController.CharacterState.Walk; // 타겟이 null인 상태에서 이동 명령시 베이스 포지션으로 복귀

                if ((_controller.BasePosition - transform.position).magnitude < 0.5f) // 베이스 포지션에 도착하였다면
                {
                    AI_State = AISTATE.Enter;
                    _controller.STATE = CreatureController.CharacterState.Walk;
                }

                break;
        }

    }

    // 한번 행동 후 대기 처리를 위한 코루틴
    IEnumerator CoNextBehaviorCoolTime(float coolTime)
    {
        _controller.Inputable = false;
        Debug.Log("스킬 쿨타임 : " + coolTime);

        yield return new WaitForSeconds(coolTime);

        _controller.Inputable = true;
    }


    // -----------------------------------------------------------------------------------








    // --------------------------------- Start & Update ----------------------------------
    void Start()
    {
        Init();
    }

    void Update()
    {
        UpdateAIState();
    }


    // -----------------------------------------------------------------------------------
}
