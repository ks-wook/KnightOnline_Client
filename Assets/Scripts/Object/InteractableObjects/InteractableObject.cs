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

    // 상호작용 처리 (필수 구현)
    public abstract void InterAct();
}
