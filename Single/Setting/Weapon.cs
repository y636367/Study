using UnityEngine;

public class Weapon : MonoBehaviour
{
    #region Variable
    [Header("�⺻ ��ġ")]
    [SerializeField]
    public int id;
    [SerializeField]
    private int prefabId;
    [SerializeField]
    private float damage;
    [SerializeField] 
    private int count;
    [SerializeField]
    private float speed;

    [Header("��ȭ ��ġ")]
    [SerializeField]
    private float UP_damage;
    [SerializeField]
    private int UP_count;
    [SerializeField]
    private float UP_speed;

    [Header("Weapon_Check")]
    [SerializeField]
    private int maxLv;
    [SerializeField]
    private int current_Lv;

    private int shot_count;
    private float shot_timer;
    private float shot_cool;

    [Header("Bullet")]
    [SerializeField]
    private GameObject Bullet_pool;
    [SerializeField]
    private bool alive;
    [SerializeField]
    private float bullet_speed;
    [SerializeField]
    private float Cool_time;
    [SerializeField]
    private float Life_Time;

    [Header("Shotting_Type")]
    [SerializeField]
    private bool persistent;
    [SerializeField]
    private bool Manual_shot;
    [SerializeField]
    private bool Auto_shot;
    [SerializeField]
    private bool Random_shot;
    [SerializeField]
    private bool Nomal_shot;
    [SerializeField]
    private bool Throw_shot;
    [SerializeField]
    private bool Bury_mine;
    [SerializeField]
    private bool Sword_slash;
    [SerializeField]
    private bool Fire_Thrower;

    [Header("Axe")]
    [SerializeField]
    private Axe axe;

    [Header("Shield")]
    [SerializeField]
    private Axe shield;

    [Header("Auto_Shot")]
    [SerializeField]
    private GameObject Auto_Gun;
    [SerializeField]
    private int per;

    [Header("Manual_Shot")]
    [SerializeField]
    private GameObject Manual_Gun;

    [Header("Random_Shot")]
    [SerializeField]
    private GameObject Random_Gun;

    [Header("Throw")]
    [SerializeField]
    private GameObject Throw_item;

    [Header("Mine")]
    [SerializeField]
    private GameObject Mine_item;

    [Header("FIre")]
    [SerializeField]
    private GameObject Fire_item;

    [Header("Sword")]
    [SerializeField]
    private GameObject Sword_item;

    [Header("Additional")]
    [SerializeField]
    private float Additional;
    [SerializeField]
    private float UP_Additional;

    [SerializeField]
    private Player player;

    private float basic_damage;
    private float basic_speed;

    [SerializeField]
    public int Weapon_num;

    [Header("Shot_Sounds")]
    [SerializeField]
    private string Shot_s;

    [Header("Fire_Thrower_Additional")]
    [SerializeField]
    private Bullet now_b;
    #endregion
    private void Awake()
    {
        player = GameManager.Instance.player;
        basic_damage = this.damage;                                                                //shield = GameManager.Instance.weaponManager.Shield_P.GetComponent<Axe>();
        basic_speed = this.speed;                                                                  //���⼭ ȣ���ϸ� null ���� ��(�� �ٷ� �ε� �� �̻��� ���� ���� ���� �߻�)
        
        Init();
    }
    private void Update()
    {
        if (!GameManager.Instance.Start_)                                                               // ���� �Ͻ�����, ���, ���� � ���� ��Ȳ���� ���߾�� �� ���� �ֱ⿡ �ڷ�ƾ�� �ƴ� Update������ ����
        {
            return;
        }

        if (persistent)                                                                                 // ���� ���� �߻�ü�� ���
        {
            if (this.shot_timer > 0)
                this.shot_timer -= Time.deltaTime;                                                      // ��Ÿ�� ���� �߻� �ϵ��� ��Ÿ�� ����
            else
            {
                if (Nomal_shot)                                                                         // �Ϲ� �߻�ü
                {
                    this.shot_cool -= Time.deltaTime;                                                   // �߻�ü �������� ��� �� �߻�ü ���� ���� ����
                    if (this.shot_cool <= 0 && this.shot_count > 0)
                    {
                        this.shot_count -= 1;
                        if (Auto_shot)                                                                  // ���Ϸ�ó, ��ź�߻��, ����, ���� ��ȭ��
                            fire_pb();
                        else if (Manual_shot)                                                           // ������, ������ ��ȭ
                            fire_sn();
                        else if (Random_shot)                                                           // �������
                            Random_fire();
                        else if (Throw_shot)                                                            // ����ź, ����ź
                            Throw();
                        else if (Bury_mine)                                                             // ����
                            Bury();
                        else if (Sword_slash)                                                           // ���
                            Sword_random();
                        this.shot_cool = this.Cool_time;
                    }
                    else if (this.shot_count <= 0)                                                      // ������ �߻�ü ���� ��ŭ �߻�
                    {
                        this.shot_count = this.count;
                        this.shot_timer = this.speed;
                    }
                }
                else
                {
                    Shotgun_fire();                                                                     // ��ź�� �߻�
                    this.shot_timer = this.speed;
                }
            }
        }

        if (Fire_Thrower)                                                                               // ȭ�� ����
        {
            if (!now_b.gameObject.activeSelf)
                soundManager.Instance.StopSoundEffect(Shot_s);
        }
    }
    /// <summary>
    /// �ڵ忡 ���� �ʱ�ȭ
    /// </summary>
    public void Init()
    {
        Bullet_pool = GameManager.Instance.pool.Bullet_P;
        axe = GameManager.Instance.weaponManager.Axe_P.GetComponent<Axe>();
        shield = GameManager.Instance.weaponManager.Shield_P.GetComponent<Axe>();

        switch (id)
        {
            case 0:                                                                                         //����
                this.current_Lv = 1;
                axe.Speed = this.speed;
                this.damage += Backend_GameData.Instance.Userstatusdatas.Attack_Power;
                Position();
                break;
            case 1:
            case 2:
            case 3:                                                                                         // 1 : �⺻ ��, �⺻ �� ��ȭ��, ���Ϸ�ó, ��ź �߻�� 2: ���� ��, ���� �� ��ȭ 3 : ����, ����, ���� ��ȭ
                this.current_Lv = 1;
                persistent = true;
                this.damage += Backend_GameData.Instance.Userstatusdatas.Attack_Power;
                this.shot_timer = this.speed -= Backend_GameData.Instance.Userstatusdatas.AttackSpeed;
                this.shot_cool = this.Cool_time;
                this.shot_count = this.count;
                break;
            case 4:                                                                                         // 4 ����ź
                this.current_Lv = 1;
                persistent = true;
                this.damage += Backend_GameData.Instance.Userstatusdatas.Attack_Power * 0.05f;
                this.shot_timer = this.speed -= Backend_GameData.Instance.Userstatusdatas.AttackSpeed;
                this.shot_cool = this.Cool_time;
                this.shot_count = this.count;
                break;
            case 5:                                                                                         // 5 ����ź
                this.current_Lv = 1;
                persistent = true;
                this.damage = 0f;
                this.shot_timer = this.speed -= Backend_GameData.Instance.Userstatusdatas.AttackSpeed;
                this.shot_cool = this.Cool_time;
                this.shot_count = this.count;
                break;
            case 6:                                                                                         // 6 ����
                this.current_Lv = 1;
                persistent = true;
                this.damage += Backend_GameData.Instance.Userstatusdatas.Attack_Power;
                this.shot_timer = this.speed -= Backend_GameData.Instance.Userstatusdatas.AttackSpeed;
                this.shot_cool = this.Cool_time;
                this.shot_count = this.count;
                break;
            case 7:                                                                                         // 7 ������, ������ ��ȭ��
                this.current_Lv = 1;
                this.damage += Backend_GameData.Instance.Userstatusdatas.Attack_Power;
                this.shot_cool = this.Cool_time;
                this.shot_count = this.count;
                Shield();
                break;
            case 8:                                                                                         // 8: ȭ�� ���� (speed ���ӽð�)
                this.current_Lv = 1;
                this.damage += Backend_GameData.Instance.Userstatusdatas.Attack_Power * 0.05f;
                this.speed += Backend_GameData.Instance.Userstatusdatas.AttackSpeed;
                this.shot_cool = this.Cool_time;
                Flame();
                break;
            case 9:                                                                                         // 9 : ��ȣź
                this.current_Lv = 1;
                shield.Speed = this.Cool_time;
                this.damage += Backend_GameData.Instance.Userstatusdatas.Attack_Power;
                this.speed -= Backend_GameData.Instance.Userstatusdatas.AttackSpeed;
                Fire_Ball();
                break;
            case 10:                                                                                        // 10 : ������
                this.current_Lv = 1;
                this.damage += Backend_GameData.Instance.Userstatusdatas.Attack_Power;
                this.shot_count = this.count;
                this.shot_cool = this.Cool_time;
                this.speed -= Backend_GameData.Instance.Userstatusdatas.AttackSpeed;
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// ��ź �߻��, ���Ϸ�ó, ����, ���� ��ȭ
    /// </summary>
    private void fire_pb()
    {
        if (!player.scanner.nearestTarget)                                                                              // ��ó Ÿ���� ���ٸ�
        {
            return;
        }

        Vector3 targetPos = player.scanner.nearestTarget.position;                                                      // Ÿ�� ����� ��ǥ ����
        Vector3 dir = targetPos - transform.position;                                                                   // ���� ����

        dir = dir.normalized;                                                                                           //���Ⱚ�� �״��, ũ�⸸ 1�� ��ȯ

        Transform bullet = GameManager.Instance.pool.Bullet_Get(prefabId).transform;                                    // Pool�Ŵ������� �Ѿ� �Ҵ�(���ٸ� �߰� ����)
        soundManager.Instance.PlaySoundEffect(Shot_s);

        bullet.parent = Bullet_pool.transform;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);                                                   // �ùٸ��� Ÿ���� ������ ȸ���� ����
        bullet.rotation = Quaternion.LookRotation(dir);                                                                 // ȸ���� ����(Ÿ���� ���ϵ���)
        bullet.localEulerAngles = new Vector3(0, bullet.localEulerAngles.y, 0);
        bullet.position = transform.position;                                                                           // ���Ʒ� Ƣ�� �� ������ ������ �� ����
        bullet.GetComponent<Bullet>().Init(this.damage, this.per, dir, alive, bullet_speed, Life_Time);                 // �ѱ��� Status�� �Ѿ� Status �Ҵ�
    }
    /// <summary>
    /// ������, ������ ��ȭ
    /// </summary>
    private void fire_sn()
    {
        Vector3 dir = Manual_Gun.transform.position- transform.position;                                                // �÷��̾ ���� �������� �߻��ϱ� ���� ManualGun���� ���� ������ �� ���� ���Ⱚ ���ϱ�

        Transform bullet = GameManager.Instance.pool.Bullet_Get(prefabId).transform;                                    // Ǯ�Ŵ������� �Ѿ� �Ҵ�

        bullet.parent = Bullet_pool.transform;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);                                                   // �ùٸ��� Ÿ���� ������ ȸ���� ����
        bullet.rotation = Quaternion.LookRotation(dir);                                                                 // ȸ���� ����(Ÿ���� ���ϵ���)
        bullet.localEulerAngles = new Vector3(0, bullet.localEulerAngles.y, 0);
        bullet.position = transform.position;                                                                           // ���Ʒ� Ƣ�� �� ������ ������ �� ����
        bullet.GetComponent<Bullet>().Init(this.damage, this.per, dir, alive, bullet_speed, Life_Time);                 // �ѱ��� Status�� �Ѿ� Status �Ҵ�
    }
    /// <summary>
    /// ���� �����ǰ� ����(ȸ��)
    /// </summary>
    private void Position()
    {
        for (int index=0;index<this.count; index++)
        {
            Transform bullet;

            if (index < axe.transform.childCount)
            {
                bullet = axe.transform.GetChild(index);                                                                 // ������ �ִ� �����յ��� ���� ī��Ʈ
            }
            else
            {
                bullet = GameManager.Instance.pool.Bullet_Get(prefabId).transform;                                      // ���� �� ���� �� ���� �ٶ��� ���Ӱ� ����
                bullet.parent = axe.transform;                                                                          // Axe ������Ʈ �Ҵ�� ������Ʈ�� �θ��, ȸ��
            }

            bullet.localPosition = Vector3.zero;                                                                        // �������� �����ǰ� ȸ������ �ʱ�ȭ �ؼ� �̻��ϰ� ��ġ�Ǵ� ���� ���
            bullet.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.up * 360 * index / count;                                                          // index �������� ȸ���� �����ؼ� �����ϰ� ��ġ
            bullet.Rotate(rotVec);                                                                                      // ȸ������ ���� ��ġ
            bullet.Translate(bullet.up * 1.35f, Space.World);

            bullet.GetComponent<Bullet>().Init(this.damage, this.per, Vector3.zero, alive, bullet_speed, Life_Time);    // -1 ���� ������
        }
    }
    /// <summary>
    /// ����, ���� ��ȭ��
    /// </summary>
    private void Shield()
    {
        Transform bullet = GameManager.Instance.pool.Bullet_Get(prefabId).transform;                                    // Ǯ�Ŵ������� �Ҵ�

        bullet.parent = shield.transform;                                                                               // Axe ������Ʈ �Ҵ�� Shield ������Ʈ�� �θ�� �Ҵ�
        bullet.position = player.transform.position;                                                                    // Player�� �߽����� ����
        bullet.Translate(Vector3.up * (1f-0.68f), Space.World);

        bullet.GetComponent<Bullet>().Init(this.damage, this.per, Vector3.zero, alive, bullet_speed, Life_Time);        // -1 ���� ������
        bullet.GetComponent<Bullet>().Init_additional(this.Additional);
    }
    /// <summary>
    /// ���� ũ��(LevelUp�� Size UP)
    /// </summary>
    private void Shield_Size()
    {
        Transform bullet;

        bullet = shield.transform.GetChild(0);
        bullet.localScale = new Vector3(bullet.localScale.x + this.Life_Time, bullet.localScale.y + this.Life_Time, bullet.localScale.z + this.Life_Time);                  // Size ����
        bullet.GetComponent<Bullet>().Init(this.damage, this.per, Vector3.zero, alive, bullet_speed, Life_Time);                                                            // -1 ���� ������
        bullet.GetComponent<Bullet>().Init_additional(this.Additional);
    }
    /// <summary>
    /// �������
    /// </summary>
    private void Random_fire()
    {
        int ran = Random.Range(0, 360);                                                                                 // �ﰢ�Լ� ���, ������ ��ġ���� 1��ŭ ������ ���� ���� �������� ����

        float x = Mathf.Cos(ran * Mathf.Deg2Rad) * 1f;
        float z = Mathf.Sin(ran * Mathf.Deg2Rad) * 1f;

        Vector3 targetPos = transform.position + new Vector3(x, 0, z);
        Vector3 dir = targetPos - transform.position;

        dir = dir.normalized;                                                                                           // ���Ⱚ�� �״��, ũ�⸸ 1�� ��ȯ

        Transform bullet = GameManager.Instance.pool.Bullet_Get(prefabId).transform;                                    // Ǯ�Ŵ������� �Ѿ� �Ҵ�

        bullet.parent = Bullet_pool.transform;
        bullet.rotation = Quaternion.LookRotation(dir);                                                                 // ȸ���� ����(Ÿ���� ���ϵ���)
        bullet.localEulerAngles = new Vector3(0, bullet.localEulerAngles.y, 0);
        bullet.position = transform.position;                                                                           // �� �Ʒ� Ƣ�� �� ������ ������ �� ����
        bullet.GetComponent<Bullet>().Init(this.damage, this.per, dir, alive, bullet_speed, Life_Time);                 // �ѱ��� Status�� �Ѿ� Status �Ҵ�
    }
    /// <summary>
    /// ��ź��, ��ź�� ��ȭ��
    /// </summary>
    private void Shotgun_fire()
    {
        int ran = Random.Range(0, 360);                                                                                 // ���� �� ���� ����

        for(int index=0;index<this.count;index++)                                                                       // �߻�ü ������ŭ �ݺ��ؼ� �� �߻�ü���� �� ���⿡�� ���� ������ ���Ⱚ ȹ��
        {
            int ran_ = ran + Random.Range(36, 72);                                                                      // �� ���⿡�� �߰� ���� ����

            float x = Mathf.Cos(ran_ * Mathf.Deg2Rad) * 1f;                                                             // �ﰢ�Լ� ���, ������ ��ġ���� 1��ŭ ������ ���� ���� ��������
            float z = Mathf.Sin(ran_ * Mathf.Deg2Rad) * 1f;

            Vector3 targetPos = transform.position + new Vector3(x, 0, z);
            Vector3 dir = targetPos - transform.position;

            dir = dir.normalized;                                                                                       // ���Ⱚ�� �״��, ũ�⸸ 1�� ��ȯ

            Transform bullet = GameManager.Instance.pool.Bullet_Get(prefabId).transform;                                // Ǯ�Ŵ������� �Ѿ� �Ҵ�

            bullet.parent = Bullet_pool.transform;
            bullet.rotation = Quaternion.LookRotation(dir);                                                             // ȸ���� ����(Ÿ���� ���ϵ���)
            bullet.localEulerAngles = new Vector3(0, bullet.localEulerAngles.y, 0);                                     // �� �Ʒ� Ƣ�� �� ������ ������ �� ȸ���� ����
            bullet.position = transform.position;
            bullet.GetComponent<Bullet>().Init(this.damage, this.per, dir, alive, bullet_speed, Life_Time);             // �ѱ��� Status�� �Ѿ� Status �Ҵ�
        }
    }
    /// <summary>
    /// ����ź, ����ź
    /// </summary>
    private void Throw()
    {
        if (!player.scanner.nearestTarget)                                                                          // ��ó Ÿ���� ���ٸ�
        {
            return;
        }

        Vector3 targetPos = player.scanner.nearestTarget.position;                                                  // ���� ����� Ÿ�� ��ǥ ����

        Transform bullet = GameManager.Instance.pool.Bullet_Get(prefabId).transform;                                // Ǯ �Ŵ������� �Ѿ� �Ҵ�

        bullet.parent = Bullet_pool.transform;                                                                      
        bullet.position = transform.position;                                                                       // �� �Ʒ� Ƣ�� �� ������ ������ �� ����
        bullet.Translate(Vector3.up * 1.5f, Space.World);                                                           // ���� ���Ͱ����� �߰��� ��
        bullet.GetComponent<Bullet>().Init_Bomb(this.damage, bullet_speed, targetPos, this.per, this.count);
    }
    /// <summary>
    /// ����
    /// </summary>
    private void Bury()
    {
        int ran = Random.Range(0, 360);                                                                                 // ���� ����

        Vector3 dir = new Vector3(0, ran, 0);

        Transform bullet = GameManager.Instance.pool.Bullet_Get(prefabId).transform;                                    // Ǯ �Ŵ������� �Ѿ� �Ҵ�

        bullet.parent = Bullet_pool.transform;
        bullet.rotation = Quaternion.Euler(dir.x, dir.y, dir.z);                                                        // �� �Ʒ� Ƣ�� �� ������ ȸ�� �� ������ �� ����
        bullet.position = transform.position;        
        bullet.GetComponent<Bullet>().Init_Mine(this.damage, this.per, bullet_speed);
    }
    /// <summary>
    /// ȭ�� ����, ȭ�� ���� ��ȭ
    /// </summary>
    private void Flame()
    {
        shield = GameManager.Instance.weaponManager.Flare_P.GetComponent<Axe>();                                        // Axe�� ȸ���ϱ⿡ Shield�� �Ҵ�
        Transform bullet = GameManager.Instance.pool.Bullet_Get(prefabId).transform;
        now_b = bullet.GetComponent<Bullet>();
        soundManager.Instance.PlaySoundEffect(Shot_s);

        bullet.parent = shield.transform;
        bullet.position = player.transform.position;
        bullet.Translate(Vector3.up * (1f - 0.68f), Space.World);

        bullet.GetComponent<Bullet>().Init_Flare(this.damage, this.speed, this.per, this.Cool_time);
    }
    /// <summary>
    /// ȭ�� ����, ȭ�� ���� ��ȭ ��ƼŬ ���� ������ �����ʿ��ϱ⿡ ���� �Լ�
    /// </summary>
    private void Flame_Up()
    {
        Transform bullet;

        bullet = shield.transform.GetChild(0);
        bullet.GetComponent<Bullet>().Init_Flare(this.damage, this.speed, this.per, this.Cool_time);
    }
    /// <summary>
    /// ��ȣź
    /// </summary>
    private void Fire_Ball()
    {
        shield = GameManager.Instance.weaponManager.FireBall_P.GetComponent<Axe>();                                     // ��ȣź ���忡 �Ҵ�
        shield.Speed = this.Cool_time;                                                                                  // ȸ�� �ӵ�

        for (int index = 0; index < this.count; index++)
        {
            Transform bullet;

            if (index < shield.transform.childCount)
            {
                bullet = shield.transform.GetChild(index);                                                              // ������ �ִ� �����յ��� ���� ī��Ʈ
            }
            else
            {
                bullet = GameManager.Instance.pool.Bullet_Get(prefabId).transform;                                      // ���� �� ���� �� ���� �ٶ��� ���Ӱ� ����
                bullet.parent = shield.transform;
            }

            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;                                                                 // �������� �����ǰ� ȸ������ �ʱ�ȭ �ؼ� �̻��ϰ� ��ġ�Ǵ� ���� ���

            Vector3 rotVec = Vector3.up * 360 * index / count;                                                          // ������ ���� ȸ�� �� ����
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.35f, Space.World);

            bullet.GetComponent<Flare_Ball>().Init_(this.damage, this.speed, bullet_speed, Life_Time, Bullet_pool, this.alive, this.per, Additional);       // -1 ���� ������
        }
    }
    /// <summary>
    /// ���
    /// </summary>
    private void Sword_random()
    {
        int ran = Random.Range(0, 360);                                                                                 // ���� �� ����

        Vector3 pos = transform.position + (Random.insideUnitSphere * 4.7f);                                            // ������ �������� �������� ������ ���� ���� ����

        pos.y = transform.position.y;

        Transform bullet = GameManager.Instance.pool.Bullet_Get(prefabId).transform;                                    // Ǯ �Ŵ������� �Ѿ� �Ҵ�(���� �� ���� ����)

        bullet.parent = Bullet_pool.transform;
        bullet.rotation = Quaternion.EulerRotation(new Vector3(0, ran, 0));                                             // Ƣ�� �� �����ϱ� ���� ȸ�� �� �� ������ �� ����
        bullet.position = pos;
        bullet.GetComponent<Bullet>().Init_Mine(this.damage, this.per, bullet_speed);
    }
    /// <summary>
    /// Player�� �⺻ Attack Damage�� �ѱ��� �⺻ Attack Damage�� �ջ��ؼ� ��� �� LevelUP�� ����
    /// </summary>
    public void Update_Damage()
    {
        float t_damage = basic_damage;
        t_damage += Backend_GameData.Instance.Userstatusdatas.Attack_Power;

        for(int index=0;index<current_Lv-1; index++)
        {
            t_damage += this.UP_damage;
        }

        this.damage = t_damage;
    }
    /// <summary>
    /// ��Ÿ�� ����(�Ǵ� �߻�ü �ӵ�)
    /// </summary>
    /// <param name="t_percentage"></param>
    public void Multiple_status()
    {
        float t_speed = basic_speed;
        t_speed -= t_speed * (Backend_GameData.Instance.Userstatusdatas.AttackSpeed / 100f);

        for(int index = 0; index < current_Lv - 1; index++)
        {
            t_speed -= this.UP_speed;
        }
        this.speed = t_speed;                                                                      // �����ؾ� ��Ÿ�� ������ ª����
    }
    /// <summary>
    /// ��Ÿ�� ����(���ӽð�)
    /// </summary>
    /// <param name="t_percentage"></param>
    public void R_Multiple_status(float t_percentage)
    {
        float t_speed = basic_speed;
        t_speed += t_speed * (t_percentage / 100f);

        for (int index = 0; index < current_Lv - 1; index++)
        {
            t_speed += this.UP_speed;
        }
        this.speed += t_speed;                                                                      // �����ؾ� ���� �ð��� �����
    }
    /// <summary>
    /// Player Ư�� LevelUP�� ���� Status ��ȭ
    /// </summary>
    /// <param name="pre"></param>
    /// <param name="now"></param>
    public void R_Return_Multiple(float pre, float now)
    {
        this.speed -= this.speed * (pre / 100f);
        this.R_Multiple_status(now);
    }
    /// <summary>
    /// ���� LevelUP�� �ɷ�ġ ����
    /// </summary>
    public void LevelUp()
    {
        if (this.current_Lv < maxLv)
        {
            this.current_Lv += 1;
            switch (id)
            {
                case 0:
                    this.damage += this.UP_damage;
                    axe.Speed += this.UP_speed;
                    this.count += this.UP_count;
                    Position();
                    break;
                case 1:
                case 2:
                case 3:
                case 4:
                    this.damage += this.UP_damage;
                    this.speed -= this.UP_speed;
                    this.shot_count = this.count += this.UP_count;
                    break;
                case 5:
                    this.damage = 0f;
                    this.speed -= this.UP_speed;
                    if (this.current_Lv % 2 == 0)
                        this.shot_count = this.count += this.UP_count;
                    break;
                case 6:
                    this.speed -= this.UP_speed;
                    if (this.current_Lv % 2 != 0)
                        this.damage += this.UP_damage;
                    if (this.current_Lv % 2 == 0)
                        this.shot_count = this.count += this.UP_count;
                    break;
                case 7:
                    if (this.current_Lv % 2 != 0)
                        this.speed -= this.UP_speed;
                    if (this.current_Lv % 2 == 0)
                        this.damage += this.UP_damage;
                    this.Additional += this.UP_Additional;
                    Shield_Size();
                    break;
                case 8:
                    if (this.current_Lv % 2 != 0)
                        this.damage += this.UP_damage;
                    if (this.current_Lv % 2 == 0)
                        this.speed += this.UP_speed;
                    Flame_Up();
                    break;
                case 9:
                    if (this.current_Lv % 2 == 0)
                        this.damage += this.UP_damage;
                    if (this.current_Lv % 2 != 0)
                        this.count += this.UP_count;
                    this.speed -= this.UP_speed;
                    Fire_Ball();
                    break;
                case 10:
                    if (this.current_Lv % 2 == 0)
                        this.damage += this.UP_damage;
                    if (this.current_Lv % 2 != 0)
                    {
                        this.count += this.UP_count;
                        this.speed -= this.UP_speed;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
