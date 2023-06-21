using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.Controller
{
    public partial class MyPlayerController : CreatureController
    {
        GameObject _InterActTarget;

        // Ű���� �Է½� �̵� ó��
        void GetKeyboardInput()
        {
            if (Input.anyKey == false)
                return;
            else if (!_inputable)
                return;
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                STATE = CharacterState.Sprint;
            }
            else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                STATE = CharacterState.Walk;
            }
            else if (Input.GetKey(KeyCode.F) && _interactable) // interAction
            {
                
                _InterActTarget.GetComponent<NPCTrigger>().Conversation(); // ��ȭ ����
                
                // ������Ʈ���� ������ ���ֺ����� ����
                // TODO : �ڿ������� ���ƺ����� ���� -> Lerp �̿�

                _InterActTarget.transform.LookAt(transform);
                transform.LookAt(_InterActTarget.transform);

                CamController.setCinemachineAnim("Conversation"); // ī�޶� ��ȭ���� ��ȯ
            }
            else if (Input.GetKey(KeyCode.Q) && _inputable && currentScene == Define.Scene.Game) // ���� ����(�ñر�) ���
            {
                // TEMP : �̱� ����϶� ���� Ȯ�ο�
                CamController.setCinemachineAnim("Ultimate"); // ī�޶� ��ȭ���� ��ȯ
                UltimateBackGround.gameObject.SetActive(true);

                playerAnimator.Play("Ultimate1");





                // TODO : ��Ʈ��ũ�� ������ ��Ŷ ����









            }
            else if (Input.GetKey(KeyCode.E) && _inputable == true && currentScene == Define.Scene.Game) // ���� ���� ��ų ���
            {
                // TEMP : �̱� ����϶� ���� Ȯ�ο�
                playerAnimator.Play("Skill1");





                // TODO : ��Ʈ��ũ�� ������ ��Ŷ ����






            }
            else if (Input.GetKey(KeyCode.P) && !isCoolTime) // ����ġ ȹ�� �׽�Ʈ
            {
                StartCoroutine(getExp());
                





            }


        }

        // TEMP : ����ġ �׽�Ʈ
        bool isCoolTime = false;
        IEnumerator getExp()
        {
            isCoolTime = true;
            GetExp(5);
            yield return new WaitForSeconds(1f);
            isCoolTime = false;
        }


        // Ű���� �Է� �� UI ó��
        void GetUIKeyInput()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (Managers.UI.SCENETYPE == Define.Scene.Lobby1)
                {
                    UI_LobbyScene lobbyScene = Managers.UI.SceneUI as UI_LobbyScene;
                    UI_Inventory invenUI = lobbyScene.InvenUI;
                    UI_Stat statUI = lobbyScene.StatUI;

                    if (invenUI.gameObject.activeSelf) // Inven OFF
                    {
                        // ī�޶� TPS ���� ��ȯ
                        _inputable = true;
                        CamController.setCinemachineAnim("TPS");

                        // �κ��丮 UI ��Ȱ��ȭ
                        invenUI.gameObject.SetActive(false);
                        statUI.gameObject.SetActive(false);
                    }
                    else // Inven ON
                    {
                        // ī�޶� �κ��丮���� ��ȯ
                        _inputable = false;
                        CamController.setCinemachineAnim("Inven");

                        // �κ��丮 UI Ȱ��ȭ
                        invenUI.gameObject.SetActive(true);
                        statUI.gameObject.SetActive(true);
                        invenUI.RefreshUI();
                        statUI.RefreshUI();
                    }
                }

            }
        }

        void CheckAttack()
        {
            if (currentScene != Define.Scene.Game) // ���Ӿ� ������ ���� ����
                return;

            if ((_isMultiPlay == true) && (_coSkillCooltime == null) && Input.GetMouseButtonDown(0)) // ��Ƽ �϶� ���� ó��
            {
                // �ٷ� ���¸� �����ϴ� ���� �ƴ϶� ������ �����ϰڴٴ� ��û�� �켱
                C_Skill skill = new C_Skill() { Info = new SkillInfo() };
                skill.Info.SkillId = 1; // �⺻ ��ų�̶�� ����
                Managers.Network.Send(skill); // ��ų ���� ����

                _coSkillCooltime = StartCoroutine("CoInputColltime", 0.5f); // ��Ÿ���� �����Ͽ� Ŭ���̾�Ʈ���� ������ ��Ŷ�� ������ ����
            }
            else if ((_isMultiPlay == false) && (_coSkillCooltime == null) && Input.GetMouseButtonDown(0)) // �̱� �϶� ���� ó��
            {
                // �̱� �÷��̽� ������ ó���� Ŭ�󿡼� ����Ͽ� ���� �δ��� ���δ�
                // �̱ۿ����� �Ϲ� ���ݿ� ���� ��Ŷ�� ������ �����Ƿ� ��Ŷ�� ��Ÿ���� �ο����� �ʴ´�

                // TEMP : ��Ÿ
                UseSkill(1);
            }
        }

        public void UseSkill(int skillId)
        {
            if (skillId == 1) // �⺻ ����
            {
                StartCoroutine(CoAttack());
            }

            // TODO : ���� ���� �� ���� ���� ��ų�� ���� ����� ���⼭ ó���ؾ���








        }

        // ��ų �ߵ�
        IEnumerator CoAttack()
        {
            _inputable = false;

            slashEffectController.Play();
            playerAnimator.SetFloat("normalAttack", 1);
            STATE = CharacterState.Attack; // ���� ������Ʈ

            yield return new WaitForSeconds(0.4f);

            slashEffectController.Stop();
            playerAnimator.SetFloat("normalAttack", 0);
            STATE = CharacterState.Idle;
        }


        IEnumerator CoKnockBack()
        {
            _inputable = false;

            // �ִϸ��̼� ��� �ð����� �������� ���Ѵ�
            yield return new WaitForSeconds(1.05f);

            STATE = CharacterState.Idle;

            _inputable = true;
        }


        private void OnTriggerEnter(Collider other)
        {

            if (other.gameObject.layer == LayerMask.NameToLayer("Object"))
            {
                Debug.Log($"{other.gameObject.name} Object");

                // ��ȣ�ۿ� UI Ȱ��ȭ
                _interactable = true;
                _InterActTarget = other.gameObject;
                Managers.UI.ShowPopupUI<UI_InteractPopup>();
            }
            

        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Object") && _inputable)
            {
                Debug.Log($"{other.gameObject.name} trigger exit");

                // ��ȣ�ۿ� UI ��Ȱ��ȭ
                _interactable = false;
                Managers.UI.CloseAllPopupUI();
            }

        }

        // �̺�Ʈ ���� ex) ��ȭ, ķ ��ȯ�� �ִ� ��ų ���� ��
        public void OnEndEvent()
        {
            CamController.setCinemachineAnim("TPS");

            // ��ȭ ���Ḧ �˸�
            _InterActTarget.GetComponent<NPCTrigger>().OnEndInterAct();
        }
    }
}
