using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class UI_InteractPopup : UI_Popup
{
    // Popup
    public UI_Interact InteractUI { get; private set; }

    public override void Init()
    {
        base.Init();


        InteractUI = GetComponentInChildren<UI_Interact>();
    }
}

