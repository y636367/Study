using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    #region Variable
    [SerializeField]
    private Transform p_transform;                                                                          // 플레이어 트랜스폼
    [SerializeField]
    private float followspeed;                                                                              // 카메라 추적 속도
    [SerializeField]
    private float DefalutFollowSpeed;                                                                       // 카메라 추적 기본 속도
    [SerializeField]            
    private float MaxFollowSpeed;                                                                           // 카메라 추적 최고 속도
    [SerializeField]
    private float zoomspeed;                                                                                // 줌 속도
    [SerializeField]
    private float sensitivity;                                                                              // 마우스 감도
    [SerializeField]
    private float clampAngle;                                                                               // 회전 제한각
    [SerializeField]
    private float smoothness;                                                                               // 카메라 이동 시 부드러움 적용할 수치값
    [SerializeField]
    private float minDistance;                                                                              // 카메라와 플레이어 간의 최소 거리
    [SerializeField]   
    private float maxDistance;                                                                              // 카메라와 플레이어 간의 최대 거리
    [SerializeField]
    private float nowMaxDistance;                                                                           // 현 카메라와 플레이어 간의 최대 거리

    [Space(10f)]
    [SerializeField]
    private float MovingCheckCount;

    private float rotX;                                                                                     // 마우스 회전값 X
    private float rotY;                                                                                     // 마우스 회전값 Y

    private float mouseX;
    private float mouseY;

    private Transform mainCamera;
    private Vector3 dirNormalized;                                                                          // 방향 값
    private Vector3 finalDir;

    private Quaternion previousRotaition;    

    [SerializeField]
    private float finalDistance;
    #endregion
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        Cursor_Locking_Hide();
        Init();
    }
    /// <summary>
    /// 변수 값들 초기화
    /// </summary>
    private void Init()
    {
        mainCamera =  Camera.main.transform;

        rotX = transform.localRotation.eulerAngles.x;                                                       // 현재 카메라의 회전 값으로 초기화
        rotY = transform.localRotation.eulerAngles.y;

        nowMaxDistance = 5.5f;
        followspeed = DefalutFollowSpeed;

        dirNormalized = mainCamera.localPosition.normalized;                                                // 카메라의 위치 값 및 방향 초기화
        finalDistance = mainCamera.localPosition.magnitude;
    }
    /// <summary>
    /// 마우스 커서 고정 및 숨기기
    /// </summary>
    public void Cursor_Locking_Hide()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    /// <summary>
    /// 마우스 커서 고정풀기 및 보이기
    /// </summary>
    public void Cursor_UnLocking_Show()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void Update()
    {
        if (!GameManager.instance.isStart)
            return;

        if (GameManager.instance.isPause)
            return;

        MouseMove();
    }
    private void LateUpdate()
    {
        if (!GameManager.instance.isStart)
            return;

        if (GameManager.instance.isPause)
            return;

        Zoom();
        DistanceCheck();
        FollowCamera();
    }
    /// <summary>
    /// 마우스 이동 값에 대한 카메라 각도 계산
    /// </summary>
    private void MouseMove()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        rotX += -(mouseY) * sensitivity * Time.deltaTime;                                 // X 축 회전을 하려면 마우스 위아래로               
        rotY += mouseX * sensitivity * Time.deltaTime;                                    // Y 축 회전을 하려면 마우스 좌우로

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);                                                  // 마우스 상하 각도 제한

        if(Mathf.Abs(mouseX)>Mathf.Epsilon && Mathf.Abs(mouseY) > Mathf.Epsilon)                                // 마우스 조작 여부 확인
        {
            GameManager.instance.CameraisMoving = true;
        }
        else
        {
            GameManager.instance.CameraisMoving = false;                                                    
        }

        Quaternion rot = Quaternion.Euler(rotX, rotY, 0);
        transform.rotation = rot;                                                                           // 최종 회전 값
    }
    /// <summary>
    /// 카메라가 플레이어를 따라가는 함수
    /// </summary>
    private void FollowCamera()
    {
        transform.position = Vector3.MoveTowards(transform.position, p_transform.position, followspeed * Time.deltaTime);                            // 플레이어를 따라 이동하는 카메라 위치 값

        finalDir = transform.TransformPoint(dirNormalized * nowMaxDistance);                                                                         // 카메라와 플레이어 간의 최대 거리 만틈 사잇값 및 카메라가 플레이어를 바라보는 방향 값

        RaycastHit hit;
        int layerMask = (-1) - (1 << LayerMask.NameToLayer("NotObstacle") | 1 << LayerMask.NameToLayer("Player")
            | 1 << LayerMask.NameToLayer("NotDetection"));                                                                                           // 비장애물 레이어로 지정된 오브젝트 제외하고 충돌체크

        if(Physics.Linecast(transform.position,finalDir, out hit, layerMask))
        {
            finalDistance = Mathf.Clamp(hit.distance, 0f, nowMaxDistance);                                                                           // 플레이어가 아닌 다른 오브젝트와 충돌 시 지정해놓은 최소값, 최대값 이 사이에서 적절한 값 계산, 카메라 위치 계산
        }
        else
        {
            finalDistance = nowMaxDistance;                                                                                                          // 현재 카메라와 플레이어간 최대 거리를 최종 거리로 설정
        }

        Vector3 ve = Vector3.zero;
        mainCamera.localPosition = Vector3.SmoothDamp(mainCamera.localPosition, dirNormalized * finalDistance, ref ve, smoothness * Time.deltaTime); // 컴포넌트에 등록된 메인카메라를 마우스에 의한 이 스크립트가 적용된 오브젝트의 포지션 및 각도 변환 시 그에 맞게 대응하여 카메라와 플레이어 간의 거리 보간
    }
    /// <summary>
    /// 카메라 줌 함수
    /// </summary>
    private void Zoom()
    {
        float zoomAmount = Input.GetAxisRaw("Mouse ScrollWheel") * zoomspeed;                                                                        // 마우스 휠로 줌 양 조저
        nowMaxDistance -= zoomAmount;                                                                                                                // 현재 카메라와 플레이어간 최대 거리에 도합

        nowMaxDistance = Mathf.Clamp(nowMaxDistance, minDistance, maxDistance);                                                                      // clamp로 설정된 최소 거리와 최대 거리 사이 고정
    }
    /// <summary>
    /// 리스폰과 같이 Player를 특정 장소로 이동 시켰을시 거리에 따라 카메라의 추적속도 변경
    /// </summary>
    private void DistanceCheck()
    {
        if ((p_transform.position - transform.position).magnitude > 10)
            followspeed = MaxFollowSpeed;
        else
            followspeed = DefalutFollowSpeed;
    }
}
// magnitude : 벡터 크기 계산
// TransformPoint : 로컬 좌표축 기준으로 계산한 위치 벡터에 대해 글로벌 좌표축 기준으로 계산한 결과를 반환
// Linecast : Raycast와 달리 정확히 시작점과 종료점을 알고 있을때 사용
// ref : 초기화 된 변수가 함수에서 사용되었을때 값이 변경되면 반영

/* -----------------------------------------------------------------
 *                          사용 시 주의!
 * 따라가는 오브젝트가 만일 애니메이션, 또는 움직임으로 인하여 회전값, 위치값 등이 변경된다면
 * 플레이어에 심어둔(카메라가 따라가게끔 설정한) Follow Object를 다른 Layer로 하여금 위와 같이 검출되게끔 하고
 * 나머지 움직임의 본체(오브젝트 및 그와 관련된 Body 및 Joint 들은 Player로 하여 검출을 못하게끔 해야만
 * 카메라 흔들림 현상이 없다. 꼭 주의 할 것
 -------------------------------------------------------------------*/

