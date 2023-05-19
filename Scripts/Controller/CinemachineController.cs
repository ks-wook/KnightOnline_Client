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

        private Transform cinemachineFSM;
        private CinemachineVirtualCamera _followCam;
        private CinemachineVirtualCamera _invenCam;
        private CinemachineVirtualCamera _conversationCam;

        public Animator cinemachineAnimator;

        void Awake()
        {
            TPSCam = GameObject.Find("CM 3rdPerson Normal").transform;
            _followCam = TPSCam.GetComponent<CinemachineVirtualCamera>();

            InvenCam = GameObject.Find("CMInven").transform;
            _invenCam = InvenCam.GetComponent<CinemachineVirtualCamera>();

            ConversationCam = GameObject.Find("CM Conversation").transform;
            _conversationCam = ConversationCam.GetComponent<CinemachineVirtualCamera>();

            cinemachineFSM = GameObject.Find("CM StateDrivenCamera1").transform;
            cinemachineAnimator = cinemachineFSM.GetComponent<Animator>();

            cinemachineAnimator.Play("TPS");
        }

        public void setTarget(Transform target)
        {
            _followCam.Follow = target;
            _invenCam.LookAt = target;
            _invenCam.Follow = target;
            _conversationCam.Follow = target;
            _conversationCam.LookAt = target;
        }

        public void setCinemachineAnim(string cam)
        {
            cinemachineAnimator.Play(cam);
        }
    }
}

