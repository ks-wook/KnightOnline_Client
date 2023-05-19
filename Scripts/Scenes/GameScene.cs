using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    UI_GameScene _sceneUI;
    

    void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;
        Managers.UI.SCENETYPE = Define.Scene.Game;


        Screen.SetResolution(640, 480, false);

        _sceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();
    }

    public override void Clear()
    {
        throw new System.NotImplementedException();
    }

}
