using Cinemachine;
using Data;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * 플레이어 조작을 담당하는 컨트롤러 스크립트
 */

namespace Assets.Scripts.Controller
{
    public partial class MyPlayerController : CreatureController
    {
        object _lock = new object();


        [SerializeField]
        private Transform Player;

        // ------------------- Cinemahcine Update 관련 변수 ---------------------
        [HideInInspector]
        public CinemachineController CinemachineController;
        
        // ----------------------------------------------------------------------



        // ----------------------- Move Update 관련 변수 ------------------------
        [HideInInspector]
        public bool useCharacterForward = false;

        private float speed = 0f;
        private float direction = 0f;
        public float turnSpeed = 10f;

        private Quaternion freeRotation;
        private float turnSpeedMultiplier;
        private Vector3 targetDirection;
        private Vector2 input;
        // ----------------------------------------------------------------------




        // ----------------------- State Update 관련 변수------------------------

        // 애니메이션 및 particleSystem
        [HideInInspector]
        public Animator PlayerAnimator; // 플레이어 애니메이터

        [HideInInspector]
        public ParticleSystem SlashEffectController; // 검기 이펙트

        [HideInInspector]
        public ParticleSystem DashEffectController; // 대쉬 이펙트

        [HideInInspector]
        public ParticleSystem BattleSkillEffectController; // 전투 스킬 이펙트

        [HideInInspector]
        public ParticleSystem UltimateSkillEffectController; // 궁극기 이펙트

        [HideInInspector]
        public Transform UltimateBackGround; // 궁극기 연출용 배경

        [HideInInspector]
        public bool EnableUltimate = false; // 궁극기 사용 가능 여부
        const int UltimateCount = 6; // 궁극기를 사용하기 위해 필요한 스택
        public int UltimateStack
        {
            get { return _ultimateStack; }
            set
            {
                Debug.Log("현재 궁극기 스택" + _ultimateStack);

                _ultimateStack = value;

                UI_GameScene sceneUI = Managers.UI.SceneUI as UI_GameScene;
                UI_PlayerStatus statusUI = sceneUI.PlayerStatusUI;

                statusUI.SetUltimateSlider(UltimateCount, UltimateStack);
            }
        }


        [HideInInspector]
        public bool EnableBattleSkill = true; // 전투 스킬 사용 가능 여부
        List<float> SkillDamages = new List<float>(); // 스킬 별 계수
        List<float> SkillCoolTimes = new List<float>(); // 스킬 별 쿨타임


        [HideInInspector]
        public int WeaponDamage { get; private set; } // 무기만의 공격력

        [HideInInspector]
        public int TotalDamage // 무기 공격력을 합친 전체 공격력
        {
            get { return STAT.Attack + WeaponDamage; }
        }

        [HideInInspector]
        public int ArmorDefence { get; private set; }

        [HideInInspector]
        public bool _isMultiPlay = false; // 기본으로 싱글 플레이로 설정

        [HideInInspector]
        private Define.Scene currentScene; // 현재 씬 타입 캐싱



        bool _updated = false; // 서버상태 업데이트 여부 1. STATE가 바뀌거나 2. 위치가 바뀌거나
        bool _interactable = false; // 상호작용 가능 여부
        bool _inputable = true; // 키 입력 가능 여부


        Transform _handGrip = null; // 손에 쥔 무기 프리팹
        Transform _backGrip = null; // 등에 멘 무기 프리팹
        Transform _lastWeapon = null; // 마지막에 장착하고 있던 무기의 프리팹
        public int LastWeaponTemplatedId = 0; // 마지막에 장착하고 있던 무기의 도감 넘버
        
        // ----------------------------------------------------------------------




        // ------------------------- 플레이어 초기화 ----------------------------
        protected override void Init()
        {
            base.Init();

            InitAnimation();
            InitParticlesys();
            InitCAM();
            InitSkill();


            currentScene = Managers.Scene.CurrentScene.SceneType; // 현재 씬 상태 획득

            if(currentScene == Define.Scene.Game)
            {
                string curSceneName = Managers.Scene.GetCurrentSceneName();
                if (curSceneName == "RaidBoss") // 현재 씬이 레이드인 경우 멀티 플레이 모드로 전환한다.
                {
                    _isMultiPlay = true;
                }
            }

            // 처음 시작 시 최대 Hp 부터 시작
            HP = STAT.MaxHp;
        }

        private void InitAnimation()
        {
            // 애니메이터 획득
            PlayerAnimator = transform.GetComponent<Animator>();

            // 애니메이션 재생동안 이동이 불가능한 시간 설정
            _nomalAttackAnimCool = new WaitForSeconds(NomalAttackAnimCool);
            _battleSkillAnimCool = new WaitForSeconds(BattleSkillAnimCool);
            _ultimateAnimCool = new WaitForSeconds(UltimateAnimCool);
        }

        private void InitParticlesys() // 애니메이터 및 이펙트를 위한 파티클 시스템 획득
        {
            _handGrip = transform.GetChild(0)
                .Find("Root/Hips/Spine_01/Spine_02/Spine_03/Clavicle_R/Shoulder_R/Elbow_R/Hand_R/HandGrip");

            // 이펙트 미지정 시 직접 검색하여 획득
            if(SlashEffectController == null) // 검기 이펙트 획득
                SlashEffectController = _handGrip.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();

            // 무기가 등에 장착되는 그립
            _backGrip = transform.GetChild(0)
                .Find("Root/Hips/Spine_01/Spine_02/BackGrip");


            // 대쉬 이펙트 획득
            if(DashEffectController == null)
            {
                Transform dashParticle = transform.Find("DashParticle");
                DashEffectController = dashParticle.GetComponent<ParticleSystem>();
            }

            // 전투 스킬 및 궁극기 이펙트 획득
            UltimateBackGround = transform.Find("UltimateBackground");

            if(BattleSkillEffectController == null)
            {
                BattleSkillEffectController = 
                    transform.Find("BattleSkillEffect").GetComponent<ParticleSystem>();
            }

            if(UltimateSkillEffectController == null)
            {
                UltimateSkillEffectController = 
                    transform.Find("UltimateSkillEffect").GetComponent<ParticleSystem>();
            }
        }


        private void InitCAM()
        {
            Transform cc = GameObject.Find("CinemachineController").transform;
            
            if (cc != null)
            {
                CinemachineController = cc.GetComponent<CinemachineController>();

                CinemachineController.InitTarget(transform);
                CinemachineController.STATE = CinemachineController.CamState.TPS;
            }

        }

        // 스킬 json 파일 데이터로부터 파싱된 스킬 데이터를 현재 플레이어 캐릭터에 입력
        private void InitSkill()
        {
            monsterLayer = LayerMask.GetMask("Monster"); // 공격가능한 레이어 지정

            if (Managers.Data.SkillDict.TryGetValue(NormalAttackId, out NormalAttackInfo))
            {
                SkillDamages.Add(NormalAttackInfo.damage);
                SkillCoolTimes.Add(NormalAttackInfo.cooldown);
            }
            else
            {
                Debug.Log("기본 공격에 대한 스킬 정보가 존재하지 않습니다.");
            }


            if (Managers.Data.SkillDict.TryGetValue(BattleSkillId, out BattleSkillInfo))
            {
                SkillDamages.Add(BattleSkillInfo.damage);
                SkillCoolTimes.Add(BattleSkillInfo.cooldown);
            }
            else
            {
                Debug.Log("전투 스킬에 대한 스킬 정보가 존재하지 않습니다.");
            }

            if (Managers.Data.SkillDict.TryGetValue(UltimateId, out UltimateInfo))
            {
                SkillDamages.Add(UltimateInfo.damage);
                SkillCoolTimes.Add(UltimateInfo.cooldown);
            }
            else
            {
                Debug.Log("궁극기에 대한 스킬 정보가 존재하지 않습니다.");
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
                        Debug.Log("Refresh Stat");

                        LastWeaponTemplatedId = item.TemplateId;

                        // 아이템 프리팹 교체
                        if (_lastWeapon != null)
                            _lastWeapon.gameObject.SetActive(false);

                        if (currentScene == Define.Scene.Lobby1)
                        {
                            _lastWeapon = _backGrip.Find("Weapon_Prefab_" + item.TemplateId.ToString());
                            _lastWeapon.gameObject.SetActive(true);
                        }
                        else if (currentScene == Define.Scene.Game)
                        {
                            _lastWeapon = _handGrip.Find("Weapon_Prefab_" + item.TemplateId.ToString());
                            _lastWeapon.gameObject.SetActive(true);
                        }

                        break;
                    case ItemType.Armor: // 방어구의 경우 능력치만 갱신한다.
                        ArmorDefence += ((Armor)item).Defence;
                        break;
                }
            }

        }

        // 현재 플레이어의 HP
        public override int HP
        {
            get { return STAT.Hp; }
            set
            {
                STAT.Hp = value;

                if(Managers.Scene.CurrentScene.SceneType == Define.Scene.Game)
                {
                    UI_GameScene sceneUI = Managers.UI.SceneUI as UI_GameScene;
                    UI_PlayerStatus statusUI = sceneUI.PlayerStatusUI;

                    statusUI.SetHpSlider(STAT.MaxHp, STAT.Hp);
                }
                else if (Managers.Scene.CurrentScene.SceneType == Define.Scene.Lobby1)
                {
                    UI_LobbyScene sceneUI = Managers.UI.SceneUI as UI_LobbyScene;
                    UI_PlayerStatus statusUI = sceneUI.PlayerStatusUI;

                    statusUI.SetHpSlider(STAT.MaxHp, STAT.Hp);
                }

            }
        }

        // ----------------------------------------------------------------------



        // ---------------------------- Position --------------------------------
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

        public override Vector3 VectorPosInfo
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

                // get the right-facing direction of the referenceTransform
                var right = Camera.main.transform.TransformDirection(Vector3.right);

                // determine the direction the player will face based on input and the referenceTransform's right and forward directions
                targetDirection = input.x * right + input.y * forward;
            }
            else
            {
                turnSpeedMultiplier = 0.2f;
                var forward = transform.TransformDirection(Vector3.forward);
                forward.y = 0;

                // get the right-facing direction of the referenceTransform
                var right = transform.TransformDirection(Vector3.right);
                targetDirection = input.x * right + Mathf.Abs(input.y) * forward;
            }
        }


        public void MovePosition()
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

        // ---------------------------------------------------------------



        // --------------------------- State -----------------------------
        // 애니메이션 관련 업데이트
        public override CharacterState STATE
        {
            get { return _state; }
            set
            {
                if (STATE == value)
                    return;

                if(_state == CharacterState.Sprint)
                {
                    if (DashEffectController.isPlaying)
                        DashEffectController.Stop(); // 대시 이펙트 재생 중지
                }

                _state = value;

                switch (_state)
                {
                    case CharacterState.Idle:
                        OnIdle();
                        break;
                    case CharacterState.Walk:
                        OnWalk();
                        break;
                    case CharacterState.Sprint:
                        OnSprint();
                        break;
                    case CharacterState.NomalAttack:
                        OnAttack(1);
                        break;
                    case CharacterState.BattleSkill:
                        OnAttack(2);
                        break;
                    case CharacterState.Ultimate:
                        OnAttack(3);
                        break;
                    case CharacterState.KnockBack:
                        OnKnockback();
                        break;
                    case CharacterState.Dead:
                        OnDead();
                        break;
                }

                _updated = true; // 상태에 변화가 있으므로 갱신

            }
        }

        public override void OnIdle()
        {
            PlayerAnimator.SetFloat("speed", 0);
        }

        public override void OnWalk()
        {
            PlayerAnimator.SetFloat("speed", 1);
        }

        public override void OnSprint()
        {
            PlayerAnimator.SetFloat("speed", 2);
        }

        public override void OnAttack(int skillId)
        {
            if(skillId > 0)
            {
                if (skillId == 2) // 전투스킬
                {
                    if (Managers.Scene.CurrentScene.SceneType == Define.Scene.Game)
                    {
                        UI_GameScene sceneUI = Managers.UI.SceneUI as UI_GameScene;
                        UI_PlayerStatus statusUI = sceneUI.PlayerStatusUI;

                        statusUI.StartBattleSkillCooldown(SkillCoolTimes[skillId - 1]);
                    }
                }
                else if (skillId == 3) // 궁극기
                {
                    if (Managers.Scene.CurrentScene.SceneType == Define.Scene.Game)
                    {
                        EnableUltimate = false;
                        UltimateStack = 0; // 궁극기 사용시 궁극기 게이지 초기화
                    }
                }

                StartCoroutine("CoAttack", skillId);
            }
            
        }

        public override void OnKnockback()
        {
            StartCoroutine("CoKnockBack");
        }

        public override void OnDead()
        {
            _inputable = false;
            PlayerAnimator.Play("Death");
        }


        void UpdateState()
        {

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
                default:
                    UpdateIdle();
                    break;
            }

            PosInfo.State = (CreateureState)STATE;
            CheckUpdatedFlag();
        }

        protected override void UpdateIdle()
        {
            if(currentScene == Define.Scene.Game)
                CheckAttack();
            PosInfo.State = CreateureState.Idle;
            _inputable = true;
        }

        protected override void UpdateWalk()
        {
            if (currentScene == Define.Scene.Game)
                CheckAttack();

            if (_inputable)
                MovePosition();

            PosInfo.State = CreateureState.Walk;

        }

        protected override void UpdateSprint()
        {
            if (!DashEffectController.isPlaying) // 대쉬 이펙트 재생
                DashEffectController.Play();

            if (STATE == CharacterState.Idle || STATE == CharacterState.Walk || STATE == CharacterState.Sprint)
            {
                STATE = CharacterState.Sprint;
            }

            if (currentScene == Define.Scene.Game)
                CheckAttack();

            if (_inputable)
                MovePosition();

            PosInfo.State = CreateureState.Sprint;

        }



        public void GetExp(int additionalExp)
        {
            lock(_lock)
            {
                C_GetExp expPacket = new C_GetExp();
                expPacket.TotalExp = this.STAT.TotalExp + additionalExp;
                Managers.Network.Send(expPacket);
            }
        }

        public void LevelUp(S_LevelUp levelUpPacket)
        {
            StatInfo newStat = null;
            Managers.Data.StatDict.TryGetValue(levelUpPacket.NewLevel, out newStat);

            STAT.Level = levelUpPacket.NewLevel;
            STAT.TotalExp = levelUpPacket.TotalExp;
            STAT.MaxHp = newStat.MaxHp;
            STAT.Hp = newStat.MaxHp;
            STAT.Attack = newStat.Attack;
        }


        // -------------------------------------------------------------------









        // -------------------------- Network Update -------------------------
        void CheckUpdatedFlag()
        {
            // 업데이트 직전에 위치를 갱신
            VectorPosInfo = transform.position;

            if (_updated)
            {
                // 멀티 에서만 이동 패킷 전송
                if(_isMultiPlay)
                {
                    // 갱신된 위치를 서버에 전송 (서버와 위치 동기화)
                    C_Move movePacket = new C_Move();
                    movePacket.PosInfo = PosInfo;
                    movePacket.PosInfo.DirInfo = DirInfo;
                    Managers.Network.Send(movePacket);
                    _updated = false;
                }
                
            }
        }

        // 스킬 쿨타임 관리 -> 패킷에 의한 네트워크 자원 낭비 방지
        Coroutine CoSkillPacketCooltime = null;
        IEnumerator CoInputColltime(float time)
        {
            yield return new WaitForSeconds(time);
            CoSkillPacketCooltime = null;
        }

        // -------------------------------------------------------------





        // --------------------- Start & Update -------------------------
        void Start()
        {
            Init();
            RefreshAdditionalStat(); // 아이템에 의한 추가 능력치 refresh
        }

        void Update()
        {

            UpdateState();
        }

        // -------------------------------------------------------------


    }
}
