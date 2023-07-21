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
    CreatureController _controller;
    Animator _animator;

    void setCinemachine(string cam) { }

    void SetCamearaMask(string maskName) { }

    void OnSkillHit(int skillId) { }

    void PlayAudio(string path) { }
}
