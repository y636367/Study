using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region Variable
    [SerializeField]
    public float Damage;
    [SerializeField]
    private int per;
    [SerializeField]
    private int count;

    Rigidbody rigid;

    [SerializeField]
    private bool Slower;
    [SerializeField]
    private bool Stun;
    [SerializeField]
    private bool Bombs;
    [SerializeField]
    private bool Launcher;
    [SerializeField]
    private bool Mine;
    [SerializeField]
    private bool Shield;
    [SerializeField]
    private bool Flare;
    [SerializeField]
    private bool Flare_Boom;

    private Vector3 target;
    [SerializeField]
    private float speed;

    private Vector3 t_speed;
    private bool done = false;

    [SerializeField]
    public float additional;

    [SerializeField]
    private float Cool_time;

    [SerializeField]
    private Flare_Ball FB;

    [Header("Bomb_Sound")]
    [SerializeField]
    private string Bomb_s;
    #endregion
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (!GameManager.Instance.Start_)                                                                                       // ���� �Ͻ�����, ���, Ŭ���� �� ���� ���߱� ���� �ڷ�ƾ�� �ƴ� Update ������ ����
        {
            if (!done)                                                                                                          // �Ͻ�����
            {
                try
                {
                    done = true;
                    t_speed = rigid.velocity;
                    rigid.velocity = Vector3.zero;
                }
                catch(MissingComponentException) { }
            }
        }
        else
        {
            if (done)                                                                                                           // �ٽ� �ӷ� �ο� ���
            {
                try
                {
                    done = false;
                    rigid.velocity = t_speed;
                }
                catch (MissingComponentException) { }
            }

            if (Slower || Stun)                                                                                                 // ����ź, ����ź ������ ��ǥ���� �������׸��� ���󰡴� 
            {
                transform.position = Vector3.Slerp(transform.position, target, speed);
            }
            else if (Mine)                                                                                                      // ���� ���� ó��
            {
                this.speed -= Time.deltaTime;
                if (this.speed < 0)
                {
                    GameObject effect = GameManager.Instance.pool.Effect_Get(10);
                    soundManager.Instance.PlaySoundEffect(Bomb_s);
                    effect.GetComponent<Bomb>().Damage = this.Damage;
                    effect.transform.position = this.transform.position;
                    rigid.velocity = Vector3.zero;
                    gameObject.SetActive(false);
                }
            }
        }
    }
    /// <summary>
    /// ��ƼŬ ����(�̺�Ʈ �Լ�)
    /// </summary>
    private void OnParticleSystemStopped()
    {
        soundManager.Instance.StopSoundEffect(Bomb_s);
        this.gameObject.SetActive(false);
    }
    /// <summary>
    /// �Ѿ� ������ Status�� ���� �ʱ�ȭ
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="per"></param>
    /// <param name="dir"></param>
    /// <param name="t_alive"></param>
    /// <param name="t_speed"></param>
    /// <param name="life_time"></param>
    public void Init(float damage, int per, Vector3 dir, bool t_alive, float t_speed, float life_time)
    {
        this.Damage = damage;
        this.per = per;

        if (per > -1)
        {
            rigid.velocity = dir * t_speed;                                                                                                 // �ӷ�
        }

        if (!t_alive)
        {
            Invoke("Extinction", life_time);
        }
    }
    /// <summary>
    /// ���� ��ź ���� �ʱ�ȭ �Լ�
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="t_speed"></param>
    /// <param name="targetPos"></param>
    /// <param name="per"></param>
    /// <param name="count"></param>
    public void Init_Bomb(float damage, float t_speed, Vector3 targetPos, int per, int count)
    {
        this.Damage = damage;
        this.per = per;
        this.count = count;
        target = targetPos;
        speed = t_speed;
    }
    /// <summary>
    /// ���� ����, ��� ���� �ʱ�ȭ �Լ�
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="per"></param>
    /// <param name="t_speed"></param>
    public void Init_Mine(float damage, int per, float t_speed)
    {
        this.Damage = damage;
        this.per = per;
        speed = t_speed;
    }
    /// <summary>
    /// ���� �� �߰� ȿ���� ���� ���� �� �߰� �Լ�
    /// </summary>
    /// <param name="t_data"></param>
    public void Init_additional(float t_data)
    {
        this.additional = t_data;
    }
    /// <summary>
    /// ȭ�� ���� ��ƼŬ �� ȭ�������� Status�� �°� ���ӽð�, ������ �� �� ����
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="t_speed"></param>
    /// <param name="per"></param>
    /// <param name="cool"></param>
    public void Init_Flare(float damage, float t_speed, int per, float cool)
    {
        var particle = this.gameObject.GetComponent<ParticleSystem>();                                                          // GetComponent<ParticleSystem>().duration �� �Ϻ� �ڵ�� �� �̻� ������ �ʱ⿡ main���� ������ ��

        this.Damage = damage;
        this.per = per;
        this.Cool_time = cool;
        this.gameObject.GetComponent<Small_Particle_Control>().Particle_Clear();
        ParticleSystem.MainModule mainModule = particle.main;                                                                   // ��ƼŬ �ý����� MainModule ���� (�ֿ伳���� ������ �� �ִ� ���)
        mainModule.duration = t_speed;                                                                                          // �缳���� ���ؼ� ��ƼŬ�� �����ų� ���� ���¿����� ����
        this.gameObject.GetComponent<Small_Particle_Control>().Particle_Resume();

        this.additional = this.speed = mainModule.duration + this.Cool_time + 1.25f;

        gameObject.GetComponentInParent<Axe>().Get_Flame(this.Cool_time);
    }
    /// <summary>
    /// �̺�Ʈ �Լ��� ��Ȱ��ȭ
    /// </summary>
    private void Extinction()
    {
        gameObject.SetActive(false);
    }
    /// <summary>
    /// �� ������ Ư���� �°� ȿ�� ����
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Wall"))                                                                                                            // �ʵ��� ��輱(��)�� ���� �� Activefalse
            gameObject.SetActive(false);

        if (Slower || Stun)
        {
            if ((!other.CompareTag("Enemy") && !other.CompareTag("Ground")) || per == -1)                                                       //-1 ���� ������ ����
                return;
        }
        else
            if (!other.CompareTag("Enemy") || per == -1)                                                                                        //-1 ���� ������ ����
                return;


        per--;                                                                                                                                  // �ǰ�ü �ϳ��� ���������� ����

        if (per == -1)
        {
            if (Bombs)                                                                                                                          // ��ź ����Ʈ ������ ���ÿ� ����Ʈ ���÷��� �������� ���� Bullet Status ����
            {
                GameObject effect = GameManager.Instance.pool.Effect_Get(6);
                soundManager.Instance.PlaySoundEffect(Bomb_s);
                effect.GetComponent<Bomb>().Damage = this.Damage;
                effect.transform.position = this.transform.position;
                rigid.velocity = Vector3.zero;
                gameObject.SetActive(false);                                                                                                    // ����Ʈ ������ ���ÿ� Bullet�� ActiveFalse
            }
            else if (Launcher)                                                                                                                  // ��ó ����Ʈ ������ ���ÿ� ����Ʈ ���÷��� �������� ���� Bullet Status ����
            {
                GameObject effect = GameManager.Instance.pool.Effect_Get(7);
                effect.GetComponent<Bomb>().Damage = this.Damage;
                effect.transform.position = this.transform.position;
                rigid.velocity = Vector3.zero;
                gameObject.SetActive(false);                                                                                                    // ����Ʈ ������ ���ÿ� Bullet�� ActiveFalse
            }
            else if (Slower)                                                                                                                    // Slower ����Ʈ ������ ���ÿ� ����Ʈ ���÷��� ȿ���� ���� Bullet Status ����
            {   
                GameObject effect = GameManager.Instance.pool.Effect_Get(8);
                soundManager.Instance.PlaySoundEffect(Bomb_s);
                effect.GetComponent<Bomb>().Damage = this.Damage;
                effect.GetComponent<Bomb>().Count = this.count;
                effect.transform.position = this.transform.position;
                rigid.velocity = Vector3.zero;
                gameObject.SetActive(false);                                                                                                    // ����Ʈ ������ ���ÿ� Bullet�� ActiveFalse
            }
            else if (Stun)                                                                                                                      // ����ź ����Ʈ ������ ���ÿ� ����Ʈ ���÷��� ȿ���� ���� Bullet Status ����
            {
                GameObject effect = GameManager.Instance.pool.Effect_Get(9);
                soundManager.Instance.PlaySoundEffect(Bomb_s);
                effect.GetComponent<Bomb>().Count = this.count;
                effect.transform.position = this.transform.position;
                rigid.velocity = Vector3.zero;
                gameObject.SetActive(false);                                                                                                    // ����Ʈ ������ ���ÿ� Bullet�� ActiveFalse
            }
            else if (Flare_Boom)                                                                                                                // ��ȣź ����Ʈ ������ ���ÿ� ����Ʈ �������� ���� Bullet Stats ����
            {
                GameObject effect = GameManager.Instance.pool.Effect_Get(11);
                effect.GetComponent<Bomb>().Damage = this.Damage;
                effect.transform.position = this.transform.position;
                rigid.velocity = Vector3.zero;
                gameObject.SetActive(false);                                                                                                    // ����Ʈ ������ ���ÿ� Bullet�� ActiveFalse
            }
            else
            {
                rigid.velocity = Vector3.zero;                                                                                                  //�ٽ� ���� �ֱ⿡ �ʱ�ȭ
                gameObject.SetActive(false);
            }
        }
    }
    /// <summary>
    /// ȭ�� ������ ��� ������ ���ÿ� ���� ���
    /// </summary>
    private void OnEnable()
    {
        if (Flare)
            soundManager.Instance.PlaySoundEffect(Bomb_s);
    }
}
