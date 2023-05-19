using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Button : MonoBehaviour
{
    enum Buttons
    {
        PointButton
    }

    enum Texts
    {
        PointText,
        Scoretext
    }

    private void Start()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
    }

    void Bind<T>(Type type)
    {
        string[] names = Enum.GetNames(type);


    }


    int _score = 0;

    public void OnButtonClicked()
    {

    }
}
