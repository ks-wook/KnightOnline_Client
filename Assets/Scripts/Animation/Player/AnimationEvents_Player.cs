using Assets.Scripts.Controller;
using Google.Protobuf.Protocol;
using System.Collections.Generic;
using UnityEngine;

/*
 * 다른 플레이어의 애니메이션 이벤트 핸들러
 * 
 * 코드 재활용 및 오류 방지를 위해 함수만 정의하고 사용하진 않는다.
 */

public class AnimationEvents_Player : MonoBehaviour
{
    [SerializeField]
    Transform _myPlayerObject;

    CreatureController _controller;
    Animator _animator;

    private void Start()
    {
        if (_myPlayerObject == null)
            _myPlayerObject = transform.parent;

        // 애니메이션 재생을 위해 컴포넌트를 가져옴
        _animator = transform.GetComponent<Animator>();

        // 스테이트 조절을 위해 컨트롤러를 가져온다
        _controller = _myPlayerObject.GetComponent<CreatureController>();
    }


    // 시네머신 카메라 변경
    void setCinemachine(string cam) { }

    void SetCamearaMask(string maskName) { }

    // 기본 공격 명중
    void OnSkillHit(int skillId) { }

    void PlayAudio(string path) { }
}
