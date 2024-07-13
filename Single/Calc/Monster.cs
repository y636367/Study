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

    [Header("�������ͽ�")]
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
    #region ����
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

        state = State.Idle;                                                                         // Monster(Boss) ���� �ʱ�ȭ
    }
    private void Update()
    {
        if (!GameManager.Instance.Start_ || GameManager.Instance.Player_Dead)                               // ���� ����, �Ͻ�����, ���
        {
            Stop_Animation();
            return;
        }
        else
            Resume_Animation();

        FreezeRotation();                                                                                  // ���� �� �ӷ� �ʱ�ȭ

        if (!Middle)
        {
            if (!Second_Pahse)
            {
                if (invincibile)                                                                            // ��������(2������ ����)
                {
                    Anger_Motion();                                                                         // 2������ ���� �ð��� ǥ���� ���� �ִϸ��̼� �� ���� ����
                }
                Move();
            }
            else                                                                                            // 2������ �Ͻ�
            {
                if (!Now_Skill)                                                                             // ���� ��ų�� ���� �ִ� ���°� �ƴ϶��
                {
                    Skill_Cool -= Time.deltaTime;                                                           // ��Ÿ�� ����

                    if (Skill_Cool <= 0)                                                                    // ��Ÿ�� ����
                    {
                        Skill_Motion();                                                                     // ��ų ����
                        Skill_Cool = t_skill_cool;                                                          // ��Ÿ�� �ʱ�ȭ    
                    }
                    Move();
                }
                else
                {
                    if (Category != Name.Goast && Category != Name.Beetle && Category != Name.Turtle)       // ����, ǳ����, �ź��̰� �ƴ϶��
                        Now_Skill_Time();                                                                   // ��ų��� ���� Ȯ�� ���� Move�� ����
                }
            }
        }
        else
            Move();
    }
    /// <summary>
    /// Player���Է� ����
    /// </summary>
    private void Move()
    {
        if (run)                                                                                           // �̵� ���¶��
        {
            agent.speed = speed;                                                                           // NavMesh �ӵ� ����
            agent.SetDestination(Player.position);                                                         // NavMesh ������ ����
        }
        else                                                                                               // �ƴ϶��(���, ��ų ��� ��)
        {
            agent.velocity = Vector3.zero;                                                                 // Navmesh �ӷ� �ʱ�ȭ
        }
    }
    /// <summary>
    /// ���� �� ���� Move�� ����(�ִϸ��̼� �̺�Ʈ ����)
    /// </summary>
    private void Birth()
    {
        run = true;
        state = State.Run;
        animator.SetTrigger("Move");
    }
    /// <summary>
    /// �ӷ� �� ȸ�� �� �ʱ�ȭ(���� Ƣ�°� ����)
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
    /// ���� ��� ó�� �Լ�
    /// </summary>
    private void Death()
    {
        try
        {
            if (!Boss_Skill)                                                                                                                // Red Slime �� ��ų�� ������ Ŭ���� ����Ѱ� �ƴ϶��
                GameManager.Instance.drop.Category(7, -1, this.transform);                                                                  // ���� ����
        }
        catch (NullReferenceException) { }

        if(!Clone)
            gameObject.SetActive(false);
    }
    /// <summary>
    /// Spawn ����Ʈ ����(�ִϸ��̼� �̺�Ʈ ����)
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
    /// Death ����Ʈ ����(�ִϸ��̼� �̺�Ʈ ����)
    /// </summary>
    private void Death_Effect_On()
    {
        try
        {
            GameObject effect = GameManager.Instance.pool.Effect_Get(Middle ? 4 : 5);                                               // ����Ʈ ����
            soundManager.Instance.PlaySoundEffect(Spawn_s);
            effect.transform.position = this.transform.position;
        }
        catch (NullReferenceException) { }
        Death();                                                                                                                    // ��� �� ó�� �Լ�
    }
    private void OnEnable()
    {
        try                                                                                                                     // Loadding Monster�� GameManager�� ���ʿ��� ���
        {
            Player = GameManager.Instance.player.GetComponent<Transform>();
        }
        catch (NullReferenceException) { }
        agent.enabled = true;                                                                                                   // Spawn�� ���� ���� �� Status �ʱ�ȭ
        isLive = true;
        Physical_strength = MaxHealth;
        dust_count = 0;
        Now_Skill = false;
        Second_Pahse = false;
        run = false;

        this.GetComponent<Collider>().enabled = true;

        for (int index = 0; index < Skin.Length; index++)                                                                       // �ǰ� �� �ٽ� ���ƿö��� ���� ���� Skinrender �� ����
        {
            Skin[index].material.color = Existing[index];
        }

        Skill_Cool = t_skill_cool;                                                                                              // Skill Cool Time ����
    }
    /// <summary>
    /// �ʱ�ȭ(�ܰ� �� ������ �ð� �� ��� ���̵��� ���� Status �ʱ�ȭ)
    /// </summary>
    /// <param name="t_data"></param>
    public void Init(SpawnData t_data, int t_count)
    {
        MaxHealth = t_data.Health;
        Physical_strength = t_data.Health;
        Defensive_power = t_data.Defensive;
        Attack_power = t_data.Attack;

        MaxHealth = GameManager.Instance.spawnData.Stage_Multiple(MaxHealth);                                                           // ���� Stage�� ���� ���� �� �ݿ�
        Defensive_power = GameManager.Instance.spawnData.Stage_Multiple(Defensive_power);
        Attack_power = GameManager.Instance.spawnData.Stage_Multiple(Attack_power);

        Upgrade_Multiple(t_count);

        Physical_strength = MaxHealth;
    }
    /// <summary>
    /// Player�� ������ �ð��� ����� ���̵� ������ ���� Boss �Ϻ� Status ���� �Լ�
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
    /// RedSlime�� Skill�� ������ Clone�� Status ����
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
    /// Player�� ���� ���ܿ� ������� �ǰ� ȿ���� ���� TriggerEnter
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (!invincibile)                                                                                                   // �������°� �ƴ϶��(Anger)
        {
            if (other.CompareTag("Shield"))                                                                                 // ���� - �ӵ� ������
            {
                s_speed = speed - other.GetComponent<Bullet>().additional;

                this.Slow = true;
                speed = s_speed;
            }
            else if (other.CompareTag("Bullet"))                                                                            // �Ѿ� - �ǰ�
            {
                try
                {
                    Physical_strength -= other.GetComponent<Bullet>().Damage * (1 / (1 + Defensive_power));                       // �Ѿ�
                    Hit_Damage();
                }
                catch (NullReferenceException) { }
                try
                {
                    Physical_strength -= other.GetComponent<Bomb>().Damage * (1 / (1 + Defensive_power));                         // ��ź
                    Hit_Damage();
                }
                catch (NullReferenceException) { }
            }

            if (Physical_strength > 0)
            {
                Check_Health();                                                                                             // Boss ü�� ���¿� ���� ������ ������ ���� Ȯ�� �Լ�
            }
            else
            {
                Death_state();                                                                                              // ü���� 0���� �̹Ƿ� ���ó��
            }
        }
    }
    /// <summary>
    /// Ư�� ȿ�� ó�� �Լ�
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (!invincibile)
        {
            if (other.CompareTag("Slower"))                                                                                     // Slower ���⿡ ����ؼ� ��� �ɽ�
            {
                Slower_Timer += Time.deltaTime;

                s_speed = speed - (other.GetComponent<Bomb>().Damage * 0.5f);                                                   // �ӵ� ������

                this.Slow = true;
                speed = s_speed;
                Invoke("Debuff_Off", 1.5f);                                                                                     // Exit �� �ٷ� ���� �ʱ�ȭ�� �ƴ� ���� �ð� ���� ���� �ʱ�ȭ
                if (Slower_Timer > 0.5f)                                                                                        // Damage ȿ�� �Բ� �ִٸ� ���� ü�� ����
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
            else if (other.CompareTag("Stun"))                                                                                  // Stun ���⿡ ����ؼ� ��� �ɽ�
            {
                s_speed = 0f;

                this.Stun = true;
                speed = s_speed;

                animator.speed = 0f;                                                                                            // ����

                Invoke("Debuff_Off", 1.5f * 0.5f);                                                                              // Exit �� �ٷ� ���� �ʱ�ȭ�� �ƴ� ���� �ð� ���� ���� �ʱ�ȭ
            }
            else if (other.CompareTag("Shield"))                                                                                // Shield ���⿡ ����ؼ� ��� �ɽ�
            {
                Shield_Timer += Time.deltaTime;

                if (Shield_Timer > 0.5f)                                                                                        // ������ ���� ������ �Դ� ���� ���� ���� �ð� ���� ������ �ްԲ� ����
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
    /// Exit �� ����� �ִ� Ư�� ������ ��� ����� ����
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Shield"))
            Debuff_Off();
    }
    /// <summary>
    /// ȭ�� ����, ���� ���� �� ���� ��ƼŬ�� ���� ������ ó��
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
    /// Player���� ����(Boss)�� ���˽� Player �ǰ�
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
    /// Player�� Boss�� �ִ� ü�¿� ����Ͽ� ������ ó�� �Լ�
    /// </summary>
    private void Hit_Player()
    {
        GameManager.Instance.player.On_Damage(MaxHealth);
        UIManager.instance.HealthBar.Update_HUD();
    }
    /// <summary>
    /// Slower, Stun ���� ����� ����
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
    /// ���ó�� ���� �Լ�
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
    /// �ǰ� ó�� �ð� ȿ��
    /// </summary>
    private void Hit_Damage()
    {
        StartCoroutine(OnDamage());
    }
    /// <summary>
    /// �ǰ� ���� ǥ���� ���� SKinRender ��ȯ �� �ǰ� �Ҹ� ���
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
    /// Boss ü�� ������ ���������� 2����� ���� ���� ����
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
    /// 2������ ���� �� ����Ʈ ���� �Լ�
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
    /// 2������ ���� ��� ��� �� Player ���Է� �����ϴ� ���� �����ϱ� ���� ���� ���� �Լ�
    /// </summary>
    private void Enhance()
    {
        StopCoroutine(OnDamage());
        run = false;
        agent.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezePosition;
    }
    /// <summary>
    /// �� ����(Boss)�� 2������ ������ ���� �ð��� ǥ��(�ִϸ��̼�)
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
    /// ����(Boss) ���¿� ���� Skill �Լ� ����
    /// </summary>
    private void Skill_Motion()
    {
        int num = UnityEngine.Random.Range(0, 50);

        switch (Category)
        {
            case Name.Goast:                                                                                // ����
                if (num > 15)
                {
                    animator.SetTrigger("Skill");
                    Now_Skill = true;
                    bossskill.Skills();
                }
                break;
            case Name.Beetle:                                                                               // ����
            case Name.Mushroom:                                                                             // ��� ��ȯ
            case Name.Turtle:                                                                               // �ź��� ���
                if (num > 20)
                {
                    animator.SetTrigger("Skill");
                    Now_Skill = true;
                    bossskill.Skills();
                }
                break;
            case Name.SkullKing:                                                                            // ü�� ȸ��
            case Name.RedSlime:                                                                             // �п�(���� ü�� ���)
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
    /// ��ų ��� �ִϸ��̼� ����
    /// </summary>
    public void Now_Skill_Time()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Skill") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)                      // ��ų��� �ִϸ��̼� ���� Ȯ��
        {
            animator.SetTrigger("Skill_End");
            Now_Skill = false;
            agent.isStopped = false;                                                                                                                        
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Spin"))                                                                                    // �ź��� ȸ�� �ִϸ��̼� ����
        {
            animator.SetTrigger("Skill_End");
            Now_Skill = false;
            agent.isStopped = false;
        }
    }
    /// <summary>
    /// �ִϸ��̼� �Ͻ�����
    /// </summary>
    private void Stop_Animation()
    {
        animator.speed = 0f;
        agent.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezePosition;
    }
    /// <summary>
    /// �ִϸ��̼� �ٽ� ���
    /// </summary>
    public void Resume_Animation()
    {
        animator.speed = 1f;
        agent.speed = speed;
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotationX;
        rb.constraints = RigidbodyConstraints.FreezeRotationZ;
    }
    #region ���� ����
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
