using Assets.Scripts.Controller;
using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Adventure : UI_Base
{
    GameObject Buttons;
    GameObject MainStage;
    GameObject BossRaid;


    enum Images
    {
        Buttons,
        MainStage,
        BossRaid,

        MainStageBtn,
        MainStage1_1,
        MainStage1_2,
        MainStage1_3,

        RaidBtn,
        RaidMatchBtn,
        CloseBtn
    }

    public override void Init()
    {

        Bind<Image>(typeof(Images));

        GetImage((int)Images.MainStageBtn).gameObject.BindEvent(OnClickMainStageBtn);
        GetImage((int)Images.RaidBtn).gameObject.BindEvent(OnClickRaidBtn);
        GetImage((int)Images.RaidMatchBtn).gameObject.BindEvent(OnClickRaidMatchBtn);
        GetImage((int)Images.CloseBtn).gameObject.BindEvent(OnClickCloseBtn);

        // �������� ��ư ���ε�
        // GetImage((int)Images.MainStage1_1).gameObject.BindEvent(OnClickButton);
        // GetImage((int)Images.MainStage1_2).gameObject.BindEvent(OnClickButton);
        // GetImage((int)Images.MainStage1_3).gameObject.BindEvent(OnClickButton);

        Buttons = GetImage((int)Images.Buttons).gameObject;
        MainStage = GetImage((int)Images.MainStage).gameObject;
        BossRaid = GetImage((int)Images.BossRaid).gameObject;

        ClearUI();
        RefreshUI();
    }

    public void OnClickMainStageBtn(PointerEventData evt)
    {
        // ���̵� UI ON
        Buttons.SetActive(false);
        MainStage.SetActive(true);
        BossRaid.SetActive(false);
    }


    public void OnClickRaidBtn(PointerEventData evt)
    {
        // ���̵� UI ON
        Buttons.SetActive(false);
        MainStage.SetActive(false);
        BossRaid.SetActive(true);
    }

    // UI off
    public void OnClickCloseBtn(PointerEventData evt)
    {
        GameObject.Find("CinemachineController").GetComponent<CinemachineController>().setCinemachineAnim("TPS");
        transform.gameObject.SetActive(false);
    }

    // ���̵� ��Ī ��û
    public void OnClickRaidMatchBtn(PointerEventData evt)
    {

        C_RaidMatch raidReq = new C_RaidMatch();
        raidReq.Req = true; // true �� �� ��û
        raidReq.Name = PacketHandler.PlayerName;
        Managers.Network.Send(raidReq);

        Managers.UI.ShowPopupUI<UI_LobbyPopup>();

        Debug.Log("��Ī ��û ��Ŷ ����");
    }

    public void ClearUI()
    {
        Buttons.SetActive(true);
        MainStage.SetActive(false);
        BossRaid.SetActive(false);
    }

    public void RefreshUI()
    {
        // TODO : ������ ���̽����� �������� Ŭ���� ������ �޾ƿ� UI�� �Ѹ�
    }
}
