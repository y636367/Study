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
        if (!NowRagDoll)                                                                                    // Ragdoll 상태에서만 물리 연산 진행
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
    /// 캐릭터 바디 각 콜라이더 가져오기
    /// </summary>
    private void GetCollider()
    {
        colliders = Skeleton.GetComponentsInChildren<Collider>();
    }
    /// <summary>
    /// 캐릭터 바디 각 리지드바디 가져오기
    /// </summary>
    private void GetRigidbody()
    {
        rigidbodies = Skeleton.GetComponentsInChildren<Rigidbody>();
    }
    /// <summary>
    /// bool값에 따른 isKinematic 여부
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
    /// bool값에 따른 Collider.enabled값 여부
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
    /// 래그돌 중력 활성화,비활성화 여부
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
    /// Ragdoll 이전의 벡터의 값 (방향, 힘) 적용
    /// </summary>
    public void GetVelocity()
    {
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.velocity = Player.instance.MoveDir;
        }
    }
    /// <summary>
    /// Ragdoll (방향, 힘) 초기화
    /// </summary>
    public void InitVelocity()
    {
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.velocity = Vector3.zero;
        }
    }
    /// <summary>
    /// 캐릭터 바디 Ragdoll을 위한 각 부위 별 콜라이더, 리지드바디 설정 변환
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
    /// 캐릭터 바디 Ragdoll을 위한 각 부위 별 콜라이더, 리지드바디 설정 변환
    /// </summary>
    public void RagdollOff()
    {
        NowRagDoll = false;

        RigidbodyisKinematic(true);
        RigidbodyGravity(false);
        ColliderisEnabled(false);
    }
    /// <summary>
    /// 래그돌 매쉬 바디 비활성화
    /// </summary>
    public void  PauseRagdoll()
    {
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 래그돌 매쉬 바디 활성화
    /// </summary>
    public void UnPauseRagdoll()
    {
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// Ragdoll 이후 빠른 속력으로 인하여 추락시 바닥이나 벽과 같은 오브젝트의 Collider 뚫림 방지를 위한 각 몸체 Rigidbody 속력 제한
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
