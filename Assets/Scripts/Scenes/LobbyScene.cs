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


        Screen.SetResolution(800, 500, false);

        _sceneUI = Managers.UI.ShowSceneUI<UI_LobbyScene>();

        // 오브젝트 매니저에 NPC들을 추가 한다.
        Managers.Object.GetNPC();


        string curSceneName = Managers.Scene.GetCurrentSceneName();

        // 씬에 맞는 bgm 재생
        Managers.Sound.Play("Bgm/" + curSceneName, Define.Sound.Bgm);
    }

    public override void Clear()
    {
        
    }
}
