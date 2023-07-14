using Assets.Scripts.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ��� ���۽��� �̺�Ʈ�� ���̳� öâ�� ���� ������Ʈ�� ������ ��ũ��Ʈ
 */

public class OpenGimmick : Gimmick
{
    [Header("Target")]

    [SerializeField]
    [Tooltip("���� �� ���� ������Ʈ")]
    Transform openTarget;

    [SerializeField]
    [Tooltip("������ ������ Ÿ��")]
    Define.OpenType openType;

    [SerializeField]
    [Tooltip("������Ʈ�� ������ �����µ��� �ɸ��� �ð�")]
    float duration;

    [SerializeField]
    [Tooltip("������Ʈ�� ������ ����")]
    float amount;

    [SerializeField]
    [Tooltip("������Ʈ�� ������ �ӵ� (0�� ����� ���� ������)")]
    float accel;

    Vector3 destPosition;

    protected override void Init()
    {
        base.Init();

        switch (openType)
        {
            case Define.OpenType.Up:
                destPosition = openTarget.localPosition + new Vector3(0, amount, 0);
                break;
            case Define.OpenType.Down:
                destPosition = openTarget.localPosition - new Vector3(0, amount, 0);
                break;
        }


    }

    public override void CountUp()
    {
        if (++Condition >= GimmicConditionCount)
        {
            Open();
        }
    }

    protected override void Open()
    {
        // TODO : ī�޶� ���� ȿ��
        Transform focus = camTarget.Find("Focus");
        if(focus != null) // Ÿ���� �Ʒ��� "Focus"��� �̸��� ��ü�� �ִ� ���
            camTarget = camTarget.Find("Focus"); // �ش� ��ü�� ��Ŀ���Ѵ�.

        _cinemachineController.ZoomFocus(camTarget, camPosition, duration);

        // TODO : ���� �ڷ�ƾ ȣ��
        StartCoroutine("CoOpen");
    }

    IEnumerator CoOpen()
    {
        float elapsedTime = 0f; // ��� �ð�

        while (elapsedTime < duration)
        {
            // ���� ��ġ�� ��ǥ ��ġ ���̿��� ������ ���� �̿��Ͽ� ��ġ ����
            openTarget.localPosition = 
                Vector3.Lerp(openTarget.localPosition, destPosition, elapsedTime / (duration * accel));

            // ��� �ð� ����
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // �̵� �Ϸ� �� ���������� ��ǥ ��ġ�� ��Ȯ�� ���߱�
        openTarget.position = destPosition;

    }

    // ------------------------------- Start ---------------------------------
    void Start()
    {
        Init();
    }

    // ------------------------------------------------------------------------

}
