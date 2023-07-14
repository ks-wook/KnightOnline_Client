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
    [SerializeField]
    Transform _myPlayerObject;

    CreatureController _controller;
    Animator _animator;

    private void Start()
    {
        if (_myPlayerObject == null)
            _myPlayerObject = transform.parent;

        // �ִϸ��̼� ����� ���� ������Ʈ�� ������
        _animator = transform.GetComponent<Animator>();

        // ������Ʈ ������ ���� ��Ʈ�ѷ��� �����´�
        _controller = _myPlayerObject.GetComponent<CreatureController>();
    }


    // �ó׸ӽ� ī�޶� ����
    void setCinemachine(string cam) { }

    void SetCamearaMask(string maskName) { }

    // �⺻ ���� ����
    void OnSkillHit(int skillId) { }

    void PlayAudio(string path) { }
}
