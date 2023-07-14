using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class UI_DialougePopup : UI_Popup
{
    // Popup
    public UI_Dialouge DialougeUI { get; private set; }

    public override void Init()
    {
        base.Init();


        DialougeUI = GetComponentInChildren<UI_Dialouge>();
    }
}

