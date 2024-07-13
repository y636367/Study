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
        if (!GameManager.Instance.Start_)                                                                                       // 게임 일시정지, 사망, 클리어 시 도중 멈추기 위해 코루틴이 아닌 Update 문으로 설정
        {
            if (!done)                                                                                                          // 일시정지
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
            if (done)                                                                                                           // 다시 속력 부여 재생
            {
                try
                {
                    done = false;
                    rigid.velocity = t_speed;
                }
                catch (MissingComponentException) { }
            }

            if (Slower || Stun)                                                                                                 // 연막탄, 섬광탄 설정된 목표까지 포물선그리며 날라가는 
            {
                transform.position = Vector3.Slerp(transform.position, target, speed);
            }
            else if (Mine)                                                                                                      // 지뢰 폭발 처리
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
    /// 파티클 정지(이벤트 함수)
    /// </summary>
    private void OnParticleSystemStopped()
    {
        soundManager.Instance.StopSoundEffect(Bomb_s);
        this.gameObject.SetActive(false);
    }
    /// <summary>
    /// 총알 무기의 Status에 따라 초기화
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
            rigid.velocity = dir * t_speed;                                                                                                 // 속력
        }

        if (!t_alive)
        {
            Invoke("Extinction", life_time);
        }
    }
    /// <summary>
    /// 무기 폭탄 설정 초기화 함수
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
    /// 무기 지뢰, 대검 설정 초기화 함수
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
    /// 무기 중 추가 효과를 가진 설정 값 추가 함수
    /// </summary>
    /// <param name="t_data"></param>
    public void Init_additional(float t_data)
    {
        this.additional = t_data;
    }
    /// <summary>
    /// 화염 방사기 파티클 의 화염방사기의 Status에 맞게 지속시간, 데미지 등 값 갱신
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="t_speed"></param>
    /// <param name="per"></param>
    /// <param name="cool"></param>
    public void Init_Flare(float damage, float t_speed, int per, float cool)
    {
        var particle = this.gameObject.GetComponent<ParticleSystem>();                                                          // GetComponent<ParticleSystem>().duration 및 일부 코드는 더 이상 사용되지 않기에 main으로 접근할 것

        this.Damage = damage;
        this.per = per;
        this.Cool_time = cool;
        this.gameObject.GetComponent<Small_Particle_Control>().Particle_Clear();
        ParticleSystem.MainModule mainModule = particle.main;                                                                   // 파티클 시스탬의 MainModule 접근 (주요설정에 접근할 수 있는 모듈)
        mainModule.duration = t_speed;                                                                                          // 재설정을 위해선 파티클이 끝나거나 정지 상태에서만 가능
        this.gameObject.GetComponent<Small_Particle_Control>().Particle_Resume();

        this.additional = this.speed = mainModule.duration + this.Cool_time + 1.25f;

        gameObject.GetComponentInParent<Axe>().Get_Flame(this.Cool_time);
    }
    /// <summary>
    /// 이벤트 함수로 비활성화
    /// </summary>
    private void Extinction()
    {
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 각 무기의 특성에 맞게 효과 적용
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Wall"))                                                                                                            // 필드의 경계선(벽)에 맞을 시 Activefalse
            gameObject.SetActive(false);

        if (Slower || Stun)
        {
            if ((!other.CompareTag("Enemy") && !other.CompareTag("Ground")) || per == -1)                                                       //-1 관통 무한을 뜻함
                return;
        }
        else
            if (!other.CompareTag("Enemy") || per == -1)                                                                                        //-1 관통 무한을 뜻함
                return;


        per--;                                                                                                                                  // 피격체 하나씩 닿을때마다 감소

        if (per == -1)
        {
            if (Bombs)                                                                                                                          // 폭탄 이펙트 생성과 동시에 이펙트 스플래쉬 데미지를 위한 Bullet Status 설정
            {
                GameObject effect = GameManager.Instance.pool.Effect_Get(6);
                soundManager.Instance.PlaySoundEffect(Bomb_s);
                effect.GetComponent<Bomb>().Damage = this.Damage;
                effect.transform.position = this.transform.position;
                rigid.velocity = Vector3.zero;
                gameObject.SetActive(false);                                                                                                    // 이펙트 생성과 동시에 Bullet은 ActiveFalse
            }
            else if (Launcher)                                                                                                                  // 런처 이펙트 생성과 동시에 이펙트 스플래쉬 데미지를 위한 Bullet Status 설정
            {
                GameObject effect = GameManager.Instance.pool.Effect_Get(7);
                effect.GetComponent<Bomb>().Damage = this.Damage;
                effect.transform.position = this.transform.position;
                rigid.velocity = Vector3.zero;
                gameObject.SetActive(false);                                                                                                    // 이펙트 생성과 동시에 Bullet은 ActiveFalse
            }
            else if (Slower)                                                                                                                    // Slower 이펙트 생성과 동시에 이펙트 스플래쉬 효과를 위한 Bullet Status 설정
            {   
                GameObject effect = GameManager.Instance.pool.Effect_Get(8);
                soundManager.Instance.PlaySoundEffect(Bomb_s);
                effect.GetComponent<Bomb>().Damage = this.Damage;
                effect.GetComponent<Bomb>().Count = this.count;
                effect.transform.position = this.transform.position;
                rigid.velocity = Vector3.zero;
                gameObject.SetActive(false);                                                                                                    // 이펙트 생성과 동시에 Bullet은 ActiveFalse
            }
            else if (Stun)                                                                                                                      // 섬광탄 이펙트 생성과 동시에 이펙트 스플래쉬 효과를 위한 Bullet Status 설정
            {
                GameObject effect = GameManager.Instance.pool.Effect_Get(9);
                soundManager.Instance.PlaySoundEffect(Bomb_s);
                effect.GetComponent<Bomb>().Count = this.count;
                effect.transform.position = this.transform.position;
                rigid.velocity = Vector3.zero;
                gameObject.SetActive(false);                                                                                                    // 이펙트 생성과 동시에 Bullet은 ActiveFalse
            }
            else if (Flare_Boom)                                                                                                                // 신호탄 이펙트 생성과 동시에 이펙트 데미지를 위한 Bullet Stats 설정
            {
                GameObject effect = GameManager.Instance.pool.Effect_Get(11);
                effect.GetComponent<Bomb>().Damage = this.Damage;
                effect.transform.position = this.transform.position;
                rigid.velocity = Vector3.zero;
                gameObject.SetActive(false);                                                                                                    // 이펙트 생성과 동시에 Bullet은 ActiveFalse
            }
            else
            {
                rigid.velocity = Vector3.zero;                                                                                                  //다시 쓸수 있기에 초기화
                gameObject.SetActive(false);
            }
        }
    }
    /// <summary>
    /// 화염 방사기의 경우 생성과 동시에 사운드 재생
    /// </summary>
    private void OnEnable()
    {
        if (Flare)
            soundManager.Instance.PlaySoundEffect(Bomb_s);
    }
}
