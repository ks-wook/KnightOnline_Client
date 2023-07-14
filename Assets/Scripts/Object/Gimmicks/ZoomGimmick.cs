using Assets.Scripts.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ��� ���۽��� �̺�Ʈ�� ������ �����ϴ� ��ũ��Ʈ
 */

public class ZoomGimmick : Gimmick
{
    [Header("Target")]

    [SerializeField]
    [Tooltip("������ ��ü�� Ÿ��")]
    Define.TargetType targetType;

    [SerializeField]
    [Tooltip("������ ��ü�� �ð�")]
    float duration;

    [SerializeField]
    [Tooltip("���� �� ī�޶� ���� ����")]
    bool isFix = false;

    [SerializeField]
    [Tooltip("���� �� ��� �ı� ����(1ȸ������ ������ ��������)")]
    bool isOnce;

    MonsterController _monsterController;

    protected override void Init()
    {
        base.Init();

        switch (targetType)
        {
            case Define.TargetType.Monster:
                _monsterController = camTarget.GetComponentInChildren<MonsterController>();
                break;
            case Define.TargetType.Object:
                break;
        }

    }

    public override void CountUp()
    {
        if (++Condition >= GimmicConditionCount)
        {
            Zoom();
        }
    }

    // ī�޶� ���� �̺�Ʈ
    protected override void Zoom()
    {
        if(camTarget != null)
            camTarget = camTarget.Find("Focus"); // Ÿ���� �Ʒ��� "Focus"��� �̸��� ��ü�� �־����
        
        if(_cinemachineController != null)
            _cinemachineController.ZoomFocus(camTarget, camPosition, duration, isFix);

        if (_monsterController != null) // ������ ��� Ư�� �ִϸ��̼��� �����ų �� �ִ�.
        {
            _monsterController.Animator.Play("Start");

            if (_monsterController.StartEffect != null)
            {
                _monsterController.StartEffect.gameObject.SetActive(true);
                _monsterController.StartEffect.Stop();
                _monsterController.StartEffect.Play();
            }

        }

        if(isOnce)
            Destroy(gameObject, duration + 1.0f);
    }

    // �÷��̾ Ʈ���Ÿ� ���� �� �̺�Ʈ�� ���۽�ų ���� �ִ�.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CountUp();
        }
    }


    // ------------------------------- Start ---------------------------------
    private void Start()
    {
        Init();
    }

    // ------------------------------------------------------------------------

}
