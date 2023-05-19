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

    // ����Ʈ ������ ����
    public void SetQuestItem(Quest quest)
    {
        // ����Ʈ ���� �ʱ�ȭ
        if(QuestName != null && QuestTip != null)
        {
            QuestName.text = quest.Name;
            QuestTip.text = quest.Tip;
        }

        // ����Ʈ Ŭ����� '���� ȹ�� ����' UI�� Ȱ��ȭ
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
