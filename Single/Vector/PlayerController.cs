using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class PlayerController : MonoBehaviour
{
    public static bool s_canPressKey = true;

    [SerializeField]
    float moveSpeed = 3;

    Rigidbody rb;

    Vector3 dir = new Vector3();
    //방향 벡터
    public Vector3 destination= new Vector3();
    //목적지 벡터(이동을 위한)

    Vector3 originPos= new Vector3();

    [SerializeField]
    float spintSpeed = 270;
    //회전 각
    Vector3 rotDir=new Vector3();
    //방향 회전
    Quaternion destRot=new Quaternion();
    //목표 회전 값(얼만큼 회전 시킬지)

    [SerializeField]
    float recoilPosY = 0.25f;
    [SerializeField]
    float recoilSpeed = 1.5f;
    //큐브 들썩임 표현 변수

    bool canMove = true;
    bool isFalling = false;

    [SerializeField]
    Transform fakeCube = null;
    //가짜 큐브를 먼저 돌리고, 돌아간 만큼의 값을 목표 회전값으로 삼음
    [SerializeField]
    Transform realCube = null;

    TimingManager timingManager;
    CameraController cameraController;
    StatusManager statusManager;

    void Start()
    {
        timingManager = FindObjectOfType<TimingManager>();
        cameraController = FindObjectOfType<CameraController>();
        statusManager = FindObjectOfType<StatusManager>();

        rb= GetComponentInChildren<Rigidbody>();
        //자식객체에 리지드바디가 있다면 객체 가져옴

        originPos= transform.position;
    }

    public void Initialized()
    {//초기화
        transform.position = Vector3.zero;
        destination= Vector3.zero;
        realCube.localPosition = Vector3.zero;
        canMove = true;
        s_canPressKey = true;
        isFalling=false;
        rb.useGravity = false;
        rb.isKinematic= true;
    }
    void Update()
    {
        if (GameManager.Instance.isStartGame)
        {
            CheckFalling();

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.W))
            {
                if (canMove && s_canPressKey && !isFalling)
                {
                    calc();

                    if (timingManager.CheckTiming())
                    //판정 체크
                    {
                        StartAction();
                    }
                }
            }
        }
    }
    void calc()
    {
        dir.Set(Input.GetAxisRaw("Vertical"), 0, Input.GetAxisRaw("Horizontal"));
        //방향 계산

        destination = transform.position + new Vector3(dir.z, 0, dir.x);
        //이동 목표값 계산

        rotDir = new Vector3(-dir.x, 0, dir.z);
        //회전 목표 값 계산
        fakeCube.RotateAround(transform.position, rotDir, spintSpeed);
        destRot = fakeCube.rotation;
        //공전 값 계산
    }
    void StartAction()
    {
        StartCoroutine(MoveCo());
        StartCoroutine(SpinCo());
        StartCoroutine(RecoilCo());
        StartCoroutine(cameraController.zoomCo());
    }

    IEnumerator MoveCo()
    {
        canMove = false;

        while (Vector3.SqrMagnitude(transform.position - destination) >= 0.001f)//0에 가까워 질수록 목표지점에 가까워 진다는 뜻
        //Destination도 가능 하지만 좀 더 가볍고 제곱근을 리턴해주는 SqlMagnitude사용
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
            yield return null;
            //자연스럽게 목표까지 이동(프레임 단위)
        }
        transform.position = destination;
        //오차가 있을 수 있기에 정확하게 이동

        canMove= true;
    }

    IEnumerator SpinCo()
    {
        while (Quaternion.Angle(realCube.rotation, destRot)>0.5f)
            //실제 회전
        {
            realCube.rotation = Quaternion.RotateTowards(realCube.rotation, destRot, spintSpeed * Time.deltaTime);
            yield return null;
        }
        realCube.rotation = destRot;
        //오차가 있을 수 있기에 정확하게 회전
    }
    IEnumerator RecoilCo()
    {
        while (realCube.position.y < recoilPosY)
        {
            realCube.position += new Vector3(0, recoilSpeed * Time.deltaTime, 0);
            yield return null;
        }
        while (realCube.position.y > 0)
        {
            realCube.position -= new Vector3(0, recoilSpeed * Time.deltaTime, 0);
            yield return null;
        }
        realCube.localPosition = new Vector3(0, 0, 0);
        //원위치
    }
    void CheckFalling()
        //Raycast를 사용해서 플레이어 바닥에 있는지 없는지 확인
    {
        if (!isFalling&canMove)
        {
            if (!Physics.Raycast(transform.position, Vector3.down, 1.1f))
            {
                Falling();
            }
        }
    }
    void Falling()
        //Rigidbody 조작
    {
        isFalling= true;
        rb.useGravity = true;
        rb.isKinematic = false;
    }
    public void ResetFalling()
    {
        statusManager.DecreaseHp(1);
        AudioManager.instance.PlaySfx("Falling");
        //체력 감소

        if (!statusManager.IsDead())
        //죽었는지 여부 확인
        {
            isFalling = false;
            rb.useGravity = false;
            rb.isKinematic = true;

            transform.position = originPos;
            realCube.localPosition = new Vector3(0, 0, 0);
            //부모 객체, 자식객체 원위치

        }
    }
}

