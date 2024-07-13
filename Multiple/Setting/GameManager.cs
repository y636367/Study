using System;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class After_ : UnityEvent { };                                                       // 이벤트 적용을 위한 인스턴스 클래스 생성
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
    public float MaxGameTime;                                                                           // 분 * 초
    public bool Start_ = true;                                                                          // 총괄 진행 루프 담당
    public bool Infinity_Check = false;                                                                 // 무한모드인지 확인

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
        PD_Control.Instance.StageManager_.Get_Time();                                                               // 난이도에 따른 게임 시간 초기화

        current_Level = 1;                                                                                          // 플레이어 인게임 life, 경험치 초기화
        Require_Exp = Mathf.Floor(9.3f * Mathf.Pow(current_Level + 1, 2.7f));
    }
    /// <summary>
    /// Player가 선택한 무기 스폰
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
    /// Coin 아이템 획득 시 실행 함수
    /// </summary>
    /// <param name="Value"></param>
    public void Get_Coin(int Value)
    {
        current_Coin += Value;

        UIManager.instance.CurrentCoin.Update_HUD();                                // Coin Hud UPdate
    }
    /// <summary>
    /// Exp 아이템 획득 시 실행 함수
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
    /// LevelUp으로 인한 패널 온 및 후처리 함수
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
    /// 이벤트 함수 사용 및 LevelUP시 일시 정지
    /// </summary>
    public void Stop()
    {
        Start_ = false;
        Time.timeScale = 0;
        particlecontrol.Effect_Pause();
    }
    /// <summary>
    /// 이벤트 함수 사용 및 LevelUP후 함수
    /// </summary>
    public void Resume()
    {
        Start_ = true;
        Time.timeScale = 1;
        player.Resume_Animation();
        particlecontrol.Particles_Resume();
    }
    /// <summary>
    /// 보물 획득으로 인한 Time.timeScale을 제외한 나머지 정지 함수
    /// </summary>
    public void Stop_2()
    {
        Start_ = false;
        particlecontrol.Effect_Pause();
    }
    /// <summary>
    /// 보물 획득으로 인한 Time.timeScale을 제외한 나머지 재게
    /// </summary>
    public void Resume_2()
    {
        Start_ = true;
        player.Resume_Animation();
        particlecontrol.Particles_Resume();
    }
    /// <summary>
    /// Player 사망으로 인한 게임 종료 함수
    /// </summary>
    public void Game_End()
    {
        Start_ = false;                                                                                                 // 사망하였기에 모두 정지
        Reset_Temp_DataUPdate();
        Backend_GameData.Instance.Userdatas.Coin += current_Coin;
        Score_Calculation();
        uimanager.Score_Panel_on();
    }
    /// <summary>
    /// Clear로 인한 갱신 함수
    /// </summary>
    private void Clear_Condition()
    {
        if (!Infinity_Check)
            if (GameTime > MaxGameTime && MaxGameTime != 0)
            {
                GameTime = MaxGameTime;
                Start_ = false; // 클리어 하였기에 모두 일시 정지
                Reset_Temp_DataUPdate();
                Clear = true;
                Backend_GameData.Instance.Cleardatas.High_Stage = PD_Control.Instance.StageManager_.Stage_num;        // 스테이지 클리어 정보 반영
                Backend_GameData.Instance.Userdatas.Coin += current_Coin;
                Score_Calculation();
                uimanager.Score_Panel_on();
            }
    }
    /// <summary>
    /// 게임 종료 후 Socre 계산을 위한 함수
    /// </summary>
    private void Score_Calculation()
    {
        Score = PD_Control.Instance.StageManager_.Result_Score() * 0.01f;
        Get_Exp_Player = (float)Math.Truncate(Score * 1000) / 2;                                                            // 소수점 셋째 자리 이하 버림

        Backend_GameData.Instance.Userdatas.NowExp += Get_Exp_Player;
    }
    /// <summary>
    /// 인게임에서 상승한 수치 반환
    /// </summary>
    public void Reset_Temp_DataUPdate()
    {
        Backend_GameData.Instance.Userstatusdatas.Attack_Power -= player.t_Attack_power;
        Backend_GameData.Instance.Userstatusdatas.AttackSpeed -= player.t_Attack_speed;
    }
}
