using Cinemachine;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Controller
{
    public partial class MyPlayerController : CreatureController
    {

        [SerializeField]
        private Transform Player;

        // ------------------- cinemahcine ---------------------
        CinemachineController _camController;



        // ------------------- move ---------------------
        public bool useCharacterForward = false;
        private float speed = 0f;
        private float direction = 0f;
        public float turnSpeed = 10f;

        private Quaternion freeRotation;
        private float turnSpeedMultiplier;
        private Vector3 targetDirection;
        private Vector2 input;



        // ------------------- update ---------------------
        public Animator playerAnimator;
        
        bool _updated = false; // 1. STATE가 바뀌거나 2. 위치가 바뀌거나
        bool _interactable = false;
        bool _inputable = true; 

        public int WeaponDamage { get; private set; }
        public int ArmorDefence { get; private set; }


        protected override void Init()
        {
            base.Init();

            InitAnimator();
            InitCAM();

            RefreshAdditionalStat();
        }

        private void InitAnimator()
        {
            playerAnimator = Player.GetComponentInChildren<Animator>();
        }

        private void InitCAM()
        {
            Transform cinemachineController = GameObject.Find("CinemachineController").transform;
            
            if (cinemachineController != null)
            {
                _camController = cinemachineController.GetComponent<CinemachineController>();

                _camController.setTarget(transform);
                _camController.setCinemachineAnim("TPS");
            }

        }

        // UI에 뿌려주기위한 스탯
        public void RefreshAdditionalStat()
        {
            WeaponDamage = 0;
            ArmorDefence = 0;

            foreach (Item item in Managers.Inven.Items.Values)
            {

                if (item.Equipped == false)
                    continue;


                switch (item.ItemType)
                {
                    case ItemType.Weapon:
                        WeaponDamage += ((Weapon)item).Damage;
                        break;
                    case ItemType.Armor:
                        ArmorDefence += ((Armor)item).Defence;
                        break;
                }
            }

        }


        // --------------- Position --------------------
        // state를 포함한 위치 정보
        public override PositionInfo PosInfo
        {
            get { return _positionInfo; }
            set
            {
                if (_positionInfo.Equals(value))
                    return;

                _positionInfo = value;
                // 이동 처리
                // TODO
            }
        }

        public override Vector3 DestPosition
        {
            get
            {
                return new Vector3(PosInfo.PosX, PosInfo.PosY, PosInfo.PosZ);
            }
            set
            {
                PosInfo.PosX = value.x;
                PosInfo.PosY = value.y;
                PosInfo.PosZ = value.z;

                _updated = true; // 위치에 변화가 있으므로 갱신
            }
        }

        public virtual void UpdateTargetDirection()
        {
            if (!useCharacterForward)
            {
                turnSpeedMultiplier = 1f;
                var forward = Camera.main.transform.TransformDirection(Vector3.forward);
                forward.y = 0;

                //get the right-facing direction of the referenceTransform
                var right = Camera.main.transform.TransformDirection(Vector3.right);

                // determine the direction the player will face based on input and the referenceTransform's right and forward directions
                targetDirection = input.x * right + input.y * forward;
            }
            else
            {
                turnSpeedMultiplier = 0.2f;
                var forward = transform.TransformDirection(Vector3.forward);
                forward.y = 0;

                //get the right-facing direction of the referenceTransform
                var right = transform.TransformDirection(Vector3.right);
                targetDirection = input.x * right + Mathf.Abs(input.y) * forward;
            }
        }


        public override void MovePosition()
        {

            input.x = Input.GetAxis("Horizontal");
            input.y = Input.GetAxis("Vertical");

            // set speed to both vertical and horizontal inputs
            if (useCharacterForward)
                speed = Mathf.Abs(input.x) + input.y;
            else
                speed = Mathf.Abs(input.x) + Mathf.Abs(input.y);

            speed = Mathf.Clamp(speed, 0f, 1f);

            if (input.y < 0f && useCharacterForward)
                direction = input.y;
            else
                direction = 0f;


            // Update target direction relative to the camera view (or not if the Keep Direction option is checked)
            UpdateTargetDirection();
            if (input != Vector2.zero && targetDirection.magnitude > 0.1f)
            {
                Vector3 lookDirection = targetDirection.normalized;
                freeRotation = Quaternion.LookRotation(lookDirection, transform.up);
                var diferenceRotation = freeRotation.eulerAngles.y - transform.eulerAngles.y;
                var eulerY = transform.eulerAngles.y;

                if (diferenceRotation < 0 || diferenceRotation > 0) eulerY = freeRotation.eulerAngles.y;
                var euler = new Vector3(0, eulerY, 0);

                
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler), turnSpeed * turnSpeedMultiplier * Time.deltaTime);
                transform.position += transform.forward * Time.deltaTime * 5.0f;

                // 위치 갱신은 플레이어 이동에 의해서만 생기는 것이 아님
                // DestPosition = transform.position;
                ForwardDir = transform.forward;
            }
            else
            {
                STATE = CharacterState.Idle;
            }
        }

        // ---------------------------------------------------



        // -------------------- State ------------------------
        // 애니메이션 관련 업데이트
        public override CharacterState STATE
        {
            get { return _state; }
            set
            {
                if (STATE == value)
                    return;

                _state = value;

                switch (_state)
                {
                    case CharacterState.Idle:
                        playerAnimator.SetFloat("speed", 0);
                        break;
                    case CharacterState.Walk:
                        playerAnimator.SetFloat("speed", 1);
                        break;
                    case CharacterState.Sprint:
                        playerAnimator.SetFloat("speed", 2);
                        break;
                    case CharacterState.Attack:
                        StartCoroutine(CoAttack());
                        break;
                    case CharacterState.KnockBack:
                        StartCoroutine(CoKnockBack());
                        break;
                    case CharacterState.Dead:
                        playerAnimator.Play("Death");
                        break;
                }

                _updated = true; // 상태에 변화가 있으므로 갱신

            }
        }

        void UpdateState()
        {

            // TPSCam.position = Vector3.MoveTowards(TPSCam.position, transform.position, 20.0f * Time.deltaTime);

            GetUIKeyInput();

            switch (STATE)
            {
                case CharacterState.Idle:
                    UpdateIdle();
                    GetKeyboardInput();
                    break;
                case CharacterState.Walk:
                    UpdateWalk();
                    GetKeyboardInput();
                    break;
                case CharacterState.Sprint:
                    UpdateSprint();
                    GetKeyboardInput();
                    break;
                case CharacterState.Attack:
                    // UpdateAttack();
                    break;
                case CharacterState.KnockBack:
                    // UpdateKnockBack();
                    break;
                case CharacterState.Dead:
                    UpdateDead();
                    break;
                default:
                    UpdateIdle();
                    break;
            }

            PosInfo.State = (CreateureState)STATE;
            CheckUpdatedFlag();
        }

        protected override void UpdateIdle()
        {
            CheckAttack();
            PosInfo.State = CreateureState.Idle;
        }

        protected override void UpdateWalk()
        {
            CheckAttack();
            if (_inputable)
                MovePosition();

            PosInfo.State = CreateureState.Walk;

        }

        protected override void UpdateSprint()
        {

            if (STATE == CharacterState.Idle || STATE == CharacterState.Walk || STATE == CharacterState.Sprint)
            {
                STATE = CharacterState.Sprint;
            }

            CheckAttack();

            if (_inputable)
                MovePosition();

            PosInfo.State = CreateureState.Sprint;

        }

        protected override void UpdateDead()
        {
            // TODO
        }

        
        // -----------------------------------------------------------




        


      

        // Network Update
        void CheckUpdatedFlag()
        {
            // 업데이트 직전에 위치를 갱신
            DestPosition = transform.position;


            if (_updated)
            {
                // TEMP 멀티 게임 씬 에서만 이동 패킷 전송
                if(Managers.UI.SCENETYPE == Define.Scene.Game)
                {
                    C_Move movePacket = new C_Move();
                    movePacket.PosInfo = PosInfo;
                    movePacket.PosInfo.DirInfo = DirInfo;
                    Managers.Network.Send(movePacket);
                    _updated = false;
                }
                
            }
        }

        // 스킬 쿨타임 관리 -> 패킷에 의한 네트워크 자원 낭비 방지
        Coroutine _coSkillCooltime = null;
        IEnumerator CoInputColltime(float time)
        {
            yield return new WaitForSeconds(time);
            _coSkillCooltime = null;
        }


        void CheckAttack()
        {
            if (_coSkillCooltime == null && Input.GetMouseButtonDown(0))
            {
                // 바로 상태를 변경하는 것이 아니라 서버에 공격하겠다는 요청이 우선
                C_Skill skill = new C_Skill() { Info = new SkillInfo() };
                skill.Info.SkillId = 1; // 기본 스킬이라고 가정
                Managers.Network.Send(skill); // 스킬 정보 전송

                _coSkillCooltime = StartCoroutine("CoInputColltime", 0.5f);
            }
        }

        public void UseSkill(int skillId)
        {
            if (skillId == 1) // 기본 공격
            {
                StartCoroutine(CoAttack());
            }
        }

        // 스킬 발동
        IEnumerator CoAttack()
        {
            _inputable = false;

            STATE = CharacterState.Attack; // 상태 업데이트
            playerAnimator.SetFloat("normalAttack", 1);

            // 0.9초 이내에 마우스가 다시 눌렸다면 2단 공격
            yield return new WaitForSeconds(0.9f);

            playerAnimator.SetFloat("normalAttack", 0);
            STATE = CharacterState.Idle;

            _inputable = true;

        }


        IEnumerator CoKnockBack()
        {
            _inputable = false;

            // 애니메이션 재생 시간동안 움직이지 못한다
            yield return new WaitForSeconds(1.05f);

            STATE = CharacterState.Idle;

            _inputable = true;
        }

        public override void OnDead()
        {
            STATE = CharacterState.Dead;
            // TODO
            // 부활 여부를 묻는 UI
        }


        // -----------------------------------------------------

        void Start()
        {
            Init();
            

        }

        void Update()
        {
            //if (EventSystem.current.IsPointerOverGameObject())
            //    return;

            UpdateState();
        }

    }
}
