using Assets.Scripts.Controller;
using Google.Protobuf.Protocol;
using System.Collections.Generic;
using UnityEngine;

/*
 * �ٸ� �÷��̾��� �ִϸ��̼� �̺�Ʈ �ڵ鷯
 * 
 * �ڵ� ��Ȱ�� �� ���� ������ ���� �Լ��� �����ϰ� ������� �ʴ´�.
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
