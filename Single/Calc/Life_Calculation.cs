using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class Life_Calculation: MonoBehaviour
{
    [System.Serializable]
    public class life_calculation : UnityEvent { };                                                                 // 후 이벤트 진행을 위한 이벤트 클래스 인스턴스 생성
    public life_calculation onlife_calculation_1 = new life_calculation();
    public life_calculation onlife_calculation_2 = new life_calculation();

    [SerializeField]
    private int MaxLife = 999;                                                                                      // 최대 토큰
    private const double IntervalSeconds = 10 * 60f;                                                                // 10분 간격으로 Life 생성

    float pice_time;
    float elapsedTime;
    [SerializeField]
    private Text Timer;

    private DateTime lastGeneratedTime;                                                                             // 마지막으로 생성된 시간
    bool Timer_On = false;
    bool Max_Life;

    public void SettingLife()
    {
        MaxLife_Ca();
        Remaining_Life();
    }
    /// <summary>
    /// Life Timer 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator Timer_Life()
    {
        float Device_s;

        elapsedTime -= pice_time;                                                                                                   // 남았던 시간 반영
        pice_time = 0;

        Device_s = System.DateTime.Now.Second;                                                                                      // 프레임 영향 안받도록 현재 재생중인(디바이스)플랫폼 시간 받아서 비교

        while (elapsedTime >= 0)
        {
            if (Device_s != System.DateTime.Now.Second)                                                                             // 초 단위로 시간 변경 확인 시
            {
                Device_s = System.DateTime.Now.Second;
                elapsedTime -= 0.333333f;                                                                                           // 1초 = 3초 이기에(왜? 아마 코드 진행하면서 어느정도 놓치는 시간때문인 듯 하다) 0.333333 씩 감소로 인 게임에서 1초 재현
            }
            int min = Mathf.FloorToInt(elapsedTime / 60);                                                                           // FloorToInt 소수점 버림
            int sec = Mathf.FloorToInt(elapsedTime % 60);

            Timer.text = string.Format("{0:D2}:{1:D2}", min, sec);                                                                  // D2 : 2 자리로 고정

            yield return null;
        }

        elapsedTime = (float)IntervalSeconds;
        Update_Life();
    }
    /// <summary>
    /// Life 생성 및 서버 시간 등록, 갱신 진행 함수
    /// </summary>
    private void Update_Life()
    {
        Backend_GameData.Instance.GetServer_Synchronous();                                                                           // 현재 시점 분기 불러와서

        Backend_GameData.Instance.Userdatas.Life += 1;                                                                               // Life 갱신
        Backend_GameData.Instance.UpdateUserDatas_();
        onlife_calculation_2?.Invoke();

        if (Backend_GameData.Instance.Userdatas.Life >= MaxLife)
        {
            Max_Life = true;
        }else
            StartCoroutine(nameof(Timer_Life));

        lastGeneratedTime = Backend_GameData.Instance.Lifedate.LifeDate = Backend_GameData.Instance.Lifedate.UTCDate;                       // 다음 Life 생성시간 계산을 위한 분기점으로 저장
        Backend_GameData.Instance.UpdateLifeData_();
    }
    /// <summary>
    /// 시각적으로 다음 Life 생성까지 남은 시간 표출 및 관련 수식
    /// </summary>
    public void Remaining_Life()
    {
        if (PD_Control.Instance.LevelUP_LifeMax)
        {
            PD_Control.Instance.LevelUP_LifeMax = false;
            Backend_GameData.Instance.Userdatas.Life = MaxLife;                                                                             // MaxLife 만틈 Life 충전
        }

        if (Backend_GameData.Instance.Userdatas.Life >= MaxLife)
        {
            Backend_GameData.Instance.Userdatas.Life = MaxLife;
            Max_Life = true;
            Backend_GameData.Instance.GetServer_Synchronous();                                                                              // 서버 시간 동기화
            lastGeneratedTime = Backend_GameData.Instance.Lifedate.LifeDate = Backend_GameData.Instance.Lifedate.UTCDate;                   // 지난 생성 분기점 불러오기

            Timer.gameObject.SetActive(false);
        }
        else
        {
            lastGeneratedTime = Backend_GameData.Instance.Lifedate.LifeDate;
            Backend_GameData.Instance.GetServer_Synchronous();

            TimeSpan Remaining_time = Backend_GameData.Instance.Lifedate.UTCDate - lastGeneratedTime;                                       // 지난 분기점과 현재 분기점을 통해 흐른 시간 계산
            double totalSeconds = Remaining_time.TotalSeconds;

            if (totalSeconds > IntervalSeconds)                                                                                             // 인터벌 시간 초과한 만큼 Life 생성
            {
                int plus_life = (int)(totalSeconds / IntervalSeconds);
                Backend_GameData.Instance.Userdatas.Life += plus_life;

                if (Backend_GameData.Instance.Userdatas.Life >= MaxLife)
                {
                    Backend_GameData.Instance.Userdatas.Life = MaxLife;
                    Max_Life = true;

                    Timer.gameObject.SetActive(false);
                }

                Backend_GameData.Instance.UpdateUserDatas_();                                                                               // User Data 갱신(Life)                                                    
                lastGeneratedTime = Backend_GameData.Instance.Lifedate.LifeDate = Backend_GameData.Instance.Lifedate.UTCDate;               // 생성된 Life, 마지막 Life 생성 시간 기점 분기점 새로 저장
                Backend_GameData.Instance.UpdateLifeData_();
            }

            pice_time = (float)(totalSeconds % IntervalSeconds);                                                                            // Life 생성하기엔 모자란 시간 저장
            elapsedTime = (float)IntervalSeconds;

            if (!Max_Life)                                                                                                                  // Life 가 최대치에 도달하지 않았다면 
            {
                Timer.gameObject.SetActive(true);
                StartCoroutine(nameof(Timer_Life));                                                                                         // 생성 코루틴 실행
            }
        }
        onlife_calculation_2?.Invoke();
    }
    /// <summary>
    /// Level에 따른 최대 소지 가능한 Life 제한
    /// </summary>
    private void MaxLife_Ca()
    {
        switch (Backend_GameData.Instance.Userdatas.Level)
        {
            case int level when level >= 1 && level <= 10:
                MaxLife = 30;
                break;
            case int level when level >= 11 && level <= 30:
                MaxLife = 40;
                break;
            case int level when level >= 31 && level <= 50:
                MaxLife = 50;
                break;
            case int level when level >= 51:
                MaxLife = 60;
                break;
        }
    }
    public void Use_the_Life()
    {
        if (Max_Life)
        {
            Max_Life = false;
            Backend_GameData.Instance.Userdatas.Life -= PD_Control.Instance.StageManager_.Life_Delete;                                      // Life 소모 갱신
            Backend_GameData.Instance.UpdateUserDatas_();
            Backend_GameData.Instance.GetServer_Synchronous();
            Remaining_Life();
            onlife_calculation_1?.Invoke();                                                                                                 // 등록된 이벤트 이행(씬 이동)
        }
        else
        {
            Backend_GameData.Instance.Userdatas.Life -= PD_Control.Instance.StageManager_.Life_Delete;                                      // Life 소모 갱신
            Backend_GameData.Instance.UpdateUserDatas_();
            onlife_calculation_1?.Invoke();
        }
    }
}
