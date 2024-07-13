using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class Monster : MonoBehaviour
{
    #region Variable
    public Animator animator;
    public NavMeshAgent agent;
    public Rigidbody rb;
    WaitForFixedUpdate wait;

    public Boss_Skill bossskill;

    enum State
    {
        Idle,
        Run,
        Death
    }

    private State state;

    public enum Name
    {
        Goast,
        Beetle,
        SkullKing,
        Mushroom,
        Turtle,
        RedSlime,
        None
    }

    [Header("스테이터스")]
    [SerializeField]
    public float Physical_strength;
    [SerializeField]
    public float Attack_power;
    [SerializeField]
    public float Defensive_power;
    [SerializeField]
    private float Skill_Cool;
    [SerializeField]
    public Name Category;

    private float t_skill_cool;
    public float MaxHealth;

    [SerializeField]
    public float speed;
    
    public bool run = false;
    public bool invincibile = false;
    public bool Clone = false;

    [SerializeField]
    private bool isLive;

    [SerializeField]
    private Transform Player;

    private float Slower_Timer = 0f;
    private float Shield_Timer = 0f;
    private float s_speed;
    public float default_speed;

    private bool Slow;
    private bool Stun;

    public bool Boss_Skill;

    [SerializeField]
    private bool Middle;

    private bool Second_Pahse = false;
    public bool Now_Skill = false;

    [SerializeField]
    private SkinnedMeshRenderer[] Skin;
    [SerializeField]
    private Color[] Existing;

    [SerializeField]
    private int Anger_Count;

    private int dust_count = 0;
    #endregion
    #region 사운드
    [Header("Damage_Sounds")]
    [SerializeField]
    private string Damage_s;

    [Header("Spawn_Sounds")]
    [SerializeField]
    private string Spawn_s;

    [Header("Dead_Sounds")]
    [SerializeField]
    private string Dead_s;

    [Header("Move_Sounds")]
    [SerializeField]
    private string Move_s;

    [Header("Anger_Sounds")]
    [SerializeField]
    private string Anger_s;

    [Header("Skill_Sounds")]
    [SerializeField]
    private string Skill_s;

    [Header("Birth_Sounds")]
    [SerializeField]
    private string Birth_s;

    [Header("Death_Sounds")]
    [SerializeField]
    private string Death_s;
    #endregion
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        bossskill=GetComponent<Boss_Skill>();
        wait = new WaitForFixedUpdate();

        Existing = new Color[Skin.Length];

        for (int index = 0; index < Skin.Length; index++)
        {
            Existing[index] = Skin[index].material.color;
        }

        t_skill_cool = Skill_Cool;

        state = State.Idle;                                                                         // Monster(Boss) 상태 초기화
    }
    private void Update()
    {
        if (!GameManager.Instance.Start_ || GameManager.Instance.Player_Dead)                               // 게임 종료, 일시정지, 사망
        {
            Stop_Animation();
            return;
        }
        else
            Resume_Animation();

        FreezeRotation();                                                                                  // 각도 및 속력 초기화

        if (!Middle)
        {
            if (!Second_Pahse)
            {
                if (invincibile)                                                                            // 무적상태(2페이즈 돌입)
                {
                    Anger_Motion();                                                                         // 2페이즈 돌입 시각적 표현을 위한 애니메이션 및 상태 변경
                }
                Move();
            }
            else                                                                                            // 2페이즈 일시
            {
                if (!Now_Skill)                                                                             // 현재 스킬을 쓰고 있는 상태가 아니라면
                {
                    Skill_Cool -= Time.deltaTime;                                                           // 쿨타임 감소

                    if (Skill_Cool <= 0)                                                                    // 쿨타임 찰시
                    {
                        Skill_Motion();                                                                     // 스킬 실행
                        Skill_Cool = t_skill_cool;                                                          // 쿨타임 초기화    
                    }
                    Move();
                }
                else
                {
                    if (Category != Name.Goast && Category != Name.Beetle && Category != Name.Turtle)       // 유령, 풍뎅이, 거북이가 아니라면
                        Now_Skill_Time();                                                                   // 스킬모션 종료 확인 상태 Move로 갱신
                }
            }
        }
        else
            Move();
    }
    /// <summary>
    /// Player에게로 접근
    /// </summary>
    private void Move()
    {
        if (run)                                                                                           // 이동 상태라면
        {
            agent.speed = speed;                                                                           // NavMesh 속도 설저
            agent.SetDestination(Player.position);                                                         // NavMesh 목적지 설정
        }
        else                                                                                               // 아니라면(사망, 스킬 사용 등)
        {
            agent.velocity = Vector3.zero;                                                                 // Navmesh 속력 초기화
        }
    }
    /// <summary>
    /// 스폰 후 상태 Move로 갱신(애니메이션 이벤트 적용)
    /// </summary>
    private void Birth()
    {
        run = true;
        state = State.Run;
        animator.SetTrigger("Move");
    }
    /// <summary>
    /// 속력 및 회전 값 초기화(몬스터 튀는거 방지)
    /// </summary>
    private void FreezeRotation()
    {
        if (run)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
    /// <summary>
    /// 몬스터 사망 처리 함수
    /// </summary>
    private void Death()
    {
        try
        {
            if (!Boss_Skill)                                                                                                                // Red Slime 으 스킬로 생성된 클론이 사망한게 아니라면
                GameManager.Instance.drop.Category(7, -1, this.transform);                                                                  // 보물 생성
        }
        catch (NullReferenceException) { }

        if(!Clone)
            gameObject.SetActive(false);
    }
    /// <summary>
    /// Spawn 이펙트 생성(애니메이션 이벤트 적용)
    /// </summary>
    private void Spawn_Effect_On()
    {
        try
        {
            GameObject effect = GameManager.Instance.pool.Effect_Get(Middle ? 1 : 2);
            effect.transform.position = this.transform.position;
        }
        catch (NullReferenceException) { }
        soundManager.Instance.PlaySoundEffect(Spawn_s);
    }
    /// <summary>
    /// Death 이펙트 생성(애니메이션 이벤트 적용)
    /// </summary>
    private void Death_Effect_On()
    {
        try
        {
            GameObject effect = GameManager.Instance.pool.Effect_Get(Middle ? 4 : 5);                                               // 이펙트 생성
            soundManager.Instance.PlaySoundEffect(Spawn_s);
            effect.transform.position = this.transform.position;
        }
        catch (NullReferenceException) { }
        Death();                                                                                                                    // 사망 후 처리 함수
    }
    private void OnEnable()
    {
        try                                                                                                                     // Loadding Monster의 GameManager가 불필요한 경우
        {
            Player = GameManager.Instance.player.GetComponent<Transform>();
        }
        catch (NullReferenceException) { }
        agent.enabled = true;                                                                                                   // Spawn후 몬스터 상태 및 Status 초기화
        isLive = true;
        Physical_strength = MaxHealth;
        dust_count = 0;
        Now_Skill = false;
        Second_Pahse = false;
        run = false;

        this.GetComponent<Collider>().enabled = true;

        for (int index = 0; index < Skin.Length; index++)                                                                       // 피격 후 다시 돌아올때를 위해 원본 Skinrender 값 저장
        {
            Skin[index].material.color = Existing[index];
        }

        Skill_Cool = t_skill_cool;                                                                                              // Skill Cool Time 설정
    }
    /// <summary>
    /// 초기화(단계 및 생존한 시간 등 고려 난이도를 위한 Status 초기화)
    /// </summary>
    /// <param name="t_data"></param>
    public void Init(SpawnData t_data, int t_count)
    {
        MaxHealth = t_data.Health;
        Physical_strength = t_data.Health;
        Defensive_power = t_data.Defensive;
        Attack_power = t_data.Attack;

        MaxHealth = GameManager.Instance.spawnData.Stage_Multiple(MaxHealth);                                                           // 현재 Stage에 따른 곱의 값 반영
        Defensive_power = GameManager.Instance.spawnData.Stage_Multiple(Defensive_power);
        Attack_power = GameManager.Instance.spawnData.Stage_Multiple(Attack_power);

        Upgrade_Multiple(t_count);

        Physical_strength = MaxHealth;
    }
    /// <summary>
    /// Player가 생존한 시간에 비례한 난이도 조절을 위한 Boss 일부 Status 상향 함수
    /// </summary>
    /// <param name="t_count"></param>
    public void Upgrade_Multiple(int t_count)
    {
        for (int index = 0; index < t_count; index++)
        {
            Defensive_power = GameManager.Instance.spawnData.Time_Boss_Upgrade(Defensive_power);
            Attack_power = GameManager.Instance.spawnData.Time_Boss_Upgrade(Attack_power);
        }
    }
    /// <summary>
    /// RedSlime의 Skill로 생성된 Clone의 Status 설정
    /// </summary>
    /// <param name="PH"></param>
    /// <param name="DP"></param>
    /// <param name="AP"></param>
    public void Red_Slime_Fake(float PH, float DP, float AP)
    {
        MaxHealth = PH;
        Physical_strength = MaxHealth;
        Defensive_power = DP;
        Attack_power = AP;
    }
    /// <summary>
    /// Player의 공격 수단에 닿았을시 피격 효과를 위한 TriggerEnter
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (!invincibile)                                                                                                   // 무적상태가 아니라면(Anger)
        {
            if (other.CompareTag("Shield"))                                                                                 // 쉴드 - 속도 느려짐
            {
                s_speed = speed - other.GetComponent<Bullet>().additional;

                this.Slow = true;
                speed = s_speed;
            }
            else if (other.CompareTag("Bullet"))                                                                            // 총알 - 피격
            {
                try
                {
                    Physical_strength -= other.GetComponent<Bullet>().Damage * (1 / (1 + Defensive_power));                       // 총알
                    Hit_Damage();
                }
                catch (NullReferenceException) { }
                try
                {
                    Physical_strength -= other.GetComponent<Bomb>().Damage * (1 / (1 + Defensive_power));                         // 폭탄
                    Hit_Damage();
                }
                catch (NullReferenceException) { }
            }

            if (Physical_strength > 0)
            {
                Check_Health();                                                                                             // Boss 체력 상태에 따란 페이즈 변경을 위한 확인 함수
            }
            else
            {
                Death_state();                                                                                              // 체력이 0이하 이므로 사망처리
            }
        }
    }
    /// <summary>
    /// 특정 효과 처리 함수
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (!invincibile)
        {
            if (other.CompareTag("Slower"))                                                                                     // Slower 무기에 계속해서 닿게 될시
            {
                Slower_Timer += Time.deltaTime;

                s_speed = speed - (other.GetComponent<Bomb>().Damage * 0.5f);                                                   // 속도 떨어짐

                this.Slow = true;
                speed = s_speed;
                Invoke("Debuff_Off", 1.5f);                                                                                     // Exit 후 바로 상태 초기화가 아닌 일정 시간 이후 상태 초기화
                if (Slower_Timer > 0.5f)                                                                                        // Damage 효과 함께 있다면 지속 체력 감소
                {
                    if (Physical_strength > 0)
                    {
                        Physical_strength -= other.GetComponent<Bomb>().Damage * (1 / (1 + Defensive_power));
                        Slower_Timer = 0f;
                    }
                    else
                    {
                        Death_state();
                    }
                }
            }
            else if (other.CompareTag("Stun"))                                                                                  // Stun 무기에 계속해서 닿게 될시
            {
                s_speed = 0f;

                this.Stun = true;
                speed = s_speed;

                animator.speed = 0f;                                                                                            // 정지

                Invoke("Debuff_Off", 1.5f * 0.5f);                                                                              // Exit 후 바로 상태 초기화가 아닌 일정 시간 이후 상태 초기화
            }
            else if (other.CompareTag("Shield"))                                                                                // Shield 무기에 계속해서 닿게 될시
            {
                Shield_Timer += Time.deltaTime;

                if (Shield_Timer > 0.5f)                                                                                        // 프레임 마다 데미지 입는 것을 방지 일정 시간 마다 데미지 받게끔 설정
                {
                    if (Physical_strength > 0)
                    {
                        Physical_strength -= other.GetComponent<Bullet>().Damage * (1 / (1 + Defensive_power));
                        StartCoroutine(OnDamage());
                        Shield_Timer = 0f;
                        Check_Health();
                    }
                    else
                    {
                        Death_state();
                    }
                }
            }
        }
    }
    /// <summary>
    /// Exit 후 디버프 있는 특정 무기의 경우 디버프 해제
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Shield"))
            Debuff_Off();
    }
    /// <summary>
    /// 화염 방사기, 폭발 무기 와 같은 파티클에 의한 데미지 처리
    /// </summary>
    /// <param name="other"></param>
    private void OnParticleCollision(GameObject other)
    {
        if (!invincibile)
        {
            if (!other.CompareTag("Bullet"))
                return;

            try
            {
                Physical_strength -= other.GetComponent<Bullet>().Damage * (1 / (1 + Defensive_power));
                Hit_Damage();
            }
            catch (NullReferenceException) { }
            try
            {
                Physical_strength -= other.GetComponent<Bomb>().Damage * (1 / (1 + Defensive_power));
                Hit_Damage();
            }
            catch (NullReferenceException) { }

            if (Physical_strength > 0)
            {
                Check_Health();
            }
            else
            {
                Death_state();
            }
        }
    }
    /// <summary>
    /// Player에게 몬스터(Boss)가 접촉시 Player 피격
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.Instance.player.Hit)
            {
                return;
            }
            else
                Hit_Player();
        }
    }
    /// <summary>
    /// Player는 Boss의 최대 체력에 비례하여 데미지 처리 함수
    /// </summary>
    private void Hit_Player()
    {
        GameManager.Instance.player.On_Damage(MaxHealth);
        UIManager.instance.HealthBar.Update_HUD();
    }
    /// <summary>
    /// Slower, Stun 등의 디버프 해제
    /// </summary>
    private void Debuff_Off()
    {
        if (this.Slow)
        {
            speed = default_speed;
            this.Slow = false;
        }
        else if (this.Stun)
        {
            speed = default_speed;
            this.Stun = false;
            animator.speed = 1f;
        }
    }
    /// <summary>
    /// 사망처리 상태 함수
    /// </summary>
    public void Death_state()
    {
        Debuff_Off();

        StopCoroutine(OnDamage());
        this.GetComponent<Collider>().enabled = false;
        rb.constraints = RigidbodyConstraints.FreezePosition;
        Physical_strength = -1f;
        run = false;
        state = State.Death;
        isLive = false;
        agent.velocity = Vector3.zero;
        animator.SetTrigger("Death");
        for (int index = 0; index < Skin.Length; index++)
        {
            Skin[index].material.color = Existing[index];
        }
        if(!Boss_Skill)
            GameManager.Instance.Boss_Time = false;
    }
    /// <summary>
    /// 피격 처리 시각 효과
    /// </summary>
    private void Hit_Damage()
    {
        StartCoroutine(OnDamage());
    }
    /// <summary>
    /// 피격 상태 표현을 위한 SKinRender 변환 및 피격 소리 재생
    /// </summary>
    /// <returns></returns>
    private IEnumerator OnDamage()
    {
        soundManager.Instance.PlaySoundEffect(Damage_s);

        for (int index = 0; index < Skin.Length; index++)
        {
            Skin[index].material.color = Color.red;
        }
        yield return new WaitForSeconds(0.1f);

        if (Physical_strength > 0)
        {
            for (int index = 0; index < Skin.Length; index++)
            {
                Skin[index].material.color = Existing[index];
            }
        }
    }
    /// <summary>
    /// Boss 체력 일정량 떨어졌을시 2페이즈를 위한 상태 돌입
    /// </summary>
    private void Check_Health()
    {
        if (!Middle)
        {
            switch (Category)
            {
                case Name.Goast:
                case Name.SkullKing:
                    if (Physical_strength <= MaxHealth * 0.75f && !Second_Pahse)
                    {
                        run = false;
                        invincibile = true;
                        animator.SetBool("Anger", true);
                        Enhance();
                        InvokeRepeating("Anger_Dust", 0, 0.5f);
                    }
                    break;
                case Name.Beetle:
                case Name.Mushroom:
                case Name.RedSlime:
                case Name.Turtle:
                    if (Physical_strength <= MaxHealth * 0.75f && !Second_Pahse)
                    {
                        run = false;
                        invincibile = true;
                        animator.SetInteger("Anger", 1);
                        Enhance();
                        InvokeRepeating("Anger_Dust", 0, 1.2f);
                    }
                    break;
            }
        }
    }
    /// <summary>
    /// 2페이즈 돌입 시 이펙트 생성 함수
    /// </summary>
    private void Anger_Dust()
    {
        dust_count += 1;

        GameObject effect = GameManager.Instance.pool.Effect_Get(1);
        effect.transform.position = this.transform.position;

        if (dust_count == 3)
            CancelInvoke("Anger_Dust");
    }
    /// <summary>
    /// 2페이즈 돌입 모션 재생 중 Player 에게로 접근하는 것을 방지하기 위해 상태 설정 함수
    /// </summary>
    private void Enhance()
    {
        StopCoroutine(OnDamage());
        run = false;
        agent.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezePosition;
    }
    /// <summary>
    /// 각 몬스터(Boss)별 2페이즈 돌입을 위한 시각적 표현(애니메이션)
    /// </summary>
    private void Anger_Motion()
    {
        switch (Category)
        {
            case Name.Goast:
            case Name.SkullKing:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Anger") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    Second_Pahse = true;
                    invincibile = false;
                    rb.constraints = RigidbodyConstraints.None;
                    rb.constraints = RigidbodyConstraints.FreezeRotationX;
                    rb.constraints = RigidbodyConstraints.FreezeRotationZ;
                    animator.SetBool("Anger", false);
                    run = true;
                }
                break;
            case Name.Beetle:
            case Name.Mushroom:
            case Name.RedSlime:
            case Name.Turtle:
                if (Anger_Count > 2)
                {
                    Anger_Count = 0;

                    Second_Pahse = true;
                    invincibile = false;
                    rb.constraints = RigidbodyConstraints.None;
                    rb.constraints = RigidbodyConstraints.FreezeRotationX;
                    rb.constraints = RigidbodyConstraints.FreezeRotationZ;
                    run = true;
                }
                else
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Anger") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                    {
                        animator.SetInteger("Anger", 2);
                        Anger_Count += 1;
                    }
                    else
                    {
                        animator.SetInteger("Anger", 1);
                    }
                }
                break;
        }
    }
    /// <summary>
    /// 몬스터(Boss) 상태에 따른 Skill 함수 연결
    /// </summary>
    private void Skill_Motion()
    {
        int num = UnityEngine.Random.Range(0, 50);

        switch (Category)
        {
            case Name.Goast:                                                                                // 은신
                if (num > 15)
                {
                    animator.SetTrigger("Skill");
                    Now_Skill = true;
                    bossskill.Skills();
                }
                break;
            case Name.Beetle:                                                                               // 돌진
            case Name.Mushroom:                                                                             // 잡몹 소환
            case Name.Turtle:                                                                               // 거북이 등껍질
                if (num > 20)
                {
                    animator.SetTrigger("Skill");
                    Now_Skill = true;
                    bossskill.Skills();
                }
                break;
            case Name.SkullKing:                                                                            // 체력 회복
            case Name.RedSlime:                                                                             // 분열(현재 체력 기반)
                if (num > 35)
                {
                    animator.SetTrigger("Skill");
                    Now_Skill = true;
                    bossskill.Skills();
                }
                break;
        }
    }
    /// <summary>
    /// 스킬 사용 애니메이션 종료
    /// </summary>
    public void Now_Skill_Time()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Skill") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)                      // 스킬사용 애니메이션 종료 확인
        {
            animator.SetTrigger("Skill_End");
            Now_Skill = false;
            agent.isStopped = false;                                                                                                                        
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Spin"))                                                                                    // 거북이 회전 애니메이션 종료
        {
            animator.SetTrigger("Skill_End");
            Now_Skill = false;
            agent.isStopped = false;
        }
    }
    /// <summary>
    /// 애니메이션 일시정지
    /// </summary>
    private void Stop_Animation()
    {
        animator.speed = 0f;
        agent.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezePosition;
    }
    /// <summary>
    /// 애니메이션 다시 재생
    /// </summary>
    public void Resume_Animation()
    {
        animator.speed = 1f;
        agent.speed = speed;
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotationX;
        rb.constraints = RigidbodyConstraints.FreezeRotationZ;
    }
    #region 사운드 모음
    public void Move_Sound()
    {
        soundManager.Instance.PlaySoundEffect(Move_s);
    }
    public void Anger_Sound()
    {
        soundManager.Instance.PlaySoundEffect(Anger_s);
    }
    public void Birth_Sound()
    {
        soundManager.Instance.PlaySoundEffect(Birth_s);
    }
    public void Death_Sound()
    {
        soundManager.Instance.PlaySoundEffect(Death_s);
    }
    public void Skill_Sound()
    {
        soundManager.Instance.PlaySoundEffect(Skill_s);
    }
    #endregion
}
