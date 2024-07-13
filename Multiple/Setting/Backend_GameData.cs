using UnityEngine;
using BackEnd;
using System;
using UnityEngine.Events;
public class Backend_GameData
{
    [System.Serializable]
    public class GameDataLoadEvent : UnityEvent { }
    public GameDataLoadEvent onGameDataLoadEvent = new GameDataLoadEvent();                         // Data ����, ����, �ε� �� �̺�Ʈ ������ ���� Event ���� ����

    private static Backend_GameData instance = null;
    public static Backend_GameData Instance                                                         // ����Ÿ���� get ������Ƽ
    {
        get
        {
            if(instance == null)
            {
                instance = new Backend_GameData();
            }

            return instance;
        }
    }
    #region Game �÷��̿� �ʿ��� ���� ������ ������ ���� �� ����, �������� ���� Ŭ���� ����
    /// <summary>
    /// Level, Coin, Life, NowExp
    /// </summary>
    private User_Datas userdatas = new User_Datas();
    public User_Datas Userdatas => userdatas;

    /// <summary>
    /// Progress
    /// </summary>
    private Clear_datas cleardatas = new Clear_datas();
    public Clear_datas Cleardatas => cleardatas;

    /// <summary>
    /// Status_value
    /// </summary>
    private User_status userstatusdatas = new User_status();
    public User_status Userstatusdatas => userstatusdatas;

    /// <summary>
    /// Status_Level
    /// </summary>
    private User_statusLevel userstatuslevel = new User_statusLevel();
    public User_statusLevel Userstatuslevel => userstatuslevel;

    /// <summary>
    /// Weapon_Level
    /// </summary>
    private User_WeaponLevel userweaponlevel = new User_WeaponLevel();
    public User_WeaponLevel Userweaponlevel => userweaponlevel;
    /// <summary>
    /// Life_Date
    /// </summary>
    private Life_Date lifedate = new Life_Date();
    public Life_Date Lifedate => lifedate;
    #endregion

    #region �ҷ��� ���� ������ ���� �� ������ ���� string ����
    private string userDataRowInDate = string.Empty;           
    private string statusDataRowInDate = string.Empty;
    private string statusLVDataRowInDate = string.Empty;
    private string WeaponLVDataRowInDate = string.Empty;
    private string ClearDataRowInDate = string.Empty;
    private string DateRowInDate = string.Empty;
    #endregion

    #region �ҷ��Դ��� Ȯ���� ���� üũ ����
    public bool On_userdata = false;
    public bool On_statusdata = false;
    public bool On_statusLvdata = false;
    public bool On_weapondata = false;
    public bool On_cleardata = false;
    public bool On_lifedate = false;
    #endregion

    /// <summary>
    /// �ڳ� �ܼ� ���̺� ���ο� ���� ���� �߰�
    /// </summary>
    public void GameDataInsert()
    {
        Insert_UserData();
        Insert_UserStatus();
        Insert_UserStatusLevel();
        Insert_UserWeaponLevel();
        Insert_UserProgress();
        InsertLifeData_();
    }
    /// <summary>
    /// ���� ������ ����
    /// </summary>
    public void Insert_UserData()
    {
        Userdatas.ResetData();                                                                      // ������ �ʱ�ȭ

        Param param_d = new Param()                                                                 // �ʱ�ȭ ��(�⺻��) ������ ���̺� ����
       {
            {"Level", Userdatas.Level},
            {"Coin", Userdatas.Coin},
            {"Life", Userdatas.Life},
            {"Nowexp", Userdatas.NowExp},
            {"Next_Exp_value", Userdatas.Next_Exp_value},
            {"Next_Exp_UP", Userdatas.Next_Exp_UP},
            {"Exp_Multiplication", Userdatas.Exp_Multiplication},
            {"WeaponNumber",Userdatas.WeaponNumber},
       };

        SendQueue.Enqueue(Backend.GameData.Insert, "Player_Data", param_d, callback =>              // �ڳ� ������ ������ ����
        {
            if (callback.IsSuccess())                                                               // ���� ��
            {
                userDataRowInDate = callback.GetInDate();                                           // ���� �� ����
#if UNITY_EDITOR
                Debug.Log($"User_Data ���� ���� : {callback}");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError($"User_Data ���� ���� : {callback}");
#endif
                return;
            }
        });
    }
    /// <summary>
    /// �������ͽ� ��ġ �� ����
    /// </summary>
    private void Insert_UserStatus()
    {
        Userstatusdatas.ResetStatus();                                                              // ������ �ʱ�ȭ

        Param param_s_ = new Param()                                                                // �ʱ�ȭ ��(�⺻��) ������ ���̺� ����
        {
            {"Max_Hp", Userstatusdatas.Max_HP},
            {"Attack_Power", Userstatusdatas.Attack_Power},
            {"Deffensive_Power", Userstatusdatas.Deffensive_Power},
            {"MoveSpeed", Userstatusdatas.MoveSpeed},
            {"AttackSpeed", Userstatusdatas.AttackSpeed}
        };

        SendQueue.Enqueue(Backend.GameData.Insert,"Status_", param_s_, callback =>                  // �ڳ� ������ ������ ����
        {
            if (callback.IsSuccess())                                                               // ���� ��
            {
                statusDataRowInDate = callback.GetInDate();                                         // ���� �� ����
#if UNITY_EDITOR
                Debug.Log($"Status_Data ���� ���� : {callback}");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError($"Status_Data ���� ���� : {callback}");
#endif
                return;
            }
        });
    }
    /// <summary>
    /// �������ͽ� level �� ����
    /// </summary>
    private void Insert_UserStatusLevel()
    {
        Userstatuslevel.ResetStatusLevel();                                                         // ������ �ʱ�ȭ

        Param param_s = new Param()                                                                 // �ʱ�ȭ ��(�⺻��) ������ ���̺� ����
        {
            {"AttackPower", Userstatuslevel.AP},
            {"DeffensivePower", Userstatuslevel.DP},
            {"PhysicalPower", Userstatuslevel.PH},
            {"AttackSpeed", Userstatuslevel.AS},
            {"MoveSpeed", Userstatuslevel.MS}
        };

        SendQueue.Enqueue(Backend.GameData.Insert, "Upgrade_Status", param_s, callback =>           // �ڳ� ������ ������ ����
        {
            if (callback.IsSuccess())                                                               // ���� ��
            {
                statusLVDataRowInDate = callback.GetInDate();                                       // ���� �� ����
#if UNITY_EDITOR
                Debug.Log($"Upgrade_Status ���� ���� : {callback}");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError($"Upgrade_Status ���� ���� : {callback}");
#endif
                return;
            }
        });
    }
    /// <summary>
    /// ���� level �� ����
    /// </summary>
    private void Insert_UserWeaponLevel()
    {
        Userweaponlevel.ResetWeapons();                                                             // ������ �ʱ�ȭ

        Param param_ws = new Param()                                                                // �ʱ�ȭ ��(�⺻��) ������ ���̺� ����
        {
            {"Pistol", Userweaponlevel.Pistol},
            {"ShotGun", Userweaponlevel.Shotgun},
            {"Mine", Userweaponlevel.Mine},
            {"SubMachinGun", Userweaponlevel.Rampage},
            {"Sniper", Userweaponlevel.Sniper},
            {"FlareGun", Userweaponlevel.Flare_gun},
            {"FireThrower", Userweaponlevel.FlareThrower},
            {"Knife", Userweaponlevel.Knife},
            {"GasShield", Userweaponlevel.GasShield},
            {"RocketLauncher", Userweaponlevel.RocketLauncer}
        };

        SendQueue.Enqueue(Backend.GameData.Insert, "Weapon_Status", param_ws, callback =>           // �ڳ� ������ ������ ����
        {
            if (callback.IsSuccess())                                                               // ���� ��
            {
                WeaponLVDataRowInDate = callback.GetInDate();                                       // ���� �� ����
#if UNITY_EDITOR
                Debug.Log($"Weapon_Status ���� ���� : {callback}");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError($"Weapon_Status ���� ���� : {callback}");
#endif
                return;
            }
        });
    }
    /// <summary>
    /// ���൵ �� ����
    /// </summary>
    private void Insert_UserProgress()
    {
        Cleardatas.ResetProgress();                                                                 // ������ �ʱ�ȭ

        Param param_p = new Param()                                                                 // �ʱ�ȭ ��(�⺻��) ������ ���̺� ����
        {
            {"L1D1", Cleardatas.S1_Difficult_1},
            {"L1D2", Cleardatas.S1_Difficult_2},
            {"L1D3", Cleardatas.S1_Difficult_3},

            {"L2D1", Cleardatas.S2_Difficult_1},
            {"L2D2", Cleardatas.S2_Difficult_2},
            {"L2D3", Cleardatas.S2_Difficult_3},

            {"L3D1", Cleardatas.S3_Difficult_1},
            {"L3D2", Cleardatas.S3_Difficult_2},
            {"L3D3", Cleardatas.S3_Difficult_3},

            {"L4D1", Cleardatas.S4_Difficult_1},
            {"L4D2", Cleardatas.S4_Difficult_2},
            {"L4D3", Cleardatas.S4_Difficult_3},

            {"L5D1", Cleardatas.S5_Difficult_1},
            {"L5D2", Cleardatas.S5_Difficult_2},
            {"L5D3", Cleardatas.S5_Difficult_3},

            {"High_Stage", Cleardatas.High_Stage}
        };

        SendQueue.Enqueue(Backend.GameData.Insert, "Progress_Chart", param_p, callback =>           // �ڳ� ������ ������ ����
        {
            if (callback.IsSuccess())                                                               // ���� ��
            {
                ClearDataRowInDate = callback.GetInDate();                                          // ���� �� ����
#if UNITY_EDITOR
                Debug.Log($"Progress ���� ���� : {callback}");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError($"Progress ���� ���� : {callback}");
#endif
                return;
            }
        });
    }
    /// <summary>
    /// Date ����
    /// </summary>
    public void InsertLifeData_()
    {
        GetServer_Synchronous();                                                                    // ���� �ð� �ҷ�����

        Lifedate.LifeDate = Lifedate.UTCDate;                                                       // ���� ���� �ð��� Life ���� �ð��� ����

        Param param_date = new Param()                                                              // Life ���� �ð� ���� �� ����
       {
            {"Date", Lifedate.LifeDate}
       };

        SendQueue.Enqueue(Backend.GameData.Insert, "Life_Date", param_date, callback =>             // �ڳ� ������ ������ ����
        {
            if (callback.IsSuccess())                                                               // ���� ��
            {
                DateRowInDate = callback.GetInDate();                                               // ���� �� ����
#if UNITY_EDITOR
                Debug.Log($"Life_Date ���� ���� : {callback}");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError($"Life_Date ���� ���� : {callback}");
#endif
                return;
            }
        });
    }
    /// <summary>
    /// �� ������ �����˻� �� ��������
    /// </summary>
    /// <returns></returns>
    public string GetDatas()
    {
        string message = string.Empty;

        message = GetUserData();                                                                    // ������ ������ ��

        if (message != "")                                                                          // ���� �̹߻� �� ���� ���� return, �ƴ϶�� ���� �߻�(���� �߻�)
        {
            return message;
        }

        message = GetUpgraede_S();

        if (message != "")
        {
            return message;
        }

        message = GetStatus();

        if (message != "")
        {
            return message;
        }

        message = GetWeapon();

        if (message != "")
        {
            return message;
        }

        message = GetProgress();

        if (message != "")
        {
            return message;
        }

        GetLifeData();

        return message;
    }
    /// <summary>
    /// User ������ ��������
    /// </summary>
    /// <returns></returns>
    private string GetUserData()
    {
        SendQueue.Enqueue(Backend.GameData.Get, "Player_Data", new Where(), callback =>                     //inDate ��(���� �� ���� ��) ��������
        {
            userDataRowInDate = callback.GetInDate();
        });

        string message = string.Empty;

        SendQueue.Enqueue(Backend.GameData.GetMyData, "Player_Data", new Where(), callback =>               // ���� �α����� ������� ���̺� ������ ��������
        {
            if (callback.IsSuccess())                                                                       // �ҷ����� ���� ��
            {
                try                                                                                         // �ҷ��� ���� �Ľ�, �ݿ�
                {
                    LitJson.JsonData gameDataJson = callback.FlattenRows();

                    if (gameDataJson.Count <= 0)                                                            // �����Ͱ� ���� ���
                    {
#if UNITY_EDITOR
                        Debug.LogError("�����Ͱ� �������� �ʽ��ϴ�. ���ο� �����͸� �����մϴ�.");
#endif
                        Insert_UserData();                                                                  // �ʱ� ������ ����
                    }
                    #region �ҷ��� ������ ����
                    Userdatas.Level = int.Parse(gameDataJson[0]["Level"].ToString());
                    Userdatas.Coin = int.Parse(gameDataJson[0]["Coin"].ToString());
                    Userdatas.Life = int.Parse(gameDataJson[0]["Life"].ToString());
                    Userdatas.NowExp = float.Parse(gameDataJson[0]["Nowexp"].ToString());
                    Userdatas.Next_Exp_value = float.Parse(gameDataJson[0]["Next_Exp_value"].ToString());
                    Userdatas.Next_Exp_UP = float.Parse(gameDataJson[0]["Next_Exp_UP"].ToString());
                    Userdatas.Exp_Multiplication = float.Parse(gameDataJson[0]["Exp_Multiplication"].ToString());
                    Userdatas.WeaponNumber = int.Parse(gameDataJson[0]["WeaponNumber"].ToString());
                    #endregion
                    On_userdata = true;                                                                     // �ҷ����� ����
                }
                catch(System.Exception e)                                                                   // �Ľ� ����
                {
                    Userdatas.ResetData();                                                                  // �ʱⰪ���� ����
#if UNITY_EDITOR
                    Debug.LogError(e);
#endif
                    message = "���� ���� �߻� �ʱ�ȭ������ �̵��մϴ�.(user_data_)";                        // ���� �޽��� ���
                    
                    SendQueue.Enqueue(Backend.BMember.Logout, callback =>                                   // ��ū ���� �� Login ������ ����
                    {
                        if (!callback.IsSuccess())
                            callback.GetMessage();
                    });
                }
            }
            else                                                                                            // �ҷ����� ����
            {
#if UNITY_EDITOR
                Debug.LogError(callback);
#endif
                message = "���� ���� �߻� �ʱ�ȭ������ �̵��մϴ�.(user_data_)";                            // ���� �޽��� ���
                
                SendQueue.Enqueue(Backend.BMember.Logout, callback =>                                       // ��ū ���� �� Login ������ ����
                {
                    if (!callback.IsSuccess())                                                              // �α׾ƿ� ����(�޽��� ���)/�α׾ƿ� ����(��ū ����)
                        callback.GetMessage();
                });
            }
        });

        return message;                                                                                     // ���� �˻縦 ���� �޽��� return
    }
    /// <summary>
    /// ���൵ ������ ��������
    /// </summary>
    /// <returns></returns>
    public string GetProgress()
    {
        SendQueue.Enqueue(Backend.GameData.Get, "Progress_Chart", new Where(), callback =>                  //inDate ��(���� �� ���� ��) ��������
        {
            ClearDataRowInDate = callback.GetInDate();
        });

        string message = string.Empty;

        SendQueue.Enqueue(Backend.GameData.GetMyData, "Progress_Chart", new Where(), callback =>            // ���� �α����� ������� ���̺� ������ ��������
        {
            if (callback.IsSuccess())                                                                       // �ҷ����� ����
            {
                try                                                                                         // �ҷ��� ���� �Ľ�, �ݿ�
                {
                    LitJson.JsonData gameDataJson = callback.FlattenRows();

                    if (callback.GetReturnValuetoJSON()["rows"].Count <= 0)                                 // �����Ͱ� ���� ���
                    {
#if UNITY_EDITOR
                        Debug.LogError("�����Ͱ� �������� �ʽ��ϴ�. ���ο� �����͸� �����մϴ�.");
#endif
                        Insert_UserProgress();                                                              // �ʱ� ������ ����
                    }
                    #region �ҷ��� ������ ����
                    Cleardatas.S1_Difficult_1 = Boolean.Parse(gameDataJson[0]["L1D1"].ToString());
                    Cleardatas.S1_Difficult_2 = Boolean.Parse(gameDataJson[0]["L1D2"].ToString());
                    Cleardatas.S1_Difficult_3 = Boolean.Parse(gameDataJson[0]["L1D3"].ToString());

                    Cleardatas.S2_Difficult_1 = Boolean.Parse(gameDataJson[0]["L2D1"].ToString());
                    Cleardatas.S2_Difficult_2 = Boolean.Parse(gameDataJson[0]["L2D2"].ToString());
                    Cleardatas.S2_Difficult_3 = Boolean.Parse(gameDataJson[0]["L2D3"].ToString());

                    Cleardatas.S3_Difficult_1 = Boolean.Parse(gameDataJson[0]["L3D1"].ToString());
                    Cleardatas.S3_Difficult_2 = Boolean.Parse(gameDataJson[0]["L3D2"].ToString());
                    Cleardatas.S3_Difficult_3 = Boolean.Parse(gameDataJson[0]["L3D3"].ToString());

                    Cleardatas.S4_Difficult_1 = Boolean.Parse(gameDataJson[0]["L4D1"].ToString());
                    Cleardatas.S4_Difficult_2 = Boolean.Parse(gameDataJson[0]["L4D2"].ToString());
                    Cleardatas.S4_Difficult_3 = Boolean.Parse(gameDataJson[0]["L4D3"].ToString());

                    Cleardatas.S5_Difficult_1 = Boolean.Parse(gameDataJson[0]["L5D1"].ToString());
                    Cleardatas.S5_Difficult_2 = Boolean.Parse(gameDataJson[0]["L5D2"].ToString());
                    Cleardatas.S5_Difficult_3 = Boolean.Parse(gameDataJson[0]["L5D3"].ToString());

                    Cleardatas.High_Stage = Int32.Parse(gameDataJson[0]["High_Stage"].ToString());
                    #endregion
                    On_cleardata = true;                                                                    // �ҷ����� ����
                }
                catch (System.Exception e)                                                                  // �Ľ� ����
                {
                    Cleardatas.ResetProgress();                                                             // �ʱⰪ���� ����
#if UNITY_EDITOR
                    Debug.LogError(e);
#endif
                    message = "���� ���� �߻� �ʱ�ȭ������ �̵��մϴ�.(Progress_data_)";                    // ���� �޽��� ���

                    SendQueue.Enqueue(Backend.BMember.Logout, callback =>                                   // ��ū ���� �� Login ������ ����
                    {
                        if (!callback.IsSuccess())                                                          // �α׾ƿ� ����(�޽��� ���)/�α׾ƿ� ����(��ū ����)
                            callback.GetMessage();
                    });
                }
            }
            else                                                                                            // �ҷ����� ����
            {
#if UNITY_EDITOR
                Debug.LogError(callback);
#endif
                message = "���� ���� �߻� �ʱ�ȭ������ �̵��մϴ�.(Progress_data_)";                        // ���� �޽��� ���

                SendQueue.Enqueue(Backend.BMember.Logout, callback =>                                       // ��ū ���� �� Login ������ ����
                {
                    if (!callback.IsSuccess())                                                              // �α׾ƿ� ����(�޽��� ���)/�α׾ƿ� ����(��ū ����)
                        callback.GetMessage();
                });
            }
        });

        return message;                                                                                     // ���� �˻縦 ���� �޽��� return
    }
    /// <summary>
    /// Status ������ ��������
    /// </summary>
    /// <returns></returns>
    public string GetStatus()
    {
        SendQueue.Enqueue(Backend.GameData.Get, "Status_", new Where(), callback =>                         //inDate ��(���� �� ���� ��) ��������
        {
            statusDataRowInDate = callback.GetInDate();
        });

        string message = string.Empty;

        SendQueue.Enqueue(Backend.GameData.GetMyData, "Status_", new Where(), callback =>                   // ���� �α����� ������� ���̺� ������ ��������
        {
            if (callback.IsSuccess())                                                                       // �ҷ����� ����
            {
                try                                                                                         // �ҷ��� ���� �Ľ�, �ݿ�
                {
                    LitJson.JsonData gameDataJson = callback.FlattenRows();

                    if (callback.GetReturnValuetoJSON()["rows"].Count <= 0)                                 // �����Ͱ� ���� ���
                    {
#if UNITY_EDITOR
                        Debug.LogError("�����Ͱ� �������� �ʽ��ϴ�. ���ο� �����͸� �����մϴ�.");
#endif
                        Insert_UserStatus();                                                                // �ʱ� ������ ����
                    }
                    #region �ҷ��� ������ ����
                    Userstatusdatas.Max_HP = float.Parse(gameDataJson[0]["Max_Hp"].ToString());
                    Userstatusdatas.Attack_Power = float.Parse(gameDataJson[0]["Attack_Power"].ToString());
                    Userstatusdatas.Deffensive_Power = float.Parse(gameDataJson[0]["Deffensive_Power"].ToString());
                    Userstatusdatas.MoveSpeed = float.Parse(gameDataJson[0]["MoveSpeed"].ToString());
                    Userstatusdatas.AttackSpeed = float.Parse(gameDataJson[0]["AttackSpeed"].ToString());
                    #endregion
                    On_statusdata = true;                                                                   // �ҷ����� ����
                }
                catch (System.Exception e)                                                                  // �Ľ� ����
                {
                    Userstatusdatas.ResetStatus();                                                          // �ʱⰪ���� ����
#if UNITY_EDITOR
                    Debug.LogError(e);
#endif
                    message = "���� ���� �߻� �ʱ�ȭ������ �̵��մϴ�.(Status_data_)";                      // ���� �޽��� ���

                    SendQueue.Enqueue(Backend.BMember.Logout, callback =>                                   // ��ū ���� �� Login ������ ����
                    {
                        if (!callback.IsSuccess())                                                          // �α׾ƿ� ����(�޽��� ���)/�α׾ƿ� ����(��ū ����)
                            callback.GetMessage();
                    });
                }
            }
            else                                                                                            // �ҷ����� ����
            {
#if UNITY_EDITOR
                Debug.LogError(callback);
#endif
                message = "���� ���� �߻� �ʱ�ȭ������ �̵��մϴ�.(Status_data_)";                          // ���� �޽��� ���
                
                SendQueue.Enqueue(Backend.BMember.Logout, callback =>                                       // ��ū ���� �� Login ������ ����
                {
                    if (!callback.IsSuccess())                                                              // �α׾ƿ� ����(�޽��� ���)/�α׾ƿ� ����(��ū ����)
                        callback.GetMessage();
                });
            }
        });

        return message;                                                                                     // ���� �˻縦 ���� �޽��� return
    }
    /// <summary>
    /// Weapon ������ �ҷ�����
    /// </summary>
    /// <returns></returns>
    public string GetWeapon()
    {
        SendQueue.Enqueue(Backend.GameData.Get, "Weapon_Status", new Where(), callback =>                   //inDate ��(���� �� ���� ��) ��������
        {
            WeaponLVDataRowInDate = callback.GetInDate();
        });

        string message = string.Empty;

        SendQueue.Enqueue(Backend.GameData.GetMyData, "Weapon_Status", new Where(), callback =>             // ���� �α����� ������� ���̺� ������ ��������
        {
            if (callback.IsSuccess())                                                                       // �ҷ����� ����
            {
                try                                                                                         // �ҷ��� ���� �Ľ�, �ݿ�
                {
                    LitJson.JsonData gameDataJson = callback.FlattenRows();

                    if (callback.GetReturnValuetoJSON()["rows"].Count <= 0)                                 // �����Ͱ� ���� ���
                    {
#if UNITY_EDITOR
                        Debug.LogError("�����Ͱ� �������� �ʽ��ϴ�. ���ο� �����͸� �����մϴ�.");
#endif
                        Insert_UserWeaponLevel();                                                           // �ʱ� ������ ����
                    }
                    #region �ҷ��� ������ ����
                    Userweaponlevel.Pistol = Int32.Parse(gameDataJson[0]["Pistol"].ToString());
                    Userweaponlevel.FlareThrower = Int32.Parse(gameDataJson[0]["FireThrower"].ToString());
                    Userweaponlevel.Flare_gun = Int32.Parse(gameDataJson[0]["FlareGun"].ToString());
                    Userweaponlevel.GasShield = Int32.Parse(gameDataJson[0]["GasShield"].ToString());
                    Userweaponlevel.Knife = Int32.Parse(gameDataJson[0]["Knife"].ToString());
                    Userweaponlevel.Mine = Int32.Parse(gameDataJson[0]["Mine"].ToString());
                    Userweaponlevel.RocketLauncer = Int32.Parse(gameDataJson[0]["RocketLauncher"].ToString());
                    Userweaponlevel.Shotgun = Int32.Parse(gameDataJson[0]["ShotGun"].ToString());
                    Userweaponlevel.Sniper = Int32.Parse(gameDataJson[0]["Sniper"].ToString());
                    Userweaponlevel.Rampage = Int32.Parse(gameDataJson[0]["SubMachinGun"].ToString());
                    #endregion
                    On_weapondata = true;                                                                   // �ҷ����� ����
                }
                catch (System.Exception e)                                                                  // �Ľ� ����
                {
                    Userweaponlevel.ResetWeapons();                                                         // ������ �ʱⰪ ����
#if UNITY_EDITOR
                    Debug.LogError(e);
#endif
                    message = "���� ���� �߻� �ʱ�ȭ������ �̵��մϴ�.(Weapon_data_)";                      // ���� �޽��� ���

                    SendQueue.Enqueue(Backend.BMember.Logout, callback =>                                   // ��ū ���� �� Login ������ ����
                    {
                        if (!callback.IsSuccess())                                                          // �α׾ƿ� ����(�޽��� ���)/�α׾ƿ� ����(��ū ����)
                            callback.GetMessage();
                    });
                }
            }
            else                                                                                            // �ҷ����� ����
            {
#if UNITY_EDITOR
                Debug.LogError(callback);
#endif           
                message = "���� ���� �߻� �ʱ�ȭ������ �̵��մϴ�.(Weapon_data_)";                          // ���� �޽��� ���

                SendQueue.Enqueue(Backend.BMember.Logout, callback =>                                       // ��ū ���� �� Login ������ ����
                {
                    if (!callback.IsSuccess())                                                              // �α׾ƿ� ����(�޽��� ���)/�α׾ƿ� ����(��ū ����)
                        callback.GetMessage();
                });
            }
        });

        return message;                                                                                     // ���� �˻縦 ���� �޽��� return
    }
    /// <summary>
    /// Upgrade ������ �ҷ�����
    /// </summary>
    /// <returns></returns>
    public string GetUpgraede_S()
    {
        SendQueue.Enqueue(Backend.GameData.Get, "Upgrade_Status", new Where(), callback =>                  //inDate ��(���� �� ���� ��) ��������
        {
            statusLVDataRowInDate = callback.GetInDate();
        });

        string message = string.Empty;

        SendQueue.Enqueue(Backend.GameData.GetMyData, "Upgrade_Status", new Where(), callback =>           // ���� �α����� ������� ���̺� ������ ��������
        {
            if (callback.IsSuccess())                                                                      // �ҷ����� ����
            {
                try                                                                                        // �ҷ��� ���� �Ľ�, �ݿ�
                {
                    LitJson.JsonData gameDataJson = callback.FlattenRows();

                    if (callback.GetReturnValuetoJSON()["rows"].Count <= 0)                                // �����Ͱ� ���� ���
                    {
#if UNITY_EDITOR
                        Debug.LogError("�����Ͱ� �������� �ʽ��ϴ�. ���ο� �����͸� �����մϴ�.");
#endif    
                        Insert_UserStatusLevel();                                                          // �ʱ� ������ ����
                    }
                    #region �ҷ��� ������ ����
                    Userstatuslevel.AP = Int32.Parse(gameDataJson[0]["AttackPower"].ToString());
                    Userstatuslevel.AS = Int32.Parse(gameDataJson[0]["AttackSpeed"].ToString());
                    Userstatuslevel.DP = Int32.Parse(gameDataJson[0]["DeffensivePower"].ToString());
                    Userstatuslevel.PH = Int32.Parse(gameDataJson[0]["PhysicalPower"].ToString());
                    Userstatuslevel.MS = Int32.Parse(gameDataJson[0]["MoveSpeed"].ToString());
                    #endregion
                    On_statusLvdata = true;                                                                // �ҷ����� ����
                }
                catch (System.Exception e)                                                                 // �Ľ� ����
                {
                    Userstatuslevel.ResetStatusLevel();                                                    // ������ �ʱⰪ ����
#if UNITY_EDITOR
                    Debug.LogError(e);
#endif  
                    message = "���� ���� �߻� �ʱ�ȭ������ �̵��մϴ�.(StatusLV_data_)";                   // ���� �޽��� ���

                    SendQueue.Enqueue(Backend.BMember.Logout, callback =>                                  // ��ū ���� �� Login ������ ����
                    {
                        if (!callback.IsSuccess())                                                         // �α׾ƿ� ����(�޽��� ���)/�α׾ƿ� ����(��ū ����)
                            callback.GetMessage();
                    });
                }
            }
            else                                                                                           // �ҷ����� ����
            {
#if UNITY_EDITOR
                Debug.LogError(callback);
#endif
                message = "���� ���� �߻� �ʱ�ȭ������ �̵��մϴ�.(StatusLV_data_)";                       // ���� �޽��� ���

                SendQueue.Enqueue(Backend.BMember.Logout, callback =>                                      // ��ū ���� �� Login ������ ����
                {
                    if (!callback.IsSuccess())                                                             // �α׾ƿ� ����(�޽��� ���)/�α׾ƿ� ����(��ū ����)
                        callback.GetMessage();
                });
            }
        });

        return message;                                                                                    // ���� �˻縦 ���� �޽��� return
    }
    /// <summary>
    /// Life ���� ����Ʈ �� ��������
    /// </summary>
    public void GetLifeData()
    {
        SendQueue.Enqueue(Backend.GameData.Get, "Life_Date", new Where(), callback =>                      //inDate ��(���� �� ���� ��) ��������
        {
            DateRowInDate = callback.GetInDate();
        });

        string message = string.Empty;

        SendQueue.Enqueue(Backend.GameData.GetMyData, "Life_Date", new Where(), callback =>                // ���� �α����� ������� ���̺� ������ ��������
        {
            if (callback.IsSuccess())                                                                      // �ҷ����� ����
            {
                try                                                                                        // �ҷ��� ���� �Ľ�, �ݿ�
                {
                    LitJson.JsonData gameDataJson = callback.FlattenRows();

                    if (gameDataJson.Count <= 0)                                                           // �����Ͱ� ���� ���
                    {
#if UNITY_EDITOR
                        Debug.LogError("�����Ͱ� �������� �ʽ��ϴ�. ���ο� �����͸� �����մϴ�.");
#endif
                        InsertLifeData_();                                                                 // �ʱ� ������ ����
                    }
                    Lifedate.LifeDate = DateTime.Parse(gameDataJson[0]["Date"].ToString());

                    On_lifedate = true;                                                                    // �ҷ����� ����
                }
                catch (System.Exception e)                                                                 // �Ľ� ����
                {
                    GetServer_Synchronous();                                                               // ���Ӱ� ���� �ð� ����ȭ
                    Lifedate.LifeDate = Lifedate.UTCDate;
#if UNITY_EDITOR
                    Debug.LogError(e);
#endif
                    message = "���� ���� �߻� �ʱ�ȭ������ �̵��մϴ�.(life_data_)";                       // ���� �޽��� ���

                    SendQueue.Enqueue(Backend.BMember.Logout, callback =>                                  // ��ū ���� �� Login ������ ����
                    {
                        if (!callback.IsSuccess())
                            callback.GetMessage();
                    });
                }
            }
            else                                                                                           // �ҷ����� ����
            {
#if UNITY_EDITOR
                Debug.LogError(callback);
#endif
                message = "���� ���� �߻� �ʱ�ȭ������ �̵��մϴ�.(life_data_)";                            // ���� �޽��� ���

                SendQueue.Enqueue(Backend.BMember.Logout, callback =>                                       // ��ū ���� �� Login ������ ����
                {
                    if (!callback.IsSuccess())                                                              // �α׾ƿ� ����(�޽��� ���)/�α׾ƿ� ����(��ū ����)
                        callback.GetMessage();
                });
            }
        });
    }
    /// <summary>
    /// User ������ ����(����)
    /// </summary>
    /// <param name="action"></param>
    public void UpdateUserDatas_(UnityAction action=null)
    {
        if (userdatas == null)
        {
#if UNITY_EDITOR
            Debug.LogError("�����Ͱ� �������� �ʽ��ϴ�." + "Insert or Load �ʿ�");
#endif
        }

        Param param_d = new Param()                                                                        // ������ ������ ���� �غ�
       {
            {"Level", Userdatas.Level},
            {"Coin", Userdatas.Coin},
            {"Life", Userdatas.Life},
            {"Nowexp", Userdatas.NowExp},
            {"Next_Exp_value", Userdatas.Next_Exp_value},
            {"Next_Exp_UP", Userdatas.Next_Exp_UP},
            {"Exp_Multiplication", Userdatas.Exp_Multiplication},
            {"WeaponNumber",Userdatas.WeaponNumber},
       };
        if (string.IsNullOrEmpty(userDataRowInDate))                                                       // data ������ ���� �� ���� ���
        {
#if UNITY_EDITOR
            Debug.LogError("������ inDate ���������ϴ�. ��ε� �մϴ�.");
#endif
            SendQueue.Enqueue(Backend.GameData.Get, "Player_Data", new Where(), callback =>                // inDate �� �ε�
            {
                userDataRowInDate = callback.GetInDate();
            });
        }
        /// <summary>
        /// ���� �������� �������� ���� ��, ���̺� ����Ǿ� �ִ� �� �� inDate Į���� ����,
        /// �����ϴ� ������ owner_inDate�� ��ġ�ϴ� row�� �˻��� ����
        /// owner_inDate�� �ڳ� �ֿܼ��� Ȯ�ΰ���
        /// </summary>
        else
        {
#if UNITY_EDITOR
            Debug.Log($"{userDataRowInDate}�� ���� ���� ������ ���� ��û");
#endif
            SendQueue.Enqueue(Backend.GameData.UpdateV2, "Player_Data", userDataRowInDate, Backend.UserInDate, param_d, callback =>         // ������ ���� ��û
            {
                if (callback.IsSuccess())                                                                   // ���� ����
                {
#if UNITY_EDITOR
                    Debug.Log($"������ ���� ���� : {callback}");
#endif
                    action?.Invoke();                                                                       // �Ŀ� ���� �ൿ�� �ִٸ� ����
                }
                else                                                                                        // ���� ����
                {
#if UNITY_EDITOR
                    Debug.LogError($"������ ���� ���� : {callback}");
#endif
                }
            });
        }
    }
    /// <summary>
    /// Status ������ ����
    /// </summary>
    /// <param name="action"></param>
    public void UpdateStatusDatas_(UnityAction action = null)
    {
        if (userstatusdatas == null)
        {
#if UNITY_EDITOR
            Debug.LogError("�����Ͱ� �������� �ʽ��ϴ�." + "Insert or Load �ʿ�");
#endif
        }

        Param param_s_ = new Param()                                                                       // ������ ������ ���� �غ�
        {
            {"Max_Hp", Userstatusdatas.Max_HP},
            {"Attack_Power", Userstatusdatas.Attack_Power},
            {"Deffensive_Power", Userstatusdatas.Deffensive_Power},
            {"MoveSpeed", Userstatusdatas.MoveSpeed},
            {"AttackSpeed", Userstatusdatas.AttackSpeed}
        };
        if (string.IsNullOrEmpty(statusDataRowInDate))                                                     // data ������ ���� �� ���� ���
        {
#if UNITY_EDITOR
            Debug.LogError("������ inDate ���������ϴ�. ��ε� �մϴ�.");
#endif
            SendQueue.Enqueue(Backend.GameData.Get, "Status_", new Where(), callback =>                    // inDate �� �ε�
            {
                statusDataRowInDate = callback.GetInDate();
            });
        }
        /// <summary>
        /// ���� �������� �������� ���� ��, ���̺� ����Ǿ� �ִ� �� �� inDate Į���� ����,
        /// �����ϴ� ������ owner_inDate�� ��ġ�ϴ� row�� �˻��� ����
        /// owner_inDate�� �ڳ� �ֿܼ��� Ȯ�ΰ���
        /// </summary>
        else
        {
#if UNITY_EDITOR
            Debug.Log($"{statusDataRowInDate}�� ���� ���� ������ ���� ��û");
#endif
            SendQueue.Enqueue(Backend.GameData.UpdateV2, "Status_", statusDataRowInDate, Backend.UserInDate, param_s_, callback =>         // ������ ���� ��û
            {
                if (callback.IsSuccess())                                                                  // ���� ����
                {
#if UNITY_EDITOR
                    Debug.Log($"������ ���� ���� : {callback}");
#endif
                    action?.Invoke();                                                                      // �Ŀ� ���� �ൿ�� �ִٸ� ����
                }
                else                                                                                       // ���� ����
                {
#if UNITY_EDITOR
                    Debug.LogError($"������ ���� ���� : {callback}");
#endif
                }
            });
        }
    }
    public void UpdateStatusLVDatas_(UnityAction action = null)
    {
        if (userstatuslevel == null)
        {
#if UNITY_EDITOR
            Debug.LogError("�����Ͱ� �������� �ʽ��ϴ�." + "Insert or Load �ʿ�");
#endif
        }
        Param param_s = new Param()                                                                        // ������ ������ ���� �غ�
        {
            {"AttackPower", Userstatuslevel.AP},
            {"DeffensivePower", Userstatuslevel.DP},
            {"PhysicalPower", Userstatuslevel.PH},
            {"AttackSpeed", Userstatuslevel.AS},
            {"MoveSpeed", Userstatuslevel.MS}
        };
        if (string.IsNullOrEmpty(statusLVDataRowInDate))                                                  // data ������ ���� �� ���� ���
        {
#if UNITY_EDITOR
            Debug.LogError("������ inDate ���������ϴ�. ��ε� �մϴ�.");
#endif
            SendQueue.Enqueue(Backend.GameData.Get, "Upgrade_Status", new Where(), callback =>            // inDate �� �ε�
            {
                statusLVDataRowInDate = callback.GetInDate();
            });
        }
        /// <summary>
        /// ���� �������� �������� ���� ��, ���̺� ����Ǿ� �ִ� �� �� inDate Į���� ����,
        /// �����ϴ� ������ owner_inDate�� ��ġ�ϴ� row�� �˻��� ����
        /// owner_inDate�� �ڳ� �ֿܼ��� Ȯ�ΰ���
        /// </summary>
        else
        {
#if UNITY_EDITOR
            Debug.Log($"{statusLVDataRowInDate}�� ���� ���� ������ ���� ��û");
#endif
            SendQueue.Enqueue(Backend.GameData.UpdateV2, "Upgrade_Status", statusLVDataRowInDate, Backend.UserInDate, param_s, callback =>         // ������ ���� ��û
            {
                if (callback.IsSuccess())                                                                  // ���� ����
                {
#if UNITY_EDITOR
                    Debug.Log($"������ ���� ���� : {callback}");
#endif
                    action?.Invoke();                                                                      // �Ŀ� ���� �ൿ�� �ִٸ� ����
                }
                else                                                                                       // ���� ����
                {
#if UNITY_EDITOR
                    Debug.LogError($"������ ���� ���� : {callback}");
#endif
                }
            });
        }
    }
    /// <summary>
    /// Weapon ������ ����
    /// </summary>
    /// <param name="action"></param>
    public void UpdateWeaponLVDatas_(UnityAction action = null)
    {
        if (userweaponlevel == null)
        {
#if UNITY_EDITOR
            Debug.LogError("�����Ͱ� �������� �ʽ��ϴ�." + "Insert or Load �ʿ�");
#endif
        }
        Param param_ws = new Param()                                                                       // ������ ������ ���� �غ�
        {
            {"Pistol", Userweaponlevel.Pistol},
            {"ShotGun", Userweaponlevel.Shotgun},
            {"Mine", Userweaponlevel.Mine},
            {"SubMachinGun", Userweaponlevel.Rampage},
            {"Sniper", Userweaponlevel.Sniper},
            {"FlareGun", Userweaponlevel.Flare_gun},
            {"FireThrower", Userweaponlevel.FlareThrower},
            {"Knife", Userweaponlevel.Knife},
            {"GasShield", Userweaponlevel.GasShield},
            {"RocketLauncher", Userweaponlevel.RocketLauncer}
        };
        // data ������ ���� �� ���� ���
        if (string.IsNullOrEmpty(WeaponLVDataRowInDate))
        {
#if UNITY_EDITOR
            Debug.LogError("������ inDate ���������ϴ�. ��ε� �մϴ�.");
#endif
            SendQueue.Enqueue(Backend.GameData.Get, "Weapon_Status", new Where(), callback =>            // inDate �� �ε�
            {
                WeaponLVDataRowInDate = callback.GetInDate();
            });
        }
        /// <summary>
        /// ���� �������� �������� ���� ��, ���̺� ����Ǿ� �ִ� �� �� inDate Į���� ����,
        /// �����ϴ� ������ owner_inDate�� ��ġ�ϴ� row�� �˻��� ����
        /// owner_inDate�� �ڳ� �ֿܼ��� Ȯ�ΰ���
        /// </summary>
        else
        {
#if UNITY_EDITOR
            Debug.Log($"{WeaponLVDataRowInDate}�� ���� ���� ������ ���� ��û");
#endif
            SendQueue.Enqueue(Backend.GameData.UpdateV2, "Weapon_Status", WeaponLVDataRowInDate, Backend.UserInDate, param_ws, callback =>         // ������ ���� ��û
            {
                if (callback.IsSuccess())                                                               // ���� ����
                {
#if UNITY_EDITOR
                    Debug.Log($"������ ���� ���� : {callback}");
#endif
                    action?.Invoke();                                                                   // �Ŀ� ���� �ൿ�� �ִٸ� ����
                }
                else                                                                                    // ���� ����
                {
#if UNITY_EDITOR
                    Debug.LogError($"������ ���� ���� : {callback}");
#endif
                }
            });
        }
    }
    /// <summary>
    /// ���൵ ������ ����
    /// </summary>
    /// <param name="action"></param>
    public void UpdateClearDatas_(UnityAction action = null)
    {
        if (Cleardatas == null)
        {
#if UNITY_EDITOR
            Debug.LogError("�����Ͱ� �������� �ʽ��ϴ�." + "Insert or Load �ʿ�");
#endif
        }
        Param param_p = new Param()                                                                     // ������ ������ ���� �غ�
        {
            {"L1D1", Cleardatas.S1_Difficult_1},
            {"L1D2", Cleardatas.S1_Difficult_2},
            {"L1D3", Cleardatas.S1_Difficult_3},

            {"L2D1", Cleardatas.S2_Difficult_1},
            {"L2D2", Cleardatas.S2_Difficult_2},
            {"L2D3", Cleardatas.S2_Difficult_3},

            {"L3D1", Cleardatas.S3_Difficult_1},
            {"L3D2", Cleardatas.S3_Difficult_2},
            {"L3D3", Cleardatas.S3_Difficult_3},

            {"L4D1", Cleardatas.S4_Difficult_1},
            {"L4D2", Cleardatas.S4_Difficult_2},
            {"L4D3", Cleardatas.S4_Difficult_3},

            {"L5D1", Cleardatas.S5_Difficult_1},
            {"L5D2", Cleardatas.S5_Difficult_2},
            {"L5D3", Cleardatas.S5_Difficult_3},

            {"High_Stage", Cleardatas.High_Stage}
        };
        // data ������ ���� �� ���� ���
        if (string.IsNullOrEmpty(ClearDataRowInDate))
        {
#if UNITY_EDITOR
            Debug.LogError("������ inDate ���������ϴ�. ��ε� �մϴ�.");
#endif
            SendQueue.Enqueue(Backend.GameData.Get, "Progress_Chart", new Where(), callback =>           // inDate �� �ε�
            {
                ClearDataRowInDate = callback.GetInDate();
            });
        }
        /// <summary>
        /// ���� �������� �������� ���� ��, ���̺� ����Ǿ� �ִ� �� �� inDate Į���� ����,
        /// �����ϴ� ������ owner_inDate�� ��ġ�ϴ� row�� �˻��� ����
        /// owner_inDate�� �ڳ� �ֿܼ��� Ȯ�ΰ���
        /// </summary>
        else
        {
#if UNITY_EDITOR
            Debug.Log($"{ClearDataRowInDate}�� ���� ���� ������ ���� ��û");
#endif
            SendQueue.Enqueue(Backend.GameData.UpdateV2, "Progress_Chart", ClearDataRowInDate, Backend.UserInDate, param_p, callback =>         // ������ ���� ��û
            {
                if (callback.IsSuccess())                                                               // ���� ����
                {
#if UNITY_EDITOR
                    Debug.Log($"������ ���� ���� : {callback}");
#endif
                    action?.Invoke();                                                                   // �Ŀ� ���� �ൿ�� �ִٸ� ����
                }
                else                                                                                    // ���� ����
                {
#if UNITY_EDITOR
                    Debug.LogError($"������ ���� ���� : {callback}");
#endif
                }
            });
        }
    }
    /// <summary>
    /// Life ���� Date ����
    /// </summary>
    /// <param name="action"></param>
    public void UpdateLifeData_(UnityAction action = null)
    {
        if (Lifedate== null)
        {
#if UNITY_EDITOR
            Debug.LogError("�����Ͱ� �������� �ʽ��ϴ�." + "Insert or Load �ʿ�");
#endif
        }
        Lifedate.LifeDate = Lifedate.UTCDate;                                                           // ���� �ð� Life ���� �ð����� �ݿ�

        Param param_date = new Param()                                                                  // ������ ������ ���� �غ�
       {
            {"Date", Lifedate.LifeDate},
       };

        if (string.IsNullOrEmpty(DateRowInDate))                                                        // data ������ ���� �� ���� ���
        {
#if UNITY_EDITOR
            Debug.LogError("������ inDate ������ �����ϴ�. ��ε� �մϴ�.");
#endif
            SendQueue.Enqueue(Backend.GameData.Get, "Life_Date", new Where(), callback =>               // inDate �� �ε�
            {
                DateRowInDate = callback.GetInDate();
            });

            GetServer_Synchronous();
            Lifedate.LifeDate = Lifedate.UTCDate;
        }
        /// <summary>
        /// ���� �������� �������� ���� ��, ���̺� ����Ǿ� �ִ� �� �� inDate Į���� ����,
        /// �����ϴ� ������ owner_inDate�� ��ġ�ϴ� row�� �˻��� ����
        /// owner_inDate�� �ڳ� �ֿܼ��� Ȯ�ΰ���
        /// </summary>
        else
        {
#if UNITY_EDITOR
            Debug.Log($"{userDataRowInDate}�� ���� ���� ������ ���� ��û");
#endif
            SendQueue.Enqueue(Backend.GameData.UpdateV2, "Life_Date", DateRowInDate, Backend.UserInDate, param_date, callback =>         // ������ ���� ��û
            {
                if (callback.IsSuccess())                                                               // ���� ����
                {
#if UNITY_EDITOR
                    Debug.Log($"������ ���� ���� : {callback}");
#endif
                    action?.Invoke();                                                                   // �Ŀ� ���� �ൿ�� �ִٸ� ����
                }
                else // ���� ����
                {
#if UNITY_EDITOR
                    Debug.LogError($"������ ���� ���� : {callback}");
#endif
                }
            });
        }
    }
    /// <summary>
    /// ���� �ð� �ҷ�����(����)
    /// </summary>
    public void GetServer_Synchronous()
    {
        BackendReturnObject servertime = Backend.Utils.GetServerTime();                                                                 // ������ ���� �ð� ��û

        string time = servertime.GetReturnValuetoJSON()["utcTime"].ToString();                                                          // ������ �ð��� ���� �ð��� ���� ���� ������ ����
        Lifedate.UTCDate = DateTime.Parse(time);
    }
}
