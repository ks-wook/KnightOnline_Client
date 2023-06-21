using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.Controller
{
    public partial class MyPlayerController : CreatureController
    {
        GameObject _InterActTarget;

        // 키보드 입력시 이동 처리
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

                CamController.setCinemachineAnim("Conversation"); // 카메라 대화시점 전환
            }
            else if (Input.GetKey(KeyCode.Q) && _inputable && currentScene == Define.Scene.Game) // 원소 폭발(궁극기) 사용
            {
                // TEMP : 싱글 모드일때 연출 확인용
                CamController.setCinemachineAnim("Ultimate"); // 카메라 대화시점 전환
                UltimateBackGround.gameObject.SetActive(true);

                playerAnimator.Play("Ultimate1");





                // TODO : 네트워크로 데미지 패킷 전송









            }
            else if (Input.GetKey(KeyCode.E) && _inputable == true && currentScene == Define.Scene.Game) // 원소 전투 스킬 사용
            {
                // TEMP : 싱글 모드일때 연출 확인용
                playerAnimator.Play("Skill1");





                // TODO : 네트워크로 데미지 패킷 전송






            }
            else if (Input.GetKey(KeyCode.P) && !isCoolTime) // 경험치 획득 테스트
            {
                StartCoroutine(getExp());
                





            }


        }

        // TEMP : 경험치 테스트
        bool isCoolTime = false;
        IEnumerator getExp()
        {
            isCoolTime = true;
            GetExp(5);
            yield return new WaitForSeconds(1f);
            isCoolTime = false;
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
                        CamController.setCinemachineAnim("TPS");

                        // 인벤토리 UI 비활성화
                        invenUI.gameObject.SetActive(false);
                        statUI.gameObject.SetActive(false);
                    }
                    else // Inven ON
                    {
                        // 카메라 인벤토리시점 전환
                        _inputable = false;
                        CamController.setCinemachineAnim("Inven");

                        // 인벤토리 UI 활성화
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
            if (currentScene != Define.Scene.Game) // 게임씬 에서만 공격 가능
                return;

            if ((_isMultiPlay == true) && (_coSkillCooltime == null) && Input.GetMouseButtonDown(0)) // 멀티 일때 공격 처리
            {
                // 바로 상태를 변경하는 것이 아니라 서버에 공격하겠다는 요청이 우선
                C_Skill skill = new C_Skill() { Info = new SkillInfo() };
                skill.Info.SkillId = 1; // 기본 스킬이라고 가정
                Managers.Network.Send(skill); // 스킬 정보 전송

                _coSkillCooltime = StartCoroutine("CoInputColltime", 0.5f); // 쿨타임을 지정하여 클라이언트에서 보내는 패킷의 개수를 통제
            }
            else if ((_isMultiPlay == false) && (_coSkillCooltime == null) && Input.GetMouseButtonDown(0)) // 싱글 일때 공격 처리
            {
                // 싱글 플레이시 데미지 처리는 클라에서 담당하여 서버 부담을 줄인다
                // 싱글에서는 일반 공격에 대해 패킷을 보내지 않으므로 패킷의 쿨타임을 부여하지 않는다

                // TEMP : 평타
                UseSkill(1);
            }
        }

        public void UseSkill(int skillId)
        {
            if (skillId == 1) // 기본 공격
            {
                StartCoroutine(CoAttack());
            }

            // TODO : 원소 전투 및 원소 폭발 스킬에 대한 사용을 여기서 처리해야함








        }

        // 스킬 발동
        IEnumerator CoAttack()
        {
            _inputable = false;

            slashEffectController.Play();
            playerAnimator.SetFloat("normalAttack", 1);
            STATE = CharacterState.Attack; // 상태 업데이트

            yield return new WaitForSeconds(0.4f);

            slashEffectController.Stop();
            playerAnimator.SetFloat("normalAttack", 0);
            STATE = CharacterState.Idle;
        }


        IEnumerator CoKnockBack()
        {
            _inputable = false;

            // 애니메이션 재생 시간동안 움직이지 못한다
            yield return new WaitForSeconds(1.05f);

            STATE = CharacterState.Idle;

            _inputable = true;
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
            CamController.setCinemachineAnim("TPS");

            // 대화 종료를 알림
            _InterActTarget.GetComponent<NPCTrigger>().OnEndInterAct();
        }
    }
}
