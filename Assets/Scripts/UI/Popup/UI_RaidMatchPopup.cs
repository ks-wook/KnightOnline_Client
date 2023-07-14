using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class UI_RaidMatchPopup : UI_Popup
{
    // Popup
    public UI_RaidMatch RaidMatchUI { get; private set; }

    public override void Init()
    {
        base.Init();

        RaidMatchUI = GetComponentInChildren<UI_RaidMatch>();

    }
}

