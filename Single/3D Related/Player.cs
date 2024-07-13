using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Variable
    private float Runnig_Weight = 0f;                                                                   // 캐릭터 무게
    private float invincibile_time = 3f;                                                                // 무적 최대 시간

    private bool isWall;                                                                                // 벽에 닿았는지 확인 변수
    public bool invincibile = false;                                                                    // 무적상태 판단 변수

    public Vector3 moveVec;                                                                             // 플레이어 움직임 관련 Vector3 변수

    public Scanner scanner;                                                                             // 적 감지 함수

    [SerializeField]
    private SkinnedMeshRenderer[] Skins;                                                                // 플레이어 Skin들
    [SerializeField]
    private Color[] Existing;

    [Header("애니메이션")]
    Animator animator;

    [Header("스테이터스")]
    [SerializeField]
    public float Physical_strength;
    [SerializeField]
    public float Move_speed;
    [SerializeField]
    public float Attack_power;
    [SerializeField]
    public float Defensive_power;
    [SerializeField]
    public float Attack_speed;

    [Header("스테이터스+")]
    [SerializeField]
    public float t_Attack_power = 0;
    [SerializeField]
    public float t_Attack_speed = 0;

    [SerializeField]
    public float current_hp;

    public bool Recovery = false;
    public float Recovery_Status;

    bool Dead = false;
    bool Resurretion = false;
    bool Landing = false;
    bool Over = false;

    bool Motion = false;

    float Recovery_time = 0f;

    [SerializeField]
    private GameObject Magnet_Collider;
    [SerializeField]
    private GameObject Resurrection_Particle;

    [Header("Damage_Sounds")]
    [SerializeField]
    private string Damage_s;

    [Header("Resurection_Sounds")]
    [SerializeField]
    private string Resurection_s;

    [Header("Dead_Sounds")]
    [SerializeField]
    private string Dead_s;

    enum State
    {
        Idle,
        Run,
        Attack,
        Death
    }
    [SerializeField]
    private State state;

    [SerializeField]
    private float Changing_Speed;

    private bool hit = false;
    public bool Hit => hit;

    Rigidbody rb;
    #endregion
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        scanner=GetComponent<Scanner>();

        Existing=new Color[Skins.Length];

        for(int index=0;index<Skins.Length;index++)
        {
            Existing[index] = Skins[index].material.color;
        }

        Get_Server_Status();

        current_hp = Physical_strength;
        state = State.Idle;
    }
    private void Update()
    {
        if (!GameManager.Instance.Start_ && !Motion)                                                            // 게임 일시정지, 클리어, 사망 시 정지
        {
            Stop_Animation();
            return;
        }

        FreezeRotation();                                                                                       // 임의의 회전으로 인한 오브젝트 튕김 방지
        StopToWall();                                                                                           // 벽 감지         

        if (!GameManager.Instance.Start_On)
            if (!Over)
            {
                Player_Move();                                                                                  // 플레이어 이동
            }

        if (Dead)                                                                                               // 각 플레이어 애니메이션 종료 감지 위해 Update
            Player_Dead();                                                                                      // Player 사망시
        if (Resurretion)
            Player_Resurrection();                                                                              // Player 부활시
        if (Landing)
            Player_Landing();                                                                                   // Player 부활 후 땅에 착지

        if (Recovery)
        {
            Recovery_time += Time.deltaTime;
            if (Recovery_time >= 1f)
            {
                Multiple_status(Recovery_Status);                                                               // 일정 시간이 지나면 체력 일정량 회복
                Recovery_time = 0;
            }
        }

        if(invincibile)                                                                                         // 무적 처리(부활, 피격 당한지 얼마 안된)
        {
            invincibile_time -= Time.deltaTime;
            if (invincibile_time < 0f)
            {
                invincibile = false;
            }
        }

    }
    /// <summary>
    /// Player 이동 함수
    /// </summary>
    private void Player_Move()
    {
        moveVec = new Vector3(GameManager.Instance.uimanager.dir.x, 0, GameManager.Instance.uimanager.dir.y);

        if (!isWall)                                                                                                            // 벽이 감지되지 않는다면
            transform.position += moveVec * Move_speed * Time.deltaTime;                                                        // 방향벡터에 속도 * 흐르는 시간만틈 Player 이동

        if (Runnig_Weight < 1.0f && moveVec != Vector3.zero)                                                                    // 움직일시 애니메이션 여부(Run 상태로 애니메이션 전환)
        {
            state = State.Run;
            Runnig_Weight += Time.deltaTime * Changing_Speed;
            animator.SetFloat("Blend", Runnig_Weight);
        }
        else if (Runnig_Weight > 0.0f && moveVec == Vector3.zero)                                                               // Idle 상태로 애니메이션 전환
        {
            state = State.Idle;
            Runnig_Weight -= Time.deltaTime * Changing_Speed;
            animator.SetFloat("Blend", Runnig_Weight);
        }

        transform.LookAt(transform.position + moveVec);                                                                         // 현재 Player의 캐릭터가 보는 방향 바라보기
    }
    /// <summary>
    /// Player 가 몬스터와 닿았을 시 피격 함수
    /// </summary>
    /// <param name="MaxH"></param>
    public void On_Damage(float MaxH)
    {
        if (!invincibile)                                                                                                               // 무적 상태가 아니라면
        {
            if (!hit)
            {
                hit = true;
                current_hp -= MaxH * (1 / (1 + Defensive_power));                                                                       // 체력 감소
                StartCoroutine(OnDamage());                                                                                             // 피격효과 실행
            }
            if (current_hp > 0)
            {
                return;
            }
            else
            {
                Dead_On_Player();                                                                                                       // 피격 후 체력이 0이하로 떨어지면 사망
            }
        }
    }
    /// <summary>
    /// 보물, 자석, 회복, 폭탄 아이템 획득시
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tresure") || other.CompareTag("Magnet") || other.CompareTag("Heal") || other.CompareTag("Bomb"))
            animator.SetTrigger("Attack");
    }
    IEnumerator OnDamage()
    {
        soundManager.Instance.PlaySoundEffect(Damage_s);

        for (int index = 0; index < Skins.Length; index++)                                                                              // 피격 표현
        {
            Skins[index].material.color = Color.red;
        }
        yield return new WaitForSeconds(0.8f);                                                                                          // 1번 데미지를 받고나서 0.8초의 유예시간 부여(계속 닿고 있다면 데미지를 안 입는다는 판정)

        if (Physical_strength > 0)                                                                                                      // 데미지를 입을 수 있음을 확인 시키기 위해 skin 원래 상태로
        {
            for (int index = 0; index < Skins.Length; index++)
            {
                Skins[index].material.color = Existing[index];
            }
        }

        hit = false;
    }
    private void FreezeRotation()
    {
        rb.angularVelocity = Vector3.zero;
    }
    /// <summary>
    /// Player 벽 감지 함수
    /// </summary>
    private void StopToWall()
    {
#if UNITY_EDITOR
        Debug.DrawRay(transform.position, transform.forward * 2, Color.black);
#endif
        isWall = Physics.Raycast(transform.position, transform.forward, 2, LayerMask.GetMask("Wall"));
    }
    /// <summary>
    /// Heal Item 획득시 체력 회복
    /// </summary>
    public void Heal()
    {
        current_hp += Mathf.Round(Physical_strength * 0.2f);
        UIManager.instance.HealthBar.Update_HUD();
    }
    /// <summary>
    /// 애니메이션 정지
    /// </summary>
    private void Stop_Animation()
    {
        animator.speed = 0f;
    }
    /// <summary>
    /// 애니메이션 다시 재생
    /// </summary>
    public void Resume_Animation()
    {
        animator.speed = 1f;
    }
    /// <summary>
    /// Player Status 에 기반하여 MagnetCollider 크기 조절 함수
    /// </summary>
    /// <param name="t_stat"></param>
    public void Magnet_Range(float t_stat)
    {
        Magnet_Collider.GetComponent<SphereCollider>().radius = t_stat;
    }
    /// <summary>
    /// Player 체력 일정량 회복 함수
    /// </summary>
    /// <param name="t_percentage"></param>
    public void Multiple_status(float t_percentage)
    {
        current_hp += Physical_strength * (t_percentage / 100f);
    }
    /// <summary>
    /// Player 사망 처리 함수
    /// </summary>
    private void Dead_On_Player()
    {
        soundManager.Instance.Pause_Bgm();
        soundManager.Instance.PlaySoundEffect(Dead_s);

        Motion = true;
        GameManager.Instance.Start_ = false;
        GameManager.Instance.Player_Dead = true;
        state = State.Death;
        animator.SetTrigger("Dead");
        Over = true;
        this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        Dead = true;

        GameManager.Instance.weaponManager.Off_the_Weapons();
        GameManager.Instance.charManager.Off_the_Characteristic();
        Magnet_Collider.SetActive(false);


    }
    /// <summary>
    /// Item (Bomb, Magnet, Heal) 획득 시 Player 캐릭터 상체 모션 함수
    /// </summary>
    public void Motion_Item()
    {
        state = State.Attack;
    }
    /// <summary>
    /// 부활 후 개임 재게를 위한 설정 함수
    /// </summary>
    private void Resurrection_On_Player()
    {
        GameManager.Instance.Resurrection = false;
        GameManager.Instance.Player_Dead = false;
        Motion = false;
        GameManager.Instance.Start_ = true;
        animator.SetTrigger("Re_On");
        Over = false;
        GameManager.Instance.weaponManager.On_the_Weapons();
        GameManager.Instance.charManager.On_the_Characteristic();
        this.gameObject.GetComponent<CapsuleCollider>().enabled = true;
        Magnet_Collider.SetActive(true);
        invincibile = true;

        current_hp = Physical_strength;

        soundManager.Instance.UnPause_Bgm();
    }
    /// <summary>
    /// Player가 죽었을시 처리 함수
    /// </summary>
    private void Player_Dead()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && animator.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
        {
            if (GameManager.Instance.Resurrection)                                                                                                  // 부활 특성이 존재한다면
            {
                Dead = false;                                                                                                                       // 사망 처리 취소 및 부활 애니메이션 재생
                animator.SetTrigger("Resurrection");
                soundManager.Instance.PlaySoundEffect(Resurection_s);
                Resurrection_Particle.SetActive(true);
                Resurretion = true;
            }
            else
            {                                                                                                                                       // 사망 처리 및 Game 종료 처리
                Dead = false;
                GameManager.Instance.Game_End();
            }
        }
    }
    /// <summary>
    /// Player 부활 애니메이션 
    /// </summary>
    private void Player_Resurrection()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && animator.GetCurrentAnimatorStateInfo(0).IsName("Resurrection_On"))
        {
            Resurretion = false;
            animator.SetTrigger("Re_On");
            Landing = true;
        }
    }
    /// <summary>
    /// Player 부활 후 게임 재게 준비 함수
    /// </summary>
    private void Player_Landing()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && animator.GetCurrentAnimatorStateInfo(0).IsName("Landing"))
        {
            Resurrection_On_Player();
            animator.SetTrigger("Landing_Over");
            Resurrection_Particle.SetActive(false);
            Landing = false;
            state = State.Idle;
        }
    }
    /// <summary>
    /// 서버에서 받아온 Player의 Status 변수 적용
    /// </summary>
    private void Get_Server_Status()
    {
        Physical_strength = Backend_GameData.Instance.Userstatusdatas.Max_HP;
        Move_speed = Backend_GameData.Instance.Userstatusdatas.MoveSpeed;
        Attack_power = Backend_GameData.Instance.Userstatusdatas.Attack_Power;
        Defensive_power = Backend_GameData.Instance.Userstatusdatas.Deffensive_Power;
        Attack_speed = Backend_GameData.Instance.Userstatusdatas.AttackSpeed;
    }
}
