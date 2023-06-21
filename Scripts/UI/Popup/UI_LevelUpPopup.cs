using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class UI_LevelUpPopup : UI_Popup
{
    // Popup
    public UI_LevelUp LevelUpUI { get; private set; }

    public override void Init()
    {
        base.Init();


        LevelUpUI = GetComponentInChildren<UI_LevelUp>();
    }

    public void SetLevelUpPop(int newLevel)
    {
        if(LevelUpUI != null)
            LevelUpUI.SetStatTexts(newLevel);
    }
}

