using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * ����, �÷��̾� Controller�� ��ӹ޴� �⺻ ��Ʈ�ѷ� ��ũ��Ʈ
 * 
 * ���Ϳ� �÷��̾� Controller���� ������ �κ��� �����Ƿ� �׷��� �Ӽ�����
 * ��� CreatureController�� �������, �� ��Ʈ�ѷ��� ��ӹ޾� �����Ѵ�
 */


public class CreatureController : MonoBehaviour
{
    public int Id { get; set; } // ������Ʈ�� ID

    // ----------------- State ���� ����, �Լ� -------------------

    protected bool Initnitialized = false; // ũ���� �ʱ�ȭ �Ϸ� ����

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
        AggroLost, // ��׷ΰ� Ǯ�� ���� (���� ����)
    }

    protected CharacterState _state = CharacterState.Idle;

    // �ִϸ��̼� ���� ������Ʈ
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

    // ���� ������ ����
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

    // Ŭ���̾�Ʈ ���� ����
    DirectionInfo _directionInfo = new DirectionInfo();
    public DirectionInfo DirInfo
    {
        get { return _directionInfo; }
        set
        {
            _directionInfo = value;
        }
    }

    // ���� �ٶ󺸴� ����
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

    // ------------------- Stat ���� ----------------------



    HPBar _hpBar; // hpǥ�ù� ������Ʈ
    public int _ultimateStack = 0; // �ñر⸦ ����ϱ� ���� ���� �ൿ ����, UltimateCount��ŭ �׿��� �� �ñر� ��������


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
        else if(Initnitialized && HP <= 0)// ü���� 0 ������ ��� ���ó��
        {
            _hpBar.SetHpRatio(0);
            STATE = CharacterState.Dead;
        }
    }
    // ----------------------------------------------------------



    // -------------------- override ���� -----------------------
    public JobSerializer creatureSerializer = new JobSerializer();

    public virtual void UseSkill() { } // ��ų ���
    protected virtual void Init()
    {
        // ũ����(����, �÷��̾�)�� HP Bar �⺻������ �߰�
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
