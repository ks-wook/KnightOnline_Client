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

    public enum TriggerState
    {
        Idle,
        Enter,
        Stay,
        Battle, // Battle ���´� ���� Ʈ������ ��� ���Ǵ� ����
        AggroLost, // ���� Ʈ���ſ��� ��׷θ� �Ұ� ���ڸ��� ���ư����ϴ� ����
        Exit
    }

    TriggerState triggerState = TriggerState.Idle;
    public TriggerState TRIGGER_STATE
    {
        get { return triggerState;  }
        set
        {
            if (triggerState == value)
                return;

            triggerState = value;
        }
    }

    public abstract void InterAct();
    
    protected virtual void OnTriggerEnter(Collider other) { }

    protected virtual void OnTriggerStay(Collider other) { }

    protected virtual void OnTriggerExit(Collider other) { }


    // ��ȣ�ۿ� ������ ������Ʈ���� �̺�Ʈ�� ����Ǿ��� �� ȣ��
    public virtual void OnEndConversation() { }

    // Ʈ���� ���¿� ���� ������Ʈ �Լ�
    public virtual void UpdateTriggerState() { }
}
