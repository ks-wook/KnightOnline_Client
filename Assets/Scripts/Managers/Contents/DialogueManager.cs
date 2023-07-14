using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NPC 대화 스크립트 초기화 매니저
public class DialogueManager 
{
    object _lock = new object();

    // 초기화를 위한 NPC 객체를 담는 Queue -> 게임 접속 시 비동기식으로 처리
    Queue<(NPCObject npcTrigger, int scriptId)> _npcObjects = new Queue<(NPCObject npcTrigger, int scriptId)>();

    Dictionary<(int, int), (string[], int)> _dialoueDict = null;

    public Dictionary<(int, int), (string[], int)> DialogueDict
    {
        get { return _dialoueDict; }
        set
        {
            _dialoueDict = value;
            FlushDialogueQueue();
        }
    }

    // 스크립트 + 퀘스트 정보를 서버로 부터 획득 후 최종적으로 오브젝트를 초기화할 Job 객체
    public class DialougeTask
    {
        Action<(string[], int)> _action;
        string[] _scripts;
        int _scriptId;

        public DialougeTask(Action<(string[], int)> action, (string[] scripts, int scriptId) scriptsAndId)
        {
            _action = action;
            _scripts = scriptsAndId.scripts;
            _scriptId = scriptsAndId.scriptId;
        }

        public void Excute()
        {
            _action.Invoke((_scripts, _scriptId));
        }
    }

    public void AddNCPQueue(NPCObject npc, int scriptId = 1)
    {
        lock(_lock)
        {
            _npcObjects.Enqueue((npc, scriptId));
        }
    }

    // 대화 스크립트 초기화를 원하는 NPC 오브젝트 등록 처리
    public void RegisterGetDialouge(NPCObject npc, int scriptId)
    {
        AddNCPQueue(npc, scriptId);
        if (DialogueDict != null) // 초기화 완료시 대기 없이 바로 처리
        {
            FlushDialogueQueue();
        }
    }

    // 대화 스크립트 초기화 대기 큐 flush
    public void FlushDialogueQueue()
    {
        // 대화 스크립트 초기화를 원하는 NPC가 있다면 처리
        while(_npcObjects.Count != 0)
        {
            (NPCObject npcTrigger, int scriptId) nPCTriggerAndScriptId = _npcObjects.Dequeue();
            NPCObject npcTrigger = nPCTriggerAndScriptId.npcTrigger;
            int scriptId = nPCTriggerAndScriptId.scriptId;

            // 퀘스트 매니저에게 초기화 등록을 요청해야 함
            DialougeTask dialogueInitTask = new DialougeTask(npcTrigger.InitDialouge, GetDialogueAndQuest(npcTrigger.ObjectID, scriptId));
            Managers.Quest.NPCDialogueTaskRegister(dialogueInitTask);
        }
    }

    public (string[], int) GetDialogueAndQuest(int NPCId, int scriptId)
    {
        // 퀘스트 진행 여부에 따라 다른 대사를 출력
        string[] dialouge = null;
        int hasQuest = 0;

        (string[], int) data = (dialouge, hasQuest);

        // DialogueDict는 NPC의 ID, 그리고 스크립트의 ID를 key로 사용한다.
        if (DialogueDict.TryGetValue((NPCId, scriptId), out data))
            return data;
    
         return (null, 0);
    }
}

