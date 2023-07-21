using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ��ȣ�ۿ��� ������ ������Ʈ���� ��ũ��Ʈ�� ���� Base�� ��ũ��Ʈ��
 * 
 * ��ȣ�ۿ� ������ ��� ������Ʈ���� �� ��ũ��Ʈ�� ��ӹ޾� ����Ѵ�.
*/

public abstract class InteractableObject : MonoBehaviour
{
    public string ObjectName { get; set; }
    public int ObjectID { get; set; }

    public enum ObjectState
    {
        Idle,
        Active,
    }

    // ��ȣ�ۿ� ó�� (�ʼ� ����)
    public abstract void InterAct();
}
