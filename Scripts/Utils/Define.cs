using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public enum Scene
    {
        Unknown,
        Login,
        Lobby1,
        Game,
    }

    public enum CamaraMode
    {
        TPSView,
        NPC,

    }

    public enum UIEvent
    {
        Click,
        Drag,
    }

}
