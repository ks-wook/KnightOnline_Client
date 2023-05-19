using Assets.Scripts.Controller;
using Google.Protobuf.Protocol;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents_Player : MonoBehaviour
{
    [SerializeField]
    Transform _myPlayerObject;


    CreatureController _controller;
    StatData_player _stat;
    Animator _animator;
    LayerMask _hittalbeMask;

    C_Skill skillPacket = null;



    private void Start()
    {
        if (_myPlayerObject == null)
            _myPlayerObject = transform.parent;

        // ������ �� �ִ� ���̾�
        _hittalbeMask = LayerMask.GetMask("Player");

        // �÷��̾��� ������ player controller�� �ִ� ������Ʈ���� ����
        // _stat = transform.GetComponent<StatData_player>();

        // �ִϸ��̼� ����� ���� ������Ʈ�� ������
        _animator = transform.GetComponent<Animator>();

        // ������Ʈ ������ ���� ��Ʈ�ѷ��� �����´�
        _controller = _myPlayerObject.GetComponent<CreatureController>();
    }

    void OnRunEvent()
    {
        // 1. Play foot sound
    }

    // ���� ���� ���
    public void OnHitted()
    {

    }

    void moveForward()
    {
        // charBody.position += transform.forward * 0.2f;
        Rigidbody rigidBody = _myPlayerObject.GetComponent<Rigidbody>();
        rigidBody.AddForce(transform.forward * 5.0f);
    }

    void moveBackward()
    {
        Rigidbody rigidBody = _myPlayerObject.GetComponent<Rigidbody>();
        rigidBody.AddForce(-transform.forward * 20.0f);
    }

    List<int> _targetIds = new List<int>();
    void OnHitEvent() 
    {
        
    }
}
