using Data;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 플레이어의 입력 처리와 관련된 코드가 작성된 스크립트
 * MyPlayerController 스크립트의 길이가 길어 따로 분리
 */


namespace Assets.Scripts.Controller
{
    public partial class MyPlayerController : CreatureController
    {
        // ------------------- 상호작용과 관련된 변수 --------------------
        GameObject _InterActTarget; // 상호작용 가능한 오브젝트 캐싱

        // ---------------------------------------------------------------


        // -------------------- 공격과 관련된 변수 -----------------------

        Collider[] targetColliders; // 플레이어 주위의 오브젝트
        MonsterController hittedMonster; // 공격이 명중한 몬스터의 컨트롤러

        Vector2 targetDir;
        int monsterLayer; // 몬스터 레이어 값

        [Header("Skill Info")]

        [SerializeField]
        [Tooltip("캐릭터 기본공격 스킬 ID")]
        int NormalAttackId;
        Skill NormalAttackInfo;

        [SerializeField]
        [Tooltip("캐릭터 전투 스킬 스킬 ID")]
        int BattleSkillId;
        Skill BattleSkillInfo;

        [SerializeField]
        [Tooltip("캐릭터 궁극기 스킬 ID")]
        int UltimateId;
        Skill UltimateInfo;


        // 애니메이션 재생시간동안에 의한 애니메이션 재생 쿨타임
        [SerializeField]
        [Tooltip("기본 공격에 대한 애니메이션 재생동안 경직되는 시간")]
        float NomalAttackAnimCool = 0.5f;

        [SerializeField]
        [Tooltip("전투 스킬에 대한 애니메이션 재생동안 경직되는 시간")]
        float BattleSkillAnimCool = 1.1f;

        [SerializeField]
        [Tooltip("기본 공격에 대한 애니메이션 재생동안 경직되는 시간")]
        float UltimateAnimCool = 5.0f;

        WaitForSeconds _nomalAttackAnimCool;
        WaitForSeconds _battleSkillAnimCool;
        WaitForSeconds _ultimateAnimCool;

        // ---------------------------------------------------------------




        // --------------------- 키보드 입력시 처리 ----------------------
        void GetKeyboardInput()
        {
            if (Input.anyKey == false)
                return;
            else if (!_inputable)
                return;
            else if (Input.GetKey(KeyCode.LeftShift)) // 달리기 상태
            {
                STATE = CharacterState.Sprint;
            }
            else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) // 걷기 상태
            {
                STATE = CharacterState.Walk;
            }
            else if (Input.GetKey(KeyCode.F) && _interactable) // InteractableObject 스크립트를 포함하는 오브젝트와 상호작용
            {
                _interactable = false;

                if (_InterActTarget != null)
                {
                    InteractableObject InterActor =
                        _InterActTarget.GetComponent<InteractableObject>();

                    InterActor.InterAct();
                }
            }
            else if (Input.GetKey(KeyCode.LeftAlt) && !isCoolTime) // 마우스 커서 표시
            {
                StartCoroutine(CursorOn());
            }


            
        }

        
        // 키보드 입력 시 UI 처리
        void GetUIKeyInput()
        {
            if (Input.GetKeyDown(KeyCode.I)) // 인벤창
            {
                if (Managers.UI.SCENETYPE == Define.Scene.Lobby1) // 현재 로비씬 일때만 인벤을 열 수 있다
                {
                    UI_LobbyScene lobbyScene = Managers.UI.SceneUI as UI_LobbyScene;
                    UI_Inventory invenUI = lobbyScene.InvenUI;
                    UI_Stat statUI = lobbyScene.StatUI;

                    if (invenUI.gameObject.activeSelf) // Inven OFF
                    {
                        // 카메라 TPS 시점 전환
                        _inputable = true;
                        CinemachineController.STATE = 
                            CinemachineController.CamState.TPS;

                        // 인벤토리 UI 비활성화
                        invenUI.gameObject.SetActive(false);
                        statUI.gameObject.SetActive(false);
                    }
                    else // Inven ON
                    {
                        // 카메라 인벤토리시점 전환
                        _inputable = false;
                        CinemachineController.STATE = 
                            CinemachineController.CamState.Inven;

                        // 인벤토리 UI 활성화
                        invenUI.gameObject.SetActive(true);
                        statUI.gameObject.SetActive(true);
                        invenUI.RefreshUI();
                        statUI.RefreshUI();
                    }
                }

            }
            else if(Input.GetKeyDown(KeyCode.Escape)) // 설정창
            {
                if(Managers.UI.ContainPopupUI<UI_SettingsPopup>() == false)
                {
                    // 설정창 UI 활성화
                    Managers.UI.ShowPopupUI<UI_SettingsPopup>();

                    CinemachineController.STATE = 
                        CinemachineController.CamState.Settings;
                }

            }
        }

        

        // 이벤트 종료처리 함수 ex) 대화, 캠 전환이 있는 스킬 연출 등
        public void OnEndConversation()
        {
            // 대화 종료를 알림
            _InterActTarget.GetComponent<NPCObject>().OnEndConversation();
        }



        // -------------------------- 플레이어 공격과 관련된 처리 -------------------------------

        // 공격을 시도 할 수 있는 지 확인 후 공격을 시도하는 함수
        void CheckAttack()
        {
            if (currentScene != Define.Scene.Game) // 게임씬 에서만 공격 가능
                return;

            // ------------------------------ 기본 공격 ------------------------------------
            if (Input.GetMouseButtonDown(0))
            {
                if ((_isMultiPlay == true) && (CoSkillPacketCooltime == null)) // 멀티 일때 공격 처리
                {
                    Debug.Log("1 스킬 사용 요청");

                    // 바로 상태를 변경하는 것이 아니라 서버에 공격하겠다는 요청이 우선
                    C_Skill skill = new C_Skill() { Info = new SkillInfo() };
                    skill.Info.SkillId = 1; // 기본 스킬이라고 가정
                    Managers.Network.Send(skill); // 스킬 정보 전송

                    CoSkillPacketCooltime = StartCoroutine("CoInputColltime", 0.5f); // 쿨타임을 지정하여 클라이언트에서 보내는 패킷의 개수를 통제
                    
                }
                else if ((_isMultiPlay == false) && (CoSkillPacketCooltime == null)) // 싱글 일때 공격 처리
                {
                    // 싱글 플레이시 데미지 처리는 클라에서 담당하여 서버 부담을 줄인다
                    // 싱글에서는 공격에 대해 패킷을 보내지 않으므로 패킷의 쿨타임을 부여하지 않는다

                    UseSkill(1);
                }
            }
            // ------------------------------ 전투 스킬 ------------------------------------
            else if (Input.GetKey(KeyCode.E) && EnableBattleSkill)
            {
                if ((_isMultiPlay == true) && (CoSkillPacketCooltime == null)) // 멀티 일때 전투 스킬 처리
                {
                    // 바로 상태를 변경하는 것이 아니라 서버에 공격하겠다는 요청이 우선
                    C_Skill skill = new C_Skill() { Info = new SkillInfo() };
                    skill.Info.SkillId = 2; // 전투 스킬
                    Managers.Network.Send(skill); // 스킬 정보 전송

                    CoSkillPacketCooltime = StartCoroutine("CoInputColltime", 0.5f); // 쿨타임을 지정하여 클라이언트에서 보내는 패킷의 개수를 통제
                }
                else if ((_isMultiPlay == false) && (CoSkillPacketCooltime == null)) // 싱글 일때 전투 스킬 처리
                {
                    UseSkill(2);
                }
            }
            // ----------------------------- 궁극기 스킬 -----------------------------------
            if(Input.GetKey(KeyCode.Q) && EnableUltimate)
            {
                if ((_isMultiPlay == true) && (CoSkillPacketCooltime == null)) // 멀티 일때 전투 스킬 처리
                {
                    // 바로 상태를 변경하는 것이 아니라 서버에 공격하겠다는 요청이 우선
                    C_Skill skill = new C_Skill() { Info = new SkillInfo() };
                    skill.Info.SkillId = 3; // 궁극기
                    Managers.Network.Send(skill); // 스킬 정보 전송

                    CoSkillPacketCooltime = StartCoroutine("CoInputColltime", 0.5f); // 쿨타임을 지정하여 클라이언트에서 보내는 패킷의 개수를 통제
                }
                else if ((_isMultiPlay == false) && (CoSkillPacketCooltime == null)) // 싱글 일때 전투 스킬 처리
                {
                    UseSkill(3);
                }
            }
        }

        // 어떤 공격을 시도할 지 결정하는 함수
        public void UseSkill(int skillId)
        {
            // 공격을 시도하기 전에 플레이어 주변 범위 내에 몬스터 오브젝트 존재시 그 방향으로 공격 시도
            targetColliders = Physics.OverlapSphere(transform.position + Vector3.up, 3.0f, monsterLayer);

            if (targetColliders.Length != 0) // 주변 범위내에 타격 가능한 오브젝트가 있다면
            {
                // 플레이어의 방향을 목표 방향으로 돌린다.
                transform.LookAt(targetColliders[0].transform.position);
            }

            // 플레이어 주위에 타격 가능한 오브젝트가 없다면 바라보는 방향으로 그대로 공격을 시전한다

            if (skillId == 1) // 기본 공격
            {
                STATE = CharacterState.NomalAttack;
            }
            else if (skillId == 2) // 전투 스킬
            {
                STATE = CharacterState.BattleSkill;
            }
            else if (skillId == 3) // 궁극기 스킬
            {
                STATE = CharacterState.Ultimate;
            }

        }


        // 공격 시 일정 시간 다른 커맨드 입력 불가능하게 처리하는 코루틴
        IEnumerator CoAttack(int skillId)
        {
            _inputable = false;

            if(skillId == 1) // 일반 공격
            {
                Debug.Log("일반 공격 실행");

                SlashEffectController.Play();
                PlayerAnimator.SetFloat("normalAttack", 1);

                yield return _nomalAttackAnimCool;

                SlashEffectController.Stop();
                PlayerAnimator.SetFloat("normalAttack", 0);
                STATE = CharacterState.Idle;
            }
            else if (skillId == 2) // 전투 스킬
            {
                Debug.Log("전투 스킬 실행");

                PlayerAnimator.Play("BattleSkill");
                BattleSkillEffectController.gameObject.SetActive(true);

                yield return _battleSkillAnimCool;

                STATE = CharacterState.Idle;
            }
            else if (skillId == 3) // 궁극기
            {
                Debug.Log("궁극기 실행");

                // 카메라 궁극기 연출 시점 전환
                CinemachineController.STATE = 
                    CinemachineController.CamState.Ultimate;
                UltimateBackGround.gameObject.SetActive(true);

                PlayerAnimator.Play("Ultimate");

                yield return _ultimateAnimCool;

                STATE = CharacterState.Idle;
            }
        }


        // 넉백 시 처리
        IEnumerator CoKnockBack()
        {
            _inputable = false;

            // 애니메이션 재생 시간동안 움직이지 못한다
            yield return new WaitForSeconds(1.05f);

            STATE = CharacterState.Idle;

            _inputable = true;
        }


        // 공격 시도 후 명중한 대상들에 대한 데미지 계산
        public void HandleDamage(int skillId, Collider[] hitColliders)
        {
            if(skillId > 0)
            {
                foreach (Collider monsterCollider in hitColliders)
                {
                    if (monsterCollider.TryGetComponent<MonsterController>(out hittedMonster))
                    {
                        // 데미지계산은 플레이어 공격력 * 스킬 계수 * 0.8 ~ 1.2 사이의 랜덤 값 -> 이후 정수로 반올림


                        int damage = Mathf.RoundToInt(Random.Range(0.8f, 1.2f) * TotalDamage * SkillDamages[0]);
                        if (_isMultiPlay) // 멀티 플레이의 경우
                        {
                            // 데미지 처리 요청 패킷을 전송
                            C_BossStatChange bossStatChange = new C_BossStatChange()
                            {
                                Damage = damage,
                            };

                            Managers.Network.Send(bossStatChange);
                        }
                        else // 싱글 플레이의 경우
                        {
                            // 클라이언트 내에서 자체 연산후 적용
                            hittedMonster.GetDamage(damage);
                        }

                    }

                }

            }
        
        }

        public void GetDamage(int damage, bool isKnockback = false)
        {
            Debug.Log("Player Get Damage : " + damage);

            if (_isMultiPlay) // 멀티 플레이의 경우 hp 감소에 대한 패킷을 서버로 전송해야함
            {
                C_ChangeHp getDamagePacket = new C_ChangeHp()
                {
                    Hp = HP - damage,
                };

                Managers.Network.Send(getDamagePacket);
            }
            else // 싱글 플레이의 경우 클라에서 데미지 처리
            {
                HP = HP - damage;
            }

        }

        // --------------------------------------------------------------------------------------








        // -------------------------------- 오브젝트 상호작용 -----------------------------------
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("InteractableObject"))
            {
                Debug.Log($"{other.gameObject.name} Object");

                _interactable = true;

                _InterActTarget = other.gameObject; // 상호작용 오브젝트 지정

                Managers.UI.ShowPopupUI<UI_InteractPopup>(); // 상호작용 UI 활성화
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("InteractableObject"))
            {
                Debug.Log($"{other.gameObject.name} trigger exit");

                // 상호작용 UI 비활성화
                _interactable = false;

                Managers.UI.CloseAllPopupUI();
            }


        }
        // -------------------------------------------------------------------------------------










        // ============================== 개발 테스트 용 코드 ===================================

        private void OnDrawGizmos()
        {

            // 플레이어 내의 타겟 스캔 범위
            /*Color c = new Color(1f, 1f, 1f, 0.5f);
            Gizmos.color = c;
            Gizmos.DrawSphere(transform.position + Vector3.up, 3.0f);*/

            // 기본 공격 범위 확인
            /*{
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position + (transform.forward * 1), 1.5f);
            }*/

            // 궁극기 공격 범위 확인
            /*{
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(transform.position + (transform.forward * 2.0f), 3.0f);
            }*/
        }



        // TEMP : 경험치 테스트
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
            questChange.IsRewarded = true; // 보상 획득 처리 요청
            Managers.Network.Send(questChange);
            yield return new WaitForSeconds(1f);

            isCoolTime = false;
        }
    }
}
