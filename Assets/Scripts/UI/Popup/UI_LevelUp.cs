using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_LevelUp : UI_Base
{
    [HideInInspector]
    public UI_Popup Popup;

    enum Texts
    {
        NewLevelText,
        PreMaxHpText,
        PreAttackText,
        NewMaxHpText,
        NewAttackText
    }

    enum Images
    {
        Background
    }
    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

        GetImage((int)Images.Background).gameObject.BindEvent(OnClickBackground);


        // TODO : �ɷ�ġ ���� ����



    }

    public void OnClickBackground(PointerEventData evt)
    {
        Managers.UI.ClosePopupUI(Popup);
    }



    // �������� ���� �ɷ�ġ ��ȭ�� ǥ��
    public void SetStatTexts(int newLevel)
    {
        GetText((int)Texts.NewLevelText).text = "Lv." + newLevel.ToString();

        StatInfo preStat = null;
        Managers.Data.StatDict.TryGetValue(newLevel - 1, out preStat);

        StatInfo newStat = null;
        Managers.Data.StatDict.TryGetValue(newLevel, out newStat);

        GetText((int)Texts.PreMaxHpText).text = preStat.MaxHp.ToString();
        GetText((int)Texts.PreAttackText).text = preStat.Attack.ToString();

        GetText((int)Texts.NewMaxHpText).text = newStat.MaxHp.ToString();
        GetText((int)Texts.NewAttackText).text = newStat.Attack.ToString();
    }
}
