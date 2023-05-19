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

        // 공격할 수 있는 레이어
        _hittalbeMask = LayerMask.GetMask("Player");

        // 플레이어의 스탯은 player controller가 있는 오브젝트에서 관리
        // _stat = transform.GetComponent<StatData_player>();

        // 애니메이션 재생을 위해 컴포넌트를 가져옴
        _animator = transform.GetComponent<Animator>();

        // 스테이트 조절을 위해 컨트롤러를 가져온다
        _controller = _myPlayerObject.GetComponent<CreatureController>();
    }

    void OnRunEvent()
    {
        // 1. Play foot sound
    }

    // 공격 받은 경우
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
