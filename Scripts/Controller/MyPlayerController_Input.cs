using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.Controller
{
    public partial class MyPlayerController : CreatureController
    {
        GameObject _InterActTarget;

        // 키보드 입력 처리
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
                
                _InterActTarget.GetComponent<NPCTrigger>().Conversation(); // 대화 시작
                
                // 오브젝트들의 방향이 마주보도록 설정
                // TODO : 자연스럽게 돌아보도록 설정 -> Lerp 이용

                _InterActTarget.transform.LookAt(transform);
                transform.LookAt(_InterActTarget.transform);

                _camController.setCinemachineAnim("Conversation"); // 카메라 대화시점 전환
            }

        }


        // 키보드 입력 시 UI 처리
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
                        // 카메라 TPS 시점 전환
                        _inputable = true;
                        _camController.setCinemachineAnim("TPS");

                        // 인벤토리 UI 비활성화
                        invenUI.gameObject.SetActive(false);
                        statUI.gameObject.SetActive(false);
                    }
                    else // Inven ON
                    {
                        // 카메라 인벤토리시점 전환
                        _inputable = false;
                        _camController.setCinemachineAnim("Inven");

                        // 인벤토리 UI 활성화
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

                // 상호작용 UI 활성화
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

                // 상호작용 UI 비활성화
                _interactable = false;
                Managers.UI.CloseAllPopupUI();
            }

        }

        // 이벤트 종료 ex) 대화, 캠 전환이 있는 스킬 연출 등
        public void OnEndEvent()
        {
            _camController.setCinemachineAnim("TPS");

            // 대화 종료를 알림
            _InterActTarget.GetComponent<NPCTrigger>().OnEndInterAct();
        }
    }
}
