using Assets.Scripts.Controller;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static QuestManager;

/*
 * NPC 오브젝트에 붙는 스크립트로 InteractableObject를 상속받아 상호작용이 가능하다.
 * 
 * 해당 오브젝트의 상호작용은 대화이다.
 */


public class NPCObject : InteractableObject
{
    [Header("NPC Info")]
    [SerializeField]
    [Tooltip("대화창에 나타나는 NPC 이름")]
    public string Name = "NPC";

    [SerializeField]
    [Tooltip("스크립트 초기화에 사용되는 NPC ID")]
    public int npcID = 0;

    // ------------------- Dialogue 관련 변수 -----------------------
    [HideInInspector]
    public string[] CurNPCDialogue = null;

    [HideInInspector]
    public int CurNPCQuestId = 0;

    [HideInInspector]
    public int CurNPCScriptId = 1;

    // 현재 NPC의 퀘스트에 대한 보상을 받을 수 있는지 여부
    [HideInInspector]
    public bool GetRewardEnable = false;

    CinemachineController _cinemachineController;
    QuestMarker _questMarker;
    // --------------------------------------------------------------




    // ------------------------- Init -------------------------------

    void Init()
    {
        ObjectID = this.npcID;

        AddQuestMarker();
        InitAnimator();

        GameObject.Find("CinemachineController")
            .TryGetComponent<CinemachineController>(out _cinemachineController);

        RefreshScript();
    }

    // 애니메이터 획득
    private void InitAnimator()
    {
        _animator = transform.GetComponent<Animator>();
        _animator.Play("Idle");
    }

    // 퀘스트 마커 추가
    private void AddQuestMarker()
    {
        GameObject go = Managers.Resource.Instantiate("UI/WorldSpace/QuestMarker", transform);
        go.name = "QuestMarker";
        _questMarker = go.GetComponent<QuestMarker>();
    }

    // 퀘스트 진행 상태에 따른 NPC 대화 스크립트 갱신
    public void RefreshScript()
    {
        Managers.Dialouge.RegisterGetDialouge(this, CurNPCScriptId); // 스크립트 처리 등록
    }

    // --------------------------------------------------------------



    // ------------------- 애니메이션 관련 변수 ---------------------
    [HideInInspector]
    public Animator _animator;

    ObjectState _state = ObjectState.Idle;
    public ObjectState STATE
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            _state = value;

            switch(_state)
            {
                case ObjectState.Idle:
                    _animator.SetBool("conversation", false);
                    break;
                case ObjectState.Active:
                    _animator.SetBool("conversation", true);
                    _animator.Play("Conversation");
                    break;
            }
        }
    }
    // --------------------------------------------------------------





    // --------------------- NPC Conversation ------------------------

    // NPC 대화 스크립트 설정 (Dialogue Manager 에서 호출)
    public void InitDialouge((string[] scripts, int questNum) scriptsAndQuest)
    {
        (CurNPCDialogue, CurNPCQuestId) = scriptsAndQuest;

        if(CurNPCQuestId != 0)
        {
            Quest currentQuest = Managers.Quest.GetPlayerQuestById(CurNPCQuestId); // 현재 플레이어의 퀘스트 목록으로부터 퀘스트 탐색

            if (currentQuest == null)
                return;

            if (currentQuest.IsCleared == false && currentQuest.IsRewarded == false) // 이미 퀘스트를 받은 상태
            {
                _questMarker.SetMarker("!", this);
                _questMarker.SetColor("gray");

                // 퀘스트를 받은 경우 현재 읽어야 할 스크립트 id + 1
                CurNPCScriptId += 1;

            }
            else if (currentQuest.IsCleared == true && currentQuest.IsRewarded == false) // 퀘스트를 클리어 && 보상 미수령
            {
                _questMarker.SetMarker("?", this);
                _questMarker.SetColor("yellow");

                // 퀘스트를 클리어한 경우 현재 읽어야 할 스크립트 id + 2
                CurNPCScriptId += 2;

            }
            else if (currentQuest.IsCleared == true && currentQuest.IsRewarded == true) // 퀘스트를 클리어 && 보상 수령
            {
                // 퀘스트의 보상까지 받은(종료) 경우 현재 읽어야 할 스크립트 id + 3
                CurNPCScriptId += 3;

                // 스크립트 처리 재등록
                Managers.Dialouge.RegisterGetDialouge(this, CurNPCScriptId); 

            }
            else // 퀘스트를 받기 전 상태
            {
                // 받을 수 있는 퀘스트가 있는 경우 노란색 '!' 표시
                _questMarker.SetMarker("!", this);
                _questMarker.SetColor("yellow");

            }

            // NPC의 id와 현재 스크립트 id를 이용하여 대화 dialogue와 현재 npc의 퀘스트 id를 획득
            (CurNPCDialogue, CurNPCQuestId) = Managers.Dialouge.GetDialogueAndQuest(this.ObjectID, CurNPCScriptId);
        }

    }

    // 퀘스트 보상 수령 완료 후 다음 스크립트로 이동
    public void SetNextDialogue(string questMarker = null)
    {
        (CurNPCDialogue, CurNPCQuestId) = Managers.Dialouge.GetDialogueAndQuest(this.ObjectID, ++CurNPCScriptId);

        Debug.Log("현재 스크립트 ID : " + CurNPCScriptId);
        Debug.Log("퀘스트 마커 : " + questMarker);

        // 퀘스트 마커 업데이트
        if(questMarker == "!")
        {
            _questMarker.SetMarker(questMarker, this);
        }
        else if(questMarker == "?")
        {
            _questMarker.SetMarker(questMarker, this);
        }
        else
        {
            _questMarker.SetMarker("", this);
        }

        if(CurNPCDialogue == null) // 대사가 없는 경우
        {
            _questMarker.SetMarker("", this);
        }
    }

    // 대화 진행 함수
    public void Conversation()
    {
        // Dialoue UI 활성화
        if (CurNPCDialogue != null)
        {
            UI_DialougePopup dialougePopupUI = Managers.UI.ShowPopupUI<UI_DialougePopup>();
            dialougePopupUI.DialougeUI.NPCName = this.Name;
            dialougePopupUI.DialougeUI.NPCdialouge = this.CurNPCDialogue;
            dialougePopupUI.DialougeUI.ConversationProceed();

            STATE = ObjectState.Active; // NPC 대화상태 전환
        }

    }

    // 대화 종료(스크립트의 마지막에서 호출)
    public override void OnEndConversation()
    {
        _cinemachineController.STATE = 
            CinemachineController.CamState.TPS;

        if (CurNPCQuestId != 0)
        {
            Quest playerQuest = Managers.Quest.GetPlayerQuestById(CurNPCQuestId);
            if(playerQuest == null) // 받은적 없는 퀘스트 이므로 추가
            {
                Quest newQuest = Managers.Quest.GetQuestById(CurNPCQuestId);
                Managers.Quest.AddNewQuest(newQuest);

                // 퀘스트 진행중일 때의 스크립트로 다시 셋팅
                SetNextDialogue("!");
            }
        }


        // 보상을 받을 수 있는 상태라면 서버로 퀘스트 ID를 통해 보상 획득 요청
        if (GetRewardEnable)
        {
            Quest playerQuest = Managers.Quest.GetPlayerQuestById(CurNPCQuestId);
            
            if (playerQuest != null) // 퀘스트 완료에 대한 경험치 획득 요청
            {
                Managers.Object.MyPlayer.GetExp(playerQuest.Exp);
            }

            C_QuestChange questChange = new C_QuestChange();
            questChange.QuestTemplatedId = CurNPCQuestId;
            questChange.IsCleared = true;
            questChange.IsRewarded = true; // 보상 획득 처리 요청

            // 보상 요청 패킷 전송
            Managers.Network.Send(questChange);

            // 다음 스크립트로 이동
            SetNextDialogue("!");
        }

        STATE = ObjectState.Idle; // idle 상태 초기화


    }

    // ----------------------------------------------------------------





    // -------------------------- Start -------------------------------
    void Start()
    {
        Init();
    }

    // ----------------------------------------------------------------



    // ------------------------ Override ------------------------------
    public override void InterAct() // NPC : 상호작용(대화)
    {
        _cinemachineController.STATE = 
            CinemachineController.CamState.Conversation;

        Managers.UI.CloseAllPopupUI();

        // 대화 시 사운드 재생
        Managers.Sound.Play("Effect/Conversation");

        // 플레이어의 방향으로 방향 전환
        transform.LookAt(Managers.Object.MyPlayer.transform);

        Conversation();
    }

    // ----------------------------------------------------------------
}
