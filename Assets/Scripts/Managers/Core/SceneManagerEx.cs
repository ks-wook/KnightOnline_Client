using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * 씬을 전환하거나 현재 씬의 정보를 담장하는 매니저 스크립트
 */

public class SceneManagerEx
{
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    // 씬의 타입을 통해 씬을 로드하는 함수
    public void LoadScene(Define.Scene type)
    {
        Managers.SceneChangeClear();
        SceneManager.LoadScene(GetSceneName(type));
    }

    // 씬의 실제 이름을 통해 씬을 로드하는 함수
    public void LoadScene(string sceneName)
    {
        Managers.SceneChangeClear();
        SceneManager.LoadScene(sceneName);
    }


    // 타입을 통해 정의한 씬의 이름을 얻는 함수
    public string GetSceneName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        return name;
    }

    // 씬의 실제 이름을 얻는 함수
    public string GetCurrentSceneName()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        return sceneName;
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }
}
