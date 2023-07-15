using Assets.Scripts.Controller;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static QuestManager;

/*
 * NPC ������Ʈ�� �ٴ� ��ũ��Ʈ�� InteractableObject�� ��ӹ޾� ��ȣ�ۿ��� �����ϴ�.
 * 
 * �ش� ������Ʈ�� ��ȣ�ۿ��� ��ȭ�̴�.
 */


public class NPCObject : InteractableObject
{
    [Header("NPC Info")]
    [SerializeField]
    [Tooltip("��ȭâ�� ��Ÿ���� NPC �̸�")]
    public string Name = "NPC";

    [SerializeField]
    [Tooltip("��ũ��Ʈ �ʱ�ȭ�� ���Ǵ� NPC ID")]
    public int npcID = 0;

    // ------------------- Dialogue ���� ���� -----------------------
    [HideInInspector]
    public string[] CurNPCDialogue = null;

    [HideInInspector]
    public int CurNPCQuestId = 0;

    [HideInInspector]
    public int CurNPCScriptId = 1;

    // ���� NPC�� ����Ʈ�� ���� ������ ���� �� �ִ��� ����
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

    // �ִϸ����� ȹ��
    private void InitAnimator()
    {
        _animator = transform.GetComponent<Animator>();
        _animator.Play("Idle");
    }

    // ����Ʈ ��Ŀ �߰�
    private void AddQuestMarker()
    {
        GameObject go = Managers.Resource.Instantiate("UI/WorldSpace/QuestMarker", transform);
        go.name = "QuestMarker";
        _questMarker = go.GetComponent<QuestMarker>();
    }

    // ����Ʈ ���� ���¿� ���� NPC ��ȭ ��ũ��Ʈ ����
    public void RefreshScript()
    {
        Managers.Dialouge.RegisterGetDialouge(this, CurNPCScriptId); // ��ũ��Ʈ ó�� ���
    }

    // --------------------------------------------------------------



    // ------------------- �ִϸ��̼� ���� ���� ---------------------
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

    // NPC ��ȭ ��ũ��Ʈ ���� (Dialogue Manager ���� ȣ��)
    public void InitDialouge((string[] scripts, int questNum) scriptsAndQuest)
    {
        (CurNPCDialogue, CurNPCQuestId) = scriptsAndQuest;

        if(CurNPCQuestId != 0)
        {
            Quest currentQuest = Managers.Quest.GetPlayerQuestById(CurNPCQuestId); // ���� �÷��̾��� ����Ʈ ������κ��� ����Ʈ Ž��

            if (currentQuest == null)
                return;

            if (currentQuest.IsCleared == false && currentQuest.IsRewarded == false) // �̹� ����Ʈ�� ���� ����
            {
                _questMarker.SetMarker("!", this);
                _questMarker.SetColor("gray");

                // ����Ʈ�� ���� ��� ���� �о�� �� ��ũ��Ʈ id + 1
                CurNPCScriptId += 1;

            }
            else if (currentQuest.IsCleared == true && currentQuest.IsRewarded == false) // ����Ʈ�� Ŭ���� && ���� �̼���
            {
                _questMarker.SetMarker("?", this);
                _questMarker.SetColor("yellow");

                // ����Ʈ�� Ŭ������ ��� ���� �о�� �� ��ũ��Ʈ id + 2
                CurNPCScriptId += 2;

            }
            else if (currentQuest.IsCleared == true && currentQuest.IsRewarded == true) // ����Ʈ�� Ŭ���� && ���� ����
            {
                // ����Ʈ�� ������� ����(����) ��� ���� �о�� �� ��ũ��Ʈ id + 3
                CurNPCScriptId += 3;

                // ��ũ��Ʈ ó�� ����
                Managers.Dialouge.RegisterGetDialouge(this, CurNPCScriptId); 

            }
            else // ����Ʈ�� �ޱ� �� ����
            {
                // ���� �� �ִ� ����Ʈ�� �ִ� ��� ����� '!' ǥ��
                _questMarker.SetMarker("!", this);
                _questMarker.SetColor("yellow");

            }

            // NPC�� id�� ���� ��ũ��Ʈ id�� �̿��Ͽ� ��ȭ dialogue�� ���� npc�� ����Ʈ id�� ȹ��
            (CurNPCDialogue, CurNPCQuestId) = Managers.Dialouge.GetDialogueAndQuest(this.ObjectID, CurNPCScriptId);
        }

    }

    // ����Ʈ ���� ���� �Ϸ� �� ���� ��ũ��Ʈ�� �̵�
    public void SetNextDialogue(string questMarker = null)
    {
        (CurNPCDialogue, CurNPCQuestId) = Managers.Dialouge.GetDialogueAndQuest(this.ObjectID, ++CurNPCScriptId);

        Debug.Log("���� ��ũ��Ʈ ID : " + CurNPCScriptId);
        Debug.Log("����Ʈ ��Ŀ : " + questMarker);

        // ����Ʈ ��Ŀ ������Ʈ
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

        if(CurNPCDialogue == null) // ��簡 ���� ���
        {
            _questMarker.SetMarker("", this);
        }
    }

    // ��ȭ ���� �Լ�
    public void Conversation()
    {
        // Dialoue UI Ȱ��ȭ
        if (CurNPCDialogue != null)
        {
            UI_DialougePopup dialougePopupUI = Managers.UI.ShowPopupUI<UI_DialougePopup>();
            dialougePopupUI.DialougeUI.NPCName = this.Name;
            dialougePopupUI.DialougeUI.NPCdialouge = this.CurNPCDialogue;
            dialougePopupUI.DialougeUI.ConversationProceed();

            STATE = ObjectState.Active; // NPC ��ȭ���� ��ȯ
        }

    }

    // ��ȭ ����(��ũ��Ʈ�� ���������� ȣ��)
    public override void OnEndConversation()
    {
        _cinemachineController.STATE = 
            CinemachineController.CamState.TPS;

        if (CurNPCQuestId != 0)
        {
            Quest playerQuest = Managers.Quest.GetPlayerQuestById(CurNPCQuestId);
            if(playerQuest == null) // ������ ���� ����Ʈ �̹Ƿ� �߰�
            {
                Quest newQuest = Managers.Quest.GetQuestById(CurNPCQuestId);
                Managers.Quest.AddNewQuest(newQuest);

                // ����Ʈ �������� ���� ��ũ��Ʈ�� �ٽ� ����
                SetNextDialogue("!");
            }
        }


        // ������ ���� �� �ִ� ���¶�� ������ ����Ʈ ID�� ���� ���� ȹ�� ��û
        if (GetRewardEnable)
        {
            Quest playerQuest = Managers.Quest.GetPlayerQuestById(CurNPCQuestId);
            
            if (playerQuest != null) // ����Ʈ �Ϸῡ ���� ����ġ ȹ�� ��û
            {
                Managers.Object.MyPlayer.GetExp(playerQuest.Exp);
            }

            C_QuestChange questChange = new C_QuestChange();
            questChange.QuestTemplatedId = CurNPCQuestId;
            questChange.IsCleared = true;
            questChange.IsRewarded = true; // ���� ȹ�� ó�� ��û

            // ���� ��û ��Ŷ ����
            Managers.Network.Send(questChange);

            // ���� ��ũ��Ʈ�� �̵�
            SetNextDialogue("!");
        }

        STATE = ObjectState.Idle; // idle ���� �ʱ�ȭ


    }

    // ----------------------------------------------------------------





    // -------------------------- Start -------------------------------
    void Start()
    {
        Init();
    }

    // ----------------------------------------------------------------



    // ------------------------ Override ------------------------------
    public override void InterAct() // NPC : ��ȣ�ۿ�(��ȭ)
    {
        _cinemachineController.STATE = 
            CinemachineController.CamState.Conversation;

        Managers.UI.CloseAllPopupUI();

        // ��ȭ �� ���� ���
        Managers.Sound.Play("Effect/Conversation");

        // �÷��̾��� �������� ���� ��ȯ
        transform.LookAt(Managers.Object.MyPlayer.transform);

        Conversation();
    }

    // ----------------------------------------------------------------
}
