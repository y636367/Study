using System;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class After_ : UnityEvent { };                                                       // �̺�Ʈ ������ ���� �ν��Ͻ� Ŭ���� ����
    public After_ after_Timer = new After_();
    public After_ after_Exp = new After_();

    #region Variable
    public static GameManager Instance;
    public PoolManager pool;
    public Player player;
    public Spawner spawner;
    public DropManager drop;
    public SpawnData spawnData;
    public WeaponManager weaponManager;
    public Char_Manager charManager;
    public Level_Up uilevelup;
    public Particle_Control particlecontrol;
    public UIManager uimanager;
    public Item_Chart w_chart;
    public Item_Chart c_chart;
    public Tresure_UI tresureui;
    public Get_Wall walls;
    public Button_Manager buttonmanager;

    public float GameTime;
    public float MaxGameTime;                                                                           // �� * ��
    public bool Start_ = true;                                                                          // �Ѱ� ���� ���� ���
    public bool Infinity_Check = false;                                                                 // ���Ѹ������ Ȯ��

    public bool Boss_Time = false;
    public bool Clear = false;
    public bool Start_On = true;

    [SerializeField]
    public float Minute_Check = 60f;
    public int Minute = 0;
    public int Upgrade_Count = 0;

    [Header("Player_Infomation")]
    [SerializeField]
    public int current_Coin;
    [SerializeField]
    public int current_Kill;
    [SerializeField]
    public int current_Exp;
    [SerializeField]
    public int current_Level;
    [SerializeField]
    public float Total_Exp;
    [SerializeField]
    public float Require_Exp;

    [SerializeField]
    public int Max_Level = 72;

    [SerializeField]
    public bool Resurrection = false;
    public bool Player_Dead = false;

    [SerializeField]
    public float Score;
    public float Get_Exp_Player;
    #endregion
    private void Awake()
    {
        Instance = this;
        PD_Control.Instance.StageManager_.Get_Time();                                                               // ���̵��� ���� ���� �ð� �ʱ�ȭ

        current_Level = 1;                                                                                          // �÷��̾� �ΰ��� life, ����ġ �ʱ�ȭ
        Require_Exp = Mathf.Floor(9.3f * Mathf.Pow(current_Level + 1, 2.7f));
    }
    /// <summary>
    /// Player�� ������ ���� ����
    /// </summary>
    public void Setting_() 
    {
        uilevelup.Select(Backend_GameData.Instance.Userdatas.WeaponNumber);

        after_Timer?.Invoke();
        after_Exp?.Invoke();
    }
    private void Update()
    {
        if(!Start_On)
            if (Start_ && !Boss_Time)
            {
                GameTime += Time.deltaTime;
                Minute_Check -= Time.deltaTime;

                if (Minute_Check <= 0f)
                {
                    Minute++;
                    spawner.Spawn_Point_Time_Update();
                    Minute_Check = 60f;
                    Upgrade_Count++;
                    spawnData.Upgarde_Count_Reset();
                    spawner.MiddleBoss_Spawn_Check = true;
                    spawner.Boss_Spawn_Check = true;
                }
                Timer_HUD();                                                                // Timer HUD Update
                Clear_Condition();                                                          // Clear Check
            }
    }
    public void Timer_HUD()
    {
        after_Timer?.Invoke();
    }
    /// <summary>
    /// Coin ������ ȹ�� �� ���� �Լ�
    /// </summary>
    /// <param name="Value"></param>
    public void Get_Coin(int Value)
    {
        current_Coin += Value;

        UIManager.instance.CurrentCoin.Update_HUD();                                // Coin Hud UPdate
    }
    /// <summary>
    /// Exp ������ ȹ�� �� ���� �Լ�
    /// </summary>
    /// <param name="Value"></param>
    public void Get_Exp(int Value)
    {
        Total_Exp = current_Exp += Value;

        if (Require_Exp <= current_Exp)
        {
            current_Exp -= (int)Require_Exp;
            Level_Up();
            Require_Exp = Mathf.Floor(9.3f * Mathf.Pow(current_Level + 1, 2.7f));
        }
        after_Exp?.Invoke();
    }
    /// <summary>
    /// LevelUp���� ���� �г� �� �� ��ó�� �Լ�
    /// </summary>
    public void Level_Up()
    {
        if (current_Level < Max_Level)
        {
            current_Level += 1;
            uilevelup.Show();
        }
    }
    /// <summary>
    /// �̺�Ʈ �Լ� ��� �� LevelUP�� �Ͻ� ����
    /// </summary>
    public void Stop()
    {
        Start_ = false;
        Time.timeScale = 0;
        particlecontrol.Effect_Pause();
    }
    /// <summary>
    /// �̺�Ʈ �Լ� ��� �� LevelUP�� �Լ�
    /// </summary>
    public void Resume()
    {
        Start_ = true;
        Time.timeScale = 1;
        player.Resume_Animation();
        particlecontrol.Particles_Resume();
    }
    /// <summary>
    /// ���� ȹ������ ���� Time.timeScale�� ������ ������ ���� �Լ�
    /// </summary>
    public void Stop_2()
    {
        Start_ = false;
        particlecontrol.Effect_Pause();
    }
    /// <summary>
    /// ���� ȹ������ ���� Time.timeScale�� ������ ������ ���
    /// </summary>
    public void Resume_2()
    {
        Start_ = true;
        player.Resume_Animation();
        particlecontrol.Particles_Resume();
    }
    /// <summary>
    /// Player ������� ���� ���� ���� �Լ�
    /// </summary>
    public void Game_End()
    {
        Start_ = false;                                                                                                 // ����Ͽ��⿡ ��� ����
        Reset_Temp_DataUPdate();
        Backend_GameData.Instance.Userdatas.Coin += current_Coin;
        Score_Calculation();
        uimanager.Score_Panel_on();
    }
    /// <summary>
    /// Clear�� ���� ���� �Լ�
    /// </summary>
    private void Clear_Condition()
    {
        if (!Infinity_Check)
            if (GameTime > MaxGameTime && MaxGameTime != 0)
            {
                GameTime = MaxGameTime;
                Start_ = false; // Ŭ���� �Ͽ��⿡ ��� �Ͻ� ����
                Reset_Temp_DataUPdate();
                Clear = true;
                Backend_GameData.Instance.Cleardatas.High_Stage = PD_Control.Instance.StageManager_.Stage_num;        // �������� Ŭ���� ���� �ݿ�
                Backend_GameData.Instance.Userdatas.Coin += current_Coin;
                Score_Calculation();
                uimanager.Score_Panel_on();
            }
    }
    /// <summary>
    /// ���� ���� �� Socre ����� ���� �Լ�
    /// </summary>
    private void Score_Calculation()
    {
        Score = PD_Control.Instance.StageManager_.Result_Score() * 0.01f;
        Get_Exp_Player = (float)Math.Truncate(Score * 1000) / 2;                                                            // �Ҽ��� ��° �ڸ� ���� ����

        Backend_GameData.Instance.Userdatas.NowExp += Get_Exp_Player;
    }
    /// <summary>
    /// �ΰ��ӿ��� ����� ��ġ ��ȯ
    /// </summary>
    public void Reset_Temp_DataUPdate()
    {
        Backend_GameData.Instance.Userstatusdatas.Attack_Power -= player.t_Attack_power;
        Backend_GameData.Instance.Userstatusdatas.AttackSpeed -= player.t_Attack_speed;
    }
}
