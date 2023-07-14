using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_StageClearPopup : UI_Popup
{
    public UI_StageClear StageClearUI { get; private set; }

    public override void Init()
    {
        base.Init();


        StageClearUI = GetComponentInChildren<UI_StageClear>();
    }
}
