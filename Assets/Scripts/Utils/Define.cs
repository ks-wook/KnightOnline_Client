using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public enum Scene
    {
        Unknown,
        Login,
        Loading,
        Lobby1,
        Game,
        RaidBoss,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

    public enum UIEvent
    {
        Click,
        Drag,
    }

    public enum MonsterType
    {
        Skeleton_Soldier,
        Giant_Ork,
        Rock_Golem,
    }

    public enum TargetType
    {
        Object,
        Monster,
    }

    public enum OpenType
    {
        Up,
        Down,
    }

}
