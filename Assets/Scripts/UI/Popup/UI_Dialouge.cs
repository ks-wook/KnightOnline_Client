using Assets.Scripts.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Dialouge : UI_Base
{
    [SerializeField]
    [Tooltip("대화 텍스트 속도")]
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

    // 스크립트 진행 처리
    public void ConversationProceed()
    {
        if (NPCdialouge.Length > _dialougeIdx) // 뒤에 대사가 더 있는 경우
        {
            if(DialogueCoroutine == null)
            {
                GetText((int)Texts.DialogueText).text = "";
                DialogueCoroutine = StartCoroutine("TypeLine", NPCdialouge[_dialougeIdx++]);
            }

        }
        else // 대사가 끝난 경우
        {
            _dialougeIdx = 0;
            GetText((int)Texts.DialogueText).text = "";

            Managers.UI.CloseAllPopupUI(); // 대화창 UI 비활성화
            Managers.Object.MyPlayer.OnEndConversation(); // 대화종료를 플레이어에게 알림
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

    // Dialogue UI 클릭 시 이벤트
    public void OnDialougeBackgroundClick(PointerEventData evt)
    {
        if (NPCdialouge == null)
            return;

        ConversationProceed();
    }
}
