using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [SerializeField]
    private GameObject Skeleton;
    [SerializeField]
    private float LimitVelocity;

    public bool NowRagDoll;

    [SerializeField]
    private Rigidbody[] rigidbodies;
    private Collider[] colliders;
    private void Update()
    {
        if (!NowRagDoll)                                                                                    // Ragdoll ���¿����� ���� ���� ����
            return;
        else
        {
            LimitbodyVelocity();
        }
    }
    public void Init()
    {
        GetCollider();
        GetRigidbody();

        RigidbodyGravity(false);

        RigidbodyisKinematic(true);
        ColliderisEnabled(false);
    }
    /// <summary>
    /// ĳ���� �ٵ� �� �ݶ��̴� ��������
    /// </summary>
    private void GetCollider()
    {
        colliders = Skeleton.GetComponentsInChildren<Collider>();
    }
    /// <summary>
    /// ĳ���� �ٵ� �� ������ٵ� ��������
    /// </summary>
    private void GetRigidbody()
    {
        rigidbodies = Skeleton.GetComponentsInChildren<Rigidbody>();
    }
    /// <summary>
    /// bool���� ���� isKinematic ����
    /// </summary>
    /// <param name="state"></param>
    private void RigidbodyisKinematic(bool state)
    {
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = state;
        }
    }
    /// <summary>
    /// bool���� ���� Collider.enabled�� ����
    /// </summary>
    /// <param name="state"></param>
    private void ColliderisEnabled(bool state)
    {
        foreach(Collider collider in colliders)
        {
            collider.enabled = state;
        }
    }
    /// <summary>
    /// ���׵� �߷� Ȱ��ȭ,��Ȱ��ȭ ����
    /// </summary>
    /// <param name="state"></param>
    private void RigidbodyGravity(bool state)
    {
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.useGravity = state;
        }
    }
    /// <summary>
    /// Ragdoll ������ ������ �� (����, ��) ����
    /// </summary>
    public void GetVelocity()
    {
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.velocity = Player.instance.MoveDir;
        }
    }
    /// <summary>
    /// Ragdoll (����, ��) �ʱ�ȭ
    /// </summary>
    public void InitVelocity()
    {
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.velocity = Vector3.zero;
        }
    }
    /// <summary>
    /// ĳ���� �ٵ� Ragdoll�� ���� �� ���� �� �ݶ��̴�, ������ٵ� ���� ��ȯ
    /// </summary>
    public void RagdollOn()
    {
        RigidbodyisKinematic(false);

        if(!NowRagDoll)
            GetVelocity();

        RigidbodyGravity(true);
        ColliderisEnabled(true);

        NowRagDoll = true;
    }
    /// <summary>
    /// ĳ���� �ٵ� Ragdoll�� ���� �� ���� �� �ݶ��̴�, ������ٵ� ���� ��ȯ
    /// </summary>
    public void RagdollOff()
    {
        NowRagDoll = false;

        RigidbodyisKinematic(true);
        RigidbodyGravity(false);
        ColliderisEnabled(false);
    }
    /// <summary>
    /// ���׵� �Ž� �ٵ� ��Ȱ��ȭ
    /// </summary>
    public void  PauseRagdoll()
    {
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// ���׵� �Ž� �ٵ� Ȱ��ȭ
    /// </summary>
    public void UnPauseRagdoll()
    {
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// Ragdoll ���� ���� �ӷ����� ���Ͽ� �߶��� �ٴ��̳� ���� ���� ������Ʈ�� Collider �ո� ������ ���� �� ��ü Rigidbody �ӷ� ����
    /// </summary>
    private void LimitbodyVelocity()
    {
        foreach(var rb in rigidbodies)
        {
            if (rb.velocity.magnitude > LimitVelocity)
            {
                rb.velocity = rb.velocity.normalized * LimitVelocity;
            }
        }
    }
}
