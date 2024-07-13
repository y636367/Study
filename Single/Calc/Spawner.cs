using UnityEngine;

public class Spawner : MonoBehaviour
{
    #region Variable
    [SerializeField]
    private GameObject Nomal_Points;
    [SerializeField]
    private GameObject Boss_Points;

    [SerializeField]
    private Transform[] spawnPoint;

    [SerializeField]
    private Transform[] BossPoint;

    [Header("Nomal")]
    [SerializeField]
    private SpawnData[] spawnData;

    [Header("Middle_Boss")]
    [SerializeField]
    private SpawnData[] MIddleBoss_Data;

    [Header("Boss")]
    [SerializeField]
    private SpawnData[] Boss_Data;

    private float timer;

    [Header("소환 간격")]
    [SerializeField]
    private float First;
    [SerializeField]
    private float Scond;
    [SerializeField]
    private float Third;
    [SerializeField]
    private float Fourth;
    [SerializeField]
    private float Fifth;
    [SerializeField]
    private float Last;

    private float Spawn_Time;

    [SerializeField]
    public int Nomal_Spawn_Count = 0;
    [SerializeField]
    public int Middle_Spawn_Count = 0;
    [SerializeField]
    public int Boss_Spawn_Count = 0;

    public bool MiddleBoss_Spawn_Check = false;
    public bool Boss_Spawn_Check = false;

    [SerializeField]
    private int MaxSpawnCount = 70;
    #endregion
    private void Awake()
    {
        spawnPoint=Nomal_Points.GetComponentsInChildren<Transform>(false);                                       //자기 자신도 포함되기에 인덱스 번호 0번은 자신을 뜻함
        BossPoint =Boss_Points.GetComponentsInChildren<Transform>(false);

        Spawn_Time = First;
    }
    private void Update()
    {
        if (!GameManager.Instance.Start_On)                                                                                 // 게임 일시정지, 사망, 클리어 시 Spawn 일괄 정지를 위해 코루틴이 아닌 Update문으로 갱신
            if (GameManager.Instance.Start_ && !GameManager.Instance.Boss_Time)                                             // 게임이 시작되고, 보스가 Spawn된게 아니라면
            {
                timer += Time.deltaTime;                                                                                    // Player가 생존한 시간에 따라 Spawn 속도 갱신

                if (timer >= Spawn_Time)
                {
                    timer = 0;
                    Boss_s_Spawn();                                                                                         // Middle, Boss Spawn 함수
                    Spawn();                                                                                                // 몬스터 Spawn
                }

            }
    }
    /// <summary>
    /// Boss 및 MIddleBoss Spawn 정리 함수
    /// </summary>
    private void Boss_s_Spawn()
    {
        if (GameManager.Instance.Minute == 0)
            return;
        else                                                                                                                                // 생존 시간이 1분이상 경과 하면
        {
            if (GameManager.Instance.Minute % 15 == 0 && Boss_Spawn_Check)                                                                  // 15분 배수 생존, Spawn 가능 상태일시
            {
                GameManager.Instance.Boss_Time = true;
                Boss_spawn();                                                                                                               // Boss Spawn
                Boss_Spawn_Check = false;                                                                                                   // Spawn 가능 해제
            }
            else if (GameManager.Instance.Minute % 3 == 0 && GameManager.Instance.Minute % 15 != 0 && MiddleBoss_Spawn_Check)               // 3분 배수 생존, 15분 배수와 겹치지 않고, Spawn 가능 상태일시
            {
                GameManager.Instance.Boss_Time = true;
                Middle_Boss_Spawn();                                                                                                        // MiddleBoss Spawn
                MiddleBoss_Spawn_Check = false;                                                                                             // Spawn 가능 해제
            }
        }
    }
    /// <summary>
    /// Spawn 포인트에서 생성될 몬스터 수, 몬스터 Spawn Time 갱신
    /// </summary>
    public void Spawn_Point_Time_Update()
    {
        switch (GameManager.Instance.Minute)
        {
            case int minute when minute < 2:
                Spawn_Time = First;
                Nomal_Spawn_Count = spawnPoint.Length / 4;
                break;
            case int minute when minute < 3:
                Spawn_Time = Scond;
                Nomal_Spawn_Count = (spawnPoint.Length / 4) + 2;
                break;
            case int minute when minute < 5:
                Spawn_Time = Third;
                Nomal_Spawn_Count = spawnPoint.Length / 2;
                break;
            case int minute when minute < 6:
                Spawn_Time = Fourth;
                Nomal_Spawn_Count = (spawnPoint.Length / 2) + 2;
                break;
            case int minute when minute < 8:
                Spawn_Time = Fifth;
                Nomal_Spawn_Count = spawnPoint.Length - 2;
                break;
            case int minute when minute < 10:
                Spawn_Time = Last;
                Nomal_Spawn_Count = spawnPoint.Length;
                break;
        }
    }
    /// <summary>
    /// 일반 몬스터 Spawn 함수
    /// </summary>
    private void Spawn()
    {
        int Count = 0;
        int num = 0;
        int status = 0;

        num = Monster_Num(num);                                                                                 // 몬스터 외형 설정
        status = Monster_Status(status);                                                                        // 몬스터 Status 설정

        while (Count < Nomal_Spawn_Count && GameManager.Instance.pool.EnemyCount < MaxSpawnCount)               // Spawn 가능한 카운트만큼 Spawn
        {
            GameObject enemy = GameManager.Instance.pool.Get(Random.Range(0 + num, 3 + num));                   // 풀 매니저에 의한 몬스터 할당(없으면 새로 생성)
            enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;                 // Spawn 포인트에 랜덤하게 할당
            enemy.GetComponent<Slime>().monster_num = num;
            enemy.GetComponent<Slime>().Boss_Skill = false;                                                     // Boss Skill로 생성됬는지에 대한 판단 변수
            enemy.GetComponent<Slime>().Init(spawnData[status]);                                                // 몬스터 Status 초기화
            enemy.GetComponent<Slime>().default_speed = enemy.GetComponent<Slime>().speed;
            Count++;
        }
    }
    /// <summary>
    /// 게임의 난이도와 현재 생존한 시간에 따른 몬스터의 종류 설정
    /// </summary>
    /// <param name="t_num"></param>
    /// <returns></returns>
    private int Monster_Num(int t_num)
    {
        int Current_Difficult = PD_Control.Instance.StageManager_.Difficult;
        float Current_Time = GameManager.Instance.GameTime;


        if (Current_Difficult == 0 || Current_Difficult == 1)
        {
            if (Current_Time < 3.0f * 60f)
            {
                t_num = 0;                                                                              // 1 단계
            }
            else if (Current_Time < 6.0f * 60f)
            {
                t_num = 4;                                                                              // 2 단계
            }
            else
            {
                t_num = 8;                                                                              // 3 단계
            }
        }
        else
        {
            if (Current_Time < 6.0f * 60f)
            {
                t_num = 0;
            }
            else if (Current_Time < 15.0f * 60f)
            {
                t_num = 4;
            }
            else
            {
                t_num = 8;
            }
        }
        return t_num;
    }
    /// <summary>
    /// 게임의 난이도와 현재 생존한 시간에 따른 몬스터 Status 설정
    /// </summary>
    /// <param name="t_status"></param>
    /// <returns></returns>
    private int Monster_Status(int t_status)
    {
        int Current_Difficult = PD_Control.Instance.StageManager_.Difficult;
        float Current_Time = GameManager.Instance.GameTime;


        if (Current_Difficult == 0)                                                                 // 난이도 1
        {
            if (Current_Time < 3.0f * 60f)                                                         // 생존 시간에 따른 Status 설정
            {
                t_status = 0;
            }
            else if (Current_Time < 6.0f * 60f)
            {
                t_status = 4;
            }
            else
            {
                t_status = 8;
            }
        }
        else if(Current_Difficult == 1)                                                             // 난이도 2
        {
            if (Current_Time < 3.0f * 60f)
            {
                t_status = 1;
            }
            else if (Current_Time < 6.0f * 60f)
            {
                t_status = 5;
            }
            else
            {
                t_status = 9;
            }
        }
        else if(Current_Difficult == 2)                                                             // 난이도 3
        {
            if (Current_Time < 6.0f * 60f)
            {
                t_status = 2;
            }
            else if (Current_Time < 15.0f * 60f)
            {
                t_status = 6;
            }
            else
            {
                t_status = 10;
            }
        }
        else                                                                                        // 난이도 무제한
        {
            if (Current_Time < 6.0f * 60f)
            {
                t_status = 3;
            }
            else if (Current_Time < 15.0f * 60f)
            {
                t_status = 7;
            }
            else
            {
                t_status = 11;
            }
        }
        return t_status;
    }
    /// <summary>
    /// 중간 보스 Spawn 함수
    /// </summary>
    private void Middle_Boss_Spawn()
    {
        int status = 0;

        Boss_Status(status);                                                                                                // Boss Spawn 특성 설정

        GameObject enemy = GameManager.Instance.pool.Middle_Get(Random.Range(0, 10));                                       // Boss Spawn 포인트 중 랜덤 한 곳 설정
        enemy.transform.position = BossPoint[Random.Range(1, BossPoint.Length)].position;
        enemy.GetComponent<Monster>().Init(MIddleBoss_Data[status], Middle_Spawn_Count);                                    // Status 및 설정 초기화
        enemy.GetComponent<Monster>().default_speed = enemy.GetComponent<Monster>().speed;

        Middle_Spawn_Count++;
    }
    /// <summary>
    /// 보스 Spawn 함수
    /// </summary>
    private void Boss_spawn()
    {
        int status = 0;

        Boss_Status(status);                                                                                                // Boss Spawn 특성 설정

        GameObject enemy = GameManager.Instance.pool.Final_Get(Random.Range(0, 10));                                        // Boss Spawn 포인트 중 랜덤 한 곳 설정
        enemy.transform.position = BossPoint[Random.Range(1, BossPoint.Length)].position;
        enemy.GetComponent<Monster>().Init(Boss_Data[status], Boss_Spawn_Count);                                            // Status 및 설정 초기화
        enemy.GetComponent<Monster>().default_speed = enemy.GetComponent<Monster>().speed;
        enemy.GetComponent<Monster>().Boss_Skill = false;                                                                   // Skill 상태 False

        Boss_Spawn_Count++;
    }
    /// <summary>
    /// Spawn 될 Boss Status 설정
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    private int Boss_Status(int status)
    {
        int Current_Difficult = PD_Control.Instance.StageManager_.Difficult;                                                // 난이도에 따른 Status 설정

        switch (Current_Difficult)
        {
            case 0:
                status = 0;
                break;
            case 1:
                status = 1;
                break;
            case 2:
                status = 2;
                break;
            default:
                status = 3;
                break;
        }
        return status;
    }
    /// <summary>
    /// Mushroom의 보스 스킬에 의한 일반 몹 Spawn 함수
    /// </summary>
    public void Spawn_Boss_Skill()
    {
        int Count = 0;
        int num = 0;
        int status = 0;

        num = Monster_Num(num);                                                                                 // 몬스터 외형 설정
        status = Monster_Status(status);                                                                        // 몬스터 Status 설정

        while (Count < spawnPoint.Length)                                                                       // Spawn 가능한 카운트만큼 Spawn
        {
            GameObject enemy = GameManager.Instance.pool.Get(Random.Range(0 + num, 3 + num));                   // 풀 매니저에 의한 몬스터 할당(없으면 새로 생성)
            enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;                 // Spawn 포인트에 랜덤하게 할당
            enemy.GetComponent<Slime>().monster_num = num;
            enemy.GetComponent<Slime>().Boss_Skill = true;                                                      // Boss Skill로 생성됬는지에 대한 판단 변수
            enemy.GetComponent<Slime>().Init(spawnData[status]);                                                // 몬스터 Status 초기화
            Count++;
        }
    }
    /// <summary>
    /// Red_Slime의 스킬로 생성된 분열체 생성 함수
    /// </summary>
    /// <param name="PH"></param>
    /// <param name="DP"></param>
    /// <param name="AP"></param>
    public void red_Slime_Skill(float PH, float DP, float AP)
    {
        GameObject enemy = GameManager.Instance.pool.Final_Get(9);                                                          // 풀 매니저에 몬스터 할당(없을 시 새로 생성)
        enemy.transform.position = BossPoint[Random.Range(1, BossPoint.Length)].position;                                   // Boss Spawn 포인트중 랜덤한 곳 설정
        enemy.GetComponent<Monster>().Red_Slime_Fake(PH, DP, AP);                                                           // RedSlime의 현재 Status 기반 Status 설정
        enemy.GetComponent<Monster>().Boss_Skill = true;                                                                    // Skill로 생성된 몬스터인지 판단 함수
    }
}
// 직렬화
[System.Serializable]
public class SpawnData
{
    public float Attack;
    public float Health;
    public float Defensive;

    /// <summary>
    /// Stage의 넘버링에 따른 일반 몬스터의 Status 곱 계산 함수
    /// </summary>
    /// <param name="t_data"></param>
    /// <returns></returns>
    public float Stage_Multiple(float t_data)
    {
        int Count = 0;

        while (Count < PD_Control.Instance.StageManager_.Stage_num)
        {
            t_data *= 1.5f;
            Count += 1;
        }
        return t_data;
    }
    /// <summary>
    /// Player가 생존한 시간에 비례하여 일부 Status 곱 계산(일반 몬스터)
    /// </summary>
    /// <param name="t_data"></param>
    /// <returns></returns>
    public float Time_Upgrade(float t_data)
    {
        return t_data * 1.1f;
    }
    /// <summary>
    /// Player가 생존한 시간에 비례하여 일부 Status 곱 계산(보스)
    /// </summary>
    /// <param name="t_data"></param>
    /// <returns></returns>
    public float Time_Boss_Upgrade(float t_data)
    {
        return t_data * 1.2f;
    }
    /// <summary>
    /// Player의 생존시간에 비례하여 중간 보스 및 보스의 Status 곱 계산을 위한 변수 값 설정 함수
    /// </summary>
    public void Upgarde_Count_Reset()
    {
        if (PD_Control.Instance.StageManager_.Difficult == 0 || PD_Control.Instance.StageManager_.Difficult == 1)
        {
            if (GameManager.Instance.Minute == 3 || GameManager.Instance.Minute == 6 || GameManager.Instance.Minute == 9)
            {
                GameManager.Instance.Upgrade_Count = 0;
            }
        }
        else
        {
            if (GameManager.Instance.Minute == 6 || GameManager.Instance.Minute == 15 || GameManager.Instance.Minute == 24)
            {
                GameManager.Instance.Upgrade_Count = 0;
            }
        }
    }
}