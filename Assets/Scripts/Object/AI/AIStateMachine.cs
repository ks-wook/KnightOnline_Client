using UnityEngine;


/*
 * AI 스크립트 상속용 스크립트
 */

public abstract class AIStateMachine : MonoBehaviour
{
    public enum AISTATE
    {
        Idle, // AI 활성화 이전 기본 상태
        Enter,
        Battle, // Battle 상태는 몬스터 트리거의 경우 사용되는 상태
        AggroLost, // 몬스터 트리거에서 어그로를 잃고 제자리로 돌아가야하는 상태
        Exit
    }

    AISTATE _aiState = AISTATE.Idle;

    public AISTATE AI_State
    {
        get { return _aiState; }
        set
        {
            if (_aiState == value)
                return;

            _aiState = value;
        }
    }
    protected virtual void OnTriggerEnter(Collider other) { }

    protected virtual void OnTriggerExit(Collider other) { }


    // AI 상태 업데이트 함수
    public virtual void UpdateAIState() { }
}

