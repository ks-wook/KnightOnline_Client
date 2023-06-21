using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static DialogueManager;

public class QuestManager
{
    // ------------------ Quest 초기화 관련 변수 ---------------------
    public bool IsRecievedPlayerQuestsByServer = false;
    public bool IsAddedPlayerQuests = false;

    public int PlayerQuestInitSize = 0;
    // ---------------------------------------------------------------




    // ------------------------ Quest 객체 ---------------------------
    public class Quest
    {
        public int ID = 0;
        public string Type = null;
        public string Name = null;
        public string Tip = null;
        public string Object = null;
        public bool IsCleared = false;
        public bool IsRewarded = false;

        public Quest(int ID, string Type, string Name, string Tip, String Object, bool IsCleared = false, bool IsRewarded = false)
        {
            this.ID = ID;
            this.Type = Type;
            this.Name = Name;
            this.Tip = Tip;
            this.Object = Object;
            this.IsCleared = IsCleared;
            this.IsRewarded = IsRewarded;
        }

        public void SetQuestData(Quest quest)
        {
            this.ID = quest.ID;
            this.Type = quest.Type;
            this.Name = quest.Name;
            this.Tip = quest.Tip;
            this.Object = quest.Object;
        }

        public Quest(int ID)
        {
            Quest quest = Managers.Quest.GetQuestById(ID);
            this.ID = ID;

            if (quest != null)
            {
                this.Type = quest.Type;
                this.Name = quest.Name;
                this.Tip = quest.Tip;
                this.Object = quest.Object;
                this.IsCleared = false;
                this.IsRewarded = false;
            }
        }

        public Quest(){ }
    }
    // -----------------------------------------------------------------------







    // ---------------------------- Quest 등록 ---------------------------------


    // 모든 퀘스트 데이터
    public Dictionary<int, Quest> _allQuests = null;
    public Dictionary<int, Quest> AllQuests
    {
        get { return _allQuests; }
        set
        {
            _allQuests = value;
            FlushQuestQueue();
        }
    }


    // 보상까지 받은(종료된 퀘스트)
    public Dictionary<int, Quest> PlayerQuests_End = new Dictionary<int, Quest>();
    
    // 플레이어가 진행중인 퀘스트 데이터 
    public Dictionary<int, Quest> _playerQuests = new Dictionary<int, Quest>();


    public Dictionary<int, Quest> PlayerQuests
    {
        get { return _playerQuests; }
        set
        {
            if (_playerQuests == value)
                return;

            _playerQuests = value;

            UI_LobbyScene lobbySceneUI = Managers.UI.SceneUI as UI_LobbyScene;
            UI_Quest questUI = lobbySceneUI.QuestUI;
            questUI.RefreshUI();
        }
    }

    // 실제로 퀘스트가 추가되는 함수
    public void PlayerQuestsAdd(Quest quest)
    {
        lock(_lock)
        {
            PlayerQuests.Add(quest.ID, quest);
            UI_LobbyScene lobbySceneUI = Managers.UI.SceneUI as UI_LobbyScene;
            UI_Quest questUI = lobbySceneUI.QuestUI;
            questUI.RefreshUI();
        }
    }

    public void PlayerQuestsRemove(Quest quest)
    {
        lock(_lock)
        {
            PlayerQuests.Remove(quest.ID);
            UI_LobbyScene lobbySceneUI = Managers.UI.SceneUI as UI_LobbyScene;
            UI_Quest questUI = lobbySceneUI.QuestUI;
            questUI.RefreshUI();
        }
    }

    private Queue<Quest> quests = new Queue<Quest>();

    object _lock = new object();

    // 퀘스트 등록을 처리
    public void AddQuestRegister(Quest quest)
    {
        lock (_lock)
        {
            // 종료된 퀘스트일 때는 PlayerQuest에 추가하지 않고 PlayerQuest_Ended에만 추가
            if (quest.IsCleared && quest.IsRewarded)
            {
                PlayerQuests_End.Add(quest.ID, quest);
                return;    
            }

            quests.Enqueue(quest);
            if (AllQuests != null) // 데이터 시트로부터 받은 모든 퀘스트 정보가 초기화 완료된 경우
                FlushQuestQueue();

        }

        Debug.Log($"{quest} 퀘스트 추가");
    }



    // 게임내에서 새로운 퀘스트 획득처리
    public void AddNewQuest(Quest quest)
    {
        AddQuestRegister(quest);

        // 추가된(진행중) 퀘스트 서버로 전송
        // 퀘스트 추가에 대해서는 답장을 받지 않음
        C_QuestChange questChange = new C_QuestChange()
        {
            QuestTemplatedId = quest.ID,
            IsCleared = false
        };

        Managers.Network.Send(questChange);
    }

    // 실제 퀘스트 등록 큐 Flush
    public void FlushQuestQueue()
    {
        // 퀘스트 초기화가 필요한 퀘스트가 쌓인 경우 처리
        foreach (Quest quest in quests)
        {
            // 퀘스트가 아직 초기화되지 않았다면 퀘스트 정보를 주입
            if(quest.Name == null)
                quest.SetQuestData(Managers.Quest.GetQuestById(quest.ID));


            // 실제 퀘스트 추가
            PlayerQuestsAdd(quest);


            // UI 갱신
            UI_LobbyScene lobbySceneUI = Managers.UI.SceneUI as UI_LobbyScene;
            UI_Quest questUI = lobbySceneUI.QuestUI;
            questUI.RefreshUI();

        }

        if(PlayerQuestInitSize == (PlayerQuests.Count + PlayerQuests_End.Count))
        {
            if (IsAddedPlayerQuests == false)
                IsAddedPlayerQuests = true;
        }
        


        FlushQuestDialogueTaskQueue();

    }
    // -------------------------------------------------------------------------







    // ---------------------------- Quest 검색 ---------------------------------

    // TemplatedID를 통한 퀘스트 정보 획득
    public Quest GetQuestById(int templatedId)
    {
        Quest quest = new Quest();
        AllQuests.TryGetValue(templatedId, out quest);
        return quest;
    }

    // TemplatedID를 통해 현재 플레이어의 퀘스트 정보 획득
    public Quest GetPlayerQuestById(int templatedId)
    {

        Quest quest = new Quest();
        if(PlayerQuests.TryGetValue(templatedId, out quest))
            return quest;
        else if(PlayerQuests_End.TryGetValue(templatedId, out quest))
            return quest;

        return null;
    }

    // -------------------------------------------------------------------------








    // -------------------------------- Quest 클리어 ---------------------------

    // 아이템 장착 관련 퀘스트클리어 요청 처리
    public void CheckEquipTypeQuest(int equipItemDbId)
    {
        List<Quest> questsList = PlayerQuests.Values.ToList();

        foreach (Quest quest in questsList)
        {
            // Equip 타입 퀘스트인 경우 조건 확인
            if(quest.Type == "Equip")
            {
                // 퀘스트 조건에 맞는 아이템 장착 시
                if (int.Parse(quest.Object) == equipItemDbId)
                    ClearQuest(quest.ID);
                
            }
        }
    }

    // 스테이지 클리어 관련 퀘스트클리어 요청 처리
    public void CheckClearTypeQuest(int stageNum)
    {
        List<Quest> questsList = PlayerQuests.Values.ToList();

        foreach (Quest quest in questsList)
        {
            // Equip 타입 퀘스트인 경우 조건 확인
            if (quest.Type == "Clear")
            {
                // TODO : 조건에 맞는 스테이지 클리어 시 처리
                




            }
        }
    }

    // 퀘스트 클리어 요청처리 -> 서버로부터 답장을 받은 후 HandleClearQuest 호출
    public void ClearQuest(int questTemplatedId)
    {
        C_QuestChange clearPacket = new C_QuestChange()
        {
            QuestTemplatedId = questTemplatedId,
            IsCleared = true
        };

        Managers.Network.Send(clearPacket);
    }

    // 퀘스트 클리어 처리 (Packet Handler로 부터 호출됨) + Packet Handler외에는 호출 금지
    public void HandleClearQuest(int questTemplatedId)
    {
        // 퀘스트 정보 수정 -> IsCleared = true;
        Quest _completedQuest = new Quest();
        if (PlayerQuests.TryGetValue(questTemplatedId, out _completedQuest))
            _completedQuest.IsCleared = true;


        // UI 갱신
        UI_LobbyScene lobbySceneUI = Managers.UI.SceneUI as UI_LobbyScene;
        UI_Quest questUI = lobbySceneUI.QuestUI;
        questUI.RefreshUI();
    }

    // 퀘스트 보상 습득 처리
    public void HandleRewardQeust()
    {
        // TODO : 퀘스트 목록에서 삭제 후, 보상 획득, Cleared에서 삭제, Rewarded에 추가

    }

    // -------------------------------------------------------------






    // --------------------- Quest Dialogue 처리 -------------------
    // 퀘스트 및 대화 스크립트가 연동되어 있고, 두 개의 정보가 서버로부터
    // 초기화 되어야 오브젝트들의 초기화를 진행할 수 있다.

    Queue<DialougeTask> _questDialogueTasks = new Queue<DialougeTask>();

    public void NPCDialogueTaskRegister(DialougeTask dialougeTask)
    {
        lock (_lock)
        {
            _questDialogueTasks.Enqueue(dialougeTask);
            FlushQuestDialogueTaskQueue();
            
        }
    }

    public void FlushQuestDialogueTaskQueue()
    {
        if (IsRecievedPlayerQuestsByServer && IsAddedPlayerQuests) // 퀘스트 정보를 서버로 부터 받아 초기화 했다면
        {
            while (_questDialogueTasks.Count > 0)
            {
                DialougeTask dialougeTask = _questDialogueTasks.Dequeue();
                dialougeTask.Excute();
            }
        }
        
    }

    // -------------------------------------------------------------

    public void Clear()
    {
        PlayerQuests.Clear();
        PlayerQuests_End.Clear();
    }

}

