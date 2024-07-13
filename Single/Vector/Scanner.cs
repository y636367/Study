using UnityEngine;

public class Scanner : MonoBehaviour
{
    [SerializeField]
    private float scanRange;                                                                                // 플레이어 주변 스캔할 범위
    [SerializeField]
    private LayerMask targetLayer;                                                                          // 감지할 Layer
    [SerializeField]
    private Collider[] targets;                                                                             // 범위 내 들어온 타겟
    [SerializeField]
    public Transform nearestTarget;                                                                         // 가장 가까운 타겟

    private void FixedUpdate()
    {
        targets = Physics.OverlapSphere(transform.position,scanRange,targetLayer);                          // 시작 위치, 반지름, (방향, 길이 생략 - 구이기 때문), 타겟 레이어
        nearestTarget = GetNearest();
    }
    /// <summary>
    /// 현재 범위내 들어온 타겟중 가장 가까운 타겟 계산
    /// </summary>
    /// <returns></returns>
    private Transform GetNearest()
    {
        Transform result = null;
        float diff = 100f;                                                                                  // 비교 전 Default 값

        Vector3 myPos = transform.position;                                                                 // Player(User) Position값과

        foreach (Collider target in targets)
        {
            Vector3 targetPos = target.transform.position;                                                  // 타겟의 Position 값의
            float curDiff=Vector3.Distance(myPos,targetPos);                                                // 거리를 구해서

            if (curDiff < diff)                                                                             // 짧다면 현재 타겟으로 설정
            {
                diff = curDiff;
                result = target.transform;
            }
        }

        return result;
    }
}
