using Data;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * �÷��̾��� �Է� ó���� ���õ� �ڵ尡 �ۼ��� ��ũ��Ʈ
 * MyPlayerController ��ũ��Ʈ�� ���̰� ��� ���� �и�
 */


namespace Assets.Scripts.Controller
{
    public partial class MyPlayerController : CreatureController
    {
        // ------------------- ��ȣ�ۿ�� ���õ� ���� --------------------
        GameObject _InterActTarget; // ��ȣ�ۿ� ������ ������Ʈ ĳ��

        // ---------------------------------------------------------------


        // -------------------- ���ݰ� ���õ� ���� -----------------------

        Collider[] targetColliders; // �÷��̾� ������ ������Ʈ
        MonsterController hittedMonster; // ������ ������ ������ ��Ʈ�ѷ�

        Vector2 targetDir;
        int monsterLayer; // ���� ���̾� ��

        [Header("Skill Info")]

        [SerializeField]
        [Tooltip("ĳ���� �⺻���� ��ų ID")]
        int NormalAttackId;
        Skill NormalAttackInfo;

        [SerializeField]
        [Tooltip("ĳ���� ���� ��ų ��ų ID")]
        int BattleSkillId;
        Skill BattleSkillInfo;

        [SerializeField]
        [Tooltip("ĳ���� �ñر� ��ų ID")]
        int UltimateId;
        Skill UltimateInfo;


        // �ִϸ��̼� ����ð����ȿ� ���� �ִϸ��̼� ��� ��Ÿ��
        [SerializeField]
        [Tooltip("�⺻ ���ݿ� ���� �ִϸ��̼� ������� �����Ǵ� �ð�")]
        float NomalAttackAnimCool = 0.5f;

        [SerializeField]
        [Tooltip("���� ��ų�� ���� �ִϸ��̼� ������� �����Ǵ� �ð�")]
        float BattleSkillAnimCool = 1.1f;

        [SerializeField]
        [Tooltip("�⺻ ���ݿ� ���� �ִϸ��̼� ������� �����Ǵ� �ð�")]
        float UltimateAnimCool = 5.0f;

        WaitForSeconds _nomalAttackAnimCool;
        WaitForSeconds _battleSkillAnimCool;
        WaitForSeconds _ultimateAnimCool;

        // ---------------------------------------------------------------




        // --------------------- Ű���� �Է½� ó�� ----------------------
        void GetKeyboardInput()
        {
            if (Input.anyKey == false)
                return;
            else if (!_inputable)
                return;
            else if (Input.GetKey(KeyCode.LeftShift)) // �޸��� ����
            {
                STATE = CharacterState.Sprint;
            }
            else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) // �ȱ� ����
            {
                STATE = CharacterState.Walk;
            }
            else if (Input.GetKey(KeyCode.F) && _interactable) // InteractableObject ��ũ��Ʈ�� �����ϴ� ������Ʈ�� ��ȣ�ۿ�
            {
                _interactable = false;

                if (_InterActTarget != null)
                {
                    InteractableObject InterActor =
                        _InterActTarget.GetComponent<InteractableObject>();

                    InterActor.InterAct();
                }
            }
            else if (Input.GetKey(KeyCode.LeftAlt) && !isCoolTime) // ���콺 Ŀ�� ǥ��
            {
                StartCoroutine(CursorOn());
            }


            
        }

        
        // Ű���� �Է� �� UI ó��
        void GetUIKeyInput()
        {
            if (Input.GetKeyDown(KeyCode.I)) // �κ�â
            {
                if (Managers.UI.SCENETYPE == Define.Scene.Lobby1) // ���� �κ�� �϶��� �κ��� �� �� �ִ�
                {
                    UI_LobbyScene lobbyScene = Managers.UI.SceneUI as UI_LobbyScene;
                    UI_Inventory invenUI = lobbyScene.InvenUI;
                    UI_Stat statUI = lobbyScene.StatUI;

                    if (invenUI.gameObject.activeSelf) // Inven OFF
                    {
                        // ī�޶� TPS ���� ��ȯ
                        _inputable = true;
                        CinemachineController.STATE = 
                            CinemachineController.CamState.TPS;

                        // �κ��丮 UI ��Ȱ��ȭ
                        invenUI.gameObject.SetActive(false);
                        statUI.gameObject.SetActive(false);
                    }
                    else // Inven ON
                    {
                        // ī�޶� �κ��丮���� ��ȯ
                        _inputable = false;
                        CinemachineController.STATE = 
                            CinemachineController.CamState.Inven;

                        // �κ��丮 UI Ȱ��ȭ
                        invenUI.gameObject.SetActive(true);
                        statUI.gameObject.SetActive(true);
                        invenUI.RefreshUI();
                        statUI.RefreshUI();
                    }
                }

            }
            else if(Input.GetKeyDown(KeyCode.Escape)) // ����â
            {
                if(Managers.UI.ContainPopupUI<UI_SettingsPopup>() == false)
                {
                    // ����â UI Ȱ��ȭ
                    Managers.UI.ShowPopupUI<UI_SettingsPopup>();

                    CinemachineController.STATE = 
                        CinemachineController.CamState.Settings;
                }

            }
        }

        

        // �̺�Ʈ ����ó�� �Լ� ex) ��ȭ, ķ ��ȯ�� �ִ� ��ų ���� ��
        public void OnEndConversation()
        {
            // ��ȭ ���Ḧ �˸�
            _InterActTarget.GetComponent<NPCObject>().OnEndConversation();
        }



        // -------------------------- �÷��̾� ���ݰ� ���õ� ó�� -------------------------------

        // ������ �õ� �� �� �ִ� �� Ȯ�� �� ������ �õ��ϴ� �Լ�
        void CheckAttack()
        {
            if (currentScene != Define.Scene.Game) // ���Ӿ� ������ ���� ����
                return;

            // ------------------------------ �⺻ ���� ------------------------------------
            if (Input.GetMouseButtonDown(0))
            {
                if ((_isMultiPlay == true) && (CoSkillPacketCooltime == null)) // ��Ƽ �϶� ���� ó��
                {
                    Debug.Log("1 ��ų ��� ��û");

                    // �ٷ� ���¸� �����ϴ� ���� �ƴ϶� ������ �����ϰڴٴ� ��û�� �켱
                    C_Skill skill = new C_Skill() { Info = new SkillInfo() };
                    skill.Info.SkillId = 1; // �⺻ ��ų�̶�� ����
                    Managers.Network.Send(skill); // ��ų ���� ����

                    CoSkillPacketCooltime = StartCoroutine("CoInputColltime", 0.5f); // ��Ÿ���� �����Ͽ� Ŭ���̾�Ʈ���� ������ ��Ŷ�� ������ ����
                    
                }
                else if ((_isMultiPlay == false) && (CoSkillPacketCooltime == null)) // �̱� �϶� ���� ó��
                {
                    // �̱� �÷��̽� ������ ó���� Ŭ�󿡼� ����Ͽ� ���� �δ��� ���δ�
                    // �̱ۿ����� ���ݿ� ���� ��Ŷ�� ������ �����Ƿ� ��Ŷ�� ��Ÿ���� �ο����� �ʴ´�

                    UseSkill(1);
                }
            }
            // ------------------------------ ���� ��ų ------------------------------------
            else if (Input.GetKey(KeyCode.E) && EnableBattleSkill)
            {
                if ((_isMultiPlay == true) && (CoSkillPacketCooltime == null)) // ��Ƽ �϶� ���� ��ų ó��
                {
                    // �ٷ� ���¸� �����ϴ� ���� �ƴ϶� ������ �����ϰڴٴ� ��û�� �켱
                    C_Skill skill = new C_Skill() { Info = new SkillInfo() };
                    skill.Info.SkillId = 2; // ���� ��ų
                    Managers.Network.Send(skill); // ��ų ���� ����

                    CoSkillPacketCooltime = StartCoroutine("CoInputColltime", 0.5f); // ��Ÿ���� �����Ͽ� Ŭ���̾�Ʈ���� ������ ��Ŷ�� ������ ����
                }
                else if ((_isMultiPlay == false) && (CoSkillPacketCooltime == null)) // �̱� �϶� ���� ��ų ó��
                {
                    UseSkill(2);
                }
            }
            // ----------------------------- �ñر� ��ų -----------------------------------
            if(Input.GetKey(KeyCode.Q) && EnableUltimate)
            {
                if ((_isMultiPlay == true) && (CoSkillPacketCooltime == null)) // ��Ƽ �϶� ���� ��ų ó��
                {
                    // �ٷ� ���¸� �����ϴ� ���� �ƴ϶� ������ �����ϰڴٴ� ��û�� �켱
                    C_Skill skill = new C_Skill() { Info = new SkillInfo() };
                    skill.Info.SkillId = 3; // �ñر�
                    Managers.Network.Send(skill); // ��ų ���� ����

                    CoSkillPacketCooltime = StartCoroutine("CoInputColltime", 0.5f); // ��Ÿ���� �����Ͽ� Ŭ���̾�Ʈ���� ������ ��Ŷ�� ������ ����
                }
                else if ((_isMultiPlay == false) && (CoSkillPacketCooltime == null)) // �̱� �϶� ���� ��ų ó��
                {
                    UseSkill(3);
                }
            }
        }

        // � ������ �õ��� �� �����ϴ� �Լ�
        public void UseSkill(int skillId)
        {
            // ������ �õ��ϱ� ���� �÷��̾� �ֺ� ���� ���� ���� ������Ʈ ����� �� �������� ���� �õ�
            targetColliders = Physics.OverlapSphere(transform.position + Vector3.up, 3.0f, monsterLayer);

            if (targetColliders.Length != 0) // �ֺ� �������� Ÿ�� ������ ������Ʈ�� �ִٸ�
            {
                // �÷��̾��� ������ ��ǥ �������� ������.
                transform.LookAt(targetColliders[0].transform.position);
            }

            // �÷��̾� ������ Ÿ�� ������ ������Ʈ�� ���ٸ� �ٶ󺸴� �������� �״�� ������ �����Ѵ�

            if (skillId == 1) // �⺻ ����
            {
                STATE = CharacterState.NomalAttack;
            }
            else if (skillId == 2) // ���� ��ų
            {
                STATE = CharacterState.BattleSkill;
            }
            else if (skillId == 3) // �ñر� ��ų
            {
                STATE = CharacterState.Ultimate;
            }

        }


        // ���� �� ���� �ð� �ٸ� Ŀ�ǵ� �Է� �Ұ����ϰ� ó���ϴ� �ڷ�ƾ
        IEnumerator CoAttack(int skillId)
        {
            _inputable = false;

            if(skillId == 1) // �Ϲ� ����
            {
                Debug.Log("�Ϲ� ���� ����");

                SlashEffectController.Play();
                PlayerAnimator.SetFloat("normalAttack", 1);

                yield return _nomalAttackAnimCool;

                SlashEffectController.Stop();
                PlayerAnimator.SetFloat("normalAttack", 0);
                STATE = CharacterState.Idle;
            }
            else if (skillId == 2) // ���� ��ų
            {
                Debug.Log("���� ��ų ����");

                PlayerAnimator.Play("BattleSkill");
                BattleSkillEffectController.gameObject.SetActive(true);

                yield return _battleSkillAnimCool;

                STATE = CharacterState.Idle;
            }
            else if (skillId == 3) // �ñر�
            {
                Debug.Log("�ñر� ����");

                // ī�޶� �ñر� ���� ���� ��ȯ
                CinemachineController.STATE = 
                    CinemachineController.CamState.Ultimate;
                UltimateBackGround.gameObject.SetActive(true);

                PlayerAnimator.Play("Ultimate");

                yield return _ultimateAnimCool;

                STATE = CharacterState.Idle;
            }
        }


        // �˹� �� ó��
        IEnumerator CoKnockBack()
        {
            _inputable = false;

            // �ִϸ��̼� ��� �ð����� �������� ���Ѵ�
            yield return new WaitForSeconds(1.05f);

            STATE = CharacterState.Idle;

            _inputable = true;
        }


        // ���� �õ� �� ������ ���鿡 ���� ������ ���
        public void HandleDamage(int skillId, Collider[] hitColliders)
        {
            if(skillId > 0)
            {
                foreach (Collider monsterCollider in hitColliders)
                {
                    if (monsterCollider.TryGetComponent<MonsterController>(out hittedMonster))
                    {
                        // ����������� �÷��̾� ���ݷ� * ��ų ��� * 0.8 ~ 1.2 ������ ���� �� -> ���� ������ �ݿø�


                        int damage = Mathf.RoundToInt(Random.Range(0.8f, 1.2f) * TotalDamage * SkillDamages[0]);
                        if (_isMultiPlay) // ��Ƽ �÷����� ���
                        {
                            // ������ ó�� ��û ��Ŷ�� ����
                            C_BossStatChange bossStatChange = new C_BossStatChange()
                            {
                                Damage = damage,
                            };

                            Managers.Network.Send(bossStatChange);
                        }
                        else // �̱� �÷����� ���
                        {
                            // Ŭ���̾�Ʈ ������ ��ü ������ ����
                            hittedMonster.GetDamage(damage);
                        }

                    }

                }

            }
        
        }

        public void GetDamage(int damage, bool isKnockback = false)
        {
            Debug.Log("Player Get Damage : " + damage);

            if (_isMultiPlay) // ��Ƽ �÷����� ��� hp ���ҿ� ���� ��Ŷ�� ������ �����ؾ���
            {
                C_ChangeHp getDamagePacket = new C_ChangeHp()
                {
                    Hp = HP - damage,
                };

                Managers.Network.Send(getDamagePacket);
            }
            else // �̱� �÷����� ��� Ŭ�󿡼� ������ ó��
            {
                HP = HP - damage;
            }

        }

        // --------------------------------------------------------------------------------------








        // -------------------------------- ������Ʈ ��ȣ�ۿ� -----------------------------------
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("InteractableObject"))
            {
                Debug.Log($"{other.gameObject.name} Object");

                _interactable = true;

                _InterActTarget = other.gameObject; // ��ȣ�ۿ� ������Ʈ ����

                Managers.UI.ShowPopupUI<UI_InteractPopup>(); // ��ȣ�ۿ� UI Ȱ��ȭ
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("InteractableObject"))
            {
                Debug.Log($"{other.gameObject.name} trigger exit");

                // ��ȣ�ۿ� UI ��Ȱ��ȭ
                _interactable = false;

                Managers.UI.CloseAllPopupUI();
            }


        }
        // -------------------------------------------------------------------------------------










        // ============================== ���� �׽�Ʈ �� �ڵ� ===================================

        private void OnDrawGizmos()
        {

            // �÷��̾� ���� Ÿ�� ��ĵ ����
            /*Color c = new Color(1f, 1f, 1f, 0.5f);
            Gizmos.color = c;
            Gizmos.DrawSphere(transform.position + Vector3.up, 3.0f);*/

            // �⺻ ���� ���� Ȯ��
            /*{
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position + (transform.forward * 1), 1.5f);
            }*/

            // �ñر� ���� ���� Ȯ��
            /*{
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(transform.position + (transform.forward * 2.0f), 3.0f);
            }*/
        }



        // TEMP : ����ġ �׽�Ʈ
        bool isCoolTime = false;
        IEnumerator CursorOn()
        {
            isCoolTime = true;
            if (Cursor.lockState == CursorLockMode.Confined)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                CinemachineController.STATE = CinemachineController.CamState.Settings;
            }
            else
            {
                CinemachineController.STATE = CinemachineController.CamState.TPS;
            }
            yield return new WaitForSeconds(1f);
            isCoolTime = false;

            
        }

        int quest = 1;
        IEnumerator test()
        {
            isCoolTime = true;

            C_QuestChange questChange = new C_QuestChange();
            questChange.QuestTemplatedId = quest++;
            questChange.IsCleared = true;
            questChange.IsRewarded = true; // ���� ȹ�� ó�� ��û
            Managers.Network.Send(questChange);
            yield return new WaitForSeconds(1f);

            isCoolTime = false;
        }
    }
}
