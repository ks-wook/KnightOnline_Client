using Assets.Scripts.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 기믹 동작시의 이벤트로 줌인을 실행하는 스크립트
 */

public class ZoomGimmick : Gimmick
{
    [Header("Target")]

    [SerializeField]
    [Tooltip("줌인할 객체의 타입")]
    Define.TargetType targetType;

    [SerializeField]
    [Tooltip("줌인할 객체의 시간")]
    float duration;

    [SerializeField]
    [Tooltip("줌인 후 카메라 고정 여부")]
    bool isFix = false;

    [SerializeField]
    [Tooltip("줌인 후 기믹 파괴 여부(1회성인지 여러번 가능한지)")]
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

    // 카메라 줌인 이벤트
    protected override void Zoom()
    {
        if(camTarget != null)
            camTarget = camTarget.Find("Focus"); // 타겟의 아래에 "Focus"라는 이름의 객체가 있어야함
        
        if(_cinemachineController != null)
            _cinemachineController.ZoomFocus(camTarget, camPosition, duration, isFix);

        if (_monsterController != null) // 몬스터인 경우 특정 애니메이션을 재생시킬 수 있다.
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

    // 플레이어가 트리거를 동작 시 이벤트를 동작시킬 수도 있다.
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
