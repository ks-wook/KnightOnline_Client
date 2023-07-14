using Assets.Scripts.Controller;
using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * 로비에서 포탈과 상호 작용 시 활성화 되는 던전 선택 창 UI에 사용된 스크립트
 */


public class UI_Adventure : UI_Base
{

    [SerializeField]
    [Tooltip("메인스테이지 선택 버튼")]
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
        // 메인스테이지 UI ON
        Buttons.SetActive(false);
        MainStage.SetActive(true);
        BossRaid.SetActive(false);

        // 클리어 정보 반영하여 UI 업데이트
        foreach (UI_MainStageBtn btn in _mainStageBtns)
        {
            btn.RefreshUI();
        }
    }

    // 스테이지 입장 처리
    public void OnClickEnterStageBtn(PointerEventData evt, string stageName)
    {

        // 서버로 C_EnterGame 전송
        C_EnterGame enterGamePacket = new C_EnterGame();
        enterGamePacket.Name = Managers.Object.MyPlayer.name;
        enterGamePacket.RoomNum = 0;
        Managers.Network.Send(enterGamePacket);



        // 씬 로드
        Managers.Scene.LoadScene(stageName);



        // TODO : 씬 로딩 시 비동기 로딩 구현









    }

    public void OnClickRaidBtn(PointerEventData evt)
    {
        // 레이드 UI ON
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

    // 레이드 매칭 요청
    public void OnClickRaidMatchBtn(PointerEventData evt)
    {
        C_RaidMatch raidReq = new C_RaidMatch();
        raidReq.Req = true; // true 일 때 요청
        raidReq.Name = PacketHandler.PlayerName;
        Managers.Network.Send(raidReq);

        Managers.UI.ShowPopupUI<UI_RaidMatchPopup>();

        Debug.Log("매칭 요청 패킷 전송");
    }

    public void ClearUI()
    {
        Buttons.SetActive(true);
        MainStage.SetActive(false);
        BossRaid.SetActive(false);
    }
}
