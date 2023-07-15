using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 상호작용이 가능한 오브젝트들의 스크립트의 가장 Base인 스크립트로
 * 
 * 상호작용 가능한 모든 오브젝트들은 이 스크립트를 상속받아 사용한다.
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
        Battle, // Battle 상태는 몬스터 트리거의 경우 사용되는 상태
        AggroLost, // 몬스터 트리거에서 어그로를 잃고 제자리로 돌아가야하는 상태
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


    // 상호작용 가능한 오브젝트에서 이벤트가 종료되었을 때 호출
    public virtual void OnEndConversation() { }

    // 트리거 상태에 따른 업데이트 함수
    public virtual void UpdateTriggerState() { }
}
