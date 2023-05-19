using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{

    private void Start()
    {

    }

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

        if(ratio == 0)
        {
            Debug.Log($"set hp 0 id : {transform.parent.GetComponent<CreatureController>().Id}");
            // 사망처리 TODO
        }
    }
}
