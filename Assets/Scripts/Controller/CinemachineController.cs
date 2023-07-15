using Cinemachine;
using System;
using System.Collections;
using UnityEngine;


/*
 * cinemachine ī�޶� �����ϱ� ���� ��Ʈ�ѷ� ��ũ��Ʈ
 */

namespace Assets.Scripts.Controller
{
    public class CinemachineController : MonoBehaviour
    {
        [HideInInspector]
        public Camera _mainCam;

        Animator cinemachineAnimator;

        // ------------------------ Cinemachine Virtual Cams -----------------------
        private Transform TPSCam;
        private Transform InvenCam;
        private Transform SettingsCam;
        private Transform ConversationCam;
        private Transform UltimateCam;
        private Transform TransferCam;

        private Transform cinemachineFSM;
        private CinemachineVirtualCamera _followCam;
        private CinemachineVirtualCamera _invenCam;
        private CinemachineVirtualCamera _settingsCam;
        private CinemachineVirtualCamera _conversationCam;
        private CinemachineVirtualCamera _ultimateCam;
        private CinemachineVirtualCamera _transferCam;

        // ---------------------------------------------------------------------------





        // --------------------- Cinemachine Shake ���� ���� -------------------------
        public CinemachineImpulseSource screenShake = null;
        public float powerAmount;

        // ---------------------------------------------------------------------------





        // --------------------------------- State -----------------------------------
        
        CamState _state = CamState.None;

        public enum CamState
        {
            None,
            TPS,
            Inven,
            Settings,
            Conversation,
            Ultimate,
            Transfer,
        }

        // �ó׸ӽ� ���� ������Ʈ
        public CamState STATE
        {
            get { return _state; }
            set
            {
                if (STATE == value)
                    return;

                _state = value;

                cinemachineAnimator.Play(Enum.GetName(typeof(CamState), _state));
            }
        }

        // ---------------------------------------------------------------------------








        // --------------------------------- Init ------------------------------------
        void Init()
        {
            if (_mainCam == null)
                _mainCam = GameObject.Find("MainCamera").GetComponent<Camera>();
            
            TPSCam = GameObject.Find("CM 3rdPerson Normal").transform;
            _followCam = TPSCam.GetComponent<CinemachineVirtualCamera>();

            InvenCam = GameObject.Find("CM Inven").transform;
            _invenCam = InvenCam.GetComponent<CinemachineVirtualCamera>();

            SettingsCam = GameObject.Find("CM Settings").transform;
            _settingsCam = SettingsCam.GetComponent<CinemachineVirtualCamera>();

            ConversationCam = GameObject.Find("CM Conversation").transform;
            _conversationCam = ConversationCam.GetComponent<CinemachineVirtualCamera>();

            UltimateCam = GameObject.Find("CM Ultimate").transform;
            _ultimateCam = UltimateCam.GetComponent<CinemachineVirtualCamera>();

            TransferCam = GameObject.Find("CM EventTransfer").transform;
            _transferCam = TransferCam.GetComponent<CinemachineVirtualCamera>();

            cinemachineFSM = GameObject.Find("CM StateDrivenCamera").transform;
            cinemachineAnimator = cinemachineFSM.GetComponent<Animator>();


            STATE = CamState.TPS;
        }

        // cinemachine Ÿ�� �ʱ�ȭ (�÷��̾�)
        public void InitTarget(Transform player)
        {
            _followCam.Follow = player;
            _invenCam.LookAt = player;
            _invenCam.Follow = player;
            _settingsCam.LookAt = player;
            _settingsCam.Follow = player;
            _conversationCam.Follow = player;
            _conversationCam.LookAt = player;
            _ultimateCam.Follow = player.Find("UltimateCamCenter");
            _ultimateCam.LookAt = player.Find("UltimateCamCenter");
        }

        // -----------------------------------------------------------------




        // ---------------------- Cinemachine Conrol -----------------------

        public void ZoomFocus(Transform target, Vector3 localPosition, float duration, bool isFix = false)
        {
            TransferCam.localPosition = localPosition;

            TransferCam.LookAt(target);

            StartCoroutine("CoZoomFocus", (duration, isFix));
        }

        IEnumerator CoZoomFocus((float, bool) durationAndIsFix)
        {
            STATE = CamState.Transfer;

            yield return new WaitForSeconds(durationAndIsFix.Item1);

            if(!durationAndIsFix.Item2)
                STATE = CamState.TPS;
        }

        // ī�޶� ��鸲 ȿ��
        public void shakeCam()
        {
            Debug.Log("ī�޶� ��鸲");
            if(screenShake == null)
            {
                screenShake = Managers.Object.MyPlayer.GetComponent<CinemachineImpulseSource>();
            }

            screenShake.GenerateImpulse();
        }

        // -----------------------------------------------------------------





        // --------------------------- Start ------------------------------
        private void Awake()
        {
            Init();
        }

        // -----------------------------------------------------------------
    }
}

