using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.Controller
{
    public partial class MyPlayerController : CreatureController
    {
        GameObject _InterActTarget;

        // Ű���� �Է� ó��
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

                _camController.setCinemachineAnim("Conversation"); // ī�޶� ��ȭ���� ��ȯ
            }

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
                        _camController.setCinemachineAnim("TPS");

                        // �κ��丮 UI ��Ȱ��ȭ
                        invenUI.gameObject.SetActive(false);
                        statUI.gameObject.SetActive(false);
                    }
                    else // Inven ON
                    {
                        // ī�޶� �κ��丮���� ��ȯ
                        _inputable = false;
                        _camController.setCinemachineAnim("Inven");

                        // �κ��丮 UI Ȱ��ȭ
                        invenUI.gameObject.SetActive(true);
                        statUI.gameObject.SetActive(true);
                        invenUI.RefreshUI();
                        statUI.RefreshUI();
                    }
                }

            }
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
            _camController.setCinemachineAnim("TPS");

            // ��ȭ ���Ḧ �˸�
            _InterActTarget.GetComponent<NPCTrigger>().OnEndInterAct();
        }
    }
}
