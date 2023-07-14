using Assets.Scripts.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 기믹 동작시의 이벤트로 문이나 철창과 같은 오브젝트가 열리는 스크립트
 */

public class OpenGimmick : Gimmick
{
    [Header("Target")]

    [SerializeField]
    [Tooltip("동작 시 열릴 오브젝트")]
    Transform openTarget;

    [SerializeField]
    [Tooltip("열리는 연출의 타입")]
    Define.OpenType openType;

    [SerializeField]
    [Tooltip("오브젝트가 완전히 열리는데에 걸리는 시간")]
    float duration;

    [SerializeField]
    [Tooltip("오브젝트가 열리는 정도")]
    float amount;

    [SerializeField]
    [Tooltip("오브젝트가 열리는 속도 (0에 가까울 수록 빠르다)")]
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
        // TODO : 카메라 줌인 효과
        Transform focus = camTarget.Find("Focus");
        if(focus != null) // 타겟의 아래에 "Focus"라는 이름의 객체가 있는 경우
            camTarget = camTarget.Find("Focus"); // 해당 객체를 포커스한다.

        _cinemachineController.ZoomFocus(camTarget, camPosition, duration);

        // TODO : 오픈 코루틴 호출
        StartCoroutine("CoOpen");
    }

    IEnumerator CoOpen()
    {
        float elapsedTime = 0f; // 경과 시간

        while (elapsedTime < duration)
        {
            // 시작 위치와 목표 위치 사이에서 보간된 값을 이용하여 위치 변경
            openTarget.localPosition = 
                Vector3.Lerp(openTarget.localPosition, destPosition, elapsedTime / (duration * accel));

            // 경과 시간 증가
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // 이동 완료 후 마지막으로 목표 위치에 정확히 맞추기
        openTarget.position = destPosition;

    }

    // ------------------------------- Start ---------------------------------
    void Start()
    {
        Init();
    }

    // ------------------------------------------------------------------------

}
