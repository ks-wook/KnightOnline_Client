using Assets.Scripts.Controller;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * ���� ������ ���� UI Ȱ��ȭ�� ���� ��Ż ������Ʈ�� ��ũ��Ʈ
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
    [Tooltip("��ȣ �ۿ�� �̺�Ʈ�� �߻���ų ���")]
    Gimmick gimmickTarget;

    [SerializeField]
    [Tooltip("��Ż ��ȣ �ۿ�� �ߵ��Ǵ� �̺�Ʈ�� ����")]
    PortalType type;


    void Init()
    {
        ObjectName = "Portal";
    }


    void PortalOn()
    {
        // ���� ����� �̿��Ͽ� ī�޶� ���� ��ȯ
        if (gimmickTarget != null) // ������ ����� �ִٸ�
            gimmickTarget.CountUp(); // ����� ����� �̺�Ʈ �߻�

        switch (type)
        {
            case PortalType.AdventureUITrigger: // ���� ���� UI On
                AdventureUIOn();
                break;
            case PortalType.ReturnToMainStageToLobby: // ���� Ŭ���� �� �κ�� ��ȯ
                ReturnToLobby();
                break;
            case PortalType.ReturnToRaidToLobby: // ���̵� Ŭ���� �� �κ�� ��ȯ
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
        if(sendLeavePacket) // ���� ���� ������ ���� �˷����ϴ� ���
        {
            // ���̵�� ���� ��Ƽ ���������� �κ�� ������ ��� ���� ���� ��Ŷ�� ��������
            C_LeaveGame leavePacket = new C_LeaveGame();
            Managers.Network.Send(leavePacket);
        }

        // �κ�� �̵�
        Managers.Scene.LoadScene(Define.Scene.Lobby1);

        C_EnterLobby enterLobbyPacket = new C_EnterLobby();
        enterLobbyPacket.Name = PacketHandler.PlayerName;
        enterLobbyPacket.IsGameToLobby = true;
        Managers.Network.Send(enterLobbyPacket);
    }

    /*void ReturnToLobby()
    {
        // �κ�� �̵�
        Managers.Scene.LoadScene(Define.Scene.Lobby1);

        C_EnterLobby enterLobbyPacket = new C_EnterLobby();
        enterLobbyPacket.Name = PacketHandler.PlayerName;
        enterLobbyPacket.IsGameToLobby = true;
        Managers.Network.Send(enterLobbyPacket);
    }*/

    IEnumerator CoAdventureUIOn()
    {
        yield return new WaitForSeconds(2f);

        // ���� ���� UI ON
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
