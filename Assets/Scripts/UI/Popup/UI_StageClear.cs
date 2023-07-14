using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_StageClear : UI_Base
{
    enum Images
    {
        Background,
    }

    public override void Init()
    {
        Bind<Image>(typeof(Images));

        GetImage((int)Images.Background).gameObject.BindEvent(OnClickBackground);

        // 클리어 사운드 재생

        Managers.Sound.StopBgm();
        Managers.Sound.Play("UI/StageClear");
    }

    public void OnClickBackground(PointerEventData evt)
    {
        Managers.UI.CloseAllPopupUI();
    }
}
