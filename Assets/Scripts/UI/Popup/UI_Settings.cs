using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * 설정창 UI를 컨트롤하기 위해 사용하는 스크립트
 */

public class UI_Settings : UI_Base
{
    [HideInInspector]
    public UI_Popup Popup;

    [SerializeField]
    [Tooltip("hp 바 위치")]
    Slider BGMSlier;

    [SerializeField]
    [Tooltip("hp 바 위치")]
    Slider SFXSlider;

    enum Images
    {
        CloseBtn,
        GameCloseBtn,
    }

    public override void Init()
    {
        Bind<Image>(typeof(Images));

        GetImage((int)Images.CloseBtn).gameObject.BindEvent(OnClickCloseBtn);
        GetImage((int)Images.GameCloseBtn).gameObject.BindEvent(OnClickGameCloseBtn);

        BGMSlier.onValueChanged.AddListener(OnBGMVolumeChange);
        SFXSlider.onValueChanged.AddListener(OnSFXVolumeChange);
    }

    public void OnClickCloseBtn(PointerEventData evt)
    {
        // 설정창 종료
        Managers.UI.ClosePopupUI(Popup);

        // 카메라 시점 전환
        Managers.Object.MyPlayer.CinemachineController.STATE = 
            Assets.Scripts.Controller.CinemachineController.CamState.TPS;

    }

    public void OnClickGameCloseBtn(PointerEventData evt)
    {
        // 게임 종료
        Application.Quit();
    }

    void OnBGMVolumeChange(float value)
    {
        // BGM 오디오 소스 볼륨 변경
        Managers.Sound.ChangeVolume(Define.Sound.Bgm, value);
    }


    void OnSFXVolumeChange(float value)
    {
        // SFX 오디오 소스 볼륨 변경
        Managers.Sound.ChangeVolume(Define.Sound.Effect, value);
    }
}
