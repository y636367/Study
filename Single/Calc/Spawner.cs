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

    [Header("��ȯ ����")]
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
        spawnPoint=Nomal_Points.GetComponentsInChildren<Transform>(false);                                       //�ڱ� �ڽŵ� ���ԵǱ⿡ �ε��� ��ȣ 0���� �ڽ��� ����
        BossPoint =Boss_Points.GetComponentsInChildren<Transform>(false);

        Spawn_Time = First;
    }
    private void Update()
    {
        if (!GameManager.Instance.Start_On)                                                                                 // ���� �Ͻ�����, ���, Ŭ���� �� Spawn �ϰ� ������ ���� �ڷ�ƾ�� �ƴ� Update������ ����
            if (GameManager.Instance.Start_ && !GameManager.Instance.Boss_Time)                                             // ������ ���۵ǰ�, ������ Spawn�Ȱ� �ƴ϶��
            {
                timer += Time.deltaTime;                                                                                    // Player�� ������ �ð��� ���� Spawn �ӵ� ����

                if (timer >= Spawn_Time)
                {
                    timer = 0;
                    Boss_s_Spawn();                                                                                         // Middle, Boss Spawn �Լ�
                    Spawn();                                                                                                // ���� Spawn
                }

            }
    }
    /// <summary>
    /// Boss �� MIddleBoss Spawn ���� �Լ�
    /// </summary>
    private void Boss_s_Spawn()
    {
        if (GameManager.Instance.Minute == 0)
            return;
        else                                                                                                                                // ���� �ð��� 1���̻� ��� �ϸ�
        {
            if (GameManager.Instance.Minute % 15 == 0 && Boss_Spawn_Check)                                                                  // 15�� ��� ����, Spawn ���� �����Ͻ�
            {
                GameManager.Instance.Boss_Time = true;
                Boss_spawn();                                                                                                               // Boss Spawn
                Boss_Spawn_Check = false;                                                                                                   // Spawn ���� ����
            }
            else if (GameManager.Instance.Minute % 3 == 0 && GameManager.Instance.Minute % 15 != 0 && MiddleBoss_Spawn_Check)               // 3�� ��� ����, 15�� ����� ��ġ�� �ʰ�, Spawn ���� �����Ͻ�
            {
                GameManager.Instance.Boss_Time = true;
                Middle_Boss_Spawn();                                                                                                        // MiddleBoss Spawn
                MiddleBoss_Spawn_Check = false;                                                                                             // Spawn ���� ����
            }
        }
    }
    /// <summary>
    /// Spawn ����Ʈ���� ������ ���� ��, ���� Spawn Time ����
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
    /// �Ϲ� ���� Spawn �Լ�
    /// </summary>
    private void Spawn()
    {
        int Count = 0;
        int num = 0;
        int status = 0;

        num = Monster_Num(num);                                                                                 // ���� ���� ����
        status = Monster_Status(status);                                                                        // ���� Status ����

        while (Count < Nomal_Spawn_Count && GameManager.Instance.pool.EnemyCount < MaxSpawnCount)               // Spawn ������ ī��Ʈ��ŭ Spawn
        {
            GameObject enemy = GameManager.Instance.pool.Get(Random.Range(0 + num, 3 + num));                   // Ǯ �Ŵ����� ���� ���� �Ҵ�(������ ���� ����)
            enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;                 // Spawn ����Ʈ�� �����ϰ� �Ҵ�
            enemy.GetComponent<Slime>().monster_num = num;
            enemy.GetComponent<Slime>().Boss_Skill = false;                                                     // Boss Skill�� ����������� ���� �Ǵ� ����
            enemy.GetComponent<Slime>().Init(spawnData[status]);                                                // ���� Status �ʱ�ȭ
            enemy.GetComponent<Slime>().default_speed = enemy.GetComponent<Slime>().speed;
            Count++;
        }
    }
    /// <summary>
    /// ������ ���̵��� ���� ������ �ð��� ���� ������ ���� ����
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
                t_num = 0;                                                                              // 1 �ܰ�
            }
            else if (Current_Time < 6.0f * 60f)
            {
                t_num = 4;                                                                              // 2 �ܰ�
            }
            else
            {
                t_num = 8;                                                                              // 3 �ܰ�
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
    /// ������ ���̵��� ���� ������ �ð��� ���� ���� Status ����
    /// </summary>
    /// <param name="t_status"></param>
    /// <returns></returns>
    private int Monster_Status(int t_status)
    {
        int Current_Difficult = PD_Control.Instance.StageManager_.Difficult;
        float Current_Time = GameManager.Instance.GameTime;


        if (Current_Difficult == 0)                                                                 // ���̵� 1
        {
            if (Current_Time < 3.0f * 60f)                                                         // ���� �ð��� ���� Status ����
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
        else if(Current_Difficult == 1)                                                             // ���̵� 2
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
        else if(Current_Difficult == 2)                                                             // ���̵� 3
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
        else                                                                                        // ���̵� ������
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
    /// �߰� ���� Spawn �Լ�
    /// </summary>
    private void Middle_Boss_Spawn()
    {
        int status = 0;

        Boss_Status(status);                                                                                                // Boss Spawn Ư�� ����

        GameObject enemy = GameManager.Instance.pool.Middle_Get(Random.Range(0, 10));                                       // Boss Spawn ����Ʈ �� ���� �� �� ����
        enemy.transform.position = BossPoint[Random.Range(1, BossPoint.Length)].position;
        enemy.GetComponent<Monster>().Init(MIddleBoss_Data[status], Middle_Spawn_Count);                                    // Status �� ���� �ʱ�ȭ
        enemy.GetComponent<Monster>().default_speed = enemy.GetComponent<Monster>().speed;

        Middle_Spawn_Count++;
    }
    /// <summary>
    /// ���� Spawn �Լ�
    /// </summary>
    private void Boss_spawn()
    {
        int status = 0;

        Boss_Status(status);                                                                                                // Boss Spawn Ư�� ����

        GameObject enemy = GameManager.Instance.pool.Final_Get(Random.Range(0, 10));                                        // Boss Spawn ����Ʈ �� ���� �� �� ����
        enemy.transform.position = BossPoint[Random.Range(1, BossPoint.Length)].position;
        enemy.GetComponent<Monster>().Init(Boss_Data[status], Boss_Spawn_Count);                                            // Status �� ���� �ʱ�ȭ
        enemy.GetComponent<Monster>().default_speed = enemy.GetComponent<Monster>().speed;
        enemy.GetComponent<Monster>().Boss_Skill = false;                                                                   // Skill ���� False

        Boss_Spawn_Count++;
    }
    /// <summary>
    /// Spawn �� Boss Status ����
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    private int Boss_Status(int status)
    {
        int Current_Difficult = PD_Control.Instance.StageManager_.Difficult;                                                // ���̵��� ���� Status ����

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
    /// Mushroom�� ���� ��ų�� ���� �Ϲ� �� Spawn �Լ�
    /// </summary>
    public void Spawn_Boss_Skill()
    {
        int Count = 0;
        int num = 0;
        int status = 0;

        num = Monster_Num(num);                                                                                 // ���� ���� ����
        status = Monster_Status(status);                                                                        // ���� Status ����

        while (Count < spawnPoint.Length)                                                                       // Spawn ������ ī��Ʈ��ŭ Spawn
        {
            GameObject enemy = GameManager.Instance.pool.Get(Random.Range(0 + num, 3 + num));                   // Ǯ �Ŵ����� ���� ���� �Ҵ�(������ ���� ����)
            enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;                 // Spawn ����Ʈ�� �����ϰ� �Ҵ�
            enemy.GetComponent<Slime>().monster_num = num;
            enemy.GetComponent<Slime>().Boss_Skill = true;                                                      // Boss Skill�� ����������� ���� �Ǵ� ����
            enemy.GetComponent<Slime>().Init(spawnData[status]);                                                // ���� Status �ʱ�ȭ
            Count++;
        }
    }
    /// <summary>
    /// Red_Slime�� ��ų�� ������ �п�ü ���� �Լ�
    /// </summary>
    /// <param name="PH"></param>
    /// <param name="DP"></param>
    /// <param name="AP"></param>
    public void red_Slime_Skill(float PH, float DP, float AP)
    {
        GameObject enemy = GameManager.Instance.pool.Final_Get(9);                                                          // Ǯ �Ŵ����� ���� �Ҵ�(���� �� ���� ����)
        enemy.transform.position = BossPoint[Random.Range(1, BossPoint.Length)].position;                                   // Boss Spawn ����Ʈ�� ������ �� ����
        enemy.GetComponent<Monster>().Red_Slime_Fake(PH, DP, AP);                                                           // RedSlime�� ���� Status ��� Status ����
        enemy.GetComponent<Monster>().Boss_Skill = true;                                                                    // Skill�� ������ �������� �Ǵ� �Լ�
    }
}
// ����ȭ
[System.Serializable]
public class SpawnData
{
    public float Attack;
    public float Health;
    public float Defensive;

    /// <summary>
    /// Stage�� �ѹ����� ���� �Ϲ� ������ Status �� ��� �Լ�
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
    /// Player�� ������ �ð��� ����Ͽ� �Ϻ� Status �� ���(�Ϲ� ����)
    /// </summary>
    /// <param name="t_data"></param>
    /// <returns></returns>
    public float Time_Upgrade(float t_data)
    {
        return t_data * 1.1f;
    }
    /// <summary>
    /// Player�� ������ �ð��� ����Ͽ� �Ϻ� Status �� ���(����)
    /// </summary>
    /// <param name="t_data"></param>
    /// <returns></returns>
    public float Time_Boss_Upgrade(float t_data)
    {
        return t_data * 1.2f;
    }
    /// <summary>
    /// Player�� �����ð��� ����Ͽ� �߰� ���� �� ������ Status �� ����� ���� ���� �� ���� �Լ�
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