using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_RewardPopup : UI_Popup
{
    public UI_Reward RewardUI { get; private set; }

    public override void Init()
    {
        base.Init();


        RewardUI = GetComponentInChildren<UI_Reward>();
        RewardUI.Popup = this;
    }



}
