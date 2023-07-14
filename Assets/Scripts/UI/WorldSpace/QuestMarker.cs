using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 퀘스트 진행 상태를 시각적으로 보여주기 위한 퀘스트 마커 스크립트
 */

public class QuestMarker : MonoBehaviour
{
    [Tooltip("퀘스트 마커의 텍스트")]
    [SerializeField]
    Text MarkerText;

    public void SetMarker(string mark, NPCObject npc)
    {
        if (mark == "!")
        {
            npc.GetRewardEnable = false;
        }
        else if (mark == "?")
        {
            npc.GetRewardEnable = true;
        }
        else
        {
            npc.GetRewardEnable = false;
        }

        MarkerText.text = mark;

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

    void Update()
    {
        // ui 방향 조절
        transform.rotation = Camera.main.transform.rotation;
    }
}
