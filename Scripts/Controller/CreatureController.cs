using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : MonoBehaviour
{
    public int Id { get; set; } // 오브젝트의 ID


    // ----------------- State 관련 변수, 함수 -------------------
    public enum CharacterState
    {
        Idle,
        Walk,
        Sprint,
        Attack,
        KnockBack,
        Dead,
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
    public virtual Vector3 DestPosition
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
        transform.position = DestPosition;
    }

    // ------------------- Stat 관련 ----------------------

    HPBar _hpBar; // hp표시바 오브젝트


    StatInfo _stat = new StatInfo();
    public StatInfo Stat
    {
        get { return _stat; }
        set
        {
            if (_stat.Equals(value))
                return;

            _stat.MergeFrom(value);
        }
    }

    public int HP
    {
        get { return Stat.Hp; }
        set 
        {
            Stat.Hp = value;
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

    protected void UpdateHpBar()
    {
        if (_hpBar == null)
            return;

        float ratio = 0.0f;
        if(Stat.MaxHp > 0)
            ratio = ((float)HP / Stat.MaxHp);

        _hpBar.SetHpRatio(ratio);
    }
    // ----------------------------------------------------------



    // -------------------- override 전용 -----------------------
    public JobSerializer creatureSerializer = new JobSerializer();


    public virtual void MovePosition() { }
    public virtual void UseSkill() { }
    protected virtual void Init() { AddHpBar(); } // 크리쳐는 HP Bar 기본적으로 추가

    public virtual void OnDead() { }
    // ----------------------------------------------------------

}
