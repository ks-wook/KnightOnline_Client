using Cinemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    public class CinemachineController : MonoBehaviour
    {
        private Transform TPSCam;
        private Transform InvenCam;
        private Transform ConversationCam;
        private Transform UltimateCam;

        private Transform cinemachineFSM;
        private CinemachineVirtualCamera _followCam;
        private CinemachineVirtualCamera _invenCam;
        private CinemachineVirtualCamera _conversationCam;
        private CinemachineVirtualCamera _ultimateCam;

        public Animator cinemachineAnimator;

        void Awake()
        {
            TPSCam = GameObject.Find("CM 3rdPerson Normal").transform;
            _followCam = TPSCam.GetComponent<CinemachineVirtualCamera>();

            InvenCam = GameObject.Find("CM Inven").transform;
            _invenCam = InvenCam.GetComponent<CinemachineVirtualCamera>();

            ConversationCam = GameObject.Find("CM Conversation").transform;
            _conversationCam = ConversationCam.GetComponent<CinemachineVirtualCamera>();

            UltimateCam = GameObject.Find("CM Ultimate").transform;
            _ultimateCam = UltimateCam.GetComponent<CinemachineVirtualCamera>();



            cinemachineFSM = GameObject.Find("CM StateDrivenCamera").transform;
            cinemachineAnimator = cinemachineFSM.GetComponent<Animator>();

            

            cinemachineAnimator.Play("TPS");
        }

        public void setTarget(Transform target) // cinemachine 타겟 설정
        {
            _followCam.Follow = target;
            _invenCam.LookAt = target;
            _invenCam.Follow = target;
            _conversationCam.Follow = target;
            _conversationCam.LookAt = target;
            _ultimateCam.Follow = target.Find("UltimateCamCenter");
            _ultimateCam.LookAt = target.Find("UltimateCamCenter");
        }

        public void setCinemachineAnim(string cam) // cinemachine 현재 상태 변경
        {
            cinemachineAnimator.Play(cam);
        }
    }
}

