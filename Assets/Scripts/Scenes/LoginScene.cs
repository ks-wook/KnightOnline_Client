using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * �α��� ȭ�鿡���� �ʱ�ȭ�� ���� ��ũ��Ʈ
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

        // ���� �´� bgm ���
        Managers.Sound.Play("Bgm/" + curSceneName, Define.Sound.Bgm);
    }

    public override void Clear()
    {

    }

}
