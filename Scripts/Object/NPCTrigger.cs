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


    // NPC ��ȭ ��ũ��Ʈ ���� (Dialogue Manager ���� ȣ��)
    public void InitDialouge((string[] scripts, int questNum) scriptsAndQuest)
    {
        (NPCDialogue, hasQuest) = scriptsAndQuest;
        Debug.Log("NPC" + ObjectID + "Setted Dialogue");


        if(hasQuest != 0)
        {
            Quest currentQuest = Managers.Quest.GetPlayerQuestById(hasQuest); // ���� �÷��̾��� ����Ʈ ������κ��� ����Ʈ Ž��

            if (currentQuest == null)
                return;


            if (currentQuest.IsCleared == false && currentQuest.IsRewarded == false) // �̹� ����Ʈ�� ���� ����
            {
                _questMarker.SetMarker("!");
                _questMarker.SetColor("gray");

                // ����Ʈ�� ���� ��� ���� �о�� �� ��ũ��Ʈ id + 1
                scriptId += 1;
            }
            else if (currentQuest.IsCleared == true && currentQuest.IsRewarded == false) // ����Ʈ�� Ŭ���� && ���� �̼���
            {
                _questMarker.SetMarker("?");
                _questMarker.SetColor("yellow");

                // ����Ʈ�� Ŭ������ ��� ���� �о�� �� ��ũ��Ʈ id + 2
                scriptId += 2;
            }
            else if (currentQuest.IsCleared == true && currentQuest.IsRewarded == true) // ����Ʈ�� Ŭ���� && ���� ����
            {
                // ����Ʈ�� ������� ����(����) ��� ���� �о�� �� ��ũ��Ʈ id + 3
                scriptId += 3;
                Managers.Dialouge.RegisterGetDialouge(this, scriptId); // ��ũ��Ʈ ó�� ����
            }
            else // ����Ʈ�� �ޱ� �� ����
            {
                // TODO : ���� �� �ִ� ����Ʈ�� �ִ� ��� ����� '!' ǥ��
                _questMarker.SetMarker("!");
                _questMarker.SetColor("yellow");
            }

            (NPCDialogue, hasQuest) = Managers.Dialouge.GetDialogueAndQuest(this.ObjectID, scriptId);
        }

    }

    public void Conversation()
    {

        // dialoue UI Ȱ��ȭ
        if (NPCDialogue != null)
        {
            Managers.UI.CloseAllPopupUI(); // Interact UI ��Ȱ��ȭ

            // Dialogue UI Ȱ��ȭ
            UI_DialougePopup dialougePopupUI = Managers.UI.ShowPopupUI<UI_DialougePopup>();
            dialougePopupUI.DialougeUI.NPCName = this.Name;
            dialougePopupUI.DialougeUI.NPCdialouge = this.NPCDialogue;
            dialougePopupUI.DialougeUI.ConversationProceed();

            STATE = ObjectState.Talk; // NPC ��ȭ���� ��ȯ
        }

    }

    // ��ȭ ����(��ũ��Ʈ�� ���������� ȣ��)
    public override void OnEndInterAct()
    {
        if(hasQuest != 0)
        {
            Quest playerQuest = Managers.Quest.GetPlayerQuestById(hasQuest);
            if(playerQuest == null) // ������ ���� ����Ʈ �̹Ƿ� �߰�
            {
                Quest newQuest = Managers.Quest.GetQuestById(hasQuest);
                Managers.Quest.AddNewQuest(newQuest);
            }
        }

        STATE = ObjectState.Idle; // idle ���� �ʱ�ȭ
    }

    // ����Ʈ ��Ŀ �߰�
    private void AddQuestMarker()
    {
        GameObject go = Managers.Resource.Instantiate("UI/WorldSpace/QuestMarker", transform);
        go.name = "QuestMarker";
        _questMarker = go.GetComponent<QuestMarker>();
    }

    // �ִϸ����� ȹ��
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
        Managers.Dialouge.RegisterGetDialouge(this, scriptId); // ��ũ��Ʈ ó�� ���
    }
}
