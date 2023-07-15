using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ���� �� ��ȯ���� �ʱ�ȭ�� ���� ��ũ��Ʈ
 */


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


        Screen.SetResolution(960, 540, false);

        _sceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();


        // ���ν��������� ���̵� ������ ���� Ÿ���� ���̳�, ���� �̸��� �ٸ��Ƿ�
        string curSceneName = Managers.Scene.GetCurrentSceneName();

        if (curSceneName == "RaidBoss") // ���� ���� ���̵��� ��쿡�� ����
        {
            _sceneUI.BossStatusUI.gameObject.SetActive(true);
            Managers.RaidGame.Init();
        }

        // ���� �´� bgm ���
        Managers.Sound.Play("Bgm/" + curSceneName, Define.Sound.Bgm);
    }

    public override void Clear()
    {
        
    }

}
