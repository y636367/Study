using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class Life_Calculation: MonoBehaviour
{
    [System.Serializable]
    public class life_calculation : UnityEvent { };                                                                 // �� �̺�Ʈ ������ ���� �̺�Ʈ Ŭ���� �ν��Ͻ� ����
    public life_calculation onlife_calculation_1 = new life_calculation();
    public life_calculation onlife_calculation_2 = new life_calculation();

    [SerializeField]
    private int MaxLife = 999;                                                                                      // �ִ� ��ū
    private const double IntervalSeconds = 10 * 60f;                                                                // 10�� �������� Life ����

    float pice_time;
    float elapsedTime;
    [SerializeField]
    private Text Timer;

    private DateTime lastGeneratedTime;                                                                             // ���������� ������ �ð�
    bool Timer_On = false;
    bool Max_Life;

    public void SettingLife()
    {
        MaxLife_Ca();
        Remaining_Life();
    }
    /// <summary>
    /// Life Timer �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator Timer_Life()
    {
        float Device_s;

        elapsedTime -= pice_time;                                                                                                   // ���Ҵ� �ð� �ݿ�
        pice_time = 0;

        Device_s = System.DateTime.Now.Second;                                                                                      // ������ ���� �ȹ޵��� ���� �������(����̽�)�÷��� �ð� �޾Ƽ� ��

        while (elapsedTime >= 0)
        {
            if (Device_s != System.DateTime.Now.Second)                                                                             // �� ������ �ð� ���� Ȯ�� ��
            {
                Device_s = System.DateTime.Now.Second;
                elapsedTime -= 0.333333f;                                                                                           // 1�� = 3�� �̱⿡(��? �Ƹ� �ڵ� �����ϸ鼭 ������� ��ġ�� �ð������� �� �ϴ�) 0.333333 �� ���ҷ� �� ���ӿ��� 1�� ����
            }
            int min = Mathf.FloorToInt(elapsedTime / 60);                                                                           // FloorToInt �Ҽ��� ����
            int sec = Mathf.FloorToInt(elapsedTime % 60);

            Timer.text = string.Format("{0:D2}:{1:D2}", min, sec);                                                                  // D2 : 2 �ڸ��� ����

            yield return null;
        }

        elapsedTime = (float)IntervalSeconds;
        Update_Life();
    }
    /// <summary>
    /// Life ���� �� ���� �ð� ���, ���� ���� �Լ�
    /// </summary>
    private void Update_Life()
    {
        Backend_GameData.Instance.GetServer_Synchronous();                                                                           // ���� ���� �б� �ҷ��ͼ�

        Backend_GameData.Instance.Userdatas.Life += 1;                                                                               // Life ����
        Backend_GameData.Instance.UpdateUserDatas_();
        onlife_calculation_2?.Invoke();

        if (Backend_GameData.Instance.Userdatas.Life >= MaxLife)
        {
            Max_Life = true;
        }else
            StartCoroutine(nameof(Timer_Life));

        lastGeneratedTime = Backend_GameData.Instance.Lifedate.LifeDate = Backend_GameData.Instance.Lifedate.UTCDate;                       // ���� Life �����ð� ����� ���� �б������� ����
        Backend_GameData.Instance.UpdateLifeData_();
    }
    /// <summary>
    /// �ð������� ���� Life �������� ���� �ð� ǥ�� �� ���� ����
    /// </summary>
    public void Remaining_Life()
    {
        if (PD_Control.Instance.LevelUP_LifeMax)
        {
            PD_Control.Instance.LevelUP_LifeMax = false;
            Backend_GameData.Instance.Userdatas.Life = MaxLife;                                                                             // MaxLife ��ƴ Life ����
        }

        if (Backend_GameData.Instance.Userdatas.Life >= MaxLife)
        {
            Backend_GameData.Instance.Userdatas.Life = MaxLife;
            Max_Life = true;
            Backend_GameData.Instance.GetServer_Synchronous();                                                                              // ���� �ð� ����ȭ
            lastGeneratedTime = Backend_GameData.Instance.Lifedate.LifeDate = Backend_GameData.Instance.Lifedate.UTCDate;                   // ���� ���� �б��� �ҷ�����

            Timer.gameObject.SetActive(false);
        }
        else
        {
            lastGeneratedTime = Backend_GameData.Instance.Lifedate.LifeDate;
            Backend_GameData.Instance.GetServer_Synchronous();

            TimeSpan Remaining_time = Backend_GameData.Instance.Lifedate.UTCDate - lastGeneratedTime;                                       // ���� �б����� ���� �б����� ���� �帥 �ð� ���
            double totalSeconds = Remaining_time.TotalSeconds;

            if (totalSeconds > IntervalSeconds)                                                                                             // ���͹� �ð� �ʰ��� ��ŭ Life ����
            {
                int plus_life = (int)(totalSeconds / IntervalSeconds);
                Backend_GameData.Instance.Userdatas.Life += plus_life;

                if (Backend_GameData.Instance.Userdatas.Life >= MaxLife)
                {
                    Backend_GameData.Instance.Userdatas.Life = MaxLife;
                    Max_Life = true;

                    Timer.gameObject.SetActive(false);
                }

                Backend_GameData.Instance.UpdateUserDatas_();                                                                               // User Data ����(Life)                                                    
                lastGeneratedTime = Backend_GameData.Instance.Lifedate.LifeDate = Backend_GameData.Instance.Lifedate.UTCDate;               // ������ Life, ������ Life ���� �ð� ���� �б��� ���� ����
                Backend_GameData.Instance.UpdateLifeData_();
            }

            pice_time = (float)(totalSeconds % IntervalSeconds);                                                                            // Life �����ϱ⿣ ���ڶ� �ð� ����
            elapsedTime = (float)IntervalSeconds;

            if (!Max_Life)                                                                                                                  // Life �� �ִ�ġ�� �������� �ʾҴٸ� 
            {
                Timer.gameObject.SetActive(true);
                StartCoroutine(nameof(Timer_Life));                                                                                         // ���� �ڷ�ƾ ����
            }
        }
        onlife_calculation_2?.Invoke();
    }
    /// <summary>
    /// Level�� ���� �ִ� ���� ������ Life ����
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
            Backend_GameData.Instance.Userdatas.Life -= PD_Control.Instance.StageManager_.Life_Delete;                                      // Life �Ҹ� ����
            Backend_GameData.Instance.UpdateUserDatas_();
            Backend_GameData.Instance.GetServer_Synchronous();
            Remaining_Life();
            onlife_calculation_1?.Invoke();                                                                                                 // ��ϵ� �̺�Ʈ ����(�� �̵�)
        }
        else
        {
            Backend_GameData.Instance.Userdatas.Life -= PD_Control.Instance.StageManager_.Life_Delete;                                      // Life �Ҹ� ����
            Backend_GameData.Instance.UpdateUserDatas_();
            onlife_calculation_1?.Invoke();
        }
    }
}
