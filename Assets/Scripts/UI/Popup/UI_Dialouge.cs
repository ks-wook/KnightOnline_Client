using Assets.Scripts.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Dialouge : UI_Base
{
    [SerializeField]
    [Tooltip("��ȭ �ؽ�Ʈ �ӵ�")]
    float TextSpeed = 0.005f;

    [HideInInspector]
    Coroutine DialogueCoroutine;

    [HideInInspector]
    public string _npcName = "NPC";
    public string NPCName
    {
        get { return _npcName; }
        set
        {
            if (_npcName == value)
                return;

            _npcName = value;
            GetText((int)Texts.NameText).text = _npcName;
        }
    }

    [HideInInspector]
    public string[] NPCdialouge = null;
    int _dialougeIdx = 0;

    enum Texts
    {
        DialogueText,
        NameText
    }

    enum Images
    {
        DialougeBackground,
    }

    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));

        GetText((int)Texts.DialogueText).gameObject.BindEvent(OnDialougeBackgroundClick);
    }

    // ��ũ��Ʈ ���� ó��
    public void ConversationProceed()
    {
        if (NPCdialouge.Length > _dialougeIdx) // �ڿ� ��簡 �� �ִ� ���
        {
            if(DialogueCoroutine == null)
            {
                GetText((int)Texts.DialogueText).text = "";
                DialogueCoroutine = StartCoroutine("TypeLine", NPCdialouge[_dialougeIdx++]);
            }

        }
        else // ��簡 ���� ���
        {
            _dialougeIdx = 0;
            GetText((int)Texts.DialogueText).text = "";

            Managers.UI.CloseAllPopupUI(); // ��ȭâ UI ��Ȱ��ȭ
            Managers.Object.MyPlayer.OnEndConversation(); // ��ȭ���Ḧ �÷��̾�� �˸�
        }
    }

    IEnumerator TypeLine(string script)
    {
        Text dialogueText = GetText((int)Texts.DialogueText);

        foreach (char c in script.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(TextSpeed);
        }

        DialogueCoroutine = null;
    }

    // Dialogue UI Ŭ�� �� �̺�Ʈ
    public void OnDialougeBackgroundClick(PointerEventData evt)
    {
        if (NPCdialouge == null)
            return;

        ConversationProceed();
    }
}
