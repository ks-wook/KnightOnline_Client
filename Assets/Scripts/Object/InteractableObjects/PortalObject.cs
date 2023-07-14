using Assets.Scripts.Controller;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * 던전 입장을 위한 UI 활성화를 위한 포탈 오브젝트의 스크립트
 */

public class PortalObject : InteractableObject
{
    enum PortalType
    {
        AdventureUITrigger,
        ReturnToMainStageToLobby,
        ReturnToRaidToLobby,
    }


    [Header("Gimmick")]

    [SerializeField]
    [Tooltip("상호 작용시 이벤트를 발생시킬 기믹")]
    Gimmick gimmickTarget;

    [SerializeField]
    [Tooltip("포탈 상호 작용시 발동되는 이벤트의 종류")]
    PortalType type;


    void Init()
    {
        ObjectName = "Portal";
    }


    void PortalOn()
    {
        // 줌인 기믹을 이용하여 카메라 시점 전환
        if (gimmickTarget != null) // 정해진 기믹이 있다면
            gimmickTarget.CountUp(); // 연결된 기믹의 이벤트 발생

        switch (type)
        {
            case PortalType.AdventureUITrigger: // 던전 선택 UI On
                AdventureUIOn();
                break;
            case PortalType.ReturnToMainStageToLobby: // 던전 클리어 후 로비로 귀환
                ReturnToLobby();
                break;
            case PortalType.ReturnToRaidToLobby: // 레이드 클리어 후 로비로 귀환
                ReturnToLobby(true);
                break;
        }
    }

    void AdventureUIOn()
    {
        StartCoroutine("CoAdventureUIOn");
    }

    void ReturnToLobby(bool sendLeavePacket = false)
    {
        if(sendLeavePacket) // 게임 에서 떠나야 함을 알려야하는 경우
        {
            // 레이드와 같이 멀티 컨텐츠에서 로비로 나가는 경우 게임 종료 패킷을 보내야함
            C_LeaveGame leavePacket = new C_LeaveGame();
            Managers.Network.Send(leavePacket);
        }

        // 로비로 이동
        Managers.Scene.LoadScene(Define.Scene.Lobby1);

        C_EnterLobby enterLobbyPacket = new C_EnterLobby();
        enterLobbyPacket.Name = PacketHandler.PlayerName;
        enterLobbyPacket.IsGameToLobby = true;
        Managers.Network.Send(enterLobbyPacket);
    }

    /*void ReturnToLobby()
    {
        // 로비로 이동
        Managers.Scene.LoadScene(Define.Scene.Lobby1);

        C_EnterLobby enterLobbyPacket = new C_EnterLobby();
        enterLobbyPacket.Name = PacketHandler.PlayerName;
        enterLobbyPacket.IsGameToLobby = true;
        Managers.Network.Send(enterLobbyPacket);
    }*/

    IEnumerator CoAdventureUIOn()
    {
        yield return new WaitForSeconds(2f);

        // 던전 선택 UI ON
        if (Managers.UI.SCENETYPE == Define.Scene.Lobby1)
        {
            UI_LobbyScene lobbyScene = Managers.UI.SceneUI as UI_LobbyScene;
            UI_Adventure adventureUI = lobbyScene.AdventureUI;

            adventureUI.gameObject.SetActive(true);
        }
    }





    // -------------------------- Start -------------------------------
    void Start()
    {
        Init();
    }

    // ----------------------------------------------------------------



    // ------------------------ Override ------------------------------
    public override void InterAct()
    {
        Managers.UI.CloseAllPopupUI();
        PortalOn();
    }

    // ----------------------------------------------------------------

}
