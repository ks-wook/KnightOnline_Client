using Assets.Scripts.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * �������� ������ ���� ��� ��ũ��Ʈ�� Base
 * 
 * ��� ��͵��� �� ��ũ��Ʈ�� ��ӹ޴´�.
 */


public class Gimmick : MonoBehaviour
{
    [Header("Gimmick Info")]

    [SerializeField]
    [Tooltip("����� ī��Ʈ�� �� ����ŭ ���� �� ��� �̺�Ʈ ����")]
    protected int GimmicConditionCount;

    [SerializeField]
    [Tooltip("�̺�Ʈ �Ϸ� �� ������ ���� EventTransfer ī�޶��� local position")]
    protected Vector3 camPosition;

    [SerializeField]
    [Tooltip("������ Ÿ�� (null�� ��� �ڽ��� Ÿ������ ��)")]
    protected Transform camTarget;


    [HideInInspector]
    public int Condition = 0; // �ش� ���� �޼� �� �̺�Ʈ �߻�

    [HideInInspector]
    protected CinemachineController _cinemachineController;


    protected virtual void Init()
    {
        GameObject.Find("CinemachineController").TryGetComponent<CinemachineController>(out _cinemachineController);
    }



    // -------------------------- Gimmick Handler -----------------------------
    // condition count ó��
    public virtual void CountUp() { }

    // ------------------------------------------------------------------------


    // ---------------------------- Event Handler -----------------------------
    // ������ ��� �̺�Ʈ
    protected virtual void Explosion() { }

    // ������ ��� �̺�Ʈ
    protected virtual void Open() { }

    // ���� ���� �� �� ȿ�� �̺�Ʈ
    protected virtual void Zoom() { }

    // ------------------------------------------------------------------------

}
