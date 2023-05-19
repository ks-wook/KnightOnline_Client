using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputManager
{
    public Action KeyAction = null;

    public void OnUpdate()
    {
        if(Input.GetMouseButton(0))
        {
            Debug.Log("attack");
            return;
        }

        if (Input.anyKey == false)
            return;

        if (KeyAction != null)
            KeyAction.Invoke();
    }
}
