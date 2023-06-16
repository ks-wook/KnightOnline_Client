using Assets.Scripts.Controller;
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AnimationEvents_MyPlayer : MonoBehaviour
{
    CreatureController _controller;
    StatData_player _stat;
    Animator _animator;
    LayerMask _hittalbeMask;

    C_Skill skillPacket = null;

    private void Start()
    {

        // 공격할 수 있는 레이어
        _hittalbeMask = LayerMask.GetMask("Monster");

        // 플레이어의 스탯은 player controller가 있는 오브젝트에서 관리
        // _stat = transform.GetComponent<StatData_player>();

        // 애니메이션 재생을 위해 컴포넌트를 가져옴
        _animator = transform.GetComponent<Animator>();

        // 스테이트 조절을 위해 컨트롤러를 가져온다
        _controller = transform.GetComponent<CreatureController>();
    }

    void OnRunEvent()
    {
        // 1. Play foot sound
    }

    // 공격 받은 경우
    public void OnHitted()
    {

    }

    void setCinemachine(string cam)
    {
        Debug.Log(cam);
        MyPlayerController myPlayerController = _controller as MyPlayerController;

        if (cam == "TPS")
            myPlayerController.UltimateBackGround.gameObject.SetActive(false);
        
        myPlayerController.CamController.setCinemachineAnim(cam);
    }

    List<int> _targetIds = new List<int>();
    void OnHitEvent()
    {
        // 주변 모든 게임 오브젝트 탐색
        Collider[] colliders = Physics.OverlapSphere(transform.position + (transform.forward * 2), 1.0f, _hittalbeMask);

        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<CreatureController>() != null)
            {
                // 공격 범위 내의 모든 오브젝트들을 찾아서 서버로 전송
                // 매 객체마다가 아닌 한번에 전송 -> 패킷 낭비 방지
                CreatureController cc = null;
                collider.TryGetComponent<CreatureController>(out cc);
                if (cc != null)
                {
                    _targetIds.Add(cc.Id);
                }

                // 서버에서 몬스터 정보가 없으므로 클라이언트에서 히트 패킷을 전송해야함
                // ex) C_Demage
            }
        }

        foreach (int id in _targetIds)
            Debug.Log($"target : {id}");

        if (_targetIds.Count != 0) // hp 변화 전달
        {
            C_ChangeHp changePacket = new C_ChangeHp();

            changePacket.TargetIds.Add(_targetIds);
            changePacket.Type = SkillType.SkillAuto;
            Managers.Network.Send(changePacket);

            Debug.Log($"attacker Id {_controller.Id}");

            _targetIds.Clear();
        }
    }
}
