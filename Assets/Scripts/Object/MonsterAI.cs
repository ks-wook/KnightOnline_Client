using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * �⺻ ������ �þ� ������ �����ϰ�, ���� �������� Player �±׸� �ް��ִ� ������Ʈ�� ���� �� 
 * Ÿ���� �����ϰ� �ش� Ÿ���� �����ϴ� ����� �ϴ� ��ũ��Ʈ
 * 
 * �÷��̾ ����ó�� FSM ���� �����Ͽ� Ʈ������ ���¸� STATE��� ������Ƽ�� ��Ÿ��
*/

public class MonsterAI : InteractableObject
{

    public GameObject Target = null;
    MonsterController _controller;


    // -------------------------------- ��ų ���� ���� ------------------------------------
    float ChaseRange; // �÷��̾���� �Ÿ��� ������ �������� ���� �Ÿ�
    float ReturnBaseRange; // BasePosition���� ���� �ִ�� ������ �� �ִ� �Ÿ�
    int UltimateCount; // UltimateStack ���� �� ���� ���� �� �ñر� ���


    // ------------------------------------------------------------------------------------

    void Init()
    {
        _controller = transform.GetComponentInParent<MonsterController>();
        if (_controller == null)
        {
            Debug.Log("VisualRangeTrigger �ʱ�ȭ ���� : ���� ��Ʈ�ѷ� ��ũ��Ʈ �˻� �Ұ���");
        }
        else
        {
            ChaseRange = _controller.ChaseRange;
            ReturnBaseRange = _controller.ReturnBaseRange;
            UltimateCount = _controller.UltimateCount;
        }

    }

    // -------------------------------- EventHandler ------------------------------------
    // Ʈ���� �����ȿ� �÷��̾ ���� ���
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.tag == "Player")
        {
            Debug.Log("�÷��̾� ���� ����");

            Target = other.gameObject;

            TRIGGER_STATE = TriggerState.Enter;
        }
    }

    // Ʈ���� ���� ������ ��ǥ�� ���� ���
    override protected void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);


        if (other.tag == "Player")
        {
            if (Target != null)
                Target = null;

            Debug.Log("�÷��̾� ���� ����");

            TRIGGER_STATE = TriggerState.Exit;
        }
    }

    // -----------------------------------------------------------------------------------





    // ------------------------------------ STATE ----------------------------------------
    public override void UpdateTriggerState()
    {
        switch(TRIGGER_STATE)
        {
            case TriggerState.Enter: // Enter ������ ���
                if(Target != null)
                {
                    if(_controller.Inputable)
                        _controller.STATE = CreatureController.CharacterState.Walk;

                    if((Target.transform.position - transform.position).magnitude < ChaseRange) // ���� �Ÿ� �̳��� Ÿ���� ���� ���
                    {
                        TRIGGER_STATE = TriggerState.Battle; // Ʈ���Ŵ� �ٽ� ��Ʋ ���·� ���ư���
                    }
                }
                break; 
            case TriggerState.Battle: // Battle ������ ���


                if (Target != null)
                {
                    if ((Target.transform.position - transform.position).magnitude > ChaseRange) // ��ǥ���� �Ÿ��� ���� �̻� �־��� ��� ������
                    {
                        TRIGGER_STATE = TriggerState.Enter;
                    }

                    if (((Target.transform.position - transform.position).magnitude < ChaseRange) && _controller.Inputable) // ��ǥ�� �����Ÿ� �̳��� ��� ���� ��
                    {

                        int behavior = UnityEngine.Random.Range(1, 10);

                        if (behavior < 3) // 20% Ȯ���� ������ ���ְų�
                        {
                            _controller.STATE = CreatureController.CharacterState.Idle;
                            StartCoroutine("CoNextBehaviorCoolTime", 2.0f);

                        }
                        else // ������ �õ��Ѵ�
                        {
                            Debug.Log(behavior);

                            if (_controller.MonsterSkillSets.Length == 3) // ������ ���� 3���� ����� ���� ���
                            {
                                Debug.Log("�ñر� ���� : " + _controller._ultimateStack + "/" + UltimateCount);
                                if (_controller._ultimateStack >= UltimateCount)
                                {
                                    StartCoroutine("CoNextBehaviorCoolTime", _controller.MonsterSkillSets[2].SkillDelay);
                                    _controller.STATE = CreatureController.CharacterState.Ultimate;
                                    _controller._ultimateStack = 0;
                                
                                }
                                else if ((behavior > 2) && (behavior < 6)) // �⺻ ���� �õ�
                                {
                                    StartCoroutine("CoNextBehaviorCoolTime", _controller.MonsterSkillSets[0].SkillDelay);
                                    _controller.STATE = CreatureController.CharacterState.NomalAttack;                                    
                                }
                                else if (behavior > 5 && (behavior < 10)) // ��ų ���� �õ�
                                {
                                    StartCoroutine("CoNextBehaviorCoolTime", _controller.MonsterSkillSets[1].SkillDelay);
                                    _controller.STATE = CreatureController.CharacterState.BattleSkill;
                                }
                            }
                            else if (_controller.MonsterSkillSets.Length == 1) // �⺻ ���ݸ� �����ϴ� ���
                            {
                                StartCoroutine("CoNextBehaviorCoolTime", _controller.MonsterSkillSets[0].SkillDelay);
                                _controller.STATE = CreatureController.CharacterState.NomalAttack;
                            }

                        }

                        // ���̽� ���������� ���� ���� �Ÿ��̻� �־��� ��� AggroLost ���·� ��ȯ
                        if ((_controller.BasePosition - transform.position).magnitude > ReturnBaseRange)
                        {
                            TRIGGER_STATE = TriggerState.AggroLost;
                        }

                    }

                }


                break;
            case TriggerState.AggroLost: // ��׷θ� �Ұ� ���ڸ��� ���ư��� ����

                Target = null;
                _controller.STATE = CreatureController.CharacterState.Walk;

                if ((_controller.BasePosition - transform.position).magnitude < 0.5f) // �ٽ� ���ڸ��� �����ߴٸ�
                {
                    // �ٽ� ���ڸ��� ���� ���
                    TRIGGER_STATE = TriggerState.Enter;
                    _controller.STATE = CreatureController.CharacterState.Idle;
                }
                

                break;
            case TriggerState.Exit: // Exit ������ ���

                _controller.STATE = CreatureController.CharacterState.Walk;

                if ((_controller.BasePosition - transform.position).magnitude < 0.5f)
                {
                    TRIGGER_STATE = TriggerState.Enter;
                    _controller.STATE = CreatureController.CharacterState.Walk;
                }

                break;
        }

        // Debug.Log("Ʈ���� ���� : " + TRIGGER_STATE);

    }

    // �ѹ� �ൿ �� ��� ó���� ���� �ڷ�ƾ
    IEnumerator CoNextBehaviorCoolTime(float coolTime)
    {
        _controller.Inputable = false;
        Debug.Log("��ų ��Ÿ�� : " + coolTime);

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
