using Assets.Scripts.Controller;
using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * �κ񿡼� ��Ż�� ��ȣ �ۿ� �� Ȱ��ȭ �Ǵ� ���� ���� â UI�� ���� ��ũ��Ʈ
 */


public class UI_Adventure : UI_Base
{

    [SerializeField]
    [Tooltip("���ν������� ���� ��ư")]
    List<UI_MainStageBtn> _mainStageBtns;


    GameObject Buttons;
    GameObject MainStage;
    GameObject BossRaid;


    enum Images
    {
        Buttons,
        MainStage,
        BossRaid,

        MainStageBtn,

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

        Buttons = GetImage((int)Images.Buttons).gameObject;
        MainStage = GetImage((int)Images.MainStage).gameObject;
        BossRaid = GetImage((int)Images.BossRaid).gameObject;

        ClearUI();
    }

    public void OnClickMainStageBtn(PointerEventData evt)
    {
        // ���ν������� UI ON
        Buttons.SetActive(false);
        MainStage.SetActive(true);
        BossRaid.SetActive(false);

        // Ŭ���� ���� �ݿ��Ͽ� UI ������Ʈ
        foreach (UI_MainStageBtn btn in _mainStageBtns)
        {
            btn.RefreshUI();
        }
    }

    // �������� ���� ó��
    public void OnClickEnterStageBtn(PointerEventData evt, string stageName)
    {

        // ������ C_EnterGame ����
        C_EnterGame enterGamePacket = new C_EnterGame();
        enterGamePacket.Name = Managers.Object.MyPlayer.name;
        enterGamePacket.RoomNum = 0;
        Managers.Network.Send(enterGamePacket);



        // �� �ε�
        Managers.Scene.LoadScene(stageName);



        // TODO : �� �ε� �� �񵿱� �ε� ����









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
        GameObject.Find("CinemachineController").GetComponent<CinemachineController>().STATE = 
            CinemachineController.CamState.TPS;

        
        transform.gameObject.SetActive(false);
    }

    // ���̵� ��Ī ��û
    public void OnClickRaidMatchBtn(PointerEventData evt)
    {
        C_RaidMatch raidReq = new C_RaidMatch();
        raidReq.Req = true; // true �� �� ��û
        raidReq.Name = PacketHandler.PlayerName;
        Managers.Network.Send(raidReq);

        Managers.UI.ShowPopupUI<UI_RaidMatchPopup>();

        Debug.Log("��Ī ��û ��Ŷ ����");
    }

    public void ClearUI()
    {
        Buttons.SetActive(true);
        MainStage.SetActive(false);
        BossRaid.SetActive(false);
    }
}
