using Assets.Scripts.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Dialouge : UI_Base
{
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
        Select1Text,
        Select2Text,
        NameText
    }

    enum Images
    {
        DialougeBackground,
        Select1Btn,
        Select2Btn
    }

    GameObject select1Btn;
    GameObject select2Btn;

    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));

        GetText((int)Texts.DialogueText).gameObject.BindEvent(OnDialougeBackgroundClick);


        select1Btn = GetImage((int)Texts.Select1Text).gameObject;
        select1Btn.BindEvent(OnSelect1BtnClick);
        select1Btn.SetActive(false);

        select2Btn = GetImage((int)Texts.Select2Text).gameObject;
        select2Btn.BindEvent(OnSelect1BtnClick);
        select2Btn.SetActive(false);
    }

    // 스크립트 진행 처리
    public void ConversationProceed()
    {
        if (NPCdialouge.Length > _dialougeIdx) // 뒤에 대사가 더 있는 경우
        {
            GetText((int)Texts.DialogueText).text = NPCdialouge[_dialougeIdx++];
        }
        else // 대사가 끝난 경우
        {
            _dialougeIdx = 0;
            GetText((int)Texts.DialogueText).text = "";

            Managers.UI.CloseAllPopupUI(); // 대화창 UI 비활성화
            Managers.Object.MyPlayer.OnEndConversation(); // 대화종료를 플레이어에게 알림
        }
    }

    // Dialogue UI 클릭 시 이벤트
    public void OnDialougeBackgroundClick(PointerEventData evt)
    {
        if (NPCdialouge == null)
            return;

        ConversationProceed();
    }


    public void OnSelect1BtnClick(PointerEventData evt)
    {
        // TODO
    }

    public void OnSelect2BtnClick(PointerEventData evt)
    {
        // TODO
    }
}
