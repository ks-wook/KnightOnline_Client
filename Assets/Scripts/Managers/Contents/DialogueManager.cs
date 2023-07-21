using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NPC ��ȭ ��ũ��Ʈ �ʱ�ȭ �Ŵ���
public class DialogueManager 
{
    object _lock = new object();

    // �ʱ�ȭ�� ���� NPC ��ü�� ��� Queue -> ���� ���� �� �񵿱������ ó��
    Queue<(NPCObject npcObject, int scriptId)> _npcObjects = new Queue<(NPCObject npcObject, int scriptId)>();

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

    // ��ũ��Ʈ + ����Ʈ ������ ������ ���� ȹ�� �� ���������� ������Ʈ�� �ʱ�ȭ�� Job ��ü
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

    // ��ȭ ��ũ��Ʈ �ʱ�ȭ�� ���ϴ� NPC ������Ʈ ��� ó��
    public void RegisterGetDialouge(NPCObject npc, int scriptId)
    {
        AddNCPQueue(npc, scriptId);
        if (DialogueDict != null) // �ʱ�ȭ �Ϸ�� ��� ���� �ٷ� ó��
        {
            FlushDialogueQueue();
        }
    }

    // ��ȭ ��ũ��Ʈ �ʱ�ȭ ��� ť flush ó��
    public void FlushDialogueQueue()
    {
        // ��ȭ ��ũ��Ʈ �ʱ�ȭ�� ���ϴ� NPC�� �ִٸ� ���� �����忡�� ó��
        while(_npcObjects.Count != 0)
        {
            (NPCObject npcObject, int scriptId) nPCAndScriptId = _npcObjects.Dequeue();
            NPCObject npc = nPCAndScriptId.npcObject;
            int scriptId = nPCAndScriptId.scriptId;

            // ����Ʈ �Ŵ������� �ʱ�ȭ ����� ��û�ؾ� ��
            DialougeTask dialogueInitTask = new DialougeTask(npc.InitDialouge, GetDialogueAndQuest(npc.ObjectID, scriptId));
            Managers.Quest.QuestDialogueRegister(dialogueInitTask);
        }
    }

    public (string[], int) GetDialogueAndQuest(int NPCId, int scriptId)
    {
        // ����Ʈ ���� ���ο� ���� �ٸ� ��縦 ���
        string[] dialouge = null;
        int hasQuest = 0;

        (string[], int) data = (dialouge, hasQuest);

        // DialogueDict�� NPC�� ID, �׸��� ��ũ��Ʈ�� ID�� key�� ����Ѵ�.
        if (DialogueDict.TryGetValue((NPCId, scriptId), out data))
            return data;
    
         return (null, 0);
    }
}

