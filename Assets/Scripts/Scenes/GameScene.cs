using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 던전 씬 전환시의 초기화를 위한 스크립트
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


        // 메인스테이지와 레이드 보스는 같은 타입의 씬이나, 서로 이름은 다르므로
        string curSceneName = Managers.Scene.GetCurrentSceneName();

        if (curSceneName == "RaidBoss") // 현재 씬이 레이드인 경우에만 실행
        {
            _sceneUI.BossStatusUI.gameObject.SetActive(true);
            Managers.RaidGame.Init();
        }

        // 씬에 맞는 bgm 재생
        Managers.Sound.Play("Bgm/" + curSceneName, Define.Sound.Bgm);
    }

    public override void Clear()
    {
        
    }

}
