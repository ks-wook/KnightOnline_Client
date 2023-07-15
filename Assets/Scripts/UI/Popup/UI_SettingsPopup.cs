using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SettingsPopup : UI_Popup
{
    public UI_Settings SettingsUI { get; private set; }

    public override void Init()
    {
        base.Init();


        SettingsUI = GetComponentInChildren<UI_Settings>();
        SettingsUI.Popup = this;
    }
}
