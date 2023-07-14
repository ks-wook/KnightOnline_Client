using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{

    public float HPRatio
    {
        get { return transform.GetChild(0).GetComponent<Slider>().value; }
        set 
        {
            transform.GetChild(0).GetComponent<Slider>().value = value;
        }
    }

    void Update()
    {
        // ui 방향 조절
        transform.rotation = Camera.main.transform.rotation;
    }

    public void SetHpRatio(float ratio)
    {
        HPRatio = ratio;
    }
}
