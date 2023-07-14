using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 플레이어와 상호 작용 가능한 레버 오브젝트의 스크립트
 * 
 * 이 오브젝트는 기믹 스크립트와 연결하여 기믹의 이벤트를 발생
 * 시키는 역할을 한다
 */


public class LeverObject : InteractableObject
{
    [Header("Gimmick")]

    [SerializeField]
    [Tooltip("상호 작용시 이벤트를 발생시킬 기믹")]
    Gimmick gimmickTarget;


    void Init()
    {
        ObjectName = "Lever";
    }

    void LeverOn()
    {
        if (gimmickTarget != null) // 정해진 기믹이 있다면
            gimmickTarget.CountUp(); // 연결된 기믹의 이벤트 발생
    }

    // -------------------------- Start -------------------------------
    void Start()
    {
        Init();
    }

    // ----------------------------------------------------------------



    // ------------------------ Override ------------------------------
    public override void InterAct() // Lever : 상호작용(연결된 기믹 이벤트 발생)
    {
        Managers.UI.CloseAllPopupUI();
        LeverOn();
    }

    // ----------------------------------------------------------------
}
