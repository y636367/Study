using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Variable
    private float Runnig_Weight = 0f;                                                                   // ĳ���� ����
    private float invincibile_time = 3f;                                                                // ���� �ִ� �ð�

    private bool isWall;                                                                                // ���� ��Ҵ��� Ȯ�� ����
    public bool invincibile = false;                                                                    // �������� �Ǵ� ����

    public Vector3 moveVec;                                                                             // �÷��̾� ������ ���� Vector3 ����

    public Scanner scanner;                                                                             // �� ���� �Լ�

    [SerializeField]
    private SkinnedMeshRenderer[] Skins;                                                                // �÷��̾� Skin��
    [SerializeField]
    private Color[] Existing;

    [Header("�ִϸ��̼�")]
    Animator animator;

    [Header("�������ͽ�")]
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

    [Header("�������ͽ�+")]
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
        if (!GameManager.Instance.Start_ && !Motion)                                                            // ���� �Ͻ�����, Ŭ����, ��� �� ����
        {
            Stop_Animation();
            return;
        }

        FreezeRotation();                                                                                       // ������ ȸ������ ���� ������Ʈ ƨ�� ����
        StopToWall();                                                                                           // �� ����         

        if (!GameManager.Instance.Start_On)
            if (!Over)
            {
                Player_Move();                                                                                  // �÷��̾� �̵�
            }

        if (Dead)                                                                                               // �� �÷��̾� �ִϸ��̼� ���� ���� ���� Update
            Player_Dead();                                                                                      // Player �����
        if (Resurretion)
            Player_Resurrection();                                                                              // Player ��Ȱ��
        if (Landing)
            Player_Landing();                                                                                   // Player ��Ȱ �� ���� ����

        if (Recovery)
        {
            Recovery_time += Time.deltaTime;
            if (Recovery_time >= 1f)
            {
                Multiple_status(Recovery_Status);                                                               // ���� �ð��� ������ ü�� ������ ȸ��
                Recovery_time = 0;
            }
        }

        if(invincibile)                                                                                         // ���� ó��(��Ȱ, �ǰ� ������ �� �ȵ�)
        {
            invincibile_time -= Time.deltaTime;
            if (invincibile_time < 0f)
            {
                invincibile = false;
            }
        }

    }
    /// <summary>
    /// Player �̵� �Լ�
    /// </summary>
    private void Player_Move()
    {
        moveVec = new Vector3(GameManager.Instance.uimanager.dir.x, 0, GameManager.Instance.uimanager.dir.y);

        if (!isWall)                                                                                                            // ���� �������� �ʴ´ٸ�
            transform.position += moveVec * Move_speed * Time.deltaTime;                                                        // ���⺤�Ϳ� �ӵ� * �帣�� �ð���ƴ Player �̵�

        if (Runnig_Weight < 1.0f && moveVec != Vector3.zero)                                                                    // �����Ͻ� �ִϸ��̼� ����(Run ���·� �ִϸ��̼� ��ȯ)
        {
            state = State.Run;
            Runnig_Weight += Time.deltaTime * Changing_Speed;
            animator.SetFloat("Blend", Runnig_Weight);
        }
        else if (Runnig_Weight > 0.0f && moveVec == Vector3.zero)                                                               // Idle ���·� �ִϸ��̼� ��ȯ
        {
            state = State.Idle;
            Runnig_Weight -= Time.deltaTime * Changing_Speed;
            animator.SetFloat("Blend", Runnig_Weight);
        }

        transform.LookAt(transform.position + moveVec);                                                                         // ���� Player�� ĳ���Ͱ� ���� ���� �ٶ󺸱�
    }
    /// <summary>
    /// Player �� ���Ϳ� ����� �� �ǰ� �Լ�
    /// </summary>
    /// <param name="MaxH"></param>
    public void On_Damage(float MaxH)
    {
        if (!invincibile)                                                                                                               // ���� ���°� �ƴ϶��
        {
            if (!hit)
            {
                hit = true;
                current_hp -= MaxH * (1 / (1 + Defensive_power));                                                                       // ü�� ����
                StartCoroutine(OnDamage());                                                                                             // �ǰ�ȿ�� ����
            }
            if (current_hp > 0)
            {
                return;
            }
            else
            {
                Dead_On_Player();                                                                                                       // �ǰ� �� ü���� 0���Ϸ� �������� ���
            }
        }
    }
    /// <summary>
    /// ����, �ڼ�, ȸ��, ��ź ������ ȹ���
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

        for (int index = 0; index < Skins.Length; index++)                                                                              // �ǰ� ǥ��
        {
            Skins[index].material.color = Color.red;
        }
        yield return new WaitForSeconds(0.8f);                                                                                          // 1�� �������� �ް��� 0.8���� �����ð� �ο�(��� ��� �ִٸ� �������� �� �Դ´ٴ� ����)

        if (Physical_strength > 0)                                                                                                      // �������� ���� �� ������ Ȯ�� ��Ű�� ���� skin ���� ���·�
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
    /// Player �� ���� �Լ�
    /// </summary>
    private void StopToWall()
    {
#if UNITY_EDITOR
        Debug.DrawRay(transform.position, transform.forward * 2, Color.black);
#endif
        isWall = Physics.Raycast(transform.position, transform.forward, 2, LayerMask.GetMask("Wall"));
    }
    /// <summary>
    /// Heal Item ȹ��� ü�� ȸ��
    /// </summary>
    public void Heal()
    {
        current_hp += Mathf.Round(Physical_strength * 0.2f);
        UIManager.instance.HealthBar.Update_HUD();
    }
    /// <summary>
    /// �ִϸ��̼� ����
    /// </summary>
    private void Stop_Animation()
    {
        animator.speed = 0f;
    }
    /// <summary>
    /// �ִϸ��̼� �ٽ� ���
    /// </summary>
    public void Resume_Animation()
    {
        animator.speed = 1f;
    }
    /// <summary>
    /// Player Status �� ����Ͽ� MagnetCollider ũ�� ���� �Լ�
    /// </summary>
    /// <param name="t_stat"></param>
    public void Magnet_Range(float t_stat)
    {
        Magnet_Collider.GetComponent<SphereCollider>().radius = t_stat;
    }
    /// <summary>
    /// Player ü�� ������ ȸ�� �Լ�
    /// </summary>
    /// <param name="t_percentage"></param>
    public void Multiple_status(float t_percentage)
    {
        current_hp += Physical_strength * (t_percentage / 100f);
    }
    /// <summary>
    /// Player ��� ó�� �Լ�
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
    /// Item (Bomb, Magnet, Heal) ȹ�� �� Player ĳ���� ��ü ��� �Լ�
    /// </summary>
    public void Motion_Item()
    {
        state = State.Attack;
    }
    /// <summary>
    /// ��Ȱ �� ���� ��Ը� ���� ���� �Լ�
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
    /// Player�� �׾����� ó�� �Լ�
    /// </summary>
    private void Player_Dead()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && animator.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
        {
            if (GameManager.Instance.Resurrection)                                                                                                  // ��Ȱ Ư���� �����Ѵٸ�
            {
                Dead = false;                                                                                                                       // ��� ó�� ��� �� ��Ȱ �ִϸ��̼� ���
                animator.SetTrigger("Resurrection");
                soundManager.Instance.PlaySoundEffect(Resurection_s);
                Resurrection_Particle.SetActive(true);
                Resurretion = true;
            }
            else
            {                                                                                                                                       // ��� ó�� �� Game ���� ó��
                Dead = false;
                GameManager.Instance.Game_End();
            }
        }
    }
    /// <summary>
    /// Player ��Ȱ �ִϸ��̼� 
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
    /// Player ��Ȱ �� ���� ��� �غ� �Լ�
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
    /// �������� �޾ƿ� Player�� Status ���� ����
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
