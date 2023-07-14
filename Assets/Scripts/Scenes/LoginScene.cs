using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 로그인 화면에서의 초기화를 위한 스크립트
 */


public class LoginScene : BaseScene
{
    UI_LoginScene _sceneUI;


    void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();


        SceneType = Define.Scene.Login;
        Managers.UI.SCENETYPE = Define.Scene.Login;

        Screen.SetResolution(800, 500, false);

        _sceneUI = Managers.UI.ShowSceneUI<UI_LoginScene>();

        string curSceneName = Managers.Scene.GetCurrentSceneName();

        // 씬에 맞는 bgm 재생
        Managers.Sound.Play("Bgm/" + curSceneName, Define.Sound.Bgm);
    }

    public override void Clear()
    {

    }

}
