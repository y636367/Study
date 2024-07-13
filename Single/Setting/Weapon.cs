using UnityEngine;

public class Weapon : MonoBehaviour
{
    #region Variable
    [Header("기본 수치")]
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

    [Header("강화 수치")]
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
        basic_speed = this.speed;                                                                  //여기서 호출하면 null 에러 남(씬 바로 로드 후 미생성 으로 인한 오류 발생)
        
        Init();
    }
    private void Update()
    {
        if (!GameManager.Instance.Start_)                                                               // 게임 일시정지, 사망, 종료 등에 의한 상황으로 멈추어야 할 때가 있기에 코루틴이 아닌 Update문으로 생성
        {
            return;
        }

        if (persistent)                                                                                 // 무기 종류 발사체인 경우
        {
            if (this.shot_timer > 0)
                this.shot_timer -= Time.deltaTime;                                                      // 쿨타임 마다 발사 하도록 쿨타임 갱신
            else
            {
                if (Nomal_shot)                                                                         // 일반 발사체
                {
                    this.shot_cool -= Time.deltaTime;                                                   // 발사체 여러개인 경우 각 발사체 마다 사이 간격
                    if (this.shot_cool <= 0 && this.shot_count > 0)
                    {
                        this.shot_count -= 1;
                        if (Auto_shot)                                                                  // 로켓런처, 유탄발사기, 권총, 권총 강화형
                            fire_pb();
                        else if (Manual_shot)                                                           // 저격총, 저격총 강화
                            fire_sn();
                        else if (Random_shot)                                                           // 기관단총
                            Random_fire();
                        else if (Throw_shot)                                                            // 연막탄, 섬광탄
                            Throw();
                        else if (Bury_mine)                                                             // 지뢰
                            Bury();
                        else if (Sword_slash)                                                           // 대검
                            Sword_random();
                        this.shot_cool = this.Cool_time;
                    }
                    else if (this.shot_count <= 0)                                                      // 지정된 발사체 개수 만큼 발사
                    {
                        this.shot_count = this.count;
                        this.shot_timer = this.speed;
                    }
                }
                else
                {
                    Shotgun_fire();                                                                     // 산탄총 발사
                    this.shot_timer = this.speed;
                }
            }
        }

        if (Fire_Thrower)                                                                               // 화염 방사기
        {
            if (!now_b.gameObject.activeSelf)
                soundManager.Instance.StopSoundEffect(Shot_s);
        }
    }
    /// <summary>
    /// 코드에 따른 초기화
    /// </summary>
    public void Init()
    {
        Bullet_pool = GameManager.Instance.pool.Bullet_P;
        axe = GameManager.Instance.weaponManager.Axe_P.GetComponent<Axe>();
        shield = GameManager.Instance.weaponManager.Shield_P.GetComponent<Axe>();

        switch (id)
        {
            case 0:                                                                                         //도끼
                this.current_Lv = 1;
                axe.Speed = this.speed;
                this.damage += Backend_GameData.Instance.Userstatusdatas.Attack_Power;
                Position();
                break;
            case 1:
            case 2:
            case 3:                                                                                         // 1 : 기본 총, 기본 총 강화형, 로켓런처, 유탄 발사기 2: 저격 총, 저격 총 강화 3 : 난사, 샷건, 샷건 강화
                this.current_Lv = 1;
                persistent = true;
                this.damage += Backend_GameData.Instance.Userstatusdatas.Attack_Power;
                this.shot_timer = this.speed -= Backend_GameData.Instance.Userstatusdatas.AttackSpeed;
                this.shot_cool = this.Cool_time;
                this.shot_count = this.count;
                break;
            case 4:                                                                                         // 4 연막탄
                this.current_Lv = 1;
                persistent = true;
                this.damage += Backend_GameData.Instance.Userstatusdatas.Attack_Power * 0.05f;
                this.shot_timer = this.speed -= Backend_GameData.Instance.Userstatusdatas.AttackSpeed;
                this.shot_cool = this.Cool_time;
                this.shot_count = this.count;
                break;
            case 5:                                                                                         // 5 섬광탄
                this.current_Lv = 1;
                persistent = true;
                this.damage = 0f;
                this.shot_timer = this.speed -= Backend_GameData.Instance.Userstatusdatas.AttackSpeed;
                this.shot_cool = this.Cool_time;
                this.shot_count = this.count;
                break;
            case 6:                                                                                         // 6 지뢰
                this.current_Lv = 1;
                persistent = true;
                this.damage += Backend_GameData.Instance.Userstatusdatas.Attack_Power;
                this.shot_timer = this.speed -= Backend_GameData.Instance.Userstatusdatas.AttackSpeed;
                this.shot_cool = this.Cool_time;
                this.shot_count = this.count;
                break;
            case 7:                                                                                         // 7 독가스, 독가스 강화형
                this.current_Lv = 1;
                this.damage += Backend_GameData.Instance.Userstatusdatas.Attack_Power;
                this.shot_cool = this.Cool_time;
                this.shot_count = this.count;
                Shield();
                break;
            case 8:                                                                                         // 8: 화염 방사기 (speed 지속시간)
                this.current_Lv = 1;
                this.damage += Backend_GameData.Instance.Userstatusdatas.Attack_Power * 0.05f;
                this.speed += Backend_GameData.Instance.Userstatusdatas.AttackSpeed;
                this.shot_cool = this.Cool_time;
                Flame();
                break;
            case 9:                                                                                         // 9 : 신호탄
                this.current_Lv = 1;
                shield.Speed = this.Cool_time;
                this.damage += Backend_GameData.Instance.Userstatusdatas.Attack_Power;
                this.speed -= Backend_GameData.Instance.Userstatusdatas.AttackSpeed;
                Fire_Ball();
                break;
            case 10:                                                                                        // 10 : 나이프
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
    /// 유탄 발사기, 로켓런처, 권총, 권총 강화
    /// </summary>
    private void fire_pb()
    {
        if (!player.scanner.nearestTarget)                                                                              // 근처 타겟이 없다면
        {
            return;
        }

        Vector3 targetPos = player.scanner.nearestTarget.position;                                                      // 타겟 존재시 목표 설정
        Vector3 dir = targetPos - transform.position;                                                                   // 방향 설정

        dir = dir.normalized;                                                                                           //방향값은 그대로, 크기만 1로 변환

        Transform bullet = GameManager.Instance.pool.Bullet_Get(prefabId).transform;                                    // Pool매니저에서 총알 할당(없다면 추가 생성)
        soundManager.Instance.PlaySoundEffect(Shot_s);

        bullet.parent = Bullet_pool.transform;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);                                                   // 올바르게 타겟을 보도록 회전값 갱신
        bullet.rotation = Quaternion.LookRotation(dir);                                                                 // 회전값 수정(타겟을 향하도록)
        bullet.localEulerAngles = new Vector3(0, bullet.localEulerAngles.y, 0);
        bullet.position = transform.position;                                                                           // 위아래 튀는 일 없도록 포지션 값 갱신
        bullet.GetComponent<Bullet>().Init(this.damage, this.per, dir, alive, bullet_speed, Life_Time);                 // 총기의 Status에 총알 Status 할당
    }
    /// <summary>
    /// 저격총, 저격총 강화
    /// </summary>
    private void fire_sn()
    {
        Vector3 dir = Manual_Gun.transform.position- transform.position;                                                // 플레이어가 보는 방향으로 발사하기 위해 ManualGun에서 현재 포지션 값 뺴서 방향값 구하기

        Transform bullet = GameManager.Instance.pool.Bullet_Get(prefabId).transform;                                    // 풀매니저에서 총알 할당

        bullet.parent = Bullet_pool.transform;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);                                                   // 올바르게 타겟을 보도록 회전값 갱신
        bullet.rotation = Quaternion.LookRotation(dir);                                                                 // 회전값 수정(타겟을 향하도록)
        bullet.localEulerAngles = new Vector3(0, bullet.localEulerAngles.y, 0);
        bullet.position = transform.position;                                                                           // 위아래 튀는 일 없도록 포지션 값 갱신
        bullet.GetComponent<Bullet>().Init(this.damage, this.per, dir, alive, bullet_speed, Life_Time);                 // 총기의 Status에 총알 Status 할당
    }
    /// <summary>
    /// 도끼 포지션값 세팅(회전)
    /// </summary>
    private void Position()
    {
        for (int index=0;index<this.count; index++)
        {
            Transform bullet;

            if (index < axe.transform.childCount)
            {
                bullet = axe.transform.GetChild(index);                                                                 // 기존에 있는 프리팹들을 먼저 카운트
            }
            else
            {
                bullet = GameManager.Instance.pool.Bullet_Get(prefabId).transform;                                      // 기존 수 보다 더 많이 바랄시 새롭게 생성
                bullet.parent = axe.transform;                                                                          // Axe 컴포넌트 할당된 오브젝트를 부모로, 회전
            }

            bullet.localPosition = Vector3.zero;                                                                        // 프리팹의 포지션과 회전값을 초기화 해서 이상하게 배치되는 오류 잡기
            bullet.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.up * 360 * index / count;                                                          // index 개수마다 회전각 설정해서 정갈하게 배치
            bullet.Rotate(rotVec);                                                                                      // 회전각에 의해 배치
            bullet.Translate(bullet.up * 1.35f, Space.World);

            bullet.GetComponent<Bullet>().Init(this.damage, this.per, Vector3.zero, alive, bullet_speed, Life_Time);    // -1 관통 무조건
        }
    }
    /// <summary>
    /// 쉴드, 쉴드 강화형
    /// </summary>
    private void Shield()
    {
        Transform bullet = GameManager.Instance.pool.Bullet_Get(prefabId).transform;                                    // 풀매니저에서 할당

        bullet.parent = shield.transform;                                                                               // Axe 컴포넌트 할당된 Shield 오브젝트를 부모로 할당
        bullet.position = player.transform.position;                                                                    // Player를 중심으로 생성
        bullet.Translate(Vector3.up * (1f-0.68f), Space.World);

        bullet.GetComponent<Bullet>().Init(this.damage, this.per, Vector3.zero, alive, bullet_speed, Life_Time);        // -1 관통 무조건
        bullet.GetComponent<Bullet>().Init_additional(this.Additional);
    }
    /// <summary>
    /// 쉴드 크기(LevelUp시 Size UP)
    /// </summary>
    private void Shield_Size()
    {
        Transform bullet;

        bullet = shield.transform.GetChild(0);
        bullet.localScale = new Vector3(bullet.localScale.x + this.Life_Time, bullet.localScale.y + this.Life_Time, bullet.localScale.z + this.Life_Time);                  // Size 갱신
        bullet.GetComponent<Bullet>().Init(this.damage, this.per, Vector3.zero, alive, bullet_speed, Life_Time);                                                            // -1 관통 무조건
        bullet.GetComponent<Bullet>().Init_additional(this.Additional);
    }
    /// <summary>
    /// 기관단총
    /// </summary>
    private void Random_fire()
    {
        int ran = Random.Range(0, 360);                                                                                 // 삼각함수 사용, 정해진 위치에서 1만큼 떨어진 원형 랜덤 방향으로 생성

        float x = Mathf.Cos(ran * Mathf.Deg2Rad) * 1f;
        float z = Mathf.Sin(ran * Mathf.Deg2Rad) * 1f;

        Vector3 targetPos = transform.position + new Vector3(x, 0, z);
        Vector3 dir = targetPos - transform.position;

        dir = dir.normalized;                                                                                           // 방향값은 그대로, 크기만 1로 변환

        Transform bullet = GameManager.Instance.pool.Bullet_Get(prefabId).transform;                                    // 풀매니저에서 총알 할당

        bullet.parent = Bullet_pool.transform;
        bullet.rotation = Quaternion.LookRotation(dir);                                                                 // 회전값 수정(타겟을 향하도록)
        bullet.localEulerAngles = new Vector3(0, bullet.localEulerAngles.y, 0);
        bullet.position = transform.position;                                                                           // 위 아래 튀는 일 없도록 포지션 값 갱신
        bullet.GetComponent<Bullet>().Init(this.damage, this.per, dir, alive, bullet_speed, Life_Time);                 // 총기의 Status에 총알 Status 할당
    }
    /// <summary>
    /// 산탄총, 산탄총 강화형
    /// </summary>
    private void Shotgun_fire()
    {
        int ran = Random.Range(0, 360);                                                                                 // 랜덤 주 방향 설정

        for(int index=0;index<this.count;index++)                                                                       // 발사체 개수만큼 반복해서 각 발사체마다 주 방향에서 조금 벌어진 방향값 획득
        {
            int ran_ = ran + Random.Range(36, 72);                                                                      // 주 방향에서 추가 방향 설정

            float x = Mathf.Cos(ran_ * Mathf.Deg2Rad) * 1f;                                                             // 삼각함수 사용, 정해진 위치에서 1만큼 떨어진 원형 랜덤 방향으로
            float z = Mathf.Sin(ran_ * Mathf.Deg2Rad) * 1f;

            Vector3 targetPos = transform.position + new Vector3(x, 0, z);
            Vector3 dir = targetPos - transform.position;

            dir = dir.normalized;                                                                                       // 방향값은 그대로, 크기만 1로 변환

            Transform bullet = GameManager.Instance.pool.Bullet_Get(prefabId).transform;                                // 풀매니저에서 총알 할당

            bullet.parent = Bullet_pool.transform;
            bullet.rotation = Quaternion.LookRotation(dir);                                                             // 회전값 수정(타겟을 향하도록)
            bullet.localEulerAngles = new Vector3(0, bullet.localEulerAngles.y, 0);                                     // 위 아래 튀는 일 없도록 포지션 및 회전값 갱신
            bullet.position = transform.position;
            bullet.GetComponent<Bullet>().Init(this.damage, this.per, dir, alive, bullet_speed, Life_Time);             // 총기의 Status에 총알 Status 할당
        }
    }
    /// <summary>
    /// 연막탄, 섬광탄
    /// </summary>
    private void Throw()
    {
        if (!player.scanner.nearestTarget)                                                                          // 근처 타겟이 없다면
        {
            return;
        }

        Vector3 targetPos = player.scanner.nearestTarget.position;                                                  // 가장 가까운 타겟 목표 설정

        Transform bullet = GameManager.Instance.pool.Bullet_Get(prefabId).transform;                                // 풀 매니저에서 총알 할당

        bullet.parent = Bullet_pool.transform;                                                                      
        bullet.position = transform.position;                                                                       // 위 아래 튀는 일 없도록 포지션 값 갱신
        bullet.Translate(Vector3.up * 1.5f, Space.World);                                                           // 기존 벡터값에서 추가가 됨
        bullet.GetComponent<Bullet>().Init_Bomb(this.damage, bullet_speed, targetPos, this.per, this.count);
    }
    /// <summary>
    /// 지뢰
    /// </summary>
    private void Bury()
    {
        int ran = Random.Range(0, 360);                                                                                 // 각도 랜덤

        Vector3 dir = new Vector3(0, ran, 0);

        Transform bullet = GameManager.Instance.pool.Bullet_Get(prefabId).transform;                                    // 풀 매니저에서 총알 할당

        bullet.parent = Bullet_pool.transform;
        bullet.rotation = Quaternion.Euler(dir.x, dir.y, dir.z);                                                        // 위 아래 튀는 일 없도록 회전 및 포지션 값 갱신
        bullet.position = transform.position;        
        bullet.GetComponent<Bullet>().Init_Mine(this.damage, this.per, bullet_speed);
    }
    /// <summary>
    /// 화염 방사기, 화염 방사기 강화
    /// </summary>
    private void Flame()
    {
        shield = GameManager.Instance.weaponManager.Flare_P.GetComponent<Axe>();                                        // Axe는 회전하기에 Shield에 할당
        Transform bullet = GameManager.Instance.pool.Bullet_Get(prefabId).transform;
        now_b = bullet.GetComponent<Bullet>();
        soundManager.Instance.PlaySoundEffect(Shot_s);

        bullet.parent = shield.transform;
        bullet.position = player.transform.position;
        bullet.Translate(Vector3.up * (1f - 0.68f), Space.World);

        bullet.GetComponent<Bullet>().Init_Flare(this.damage, this.speed, this.per, this.Cool_time);
    }
    /// <summary>
    /// 화염 방사기, 화염 방사기 강화 파티클 관련 데이터 갱신필요하기에 갱신 함수
    /// </summary>
    private void Flame_Up()
    {
        Transform bullet;

        bullet = shield.transform.GetChild(0);
        bullet.GetComponent<Bullet>().Init_Flare(this.damage, this.speed, this.per, this.Cool_time);
    }
    /// <summary>
    /// 신호탄
    /// </summary>
    private void Fire_Ball()
    {
        shield = GameManager.Instance.weaponManager.FireBall_P.GetComponent<Axe>();                                     // 신호탄 쉴드에 할당
        shield.Speed = this.Cool_time;                                                                                  // 회전 속도

        for (int index = 0; index < this.count; index++)
        {
            Transform bullet;

            if (index < shield.transform.childCount)
            {
                bullet = shield.transform.GetChild(index);                                                              // 기존에 있는 프리팹들을 먼저 카운트
            }
            else
            {
                bullet = GameManager.Instance.pool.Bullet_Get(prefabId).transform;                                      // 기존 수 보다 더 많이 바랄시 새롭게 생성
                bullet.parent = shield.transform;
            }

            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;                                                                 // 프리팹의 포지션과 회전값을 초기화 해서 이상하게 배치되는 오류 잡기

            Vector3 rotVec = Vector3.up * 360 * index / count;                                                          // 갯수에 따라 회전 각 설정
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.35f, Space.World);

            bullet.GetComponent<Flare_Ball>().Init_(this.damage, this.speed, bullet_speed, Life_Time, Bullet_pool, this.alive, this.per, Additional);       // -1 관통 무조건
        }
    }
    /// <summary>
    /// 대검
    /// </summary>
    private void Sword_random()
    {
        int ran = Random.Range(0, 360);                                                                                 // 랜덤 값 설정

        Vector3 pos = transform.position + (Random.insideUnitSphere * 4.7f);                                            // 현재의 포지션을 기준으로 원형의 랜덤 지점 설정

        pos.y = transform.position.y;

        Transform bullet = GameManager.Instance.pool.Bullet_Get(prefabId).transform;                                    // 풀 매니저에서 총알 할당(없을 시 새로 생성)

        bullet.parent = Bullet_pool.transform;
        bullet.rotation = Quaternion.EulerRotation(new Vector3(0, ran, 0));                                             // 튀는 걸 방지하기 위해 회전 값 및 포지션 값 설정
        bullet.position = pos;
        bullet.GetComponent<Bullet>().Init_Mine(this.damage, this.per, bullet_speed);
    }
    /// <summary>
    /// Player의 기본 Attack Damage와 총기의 기본 Attack Damage와 합산해서 계산 및 LevelUP시 갱신
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
    /// 쿨타임 갱신(또는 발사체 속도)
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
        this.speed = t_speed;                                                                      // 감소해야 쿨타임 간격이 짧아짐
    }
    /// <summary>
    /// 쿨타임 갱신(지속시간)
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
        this.speed += t_speed;                                                                      // 증가해야 지속 시간이 길어짐
    }
    /// <summary>
    /// Player 특성 LevelUP에 의한 Status 강화
    /// </summary>
    /// <param name="pre"></param>
    /// <param name="now"></param>
    public void R_Return_Multiple(float pre, float now)
    {
        this.speed -= this.speed * (pre / 100f);
        this.R_Multiple_status(now);
    }
    /// <summary>
    /// 무기 LevelUP시 능력치 갱신
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
