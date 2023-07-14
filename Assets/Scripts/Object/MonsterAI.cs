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

public class MonsterAI : InteractableObject
{

    public GameObject Target = null;
    MonsterController _controller;


    // -------------------------------- 스킬 관련 변수 ------------------------------------
    float ChaseRange; // 플레이어와의 거리가 벌어져 재추적에 들어가는 거리
    float ReturnBaseRange; // BasePosition으로 부터 최대로 떨어질 수 있는 거리
    int UltimateCount; // UltimateStack 값이 이 값에 도달 시 궁극기 사용


    // ------------------------------------------------------------------------------------

    void Init()
    {
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

            TRIGGER_STATE = TriggerState.Enter;
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

            TRIGGER_STATE = TriggerState.Exit;
        }
    }

    // -----------------------------------------------------------------------------------





    // ------------------------------------ STATE ----------------------------------------
    public override void UpdateTriggerState()
    {
        switch(TRIGGER_STATE)
        {
            case TriggerState.Enter: // Enter 상태인 경우
                if(Target != null)
                {
                    if(_controller.Inputable)
                        _controller.STATE = CreatureController.CharacterState.Walk;

                    if((Target.transform.position - transform.position).magnitude < ChaseRange) // 일정 거리 이내로 타겟이 들어온 경우
                    {
                        TRIGGER_STATE = TriggerState.Battle; // 트리거는 다시 배틀 상태로 돌아간다
                    }
                }
                break; 
            case TriggerState.Battle: // Battle 상태인 경우


                if (Target != null)
                {
                    if ((Target.transform.position - transform.position).magnitude > ChaseRange) // 목표와의 거리가 일정 이상 멀어진 경우 재추적
                    {
                        TRIGGER_STATE = TriggerState.Enter;
                    }

                    if (((Target.transform.position - transform.position).magnitude < ChaseRange) && _controller.Inputable) // 목표가 일정거리 이내로 들어 왔을 때
                    {

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
                                StartCoroutine("CoNextBehaviorCoolTime", _controller.MonsterSkillSets[0].SkillDelay);
                                _controller.STATE = CreatureController.CharacterState.NomalAttack;
                            }

                        }

                        // 베이스 포지션으로 부터 일정 거리이상 멀어진 경우 AggroLost 상태로 전환
                        if ((_controller.BasePosition - transform.position).magnitude > ReturnBaseRange)
                        {
                            TRIGGER_STATE = TriggerState.AggroLost;
                        }

                    }

                }


                break;
            case TriggerState.AggroLost: // 어그로를 잃고 제자리로 돌아가는 상태

                Target = null;
                _controller.STATE = CreatureController.CharacterState.Walk;

                if ((_controller.BasePosition - transform.position).magnitude < 0.5f) // 다시 제자리에 도착했다면
                {
                    // 다시 제자리로 가서 대기
                    TRIGGER_STATE = TriggerState.Enter;
                    _controller.STATE = CreatureController.CharacterState.Idle;
                }
                

                break;
            case TriggerState.Exit: // Exit 상태인 경우

                _controller.STATE = CreatureController.CharacterState.Walk;

                if ((_controller.BasePosition - transform.position).magnitude < 0.5f)
                {
                    TRIGGER_STATE = TriggerState.Enter;
                    _controller.STATE = CreatureController.CharacterState.Walk;
                }

                break;
        }

        // Debug.Log("트리거 상태 : " + TRIGGER_STATE);

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
        UpdateTriggerState();
    }

    public override void InterAct()
    {
        throw new NotImplementedException();
    }
    // -----------------------------------------------------------------------------------
}
