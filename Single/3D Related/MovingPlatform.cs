using System;
using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    #region Variable
    public Transform[] wayPoints;                                                                       // 이동가능한 지점

    public float waitTime;                                                                              // 대기시간
    public bool rotationHead;                                                                           // 목표로 머리 회전 여부 확인

    private Player t_Player;
    private CharacterController t_controller;

    public float moveSpeed;
    private int wayPointCount;
    private int wayPointIndex = 0;                                                                      // 현재 도달한 waypoint인덱스
    #endregion
    void Awake()
    {
        wayPointCount = wayPoints.Length;

        transform.position = wayPoints[wayPointIndex].position;                                             // 최초 오브젝트 위치 설정

        wayPointIndex++;
        StartCoroutine("MoveTo");
    }
    /// <summary>
    /// wayPoint들을 반복해서 움직이는 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveTo()
    {
        while (true)
        {
            while (GameManager.instance.isPause)
                yield return null;

            yield return StartCoroutine("Movement");                                                            // wayPoint[wayPointindex].position까지 이동 할때 까지 대기
            yield return new WaitForSeconds(waitTime);                                                          // waitTime 동안 대기

            if (wayPointIndex < wayPointCount - 1)                                                              // 다음 목표로 웨이포인트(목적지) 변경
            {
                wayPointIndex++;
            }
            else
                wayPointIndex = 0;                                                                              // 마지막 포인트 도달시 다시 처음 지점으로 이동
        }
    }
    /// <summary>
    /// 움직임 패널 실제 이동 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator Movement()
    {
        while (true)
        {
            while (GameManager.instance.isPause)
                yield return null;

            Vector3 prevPos = transform.position;
            transform.position = Vector3.MoveTowards(transform.position, wayPoints[wayPointIndex].position, moveSpeed * Time.deltaTime);

            if(rotationHead)
                transform.LookAt(wayPoints[wayPointIndex]);

            try
            {
                t_controller.Move(transform.position - prevPos);
            }
            catch (NullReferenceException) { }

            if (Vector3.Distance(transform.position, wayPoints[wayPointIndex].position) < 0.1f)                 // 목표위치에 거의 도달했다면 
            {
                transform.position = wayPoints[wayPointIndex].position;                                         // 현재 위치를 목표 위치로 전환 하여 오차 수정
                break;
            }
            yield return null;
        }
    }
    /// <summary>
    /// 플레이어와 이동발판 충돌시 플레이어를 자식으로 종속(발판 기준 움직임 적용)
    /// </summary>
    /// <param name="t_player"></param>
    public void Subordination(GameObject t_player)
    {
        Quaternion originalRotation = t_player.transform.rotation;
        Vector3 originalScale = t_player.transform.lossyScale;

        t_player.transform.SetParent(transform);

        SettingScale(originalRotation, originalScale, t_player);

        t_Player = t_player.GetComponent<Player>();
        t_controller = t_player.GetComponent<CharacterController>();
    }
    private void SettingScale(Quaternion o_rotation, Vector3 o_scale, GameObject player)
    {
       player.transform.rotation = o_rotation;

        Vector3 this_scale = this.transform.lossyScale;

        player.transform.localScale = new Vector3(o_scale.x / this_scale.x, o_scale.y / this_scale.y, o_scale.z / this_scale.z);
    }
    /// <summary>
    /// 더 이상 플레이어가 타고 있지 않음
    /// </summary>
    public void Clear()
    {
        t_Player = null;
        t_controller = null;
    }
}
