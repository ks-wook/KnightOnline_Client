using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * ����â UI�� ��Ʈ���ϱ� ���� ����ϴ� ��ũ��Ʈ
 */

public class UI_Settings : UI_Base
{
    [HideInInspector]
    public UI_Popup Popup;

    [SerializeField]
    [Tooltip("hp �� ��ġ")]
    Slider BGMSlier;

    [SerializeField]
    [Tooltip("hp �� ��ġ")]
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
        // ����â ����
        Managers.UI.ClosePopupUI(Popup);

        // ī�޶� ���� ��ȯ
        Managers.Object.MyPlayer.CinemachineController.STATE = 
            Assets.Scripts.Controller.CinemachineController.CamState.TPS;

    }

    public void OnClickGameCloseBtn(PointerEventData evt)
    {
        // ���� ����
        Application.Quit();
    }

    void OnBGMVolumeChange(float value)
    {
        // BGM ����� �ҽ� ���� ����
        Managers.Sound.ChangeVolume(Define.Sound.Bgm, value);
    }


    void OnSFXVolumeChange(float value)
    {
        // SFX ����� �ҽ� ���� ����
        Managers.Sound.ChangeVolume(Define.Sound.Effect, value);
    }
}
