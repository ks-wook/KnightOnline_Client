using Assets.Scripts.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Interact : UI_Base
{
    enum Texts
    {
        InterActText
    }

    public override void Init()
    {
        Bind<Text>(typeof(Texts));
    }

}