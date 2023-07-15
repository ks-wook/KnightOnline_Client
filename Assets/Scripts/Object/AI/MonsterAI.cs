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

public class MonsterAI : AIStateMachine
{
    [HideInInspector]
    public GameObject Target = null;

    [HideInInspector]
    MonsterController _controller;


    // -------------------------------- ��ų ���� ���� ------------------------------------
    // �ʿ��� ��� �������� MonsterController ��ũ��Ʈ�κ��� �ڵ����� ���´�.

    float ChaseRange; // �÷��̾���� �Ÿ��� ������ �������� ���� �Ÿ�
    float ReturnBaseRange; // BasePosition���� ���� �ִ�� ������ �� �ִ� �Ÿ�
    int UltimateCount; // UltimateStack ���� �� ���� ���� �� �ñر� ���


    // ------------------------------------------------------------------------------------

    void Init()
    {
        // ��Ʈ�ѷ� ȹ�� �� ������ ������ ���´�.
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

            AI_State = AISTATE.Enter;
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

            AI_State = AISTATE.Exit;
        }
    }

    // -----------------------------------------------------------------------------------





    // ------------------------------------ STATE ----------------------------------------
    public override void UpdateAIState()
    {
        switch(AI_State)
        {
            case AISTATE.Enter: // �÷��̾ �þ� ���� ���� ���� ����
                if(Target != null)
                {
                    if(_controller.Inputable) // ������ �� �ִ� ���¶�� ���� ����
                        _controller.STATE = CreatureController.CharacterState.Walk;

                    if((Target.transform.position - transform.position).magnitude < ChaseRange) // ���� �Ÿ� �̳��� Ÿ���� ���� ���
                    {
                        AI_State = AISTATE.Battle; // Ʈ���Ŵ� �ٽ� ��Ʋ ���·� ���ư���
                    }
                }
                break; 
            case AISTATE.Battle: // �÷��̾ ���� �������� ���ͼ� ������ �õ��� �� �ִ� ����

                if (Target != null)
                {
                    if ((Target.transform.position - transform.position).magnitude > ChaseRange) // ��ǥ���� �Ÿ��� ���� �̻� �־��� ��� ������
                    {
                        AI_State = AISTATE.Enter;
                    }

                    if (((Target.transform.position - transform.position).magnitude < ChaseRange) && _controller.Inputable) // ��ǥ�� �����Ÿ� �̳��� ��� ���� ��
                    {

                        // ���� �ൿ ����
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
                                // ������ �⺻ ���ݸ� �õ�
                                StartCoroutine("CoNextBehaviorCoolTime", _controller.MonsterSkillSets[0].SkillDelay);
                                _controller.STATE = CreatureController.CharacterState.NomalAttack;
                            }

                        }

                        // ���̽� ���������� ���� ���� �Ÿ��̻� �־��� ��� AggroLost ���·� ��ȯ
                        if ((_controller.BasePosition - transform.position).magnitude > ReturnBaseRange)
                        {
                            AI_State = AISTATE.AggroLost;
                        }

                    }

                }
                break;
            case AISTATE.AggroLost: // ���̽� ���������� ���� �����Ÿ���ŭ �־��� ����

                // ��׷θ� �Ұ� ���ڸ��� ���ư���

                Target = null; // Ÿ�� �ʱ�ȭ
                _controller.STATE = CreatureController.CharacterState.Walk; // Ÿ���� null�� ���¿��� �̵� ��ɽ� ���̽� ���������� ����

                if ((_controller.BasePosition - transform.position).magnitude < 0.5f) // ���̽� �����ǿ� �����ߴٸ�
                {
                    // Enter ���·� ���� �ٽ� ó�� ���·� ����
                    AI_State = AISTATE.Enter;
                    _controller.STATE = CreatureController.CharacterState.Idle;
                } 
                break;
            case AISTATE.Exit: // �÷��̾ �þ� ���� ������ ���� ����

                if (Target != null) // Ÿ�� �ʱ�ȭ
                    Target = null;

                _controller.STATE = CreatureController.CharacterState.Walk; // Ÿ���� null�� ���¿��� �̵� ��ɽ� ���̽� ���������� ����

                if ((_controller.BasePosition - transform.position).magnitude < 0.5f) // ���̽� �����ǿ� �����Ͽ��ٸ�
                {
                    AI_State = AISTATE.Enter;
                    _controller.STATE = CreatureController.CharacterState.Walk;
                }

                break;
        }

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
        UpdateAIState();
    }


    // -----------------------------------------------------------------------------------
}
