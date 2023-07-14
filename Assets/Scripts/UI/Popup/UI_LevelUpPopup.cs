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


        Managers.Sound.Play("UI/LevelUp");

        LevelUpUI = GetComponentInChildren<UI_LevelUp>();
        LevelUpUI.Popup = this;
    }

    public void SetLevelUpPop(int newLevel)
    {
        if(LevelUpUI != null)
            LevelUpUI.SetStatTexts(newLevel);
    }
}

