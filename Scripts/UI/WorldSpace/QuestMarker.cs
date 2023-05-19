using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestMarker : MonoBehaviour
{


    private void Start()
    {

    }

    void Update()
    {
        // ui 방향 조절
        transform.rotation = Camera.main.transform.rotation;
    }

    public void SetMarker(string mark)
    {
        transform.Find("MarkerText").GetComponent<Text>().text = mark;
    }

    public void SetColor(string color)
    {
        if(color == "yellow")
        {

        }
        else if(color == "gray")
        {

        }
    }

}
