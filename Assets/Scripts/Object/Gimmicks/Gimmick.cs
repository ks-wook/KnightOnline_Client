using Assets.Scripts.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 스테이지 연출을 위한 기믹 스크립트의 Base
 * 
 * 모든 기믹들은 이 스크립트를 상속받는다.
 */


public class Gimmick : MonoBehaviour
{
    [Header("Gimmick Info")]

    [SerializeField]
    [Tooltip("기믹의 카운트가 이 값만큼 도달 시 기믹 이벤트 실행")]
    protected int GimmicConditionCount;

    [SerializeField]
    [Tooltip("이벤트 완료 후 줌인할 때의 EventTransfer 카메라의 local position")]
    protected Vector3 camPosition;

    [SerializeField]
    [Tooltip("줌인할 타겟 (null인 경우 자신을 타겟으로 함)")]
    protected Transform camTarget;


    [HideInInspector]
    public int Condition = 0; // 해당 조건 달성 시 이벤트 발생

    [HideInInspector]
    protected CinemachineController _cinemachineController;


    protected virtual void Init()
    {
        GameObject.Find("CinemachineController").TryGetComponent<CinemachineController>(out _cinemachineController);
    }



    // -------------------------- Gimmick Handler -----------------------------
    // condition count 처리
    public virtual void CountUp() { }

    // ------------------------------------------------------------------------


    // ---------------------------- Event Handler -----------------------------
    // 폭발형 기믹 이벤트
    protected virtual void Explosion() { }

    // 오픈형 기믹 이벤트
    protected virtual void Open() { }

    // 보스 등장 시 줌 효과 이벤트
    protected virtual void Zoom() { }

    // ------------------------------------------------------------------------

}
