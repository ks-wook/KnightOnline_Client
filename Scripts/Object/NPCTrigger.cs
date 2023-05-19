using Assets.Scripts.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static QuestManager;

public class NPCTrigger : InterActionTrigger
{
    [SerializeField]
    public string Name = "NPC";

    // ------------------- Dialogue ---------------------
    [HideInInspector]
    public string[] NPCDialogue = null;

    [HideInInspector]
    public int hasQuest = 0;

    [HideInInspector]
    public int scriptId = 1;
    // ---------------------------------------------------


    QuestMarker _questMarker;
    public Animator _npcAnimator;


    ObjectState state = ObjectState.Idle;

    public ObjectState STATE
    {
        get { return state; }
        set
        {
            if (state == value)
                return;

            state = value;

            switch(state)
            {
                case ObjectState.Idle:
                    _npcAnimator.SetBool("conversation", false);
                    break;
                case ObjectState.Talk:
                    _npcAnimator.SetBool("conversation", true);
                    _npcAnimator.Play("Conversation");
                    break;
            }
        }
    }


    // NPC 대화 스크립트 설정 (Dialogue Manager 에서 호출)
    public void InitDialouge((string[] scripts, int questNum) scriptsAndQuest)
    {
        (NPCDialogue, hasQuest) = scriptsAndQuest;
        Debug.Log("NPC" + ObjectID + "Setted Dialogue");


        if(hasQuest != 0)
        {
            Quest currentQuest = Managers.Quest.GetPlayerQuestById(hasQuest); // 현재 플레이어의 퀘스트 목록으로부터 퀘스트 탐색

            if (currentQuest == null)
                return;


            if (currentQuest.IsCleared == false && currentQuest.IsRewarded == false) // 이미 퀘스트를 받은 상태
            {
                _questMarker.SetMarker("!");
                _questMarker.SetColor("gray");

                // 퀘스트를 받은 경우 현재 읽어야 할 스크립트 id + 1
                scriptId += 1;
            }
            else if (currentQuest.IsCleared == true && currentQuest.IsRewarded == false) // 퀘스트를 클리어 && 보상 미수령
            {
                _questMarker.SetMarker("?");
                _questMarker.SetColor("yellow");

                // 퀘스트를 클리어한 경우 현재 읽어야 할 스크립트 id + 2
                scriptId += 2;
            }
            else if (currentQuest.IsCleared == true && currentQuest.IsRewarded == true) // 퀘스트를 클리어 && 보상 수령
            {
                // 퀘스트의 보상까지 받은(종료) 경우 현재 읽어야 할 스크립트 id + 3
                scriptId += 3;
                Managers.Dialouge.RegisterGetDialouge(this, scriptId); // 스크립트 처리 재등록
            }
            else // 퀘스트를 받기 전 상태
            {
                // TODO : 받을 수 있는 퀘스트가 있는 경우 노란색 '!' 표시
                _questMarker.SetMarker("!");
                _questMarker.SetColor("yellow");
            }

            (NPCDialogue, hasQuest) = Managers.Dialouge.GetDialogueAndQuest(this.ObjectID, scriptId);
        }

    }

    public void Conversation()
    {

        // dialoue UI 활성화
        if (NPCDialogue != null)
        {
            Managers.UI.CloseAllPopupUI(); // Interact UI 비활성화

            // Dialogue UI 활성화
            UI_DialougePopup dialougePopupUI = Managers.UI.ShowPopupUI<UI_DialougePopup>();
            dialougePopupUI.DialougeUI.NPCName = this.Name;
            dialougePopupUI.DialougeUI.NPCdialouge = this.NPCDialogue;
            dialougePopupUI.DialougeUI.ConversationProceed();

            STATE = ObjectState.Talk; // NPC 대화상태 전환
        }

    }

    // 대화 종료(스크립트의 마지막에서 호출)
    public override void OnEndInterAct()
    {
        if(hasQuest != 0)
        {
            Quest playerQuest = Managers.Quest.GetPlayerQuestById(hasQuest);
            if(playerQuest == null) // 받은적 없는 퀘스트 이므로 추가
            {
                Quest newQuest = Managers.Quest.GetQuestById(hasQuest);
                Managers.Quest.AddNewQuest(newQuest);
            }
        }

        STATE = ObjectState.Idle; // idle 상태 초기화
    }

    // 퀘스트 마커 추가
    private void AddQuestMarker()
    {
        GameObject go = Managers.Resource.Instantiate("UI/WorldSpace/QuestMarker", transform);
        go.name = "QuestMarker";
        _questMarker = go.GetComponent<QuestMarker>();
    }

    // 애니메이터 획득
    private void InitAnimator()
    {
        _npcAnimator = this.GetComponentInChildren<Animator>();
    }

    void Start()
    {
        ObjectName = gameObject.name;
        ObjectID = int.Parse((ObjectName.Split("_"))[1]);

        AddQuestMarker();
        InitAnimator();
        Managers.Dialouge.RegisterGetDialouge(this, scriptId); // 스크립트 처리 등록
    }
}
