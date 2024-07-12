using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    private class BoneTransform
    {
        public Vector3 Position {  get; set; }                                                              // 위치 유지 위한 공개 속성
        public Quaternion Rotation { get; set; }                                                            // 회전 상태 유지를 위한 공개 속성
    }

    #region Variable
    [Header("Component")]
    private Animator animator;
    private Camera _camera;
    private CharacterController controller;
    private RaycastHit hit;
    private PlatformManager platformManager;
    public Health health;
    public Ragdoll ragdoll;
    public PlayerSoundPack PSP;
    public Health Health => health;

    [Space(10f)]
    [Header("Speed")]
    [SerializeField]
    private float WalkSpeed;
    [SerializeField]
    private float RunSpeed;
    [SerializeField]
    private float nowSpeed;
    public float NowSpeed => nowSpeed;

    [Space(10f)]
    [Header("Check")]
    [SerializeField]
    private bool toggleCameraRotation;
    [SerializeField]
    private bool isRun;
    [SerializeField]
    private bool isGrounded;
    [SerializeField]
    private bool isJump;
    [SerializeField]
    private bool JumpOk;
    [SerializeField]
    public bool isRagdoll;
    [SerializeField]
    private bool isResttingBones;
    [SerializeField]
    private bool onEdge;
    [SerializeField]
    private bool Falling;
    [SerializeField]
    private bool isLanding;
    [SerializeField]
    private bool isMovingBlock;
    [SerializeField]
    private bool isHideBlock;
    [SerializeField]
    private bool isHoldBlock;
    [SerializeField]
    public bool playerDeath;
    [SerializeField]
    private bool InputHold;
    [SerializeField]
    public bool RagdollOK;

    [Space(10f)]
    [Header("Ragdoll")]
    [SerializeField]
    private Vector3 originPosition;
    [SerializeField]
    private int stopCount;
    [SerializeField]
    private float ReturnTIme;
    [SerializeField]
    public Transform _hipsBone;
    [SerializeField]
    private float _timeToResetBone;                                                             // 뼈대 재설정 시 걸리는 시간
    [SerializeField]
    private float marginError;
    [SerializeField]
    private float _elapseResetBonesTime;                                                        // 뼈대 재설정시 까지 경과 시간
    
    private BoneTransform[] _standUpBoneTransforms;                                             // 기상 애니메이션시의 모든뼈대 변환 저장 필드
    private BoneTransform[] _ragdollBoneTransforms;                                             // 랙돌의 모든 뼈대 의 값 저장 필드
    private Transform[] _bone;                                                                  // 각 뼈대의 실제 변환 값 저장

    [Space(10f)]
    [Header("Etc")]
    [SerializeField]
    private float SphereRadius;
    [SerializeField]
    private float JumpPower;
    [SerializeField]
    private float JumpSecond;
    [SerializeField]
    private float Gravity;
    [SerializeField]
    private float smoothness;
    [SerializeField]
    private bool isBackward;
    [SerializeField]
    private float stepOffset;
    [SerializeField]
    private bool GroundCheck_Hold;
    [SerializeField]
    private float LimitGravity;
    [SerializeField]
    private float HoldTime;
    [SerializeField]
    public int LandingLevel;
    [SerializeField]
    public float LandingHeight;
    [SerializeField]
    private int WaitFrame;

    [Tooltip("Robot AttackPoint")]
    [SerializeField]
    public Transform Spine3;

    [Space(10f)]
    public float playerVelocity;

    [SerializeField]
    private Vector3 moveDir { get; set;}
    [SerializeField]
    private Vector2 wallCheck;

    public Vector3 MoveDir => moveDir;

    private GameObject block;
    private int H_Block_Number;
    private float Height;
    [SerializeField]
    private float NowPlayerPosigionY;
    private float PreviousPosition_y { get; set; }
    private bool floating;
    private bool anotherStep = true;
    private Coroutine StepCoroutine;
    #endregion
    private void Awake()
    {
        instance = this;

        _camera = Camera.main;
        ragdoll = GetComponent<Ragdoll>();
        ragdoll.Init();
        platformManager = GetComponent<PlatformManager>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        health = GetComponent<Health>();
        PSP = GetComponent<PlayerSoundPack>();

        _hipsBone = animator.GetBoneTransform(HumanBodyBones.Hips);                                         // 애니메이터 컴포넌트에서 뼈대의 골반에 해당하는 트랜스폼 할당
        _bone = _hipsBone.GetComponentsInChildren<Transform>();                                             // 모든 뼈대의 구성요소 할당
        _standUpBoneTransforms = new BoneTransform[_bone.Length];                                           // 뼈대의 구성요소 수 만큼의 배열 크기 설정
        _ragdollBoneTransforms = new BoneTransform[_bone.Length];

        for(int boneIndex=0; boneIndex<_bone.Length; boneIndex++)                                           // 각 배열 요소 초기화
        {
            _standUpBoneTransforms[boneIndex] = new BoneTransform();
            _ragdollBoneTransforms[boneIndex] = new BoneTransform();
        }

        PopulateAnimationStartBoneTransforms("GetUp", _standUpBoneTransforms);                              // 기상 애니메이션 이름과 기상 애니메이션시의 회전, 위치 값 저장할 클래스 전달
    }
    /// <summary>
    /// 초기 설정
    /// </summary>
    private void Start()
    {
        JumpOk = true;
        RagdollOK = true;
        Height = controller.height;

        PreviousPosition_y = transform.position.y;
    }
    private void Update()
    {
        if (!GameManager.instance.isStart)
            return;

        if (GameManager.instance.isPause)
            return;

        if (playerDeath)
            return;

        if (isRagdoll)
        {
            if (isResttingBones)
            {
                _elapseResetBonesTime += Time.deltaTime;
                ResttingBonesBehaviour();
            }

            return;
        }
        else
        {
            Move();
            GroundCheck();

            #region anoterAction
            if (Input.GetKey(Utils.Instance.binding.Bindings[Action.RotateCamera]))
            {
                toggleCameraRotation = true;                                                                    // 둘러보기 활성화
            }
            else
            {
                toggleCameraRotation = false;                                                                   // 둘러보기 비활성화
            }

            if (Input.GetKey(Utils.Instance.binding.Bindings[Action.Dash]))
            {
                isRun = true;                                                                                   // 뛰기 활성화
            }
            else
            {
                isRun = false;                                                                                  // 뛰기 비활성화
            }

            if (Input.GetKeyDown(Utils.Instance.binding.Bindings[Action.Ragdoll]) && !isRagdoll && RagdollOK)
            {
                isRagdoll = true;                                                                               // 랙돌 활성화
                RagdollOK = false;
                RagdollOn();
            }
            #endregion

            if (isGrounded && !isLanding && !InputHold)
            {
                InputMovement();                                                                                // 플레이어 이동
            }
        }
    }
    private void LateUpdate()
    {
        if (!GameManager.instance.isStart)
            return;

        if (GameManager.instance.isPause)
            return;

        if (playerDeath)
            return;

        if (!toggleCameraRotation && !isRagdoll && !isLanding)
        {
            RotateCamera();
        }
    }
    /// <summary>
    /// 플레이어가 인게임에서 카메라를 통해 보고 있는 방향과 캐릭터가 보고 있는 방향 통일
    /// </summary>
    private void RotateCamera()
    {
        Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));                                                                  // 현재 카메라가 보고 있는 정면 방향에서 y값을 빼고
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);                          // 플레이어와 현재 카메라가 보고 있는 회전 상태값을 기준으로 플레이어를 구면 회전(결론적으로 카메라가 보고있는 방향을 플레이어가 정면으로 향함)
    }
    /// <summary>
    /// 플레이어가 조종하는 캐릭터의 움직임 및 관련 애니메이션 조작 함수
    /// </summary>
    private void InputMovement()
    {
        nowSpeed = isRun ? RunSpeed : WalkSpeed;                                                                                                                // 캐릭터 움직임 결정
        nowSpeed *= isBackward ? 0.5f : 1f;                                                                                                                     // 뒤로 움직일 시 이동속도 절반 감소

        Vector3 forward = transform.TransformDirection(Vector3.forward);                                                                                        // 캐릭터 기준 상 하
        Vector3 right = transform.TransformDirection(Vector3.right);                                                                                            // 캐릭터 기준 좌 우

        BackWardChack();                                                                                                                                        // 정면을 기준으로 뒤로 걷는지 여부 확인

        moveDir = (forward * InputVertical() + right * InputHorizontal()).normalized * nowSpeed;                                                                // 각 벡터에 입력값(상 하 * 입력값(앞, 뒤), 좌 우 * 입력값(왼, 오))를 곱하고 더해서 방향값 계산
        moveDir = new Vector3(moveDir.x, playerVelocity, moveDir.z);

        if (Input.GetKey(Utils.Instance.binding.Bindings[Action.Jump]) && !isJump && JumpOk)                                                                    // 모든 조건성립 시 점프
        {
            PSP.Jump_S();

            GroundCheck_Hold = true;
            PreviousPosition_y = transform.position.y;
            Falling = true;
            JumpOk = false;
            isJump = true;
            animator.SetTrigger("isJump");
            playerVelocity = Mathf.Sqrt(JumpPower * -2.0f * Gravity);                                                                                           // 제곱근으로 점프 높이 조절
            moveDir = new Vector3(moveDir.x, playerVelocity, moveDir.z);

            GameObject Dust = GameManager.instance.effectpool.SpawnEffect(2, GameManager.instance.effectpool.Jumps_P.transform);                                // 점프 이펙트 생성
            Dust.transform.position = this.transform.position;

            StartCoroutine(nameof(Jumping));
        }

        float percent_X = (isRun ? 1 : 0.5f) * InputHorizontal();                                                                                               // 입력된 수평의 값(좌 우)에 상태(걷기, 뛰기)에 대한 속도값 반영
        float percent_Y = (isRun ? 1 : 0.5f) * InputVertical();                                                                                                 // 입력된 수직의 값(좌 우)에 상태(걷기, 뛰기)에 대한 속도값 반영

        animator.SetFloat("Pos X", percent_X, 0.1f, Time.deltaTime);                                                                                            // 파라미터에 계산된 Percent_X의 값을 반영
        animator.SetFloat("Pos Y", percent_Y, 0.1f, Time.deltaTime);                                                                                            // 파라미터에 계산된 Percent_Y의 값을 반영
    }
    /// <summary>
    /// 벽면에 가깝게 점프 중일때 벽면오브젝트의 지면에 닿았을 시 지면 인식으로 점프가 중단되는 것을 방지하는 함수
    /// </summary>
    /// <returns></returns>
    private IEnumerator Jumping()
    {
        yield return new WaitForSeconds(JumpSecond);                                                                    // 에디터에서 설정한 float값 만큼 대기
        GroundCheck_Hold = false;
    }
    /// <summary>
    /// 수평 입력 함수
    /// </summary>
    /// <returns></returns>
    private float InputHorizontal()
    {
        float horizontalInput = 0;

        if (Input.GetKey(Utils.Instance.binding.Bindings[Action.MoveRight]))
        {
            horizontalInput = 1;
        }
        else if (Input.GetKey(Utils.Instance.binding.Bindings[Action.MoveLeft]))
        {
            horizontalInput = -1;
        }

        return horizontalInput;
    }
    /// <summary>
    /// 수직 입력 함수
    /// </summary>
    /// <returns></returns>
    private float InputVertical()
    {
        float verticalInput = 0;

        if (Input.GetKey(Utils.Instance.binding.Bindings[Action.MoveForward]))
        {
            verticalInput = 1;
        }
        else if (Input.GetKey(Utils.Instance.binding.Bindings[Action.MoveBackward]))
        {
            verticalInput = -1f;
        }

        return verticalInput;
    }
    /// <summary>
    /// 뒤로 가는 키(리버스) 입력 여부 확인
    /// </summary>
    private void BackWardChack()
    {
        if (Input.GetKey(Utils.Instance.binding.Bindings[Action.MoveBackward]))
        {
            isBackward = true;
        }
        else
        {
            isBackward = false;
        }
    }
    /// <summary>
    /// 상자형태의 광선을 쏴서 캐릭터가 바닥에 닿아있는지 여부 확인 함수
    /// </summary>
    private void GroundCheck()
    {
        int layerMask = (-1) - (1 << LayerMask.NameToLayer("NotObstacle"));

        isGrounded = Physics.SphereCast(controller.GetComponent<Collider>().bounds.center, SphereRadius, -transform.up, out hit, Height/2, layerMask);                     // 일반 Raycast를 쏘면 점으로 여부를 검사하기에 좀 더 정확하게 확인하기 위함

        if (isGrounded && !GroundCheck_Hold)
        {
            if (Falling)
            {
                Falling = false;
                Landing();
            }

            onEdge = false;
            floating = false;
            anotherStep = true;
            StopCoroutine(nameof(PlayerHeightCheck));

            if(PreviousPosition_y != transform.position.y)
                PreviousPosition_y = transform.position.y;

            playerVelocity = -2.0f;

            if (hit.collider.gameObject.CompareTag("MovingBlock") && !isMovingBlock)                                                                                      // 이동 발판 밟을 시 이동발판의 종속으로
            {
                isMovingBlock = true;
                block = hit.collider.gameObject;
                platformManager.PlatformChange(hit.collider.gameObject, this.gameObject);
            }
            else if (hit.collider.gameObject.CompareTag("HoldBlock") && !isHoldBlock)
            {
                isHoldBlock = true;
                block = hit.collider.gameObject;
                hit.collider.GetComponent<HoldBlock>().TriggerOn();
            }
            else if (hit.collider.gameObject.CompareTag("HideBlock") && !isHideBlock)
            {
                isHideBlock = true;
                block = hit.collider.gameObject;
                H_Block_Number = hit.collider.GetComponent<HideBlock>().ColorNumber;
                hit.collider.GetComponent<HideBlock>().HM.CheckNumberAppear(H_Block_Number);
            }
            else if (hit.collider.gameObject.CompareTag("HideBlock") && isHideBlock && hit.collider.GetComponent<HideBlock>().ColorNumber != H_Block_Number)
            {
                block.GetComponent<HideBlock>().HM.CheckNumberHide(H_Block_Number);

                block = hit.collider.gameObject;
                H_Block_Number = hit.collider.GetComponent<HideBlock>().ColorNumber;
                hit.collider.GetComponent<HideBlock>().HM.CheckNumberAppear(H_Block_Number);
            }
            else
            {
                if (transform.parent != null && !isMovingBlock)
                {
                    transform.SetParent(null);
                    try
                    {
                        platformManager.ClearPlatformData();
                    }
                    catch (NullReferenceException) { }
                }
            }
        }
        else
        {
            if (StepCoroutine == null)
            {
                StepCoroutine = StartCoroutine(nameof(CalcisIgnore));
            }

            if (Physics.SphereCast(controller.GetComponent<Collider>().bounds.center, SphereRadius, -transform.up, out hit, Height / 2 + 1.3f, layerMask) && anotherStep)
            {
                return;
            }

            if (transform.parent != null)
            {
                transform.SetParent(null);
                try
                {
                    platformManager.ClearPlatformData();
                }
                catch (NullReferenceException) { }
            }

            if (!Falling)
            {
                animator.SetTrigger("isJump");
                PreviousPosition_y = transform.position.y;
            }

            animator.SetBool("nowJump", true);

            Falling = true;

            GravityCalc();

            if (isHideBlock)
            {
                isHideBlock = false;
                block.GetComponent<HideBlock>().HM.CheckNumberHide(H_Block_Number);
            }
            isMovingBlock = false;
            isHoldBlock = false;
            block = null;

            if (!floating)
            {
                floating = true;
                StartCoroutine(nameof(PlayerHeightCheck));
            }
        }
    }
    private IEnumerator CalcisIgnore()
    {
        int Count = 0;

        while (Count < WaitFrame)
        {
            yield return new WaitForEndOfFrame();
            Count++;
        }

        anotherStep = false;
        StepCoroutine = null;
    }
    /// <summary>
    /// 착지 진행 함수
    /// </summary>
    public void Landing()
    {
        bool BigDamage;

        moveDir = Vector3.zero;                                                                                         // 움직임 제한

        animator.SetFloat("Pos X", 0f);                                                                                 // animator에 남아있는 float 변수 값 초기화     
        animator.SetFloat("Pos Y", 0f);

        isJump = false;
        isLanding = true;                                                                                               // 착륙 중일때 동작 불가능 하게 하기 위한 플래그 변수 값 변경

        BigDamage = health.CalcDamage(CalcDistance_Height() - LandingHeight);

        if (!BigDamage)
        {
            LandingAction();                                                                                             // 높이에 따른 애니메이션 결정
            animator.SetBool("nowJump", false);                                                                          // 착륙 애니메이션으로 전환을 위한 애니메이터 bool값 변경
        }
        else
        {
            isRagdoll = true;                                                                                            // 랙돌 활성화
            RagdollOn();
        }
    }
    /// <summary>
    /// 플레이어 착지 시와 Falling 직전의 Y값을 기반, 거리를 계산
    /// </summary>
    /// <returns></returns>
    public float CalcDistance_Height()
    {
        float _D;

        Vector3 Prev = new Vector3(0, PreviousPosition_y, 0);
        Vector3 Now = new Vector3(0, transform.position.y, 0);

        _D = Vector3.Distance(Prev, Now);

        return _D;
    }
    /// <summary>
    /// 플레이어가 착지한 순간의 Position.y값과 이전 밟고 있던 곳의 Position.y 값의 비교를 통한 착지 모션 결정 함수
    /// </summary>
    private void LandingAction()
    {
        if (CalcDistance_Height() > LandingHeight && transform.position.y < PreviousPosition_y)                                   // 현재 착지지점, 이전 착지지점의 Y값 계산 및 높낮이 비교
        {
            PSP.Landing_B_B();
            LandingLevel = 1;
        }
        else
        {
            PSP.Landing_S_S();
            LandingLevel = -1;
        }

        PreviousPosition_y = transform.position.y;
        animator.SetInteger("Hard", LandingLevel);                                                                               // LandingLevel에 따른 착지 모션 결정

        GameObject Dust = GameManager.instance.effectpool.SpawnEffect(3, GameManager.instance.effectpool.Landings_P.transform);     // 착지 이펙트 생성
        Dust.transform.position = this.transform.position;
    }
    /// <summary>
    /// 플레이어 강제 리스폰을 위한 강제 사망 및 리스폰
    /// </summary>
    public void ForcedDeath()
    {
        health.CalcDamage(100);

        StopCoroutine(nameof(EnumCheckisCharacter));

        animator.SetFloat("Pos X", 0f);                                                                                 // animator에 남아있는 float 변수 값 초기화     
        animator.SetFloat("Pos Y", 0f);

        isJump = false;
        isLanding = true;                                                                                               // 착륙 중일때 동작 불가능 하게 하기 위한 플래그 변수 값 변경

        isRagdoll = true;                                                                                               // 랙돌 활성화
        RagdollOn();
    }
    /// <summary>
    /// 외부 데미지 수행 함수
    /// </summary>
    /// <param name="Damage"></param>
    public void GetDamage(float Damage)
    {
        health.CalcDamage(Damage);
    }
    /// <summary>
    /// 착지 종료 애니메이션 종료시 호출될 이벤트 함수
    /// </summary>
    public void LandingOver()
    {
        isLanding = false;
        Invoke(nameof(JumpPossible), 0.2f);
        animator.SetBool("nowJump", false);
        animator.SetInteger("Hard", 0);
    }
    private void JumpPossible()
    {
        JumpOk = true;
    }
    /// <summary>
    /// 모서리 부분에 캐릭터가 착지 하였을 시 지면인식이 되지 않아 공중에 떠서 이도저도 못하는 상황 직면 시 바닥으로 캐릭터를 점차 이동시키는 함수
    /// bool 로 선언해서 에디터 상으로 확인 가능
    /// </summary>
    /// <returns></returns>
    private bool isSlipChecker()
    {
        RaycastHit _hit;
        Vector3 ray_spawn_pos = transform.position + Vector3.up * wallCheck.y;                                                                      // 에디터상에서 설정한 벡터의 y값을 기준 캐릭터에서 나오는 Raycast 높이 결정

        Vector3 forward = transform.forward * wallCheck.x;                                                                                          // 캐릭터 기준 4방향으로 Raycast 발사
        Vector3 backward = -transform.forward * wallCheck.x;
        Vector3 right = transform.right * wallCheck.x;
        Vector3 left = -transform.right * wallCheck.x;

        Ray front_ray = new Ray(ray_spawn_pos, forward);
        Ray back_ray = new Ray(ray_spawn_pos, backward);
        Ray right_ray = new Ray(ray_spawn_pos, right);
        Ray left_ray = new Ray(ray_spawn_pos, left);

        float dis = wallCheck.x;                                                                                                                    // 캐릭터의 크기에 맞게끔 조절(캐릭터보다 0.03~0.05 정도 크게)

        if(Physics.Raycast(front_ray,out _hit, dis))                                                                                                // 캐릭터 기준 정면에 붙었을 시
        {
            HitForSlip(transform.forward);                                                                                                          // 캐릭터의 정면 위치 값 전달
            return true;
        }

        if(Physics.Raycast(back_ray, out _hit, dis) || Physics.Raycast(right_ray, out _hit, dis) || Physics.Raycast(left_ray, out _hit, dis))       // 캐릭터 기준 후면, 옆면에 붙었을 시
        {
            HitForSlip(hit.normal);                                                                                                                 // 캐릭터의 Hit한 지점 전달
            return true;
        }

        return false;
    }
    /// <summary>
    /// 점프 중 임을 알리는 플래그 변수 값 변경 및 캐릭터를 전달된 hitPoint로 이동 보간하여 이동
    /// </summary>
    /// <param name="slip_dir"></param>
    private void HitForSlip(Vector3 slip_dir)
    {
        if (isJump)                                                                                                                     // 혹 점프중이였다면
        {
            playerVelocity = 0f;                                                                                                        // 중력 함수에서 사용되는 float 변수의 값이 무한정 증가되는 것을 방지하기 위한 초기화 진행
            isJump = false;                                                                                                             // 플래그 변수 값 변경
        }

        transform.position = Vector3.Lerp(transform.position, slip_dir, Time.deltaTime);                                                // 캐릭터를 전달받은 HitPoint로 이동
    }                                                   
    /// <summary>
    /// 실제 이동
    /// </summary>
    private void Move()
    {
        controller.Move(moveDir * Time.deltaTime);                                                                                        // 캐릭터 컨트롤러에 방향값 일반화(대각 벡터는 값이 조금 커 다르기때문) 후 속도를 곱한값을 전달하여 실제 캐릭터 이동
    }
    /// <summary>
    /// 중력 가속도 부여
    /// </summary>
    private void GravityCalc()
    {
        if (playerVelocity > -LimitGravity)                                                                            // 일정 수준 이상의 속도로 떨어지는 것을 방지(빠른 속도로 인한 Collider 뚫림 방지)
        {
            playerVelocity += Gravity * Time.deltaTime;
        }

        moveDir = new Vector3(moveDir.x, playerVelocity, moveDir.z);
    }
    /// <summary>
    /// 일시 정지 시 캐릭터가 멈춘것처럼 보이기 위한 설정 함수
    /// </summary>
    public void isPause()
    {
        ragdoll.PauseRagdoll();                                                                         // Ragdoll 매쉬 바디(Rigidbody가 적용된) 비활성화
        animator.speed = 0;                                                                             // animator 속도 정지
        controller.enabled = false;                                                                     // 컨트롤러 비활성화 
    }
    /// <summary>
    /// 재게 시 캐릭터 다시 재생
    /// </summary>
    public void isResume()
    {
        ragdoll.UnPauseRagdoll();                                                                   // Rigidbody가 적용된 Ragdoll 매쉬 바디 활성화
        animator.speed = 1;                                                                         // animator 속도 일반속도로 재정의
        controller.enabled = true;                                                                  // 컨트롤러 활성화
    }
    /// <summary>
    /// Player 부활 함수
    /// </summary>
    public void Revive()
    {
        playerDeath = false;

        _hipsBone = animator.GetBoneTransform(HumanBodyBones.Hips);

        animator.SetFloat("Pos X", 0f);    
        animator.SetFloat("Pos Y", 0f);

        animator.SetBool("nowJump", false);
        animator.SetInteger("Hard", 0);

        RagdollOff();
    }
    /// <summary>
    /// Player 죽음 함수
    /// </summary>
    public void Death()
    {
        PSP.Death_S();

        playerDeath = true;
        Falling = false;

        GameManager.instance.totalDeath++;

        GameObject Explosion = GameManager.instance.effectpool.SpawnEffect(6, GameManager.instance.effectpool.Sparks_P.transform);                                // 사망 이펙트 생성
        Explosion.transform.position = _hipsBone.position;
    }
    /// <summary>
    /// 캐릭터의 Y값 저장(Edgy 보정 시 필요)
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayerHeightCheck()
    {
        while (Falling)
        {
            NowPlayerPosigionY = transform.position.y;
            yield return new WaitForSeconds(0.5f);
        }
    }
    /// <summary>
    /// Player의 Edgy 접지시 Ground 인식 불가 보정
    /// </summary>
    /// <param name="hit"></param>
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (Falling && !isGrounded)
        {
            if (NowPlayerPosigionY == transform.position.y)
                onEdge = isSlipChecker();
        }
        else
            onEdge = false;

        if (hit.gameObject.CompareTag("DeathZone") && !GameManager.instance.isForcedDeath)                                      // 플레이어 강제 사망 존 접촉 시 사망 적용
        {
            if (hit.gameObject.GetComponent<DeathZone>())
                if (!hit.gameObject.GetComponent<DeathZone>().Nontouch)
                    GameManager.instance.ForcedDeath();
        }
    }
    /// <summary>
    /// 포탈 접촉 시 지정된 포인트로 순간이동
    /// </summary>
    /// <param name="arrivePoint"></param>
    public void Teleport(Transform arrivePoint)
    {
        moveDir = Vector3.zero;

        if (!isRagdoll)
        {
            isRagdoll = true;
            RagdollOK = false;
            RagdollOn();
        }
        else
            ragdoll.InitVelocity();

        PreviousPosition_y = arrivePoint.position.y;
    }
    #region IdleAnimation
    /// <summary>
    /// Idle 애니메이션 전환
    /// </summary>
    public void PlayerBoard()
    {
        animator.SetBool("NowBoard", true);
        animator.SetFloat("Idle", 0f);

        animator.SetFloat("Pos X", 0);
        animator.SetFloat("Pos Y", 0);
    }
    /// <summary>
    /// Nomal 애니메이션 전환
    /// </summary>
    public void PlayerNomal()
    {
        animator.SetBool("NowBoard", false);
    }
    #endregion
    /// <summary>
    /// Stage Clear에 대한 캐릭터 애니메이션 전환
    /// </summary>
    public void StageFinish()
    {
        animator.SetTrigger("Finish");
    }
    #region Ragdoll
    /// <summary>
    /// 랙돌 On
    /// </summary>
    public void RagdollOn()
    {
        animator.SetBool("isRagdoll", true);
        animator.enabled = false;

        InputHold = true;

        controller.enabled = false;                                                                                     // 컨트롤러 비활성화
        controller.stepOffset = 0;

        ragdoll.RagdollOn();                                                                                            // Ragdoll 설정으로 Rigidbody 및 조인트가 연결된 각 몸체 활성화(Rigdbody, collider 설정 변환)

        if (!GameManager.instance.isForcedDeath)
        {
            StartCoroutine(nameof(EnumCheckisCharacter));                                                               // Ragdoll 상태의 캐릭터가 일정 시간 한 장소에 머무를 경우 Ragdoll 해제를 위한 코루틴
        }
    }
    /// <summary>
    /// 랙돌 Off
    /// </summary>
    private void RagdollOff()
    {
        moveDir = Vector3.zero;

        animator.SetFloat("Pos X", 0f);
        animator.SetFloat("Pos Y", 0f);

        ragdoll.RagdollOff();                                                                                               // Ragdoll 설정으로 Rigidbody 및 조인트가 연결된 각 몸체 비활성화

        AlignRotationToHips();                                                                                              // 애니메이션의 첫 프레임 기준 Hip(중심)의 회전값을 현재 Hip에 적용 (회전 후 포지션 재 정의, 반대의 경우 회전하면서 포지션 값이 조금 변경되기때문)
        AlignPositionToHips();                                                                                              // 애니메이션의 첫 프레임 기준 Hip(중심)의 위치값을 현재 Hip에 적용

        if (GameManager.instance.isForcedDeath)
            ReSettingPosition();

        PopulateBoneTransforms(_ragdollBoneTransforms);                                                                     // 기상 애니메이션 직전 바닥에 누워있는 캐릭터의 뼈 위치 및 회전값 저장

        isResttingBones = true;                                                                                             // 뼈대의 모든 회전, 위치 재정의가 이루어졌음을 확인
        _elapseResetBonesTime = 0;                                                                                          // 위치 보간 전 경과시간 변수 초기화

        controller.enabled = true;                                                                                          // 컨트롤러 재활성화
        controller.stepOffset = stepOffset;
        PreviousPosition_y = transform.position.y;

        Falling = false;
        LandingOver();
        isJump = false;
    }
    private void ReSettingPosition()
    {
        GameManager.instance.rm.Transmission_Player(this);
    }
    /// <summary>
    /// (기상 애니메이션 끝 부분에 이벤트 삽입)기상 애니메이션 Bool값 변형으로 탈출 및 bool 제한 변수 값 변경
    /// </summary>
    private void ReturnCharacter()
    {
        animator.SetBool("isRagdoll", false);                                                                                               // 기상 애니메이션 종료 제어
        isRagdoll = false;                                                                                                                  
    }
    /// <summary>
    /// 랙돌 후 캐릭터의 위치가 일정 시간 변하지 않고 위치한다면 랙돌 해제
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnumCheckisCharacter()
    {
        stopCount = 0;

        while (true)
        {
            while (GameManager.instance.isPause)                                                                                                // 일시정지 시 무한정 연기(딜레이)
            {
                yield return null;
            }

            if (originPosition != null)
            {
                if ((_hipsBone.position - originPosition).magnitude < marginError)                                                        // 몸통의 현재 위치와 이전 위치의 크기가 오차범위보다 작다면(marginError - 오차범위)
                {
                    stopCount++;

                    if (stopCount > 10)
                    {
                        RagdollOff();                                                                                                           // Ragdoll에서 다시 조작 가능 상태로 전환
                        break;
                    }
                    else
                        isRagdoll = true;
                }
                else
                {
                    isRagdoll = true;
                    stopCount = 0;
                }
            }
            originPosition = _hipsBone.position;                                                                                          // Hip(몸통 중심)위치 저장
            yield return new WaitForSeconds(0.1f * ReturnTIme);                                                                                 // 기상 까지 대기 시간 조절 (((0.1*n))*10초)
        }
    }
    /// <summary>
    /// 애니메이션 Hip(Spine)을 기준으로 회전값 재지정
    /// </summary>
    private void AlignRotationToHips()
    {
        Vector3 originalHipsPosition = _hipsBone.position;
        Quaternion originalHipsRotation = _hipsBone.rotation;

        Vector3 desiredDirection = _hipsBone.up * -1;                                                                           // 뼈대의 중심 아래쪽 방향의 되어 발쪽을 가리키게 설정
        desiredDirection.y = 0;                                                                                                 // 캐릭터가 정면을 향하도록 하기 위해 y 제거
        desiredDirection.Normalize();                                                                                           // 캐릭터의 현재 정방향 설정

        Quaternion fromToRotation = Quaternion.FromToRotation(transform.forward, desiredDirection);                             // 캐릭터가 정방향에서 원하고자 하는 방향으로 회전
        transform.rotation *= fromToRotation;

        _hipsBone.position = originalHipsPosition;                                                                              // 캐릭터 회전 후 뼈대의 중심 또한 같이 회전하기에 기존 값으로 재설정
        _hipsBone.rotation = originalHipsRotation;
    }
    /// <summary>
    /// 애니메이션의 Hip(Spine)을 기준으로 위치 재지정
    /// </summary>
    private void AlignPositionToHips()
    {
        Vector3 originalHipsPosition = _hipsBone.position;
        transform.position = _hipsBone.position;                                                                                // 캐릭터 뼈대의 중심을 오브젝트의 위치로 설정

        Vector3 positionOffset = _standUpBoneTransforms[0].Position;                                                            // '기상' 애니메이션에서의 뼈대의 값을 가져와
        positionOffset.y = 0;                                                                                                   // y값 제거 후(위 아래 변동 없이)
        positionOffset = transform.rotation * positionOffset;                                                                   // 캐릭터의 회전값에 이 오프셋 곱계산
        transform.position -= positionOffset;                                                                                   // 오프셋만틈의 값을 캐릭터의 위치에서 빼주어 오차 범위 계산

        if(Physics.Raycast(transform.position,Vector3.down,out RaycastHit hitInfo))
        {
            transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
        }

        _hipsBone.position = originalHipsPosition;
    }
    /// <summary>
    /// 뼈대의 현재 위치와 회전 값 저장 
    /// </summary>
    private void PopulateBoneTransforms(BoneTransform[] boneTransforms)
    {
        for(int boneIndex=0; boneIndex<boneTransforms.Length; boneIndex++)
        {
            boneTransforms[boneIndex].Position = _bone[boneIndex].localPosition;                                                        // 뼈대의 월드 좌표가 아닌 로컬 좌표값이 필요하기에 로컬좌표값 할당
            boneTransforms[boneIndex].Rotation = _bone[boneIndex].localRotation;
        }
    }
    /// <summary>
    /// 애니메이션 기준 오브젝트의 회전값 및 위치값 적용
    /// </summary>
    /// <param name="clipName"></param>
    /// <param name="boneTransforms"></param>
    private void PopulateAnimationStartBoneTransforms(string clipName, BoneTransform[] boneTransforms)
    {
        Vector3 positionBeforeSampling = transform.position;                                                                            // 샘플링 전 오브젝트의 위치, 회전 값 저장
        Quaternion rotationBeforeSampling = transform.rotation;

        foreach(AnimationClip clip in animator.runtimeAnimatorController.animationClips)                                                // 애니메이션 클립의 이름으로 '기상' 애니메이션 찾기
        {
            if(clip.name == clipName)
            {
                clip.SampleAnimation(gameObject, 0);                                                                                    // 캐릭터 오브젝트에 '기상' 애니메이션의 첫 프레임의 애니메이션을 적용, 게임 오브젝트의 위치, 회전 업데이트
                PopulateBoneTransforms(_standUpBoneTransforms);                                                                         // '기상' 애니메이션의 값을 오브젝트에 업데이트한 값들을 그대로 할당
                break;
            }
        }

        transform.position = positionBeforeSampling;                                                                                    // 샘플링 되면서 변환된 오브젝트의 위치, 회전 값을 샘플링 전의 값으로 다시 재설정
        transform.rotation = rotationBeforeSampling;
    }
    /// <summary>
    /// 래그돌 상태의 각 뼈대의 회전값, 포지션 값을 애니메이션 상태의 각 뼈대의 회전값, 포지션 값으로 보간하여 설정
    /// </summary>
    private void ResttingBonesBehaviour()
    {
        float elapsedPercentage = _elapseResetBonesTime / _timeToResetBone;                                                             // 경과 시간을 총 시간으로 나누어 경과 비율 계산

        for(int boneIndex=0;boneIndex<_bone.Length;boneIndex++)
        {
            _bone[boneIndex].localPosition = Vector3.Lerp(_ragdollBoneTransforms[boneIndex].Position,                                   // 래그돌 포지션 상태의 뼈를 애니메이션 포지션 상태의 뼈 위치로 전환, 이때 계산한 백분율로 보간
                _standUpBoneTransforms[boneIndex].Position, elapsedPercentage);

            _bone[boneIndex].localRotation = Quaternion.Lerp(_ragdollBoneTransforms[boneIndex].Rotation,                                // 래그돌 회전 상태의 뼈를 애니메이션 회전 상태의 뼈 위치로 전환
                _standUpBoneTransforms[boneIndex].Rotation, elapsedPercentage);
        }

        if (elapsedPercentage >= 1)                                                                                                     // 전환이 완료되었다면
        {
            isResttingBones = false;                                                                                                    // bool 변수 초기화
            animator.enabled = true;                                                                                                    // 애니메이터 활성화
            animator.Play("GetUp");                                                                                                     // 기상 애니메이션 재생
            StartCoroutine(nameof(InputKeyHold));                                                                                       // 기상 애니메이션 재생 후 Idle로 전환 되고 나서 재생까지 유예시간 적용
        }
    }
    /// <summary>
    /// 기상 애니메이션 직후 동시에 키 입력이 이루어질 경우 애니메이션의 부자연스러운 연결로 키 입력까지의 유예시간 적용 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator InputKeyHold()
    {
        yield return new WaitForSeconds(HoldTime);
        InputHold = false;
        RagdollOK = true;

        if (GameManager.instance.isForcedDeath)
            GameManager.instance.isForcedDeath = false;
    }
    #endregion
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(controller.GetComponent<Collider>().bounds.center + (-transform.up * Height/2), SphereRadius);                            // SphereCast

            Vector3 ray_spawn_pos = transform.position + Vector3.up * wallCheck.y;                                                                          // RaycastSlip

            Vector3 forward = transform.forward * wallCheck.x;
            Vector3 backward = -transform.forward * wallCheck.x;
            Vector3 right = transform.right * wallCheck.x;
            Vector3 left = -transform.right * wallCheck.x;

            Gizmos.DrawRay(ray_spawn_pos, forward);
            Gizmos.DrawRay(ray_spawn_pos, backward);
            Gizmos.DrawRay(ray_spawn_pos, right);
            Gizmos.DrawRay(ray_spawn_pos, left);
        }
    }
#endif
}
// SetFloat("a", b, v, d) : a = 파라미터 이름, b = 파라미터에 할당하고자 하는 값, v = 이전의 값에서 현재 할당하려 하는 값 전환 시간, d = v에서 전환 시간에 사용되기 위한 detaTime
// SampleAnimation(a, n) :  a = 오브젝트, n 프레임(시간) / 미리 렌더링 하지 않고 실시간으로 애니메이션을 오브젝트에 적용할때 사용(회전, 크기 ,위치 등의 값을 업데이트함)

