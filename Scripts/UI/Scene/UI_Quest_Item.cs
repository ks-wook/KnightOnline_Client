using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static QuestManager;

public class UI_Quest_Item : UI_Base
{
    private Text QuestName;
    private Text QuestTip;
    private Image QuestClear;

    enum Texts
    {
        QuestNameText,
        QuestTipText
    }

    enum Images
    {
        QuestClearImg
    }

    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

        QuestName = GetText((int)Texts.QuestNameText);
        QuestTip = GetText((int)Texts.QuestTipText);
        QuestClear = GetImage((int)Images.QuestClearImg);

    }

    // 퀘스트 아이템 셋팅
    public void SetQuestItem(Quest quest)
    {
        // 퀘스트 정보 초기화
        if(QuestName != null && QuestTip != null)
        {
            QuestName.text = quest.Name;
            QuestTip.text = quest.Tip;
        }

        // 퀘스트 클리어시 '보상 획득 가능' UI를 활성화
        if (quest.IsCleared)
        {
            QuestClear.gameObject.SetActive(true);
        }
        else
        {
            QuestClear.gameObject.SetActive(false);
        }
    }
}
