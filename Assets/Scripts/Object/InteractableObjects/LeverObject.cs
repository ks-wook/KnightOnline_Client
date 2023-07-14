using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * �÷��̾�� ��ȣ �ۿ� ������ ���� ������Ʈ�� ��ũ��Ʈ
 * 
 * �� ������Ʈ�� ��� ��ũ��Ʈ�� �����Ͽ� ����� �̺�Ʈ�� �߻�
 * ��Ű�� ������ �Ѵ�
 */


public class LeverObject : InteractableObject
{
    [Header("Gimmick")]

    [SerializeField]
    [Tooltip("��ȣ �ۿ�� �̺�Ʈ�� �߻���ų ���")]
    Gimmick gimmickTarget;


    void Init()
    {
        ObjectName = "Lever";
    }

    void LeverOn()
    {
        if (gimmickTarget != null) // ������ ����� �ִٸ�
            gimmickTarget.CountUp(); // ����� ����� �̺�Ʈ �߻�
    }

    // -------------------------- Start -------------------------------
    void Start()
    {
        Init();
    }

    // ----------------------------------------------------------------



    // ------------------------ Override ------------------------------
    public override void InterAct() // Lever : ��ȣ�ۿ�(����� ��� �̺�Ʈ �߻�)
    {
        Managers.UI.CloseAllPopupUI();
        LeverOn();
    }

    // ----------------------------------------------------------------
}
