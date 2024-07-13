using UnityEngine;

public class Scanner : MonoBehaviour
{
    [SerializeField]
    private float scanRange;                                                                                // �÷��̾� �ֺ� ��ĵ�� ����
    [SerializeField]
    private LayerMask targetLayer;                                                                          // ������ Layer
    [SerializeField]
    private Collider[] targets;                                                                             // ���� �� ���� Ÿ��
    [SerializeField]
    public Transform nearestTarget;                                                                         // ���� ����� Ÿ��

    private void FixedUpdate()
    {
        targets = Physics.OverlapSphere(transform.position,scanRange,targetLayer);                          // ���� ��ġ, ������, (����, ���� ���� - ���̱� ����), Ÿ�� ���̾�
        nearestTarget = GetNearest();
    }
    /// <summary>
    /// ���� ������ ���� Ÿ���� ���� ����� Ÿ�� ���
    /// </summary>
    /// <returns></returns>
    private Transform GetNearest()
    {
        Transform result = null;
        float diff = 100f;                                                                                  // �� �� Default ��

        Vector3 myPos = transform.position;                                                                 // Player(User) Position����

        foreach (Collider target in targets)
        {
            Vector3 targetPos = target.transform.position;                                                  // Ÿ���� Position ����
            float curDiff=Vector3.Distance(myPos,targetPos);                                                // �Ÿ��� ���ؼ�

            if (curDiff < diff)                                                                             // ª�ٸ� ���� Ÿ������ ����
            {
                diff = curDiff;
                result = target.transform;
            }
        }

        return result;
    }
}
