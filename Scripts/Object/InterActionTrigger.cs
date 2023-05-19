using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InterActionTrigger : MonoBehaviour
{
    public string ObjectName { get; set; }
    public int ObjectID { get; set; }

    public enum ObjectState
    {
        Idle,
        Talk,

    }

    public abstract void OnEndInterAct();
}
