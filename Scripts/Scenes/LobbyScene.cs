using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : BaseScene
{
    UI_LobbyScene _sceneUI;

    void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Lobby1;
        Managers.UI.SCENETYPE = Define.Scene.Lobby1;


        Screen.SetResolution(640, 480, false);

        _sceneUI = Managers.UI.ShowSceneUI<UI_LobbyScene>();
    }

    public override void Clear()
    {
        throw new System.NotImplementedException();
    }
}
