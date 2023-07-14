using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * 몬스터, 플레이어 Controller가 상속받는 기본 컨트롤러 스크립트
 * 
 * 몬스터와 플레이어 Controller에서 동일한 부분이 많으므로 그러한 속성들을
 * 모아 CreatureController로 만들었고, 이 컨트롤러를 상속받아 구현한다
 */


public class CreatureController : MonoBehaviour
{
    public int Id { get; set; } // 오브젝트의 ID

    // ----------------- State 관련 변수, 함수 -------------------

    protected bool Initnitialized = false; // 크리쳐 초기화 완료 여부

    public enum CharacterState
    {
        Idle,
        Walk,
        Sprint,
        NomalAttack,
        BattleSkill,
        Ultimate,
        KnockBack,
        Dead,
        AggroLost, // 어그로가 풀린 상태 (몬스터 전용)
    }

    protected CharacterState _state = CharacterState.Idle;

    // 애니메이션 관련 업데이트
    public virtual CharacterState STATE
    {
        get { return _state; }
        set { }
    }

    protected virtual void UpdateIdle() { }
    protected virtual void UpdateWalk() { }
    protected virtual void UpdateSprint() { }
    protected virtual void UpdateAttack() { }
    protected virtual void UpdateDead() { }

    // ---------------------------------------------------------


    // ----------------------- Position ------------------------
    protected PositionInfo _positionInfo = new PositionInfo();
    public virtual PositionInfo PosInfo
    {
        get { return _positionInfo; }
        set { }
    }

    // 다음 목적지 정보
    public virtual Vector3 VectorPosInfo
    {
        get
        {
            return new Vector3(PosInfo.PosX, PosInfo.PosY, PosInfo.PosZ);
        }
        set
        {
            PosInfo.PosX = value.x;
            PosInfo.PosY = value.y;
            PosInfo.PosZ = value.z;
        }
    }

    // 클라이언트 방향 정보
    DirectionInfo _directionInfo = new DirectionInfo();
    public DirectionInfo DirInfo
    {
        get { return _directionInfo; }
        set
        {
            _directionInfo = value;
        }
    }

    // 현재 바라보는 방향
    public Vector3 ForwardDir
    {
        get
        {
            return new Vector3(DirInfo.DirX, DirInfo.DirY, DirInfo.DirZ);
        }
        set
        {
            DirInfo.DirX = value.x;
            DirInfo.DirY = value.y;
            DirInfo.DirZ = value.z;
        }
    }

    public void SyncPos()
    {
        transform.position = VectorPosInfo;
    }

    // ------------------- Stat 관련 ----------------------



    HPBar _hpBar; // hp표시바 오브젝트
    public int _ultimateStack = 0; // 궁극기를 사용하기 위한 현재 행동 스택, UltimateCount만큼 쌓였을 때 궁극기 시전가능


    StatInfo _stat = new StatInfo();
    public StatInfo STAT
    {
        get { return _stat; }
        set
        {
            if (_stat.Equals(value))
                return;

            _stat.MergeFrom(value);
        }
    }

    public virtual int HP
    {
        get { return STAT.Hp; }
        set 
        {
            STAT.Hp = value;
            UpdateHpBar();
        }
    }

    protected void AddHpBar()
    {
        GameObject go = Managers.Resource.Instantiate("UI/WorldSpace/HpBar", transform);
        go.name = "HpBar";
        _hpBar = go.GetComponent<HPBar>();
        UpdateHpBar();
    }

    protected void SetHpBar(float height)
    {
        if (_hpBar != null)
        {
            _hpBar.transform.localPosition = new Vector3(0, height, 0);
            UpdateHpBar();
        }
    }

    protected void UpdateHpBar()
    {
        Debug.Log("HP remain : " + HP);

        if (_hpBar == null)
            return;

        if(HP > 0)
        {
            _hpBar.SetHpRatio((float) HP / STAT.MaxHp);
        }
        else if(Initnitialized && HP <= 0)// 체력이 0 이하인 경우 사망처리
        {
            _hpBar.SetHpRatio(0);
            STATE = CharacterState.Dead;
        }
    }
    // ----------------------------------------------------------



    // -------------------- override 전용 -----------------------
    public JobSerializer creatureSerializer = new JobSerializer();

    public virtual void UseSkill() { } // 스킬 사용
    protected virtual void Init()
    {
        // 크리쳐(몬스터, 플레이어)는 HP Bar 기본적으로 추가
        AddHpBar(); 
    }

    public virtual void OnIdle() { }

    public virtual void OnWalk() { }

    public virtual void OnSprint() { }

    public virtual void OnAttack(int skillId) { }

    public virtual void OnKnockback() { }

    public virtual void OnDead() { }
    // ----------------------------------------------------------

}
