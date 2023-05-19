using Assets.Scripts.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTrigger : InterActionTrigger
{
    Transform cinemacineController;

    private void Start()
    {
        cinemacineController = GameObject.Find("CinemachineController").transform;
        ObjectName = "Portal";
        ObjectID = 1;
    }


    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Lobby scene ui on");
        if(Managers.UI.SCENETYPE == Define.Scene.Lobby1)
        {
            UI_LobbyScene lobbyScene = Managers.UI.SceneUI as UI_LobbyScene;
            UI_Adventure adventureUI = lobbyScene.AdventureUI;

            cinemacineController.GetComponent<CinemachineController>().setCinemachineAnim("Lobby1Portal");
            
            adventureUI.gameObject.SetActive(true);
            adventureUI.RefreshUI();
        }
    }

    public override void OnEndInterAct()
    {
        
    }
}
