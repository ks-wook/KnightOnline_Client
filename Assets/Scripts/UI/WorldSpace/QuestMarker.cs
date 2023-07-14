using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * ����Ʈ ���� ���¸� �ð������� �����ֱ� ���� ����Ʈ ��Ŀ ��ũ��Ʈ
 */

public class QuestMarker : MonoBehaviour
{
    [Tooltip("����Ʈ ��Ŀ�� �ؽ�Ʈ")]
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
        // ui ���� ����
        transform.rotation = Camera.main.transform.rotation;
    }
}
