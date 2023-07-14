using Assets.Scripts.Controller;
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/*
 *  플레이어의 애니메이션 이벤트 처리를 위한 스크립트
 */ 

public class AnimationEvents_MyPlayer : MonoBehaviour
{

    Collider[] hitColliders; // 공격 시도 후 명중한 오브젝트
    MyPlayerController _myPlayerController;
    CinemachineController _cinemachineController;
    LayerMask _hittalbeMask;

    private void Start()
    {
        // 공격할 수 있는 레이어
        _hittalbeMask = LayerMask.GetMask("Monster");
        

        // 스테이트 조절을 위해 컨트롤러를 가져온다
        _myPlayerController = transform.GetComponent<MyPlayerController>();

        // 카메라 연출을 위해 시네머신 컨트롤러 획득
        _cinemachineController = GameObject.Find("CinemachineController").GetComponent<CinemachineController>();
    }

    // 시네머신 카메라 변경
    void SetCinemachine(string cam)
    {
        if (cam == "TPS")
            _myPlayerController.UltimateBackGround.gameObject.SetActive(false);

        if(cam == "TPS")
        {
            _cinemachineController.STATE = 
                CinemachineController.CamState.TPS;
        }
        else if(cam == "Inven") 
        {
            _cinemachineController.STATE =
                CinemachineController.CamState.Inven;
        }
        else if(cam == "Conversation")
        {
            _cinemachineController.STATE =
                CinemachineController.CamState.Conversation;
        }
        else if(cam == "Ultimate")
        {
            _cinemachineController.STATE =
                CinemachineController.CamState.Ultimate;
        }
        else if(cam == "Transfer")
        {
            _cinemachineController.STATE =
                CinemachineController.CamState.Transfer;
        }

    }

    void SetCameraMask(string maskName)
    {
        if (maskName == "Default")
        {
            Camera.main.cullingMask = -1;
            return;
        }

        Camera.main.cullingMask = LayerMask.GetMask(maskName);
    }

    // 기본 공격 명중
    void OnSkillHit(int skillId)
    {
        if(skillId == 1)
        {
            // 기본 공격의 범위만큼 스캔
            hitColliders = Physics.OverlapSphere(transform.position + (transform.forward * 1), 1.5f, _hittalbeMask);

            if (hitColliders.Length != 0) // 공격에 명중한 적이 있는 경우
            {
                _cinemachineController.shakeCam();
                Managers.Sound.Play("Effect/MonsterHit");
                _myPlayerController.HandleDamage(skillId, hitColliders);

                _myPlayerController.UltimateStack++; // 궁극기 게이지 상승
            }
        }
        else if(skillId == 2)
        {
            // 기본 공격과 동일한 범위
            hitColliders = Physics.OverlapSphere(transform.position + (transform.forward * 1), 1.5f, _hittalbeMask);

            if (hitColliders.Length != 0) // 공격에 명중한 적이 있는 경우
            {
                _cinemachineController.shakeCam();
                Managers.Sound.Play("Effect/MonsterHit");
                _myPlayerController.HandleDamage(skillId, hitColliders);

                _myPlayerController.UltimateStack++; // 궁극기 게이지 상승
            }
        }
        else if(skillId == 3)
        {

            _myPlayerController.UltimateSkillEffectController.gameObject.SetActive(true); // 궁극기 연출 이펙트 출력

            hitColliders = Physics.OverlapSphere(transform.position + (transform.forward * 2.0f), 3.0f, _hittalbeMask);
            if (hitColliders.Length != 0) // 공격에 명중한 적이 있는 경우
            {
                _myPlayerController.HandleDamage(skillId, hitColliders);

                _myPlayerController.UltimateStack++; // 궁극기 게이지 상승
            }



        }
        
    }

    // 효과음 재생
    void PlayAudio(string path)
    {
        Managers.Sound.Play("Creature/Player/" + path);
    }
}
