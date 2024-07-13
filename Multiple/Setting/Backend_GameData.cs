using UnityEngine;
using BackEnd;
using System;
using UnityEngine.Events;
public class Backend_GameData
{
    [System.Serializable]
    public class GameDataLoadEvent : UnityEvent { }
    public GameDataLoadEvent onGameDataLoadEvent = new GameDataLoadEvent();                         // Data 수정, 삽입, 로드 후 이벤트 진행을 위한 Event 변수 생성

    private static Backend_GameData instance = null;
    public static Backend_GameData Instance                                                         // 정적타입의 get 프로퍼티
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
    #region Game 플레이에 필요한 각종 데이터 값들을 선언 및 수정, 가져오기 위한 클래스 변수
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

    #region 불러온 게임 정보의 고유 값 저장을 위한 string 변수
    private string userDataRowInDate = string.Empty;           
    private string statusDataRowInDate = string.Empty;
    private string statusLVDataRowInDate = string.Empty;
    private string WeaponLVDataRowInDate = string.Empty;
    private string ClearDataRowInDate = string.Empty;
    private string DateRowInDate = string.Empty;
    #endregion

    #region 불러왔는지 확인을 위한 체크 변수
    public bool On_userdata = false;
    public bool On_statusdata = false;
    public bool On_statusLvdata = false;
    public bool On_weapondata = false;
    public bool On_cleardata = false;
    public bool On_lifedate = false;
    #endregion

    /// <summary>
    /// 뒤끝 콘솔 테이블에 새로운 유저 정보 추가
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
    /// 유저 데이터 삽입
    /// </summary>
    public void Insert_UserData()
    {
        Userdatas.ResetData();                                                                      // 데이터 초기화

        Param param_d = new Param()                                                                 // 초기화 한(기본값) 데이터 테이블에 삽입
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

        SendQueue.Enqueue(Backend.GameData.Insert, "Player_Data", param_d, callback =>              // 뒤끝 서버에 데이터 삽입
        {
            if (callback.IsSuccess())                                                               // 성공 시
            {
                userDataRowInDate = callback.GetInDate();                                           // 고유 값 저장
#if UNITY_EDITOR
                Debug.Log($"User_Data 삽입 성공 : {callback}");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError($"User_Data 삽입 실패 : {callback}");
#endif
                return;
            }
        });
    }
    /// <summary>
    /// 스테이터스 수치 값 삽입
    /// </summary>
    private void Insert_UserStatus()
    {
        Userstatusdatas.ResetStatus();                                                              // 데이터 초기화

        Param param_s_ = new Param()                                                                // 초기화 한(기본값) 데이터 테이블에 삽입
        {
            {"Max_Hp", Userstatusdatas.Max_HP},
            {"Attack_Power", Userstatusdatas.Attack_Power},
            {"Deffensive_Power", Userstatusdatas.Deffensive_Power},
            {"MoveSpeed", Userstatusdatas.MoveSpeed},
            {"AttackSpeed", Userstatusdatas.AttackSpeed}
        };

        SendQueue.Enqueue(Backend.GameData.Insert,"Status_", param_s_, callback =>                  // 뒤끝 서버에 데이터 삽입
        {
            if (callback.IsSuccess())                                                               // 성공 시
            {
                statusDataRowInDate = callback.GetInDate();                                         // 고유 값 저장
#if UNITY_EDITOR
                Debug.Log($"Status_Data 삽입 성공 : {callback}");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError($"Status_Data 삽입 실패 : {callback}");
#endif
                return;
            }
        });
    }
    /// <summary>
    /// 스테이터스 level 값 삽입
    /// </summary>
    private void Insert_UserStatusLevel()
    {
        Userstatuslevel.ResetStatusLevel();                                                         // 데이터 초기화

        Param param_s = new Param()                                                                 // 초기화 한(기본값) 데이터 테이블에 삽입
        {
            {"AttackPower", Userstatuslevel.AP},
            {"DeffensivePower", Userstatuslevel.DP},
            {"PhysicalPower", Userstatuslevel.PH},
            {"AttackSpeed", Userstatuslevel.AS},
            {"MoveSpeed", Userstatuslevel.MS}
        };

        SendQueue.Enqueue(Backend.GameData.Insert, "Upgrade_Status", param_s, callback =>           // 뒤끝 서버에 데이터 삽입
        {
            if (callback.IsSuccess())                                                               // 성공 시
            {
                statusLVDataRowInDate = callback.GetInDate();                                       // 고유 값 저장
#if UNITY_EDITOR
                Debug.Log($"Upgrade_Status 삽입 성공 : {callback}");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError($"Upgrade_Status 삽입 실패 : {callback}");
#endif
                return;
            }
        });
    }
    /// <summary>
    /// 무기 level 값 삽입
    /// </summary>
    private void Insert_UserWeaponLevel()
    {
        Userweaponlevel.ResetWeapons();                                                             // 데이터 초기화

        Param param_ws = new Param()                                                                // 초기화 한(기본값) 데이터 테이블에 삽입
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

        SendQueue.Enqueue(Backend.GameData.Insert, "Weapon_Status", param_ws, callback =>           // 뒤끝 서버에 데이터 삽입
        {
            if (callback.IsSuccess())                                                               // 성공 시
            {
                WeaponLVDataRowInDate = callback.GetInDate();                                       // 고유 값 저장
#if UNITY_EDITOR
                Debug.Log($"Weapon_Status 삽입 성공 : {callback}");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError($"Weapon_Status 삽입 실패 : {callback}");
#endif
                return;
            }
        });
    }
    /// <summary>
    /// 진행도 값 삽입
    /// </summary>
    private void Insert_UserProgress()
    {
        Cleardatas.ResetProgress();                                                                 // 데이터 초기화

        Param param_p = new Param()                                                                 // 초기화 한(기본값) 데이터 테이블에 삽입
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

        SendQueue.Enqueue(Backend.GameData.Insert, "Progress_Chart", param_p, callback =>           // 뒤끝 서버에 데이터 삽입
        {
            if (callback.IsSuccess())                                                               // 성공 시
            {
                ClearDataRowInDate = callback.GetInDate();                                          // 고유 값 저장
#if UNITY_EDITOR
                Debug.Log($"Progress 삽입 성공 : {callback}");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError($"Progress 삽입 실패 : {callback}");
#endif
                return;
            }
        });
    }
    /// <summary>
    /// Date 삽입
    /// </summary>
    public void InsertLifeData_()
    {
        GetServer_Synchronous();                                                                    // 서버 시간 불러오기

        Lifedate.LifeDate = Lifedate.UTCDate;                                                       // 현재 서버 시간을 Life 생성 시간에 삽입

        Param param_date = new Param()                                                              // Life 생선 시간 변수 값 삽입
       {
            {"Date", Lifedate.LifeDate}
       };

        SendQueue.Enqueue(Backend.GameData.Insert, "Life_Date", param_date, callback =>             // 뒤끝 서버에 데이터 삽입
        {
            if (callback.IsSuccess())                                                               // 성공 시
            {
                DateRowInDate = callback.GetInDate();                                               // 고유 값 저장
#if UNITY_EDITOR
                Debug.Log($"Life_Date 삽입 성공 : {callback}");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError($"Life_Date 삽입 실패 : {callback}");
#endif
                return;
            }
        });
    }
    /// <summary>
    /// 각 데이터 오류검사 후 가져오기
    /// </summary>
    /// <returns></returns>
    public string GetDatas()
    {
        string message = string.Empty;

        message = GetUserData();                                                                    // 데이터 가져온 후

        if (message != "")                                                                          // 오류 미발생 시 공백 문자 return, 아니라면 문자 발생(오류 발생)
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
    /// User 데이터 가져오기
    /// </summary>
    /// <returns></returns>
    private string GetUserData()
    {
        SendQueue.Enqueue(Backend.GameData.Get, "Player_Data", new Where(), callback =>                     //inDate 값(생성 된 고유 값) 가져오기
        {
            userDataRowInDate = callback.GetInDate();
        });

        string message = string.Empty;

        SendQueue.Enqueue(Backend.GameData.GetMyData, "Player_Data", new Where(), callback =>               // 현재 로그인한 사용자의 테이블 데이터 가져오기
        {
            if (callback.IsSuccess())                                                                       // 불러오기 성공 시
            {
                try                                                                                         // 불러온 정보 파싱, 반영
                {
                    LitJson.JsonData gameDataJson = callback.FlattenRows();

                    if (gameDataJson.Count <= 0)                                                            // 데이터가 없는 경우
                    {
#if UNITY_EDITOR
                        Debug.LogError("데이터가 존재하지 않습니다. 새로운 데이터를 생성합니다.");
#endif
                        Insert_UserData();                                                                  // 초기 데이터 생성
                    }
                    #region 불러온 데이터 삽입
                    Userdatas.Level = int.Parse(gameDataJson[0]["Level"].ToString());
                    Userdatas.Coin = int.Parse(gameDataJson[0]["Coin"].ToString());
                    Userdatas.Life = int.Parse(gameDataJson[0]["Life"].ToString());
                    Userdatas.NowExp = float.Parse(gameDataJson[0]["Nowexp"].ToString());
                    Userdatas.Next_Exp_value = float.Parse(gameDataJson[0]["Next_Exp_value"].ToString());
                    Userdatas.Next_Exp_UP = float.Parse(gameDataJson[0]["Next_Exp_UP"].ToString());
                    Userdatas.Exp_Multiplication = float.Parse(gameDataJson[0]["Exp_Multiplication"].ToString());
                    Userdatas.WeaponNumber = int.Parse(gameDataJson[0]["WeaponNumber"].ToString());
                    #endregion
                    On_userdata = true;                                                                     // 불러오기 성공
                }
                catch(System.Exception e)                                                                   // 파싱 실패
                {
                    Userdatas.ResetData();                                                                  // 초기값으로 설정
#if UNITY_EDITOR
                    Debug.LogError(e);
#endif
                    message = "서버 에러 발생 초기화면으로 이동합니다.(user_data_)";                        // 에러 메시지 출력
                    
                    SendQueue.Enqueue(Backend.BMember.Logout, callback =>                                   // 토큰 만료 및 Login 데이터 삭제
                    {
                        if (!callback.IsSuccess())
                            callback.GetMessage();
                    });
                }
            }
            else                                                                                            // 불러오기 실패
            {
#if UNITY_EDITOR
                Debug.LogError(callback);
#endif
                message = "서버 에러 발생 초기화면으로 이동합니다.(user_data_)";                            // 에러 메시지 출력
                
                SendQueue.Enqueue(Backend.BMember.Logout, callback =>                                       // 토큰 만료 및 Login 데이터 삭제
                {
                    if (!callback.IsSuccess())                                                              // 로그아웃 실패(메시지 출력)/로그아웃 성공(토큰 삭제)
                        callback.GetMessage();
                });
            }
        });

        return message;                                                                                     // 오류 검사를 위한 메시지 return
    }
    /// <summary>
    /// 진행도 데이터 가져오기
    /// </summary>
    /// <returns></returns>
    public string GetProgress()
    {
        SendQueue.Enqueue(Backend.GameData.Get, "Progress_Chart", new Where(), callback =>                  //inDate 값(생성 된 고유 값) 가져오기
        {
            ClearDataRowInDate = callback.GetInDate();
        });

        string message = string.Empty;

        SendQueue.Enqueue(Backend.GameData.GetMyData, "Progress_Chart", new Where(), callback =>            // 현재 로그인한 사용자의 테이블 데이터 가져오기
        {
            if (callback.IsSuccess())                                                                       // 불러오기 성공
            {
                try                                                                                         // 불러온 정보 파싱, 반영
                {
                    LitJson.JsonData gameDataJson = callback.FlattenRows();

                    if (callback.GetReturnValuetoJSON()["rows"].Count <= 0)                                 // 데이터가 없는 경우
                    {
#if UNITY_EDITOR
                        Debug.LogError("데이터가 존재하지 않습니다. 새로운 데이터를 생성합니다.");
#endif
                        Insert_UserProgress();                                                              // 초기 데이터 생성
                    }
                    #region 불러온 데이터 삽입
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
                    On_cleardata = true;                                                                    // 불러오기 성공
                }
                catch (System.Exception e)                                                                  // 파싱 실패
                {
                    Cleardatas.ResetProgress();                                                             // 초기값으로 설정
#if UNITY_EDITOR
                    Debug.LogError(e);
#endif
                    message = "서버 에러 발생 초기화면으로 이동합니다.(Progress_data_)";                    // 에러 메시지 출력

                    SendQueue.Enqueue(Backend.BMember.Logout, callback =>                                   // 토큰 만료 및 Login 데이터 삭제
                    {
                        if (!callback.IsSuccess())                                                          // 로그아웃 실패(메시지 출력)/로그아웃 성공(토큰 삭제)
                            callback.GetMessage();
                    });
                }
            }
            else                                                                                            // 불러오기 실패
            {
#if UNITY_EDITOR
                Debug.LogError(callback);
#endif
                message = "서버 에러 발생 초기화면으로 이동합니다.(Progress_data_)";                        // 에러 메시지 출력

                SendQueue.Enqueue(Backend.BMember.Logout, callback =>                                       // 토큰 만료 및 Login 데이터 삭제
                {
                    if (!callback.IsSuccess())                                                              // 로그아웃 실패(메시지 출력)/로그아웃 성공(토큰 삭제)
                        callback.GetMessage();
                });
            }
        });

        return message;                                                                                     // 오류 검사를 위한 메시지 return
    }
    /// <summary>
    /// Status 데이터 가져오기
    /// </summary>
    /// <returns></returns>
    public string GetStatus()
    {
        SendQueue.Enqueue(Backend.GameData.Get, "Status_", new Where(), callback =>                         //inDate 값(생성 된 고유 값) 가져오기
        {
            statusDataRowInDate = callback.GetInDate();
        });

        string message = string.Empty;

        SendQueue.Enqueue(Backend.GameData.GetMyData, "Status_", new Where(), callback =>                   // 현재 로그인한 사용자의 테이블 데이터 가져오기
        {
            if (callback.IsSuccess())                                                                       // 불러오기 성공
            {
                try                                                                                         // 불러온 정보 파싱, 반영
                {
                    LitJson.JsonData gameDataJson = callback.FlattenRows();

                    if (callback.GetReturnValuetoJSON()["rows"].Count <= 0)                                 // 데이터가 없는 경우
                    {
#if UNITY_EDITOR
                        Debug.LogError("데이터가 존재하지 않습니다. 새로운 데이터를 생성합니다.");
#endif
                        Insert_UserStatus();                                                                // 초기 데이터 생성
                    }
                    #region 불러온 데이터 삽입
                    Userstatusdatas.Max_HP = float.Parse(gameDataJson[0]["Max_Hp"].ToString());
                    Userstatusdatas.Attack_Power = float.Parse(gameDataJson[0]["Attack_Power"].ToString());
                    Userstatusdatas.Deffensive_Power = float.Parse(gameDataJson[0]["Deffensive_Power"].ToString());
                    Userstatusdatas.MoveSpeed = float.Parse(gameDataJson[0]["MoveSpeed"].ToString());
                    Userstatusdatas.AttackSpeed = float.Parse(gameDataJson[0]["AttackSpeed"].ToString());
                    #endregion
                    On_statusdata = true;                                                                   // 불러오기 성공
                }
                catch (System.Exception e)                                                                  // 파싱 실패
                {
                    Userstatusdatas.ResetStatus();                                                          // 초기값으로 설정
#if UNITY_EDITOR
                    Debug.LogError(e);
#endif
                    message = "서버 에러 발생 초기화면으로 이동합니다.(Status_data_)";                      // 에러 메시지 출력

                    SendQueue.Enqueue(Backend.BMember.Logout, callback =>                                   // 토큰 만료 및 Login 데이터 삭제
                    {
                        if (!callback.IsSuccess())                                                          // 로그아웃 실패(메시지 출력)/로그아웃 성공(토큰 삭제)
                            callback.GetMessage();
                    });
                }
            }
            else                                                                                            // 불러오기 실패
            {
#if UNITY_EDITOR
                Debug.LogError(callback);
#endif
                message = "서버 에러 발생 초기화면으로 이동합니다.(Status_data_)";                          // 에러 메시지 출력
                
                SendQueue.Enqueue(Backend.BMember.Logout, callback =>                                       // 토큰 만료 및 Login 데이터 삭제
                {
                    if (!callback.IsSuccess())                                                              // 로그아웃 실패(메시지 출력)/로그아웃 성공(토큰 삭제)
                        callback.GetMessage();
                });
            }
        });

        return message;                                                                                     // 오류 검사를 위한 메시지 return
    }
    /// <summary>
    /// Weapon 데이터 불러오기
    /// </summary>
    /// <returns></returns>
    public string GetWeapon()
    {
        SendQueue.Enqueue(Backend.GameData.Get, "Weapon_Status", new Where(), callback =>                   //inDate 값(생성 된 고유 값) 가져오기
        {
            WeaponLVDataRowInDate = callback.GetInDate();
        });

        string message = string.Empty;

        SendQueue.Enqueue(Backend.GameData.GetMyData, "Weapon_Status", new Where(), callback =>             // 현재 로그인한 사용자의 테이블 데이터 가져오기
        {
            if (callback.IsSuccess())                                                                       // 불러오기 성공
            {
                try                                                                                         // 불러온 정보 파싱, 반영
                {
                    LitJson.JsonData gameDataJson = callback.FlattenRows();

                    if (callback.GetReturnValuetoJSON()["rows"].Count <= 0)                                 // 데이터가 없는 경우
                    {
#if UNITY_EDITOR
                        Debug.LogError("데이터가 존재하지 않습니다. 새로운 데이터를 생성합니다.");
#endif
                        Insert_UserWeaponLevel();                                                           // 초기 데이터 생성
                    }
                    #region 불러온 데이터 삽입
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
                    On_weapondata = true;                                                                   // 불러오기 성공
                }
                catch (System.Exception e)                                                                  // 파싱 실패
                {
                    Userweaponlevel.ResetWeapons();                                                         // 데이터 초기값 설정
#if UNITY_EDITOR
                    Debug.LogError(e);
#endif
                    message = "서버 에러 발생 초기화면으로 이동합니다.(Weapon_data_)";                      // 에러 메시지 출력

                    SendQueue.Enqueue(Backend.BMember.Logout, callback =>                                   // 토큰 만료 및 Login 데이터 삭제
                    {
                        if (!callback.IsSuccess())                                                          // 로그아웃 실패(메시지 출력)/로그아웃 성공(토큰 삭제)
                            callback.GetMessage();
                    });
                }
            }
            else                                                                                            // 불러오기 실패
            {
#if UNITY_EDITOR
                Debug.LogError(callback);
#endif           
                message = "서버 에러 발생 초기화면으로 이동합니다.(Weapon_data_)";                          // 에러 메시지 출력

                SendQueue.Enqueue(Backend.BMember.Logout, callback =>                                       // 토큰 만료 및 Login 데이터 삭제
                {
                    if (!callback.IsSuccess())                                                              // 로그아웃 실패(메시지 출력)/로그아웃 성공(토큰 삭제)
                        callback.GetMessage();
                });
            }
        });

        return message;                                                                                     // 오류 검사를 위한 메시지 return
    }
    /// <summary>
    /// Upgrade 데이터 불러오기
    /// </summary>
    /// <returns></returns>
    public string GetUpgraede_S()
    {
        SendQueue.Enqueue(Backend.GameData.Get, "Upgrade_Status", new Where(), callback =>                  //inDate 값(생성 된 고유 값) 가져오기
        {
            statusLVDataRowInDate = callback.GetInDate();
        });

        string message = string.Empty;

        SendQueue.Enqueue(Backend.GameData.GetMyData, "Upgrade_Status", new Where(), callback =>           // 현재 로그인한 사용자의 테이블 데이터 가져오기
        {
            if (callback.IsSuccess())                                                                      // 불러오기 성공
            {
                try                                                                                        // 불러온 정보 파싱, 반영
                {
                    LitJson.JsonData gameDataJson = callback.FlattenRows();

                    if (callback.GetReturnValuetoJSON()["rows"].Count <= 0)                                // 데이터가 없는 경우
                    {
#if UNITY_EDITOR
                        Debug.LogError("데이터가 존재하지 않습니다. 새로운 데이터를 생성합니다.");
#endif    
                        Insert_UserStatusLevel();                                                          // 초기 데이터 생성
                    }
                    #region 불러온 데이터 삽입
                    Userstatuslevel.AP = Int32.Parse(gameDataJson[0]["AttackPower"].ToString());
                    Userstatuslevel.AS = Int32.Parse(gameDataJson[0]["AttackSpeed"].ToString());
                    Userstatuslevel.DP = Int32.Parse(gameDataJson[0]["DeffensivePower"].ToString());
                    Userstatuslevel.PH = Int32.Parse(gameDataJson[0]["PhysicalPower"].ToString());
                    Userstatuslevel.MS = Int32.Parse(gameDataJson[0]["MoveSpeed"].ToString());
                    #endregion
                    On_statusLvdata = true;                                                                // 불러오기 성공
                }
                catch (System.Exception e)                                                                 // 파싱 실패
                {
                    Userstatuslevel.ResetStatusLevel();                                                    // 데이터 초기값 설정
#if UNITY_EDITOR
                    Debug.LogError(e);
#endif  
                    message = "서버 에러 발생 초기화면으로 이동합니다.(StatusLV_data_)";                   // 에러 메시지 출력

                    SendQueue.Enqueue(Backend.BMember.Logout, callback =>                                  // 토큰 만료 및 Login 데이터 삭제
                    {
                        if (!callback.IsSuccess())                                                         // 로그아웃 실패(메시지 출력)/로그아웃 성공(토큰 삭제)
                            callback.GetMessage();
                    });
                }
            }
            else                                                                                           // 불러오기 실패
            {
#if UNITY_EDITOR
                Debug.LogError(callback);
#endif
                message = "서버 에러 발생 초기화면으로 이동합니다.(StatusLV_data_)";                       // 에러 메시지 출력

                SendQueue.Enqueue(Backend.BMember.Logout, callback =>                                      // 토큰 만료 및 Login 데이터 삭제
                {
                    if (!callback.IsSuccess())                                                             // 로그아웃 실패(메시지 출력)/로그아웃 성공(토큰 삭제)
                        callback.GetMessage();
                });
            }
        });

        return message;                                                                                    // 오류 검사를 위한 메시지 return
    }
    /// <summary>
    /// Life 생성 데이트 값 가져오기
    /// </summary>
    public void GetLifeData()
    {
        SendQueue.Enqueue(Backend.GameData.Get, "Life_Date", new Where(), callback =>                      //inDate 값(생성 된 고유 값) 가져오기
        {
            DateRowInDate = callback.GetInDate();
        });

        string message = string.Empty;

        SendQueue.Enqueue(Backend.GameData.GetMyData, "Life_Date", new Where(), callback =>                // 현재 로그인한 사용자의 테이블 데이터 가져오기
        {
            if (callback.IsSuccess())                                                                      // 불러오기 성공
            {
                try                                                                                        // 불러온 정보 파싱, 반영
                {
                    LitJson.JsonData gameDataJson = callback.FlattenRows();

                    if (gameDataJson.Count <= 0)                                                           // 데이터가 없는 경우
                    {
#if UNITY_EDITOR
                        Debug.LogError("데이터가 존재하지 않습니다. 새로운 데이터를 생성합니다.");
#endif
                        InsertLifeData_();                                                                 // 초기 데이터 생성
                    }
                    Lifedate.LifeDate = DateTime.Parse(gameDataJson[0]["Date"].ToString());

                    On_lifedate = true;                                                                    // 불러오기 성공
                }
                catch (System.Exception e)                                                                 // 파싱 실패
                {
                    GetServer_Synchronous();                                                               // 새롭게 서버 시간 동기화
                    Lifedate.LifeDate = Lifedate.UTCDate;
#if UNITY_EDITOR
                    Debug.LogError(e);
#endif
                    message = "서버 에러 발생 초기화면으로 이동합니다.(life_data_)";                       // 에러 메시지 출력

                    SendQueue.Enqueue(Backend.BMember.Logout, callback =>                                  // 토큰 만료 및 Login 데이터 삭제
                    {
                        if (!callback.IsSuccess())
                            callback.GetMessage();
                    });
                }
            }
            else                                                                                           // 불러오기 실패
            {
#if UNITY_EDITOR
                Debug.LogError(callback);
#endif
                message = "서버 에러 발생 초기화면으로 이동합니다.(life_data_)";                            // 에러 메시지 출력

                SendQueue.Enqueue(Backend.BMember.Logout, callback =>                                       // 토큰 만료 및 Login 데이터 삭제
                {
                    if (!callback.IsSuccess())                                                              // 로그아웃 실패(메시지 출력)/로그아웃 성공(토큰 삭제)
                        callback.GetMessage();
                });
            }
        });
    }
    /// <summary>
    /// User 데이터 갱신(수정)
    /// </summary>
    /// <param name="action"></param>
    public void UpdateUserDatas_(UnityAction action=null)
    {
        if (userdatas == null)
        {
#if UNITY_EDITOR
            Debug.LogError("데이터가 존재하지 않습니다." + "Insert or Load 필요");
#endif
        }

        Param param_d = new Param()                                                                        // 갱신한 데이터 삽입 준비
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
        if (string.IsNullOrEmpty(userDataRowInDate))                                                       // data 고유값 없을 시 에러 출력
        {
#if UNITY_EDITOR
            Debug.LogError("유저의 inDate 정보없습니다. 재로드 합니다.");
#endif
            SendQueue.Enqueue(Backend.GameData.Get, "Player_Data", new Where(), callback =>                // inDate 값 로드
            {
                userDataRowInDate = callback.GetInDate();
            });
        }
        /// <summary>
        /// 게임 데이터의 고유값이 있을 시, 테이블에 저장되어 있는 값 중 inDate 칼럼의 값과,
        /// 소유하는 유저의 owner_inDate가 일치하는 row를 검색해 수정
        /// owner_inDate는 뒤끝 콘솔에서 확인가능
        /// </summary>
        else
        {
#if UNITY_EDITOR
            Debug.Log($"{userDataRowInDate}의 게임 정보 데이터 수정 요청");
#endif
            SendQueue.Enqueue(Backend.GameData.UpdateV2, "Player_Data", userDataRowInDate, Backend.UserInDate, param_d, callback =>         // 데이터 갱신 요청
            {
                if (callback.IsSuccess())                                                                   // 수정 성공
                {
#if UNITY_EDITOR
                    Debug.Log($"데이터 수정 성공 : {callback}");
#endif
                    action?.Invoke();                                                                       // 후에 취할 행동이 있다면 진행
                }
                else                                                                                        // 수정 실패
                {
#if UNITY_EDITOR
                    Debug.LogError($"데이터 수정 실패 : {callback}");
#endif
                }
            });
        }
    }
    /// <summary>
    /// Status 데이터 갱신
    /// </summary>
    /// <param name="action"></param>
    public void UpdateStatusDatas_(UnityAction action = null)
    {
        if (userstatusdatas == null)
        {
#if UNITY_EDITOR
            Debug.LogError("데이터가 존재하지 않습니다." + "Insert or Load 필요");
#endif
        }

        Param param_s_ = new Param()                                                                       // 갱신한 데이터 삽입 준비
        {
            {"Max_Hp", Userstatusdatas.Max_HP},
            {"Attack_Power", Userstatusdatas.Attack_Power},
            {"Deffensive_Power", Userstatusdatas.Deffensive_Power},
            {"MoveSpeed", Userstatusdatas.MoveSpeed},
            {"AttackSpeed", Userstatusdatas.AttackSpeed}
        };
        if (string.IsNullOrEmpty(statusDataRowInDate))                                                     // data 고유값 없을 시 에러 출력
        {
#if UNITY_EDITOR
            Debug.LogError("유저의 inDate 정보없습니다. 재로드 합니다.");
#endif
            SendQueue.Enqueue(Backend.GameData.Get, "Status_", new Where(), callback =>                    // inDate 값 로드
            {
                statusDataRowInDate = callback.GetInDate();
            });
        }
        /// <summary>
        /// 게임 데이터의 고유값이 있을 시, 테이블에 저장되어 있는 값 중 inDate 칼럼의 값과,
        /// 소유하는 유저의 owner_inDate가 일치하는 row를 검색해 수정
        /// owner_inDate는 뒤끝 콘솔에서 확인가능
        /// </summary>
        else
        {
#if UNITY_EDITOR
            Debug.Log($"{statusDataRowInDate}의 게임 정보 데이터 수정 요청");
#endif
            SendQueue.Enqueue(Backend.GameData.UpdateV2, "Status_", statusDataRowInDate, Backend.UserInDate, param_s_, callback =>         // 데이터 갱신 요청
            {
                if (callback.IsSuccess())                                                                  // 수정 성공
                {
#if UNITY_EDITOR
                    Debug.Log($"데이터 수정 성공 : {callback}");
#endif
                    action?.Invoke();                                                                      // 후에 취할 행동이 있다면 진행
                }
                else                                                                                       // 수정 실패
                {
#if UNITY_EDITOR
                    Debug.LogError($"데이터 수정 실패 : {callback}");
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
            Debug.LogError("데이터가 존재하지 않습니다." + "Insert or Load 필요");
#endif
        }
        Param param_s = new Param()                                                                        // 갱신한 데이터 삽입 준비
        {
            {"AttackPower", Userstatuslevel.AP},
            {"DeffensivePower", Userstatuslevel.DP},
            {"PhysicalPower", Userstatuslevel.PH},
            {"AttackSpeed", Userstatuslevel.AS},
            {"MoveSpeed", Userstatuslevel.MS}
        };
        if (string.IsNullOrEmpty(statusLVDataRowInDate))                                                  // data 고유값 없을 시 에러 출력
        {
#if UNITY_EDITOR
            Debug.LogError("유저의 inDate 정보없습니다. 재로드 합니다.");
#endif
            SendQueue.Enqueue(Backend.GameData.Get, "Upgrade_Status", new Where(), callback =>            // inDate 값 로드
            {
                statusLVDataRowInDate = callback.GetInDate();
            });
        }
        /// <summary>
        /// 게임 데이터의 고유값이 있을 시, 테이블에 저장되어 있는 값 중 inDate 칼럼의 값과,
        /// 소유하는 유저의 owner_inDate가 일치하는 row를 검색해 수정
        /// owner_inDate는 뒤끝 콘솔에서 확인가능
        /// </summary>
        else
        {
#if UNITY_EDITOR
            Debug.Log($"{statusLVDataRowInDate}의 게임 정보 데이터 수정 요청");
#endif
            SendQueue.Enqueue(Backend.GameData.UpdateV2, "Upgrade_Status", statusLVDataRowInDate, Backend.UserInDate, param_s, callback =>         // 데이터 갱신 요청
            {
                if (callback.IsSuccess())                                                                  // 수정 성공
                {
#if UNITY_EDITOR
                    Debug.Log($"데이터 수정 성공 : {callback}");
#endif
                    action?.Invoke();                                                                      // 후에 취할 행동이 있다면 진행
                }
                else                                                                                       // 수정 실패
                {
#if UNITY_EDITOR
                    Debug.LogError($"데이터 수정 실패 : {callback}");
#endif
                }
            });
        }
    }
    /// <summary>
    /// Weapon 데이터 갱신
    /// </summary>
    /// <param name="action"></param>
    public void UpdateWeaponLVDatas_(UnityAction action = null)
    {
        if (userweaponlevel == null)
        {
#if UNITY_EDITOR
            Debug.LogError("데이터가 존재하지 않습니다." + "Insert or Load 필요");
#endif
        }
        Param param_ws = new Param()                                                                       // 갱신한 데이터 삽입 준비
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
        // data 고유값 없을 시 에러 출력
        if (string.IsNullOrEmpty(WeaponLVDataRowInDate))
        {
#if UNITY_EDITOR
            Debug.LogError("유저의 inDate 정보없습니다. 재로드 합니다.");
#endif
            SendQueue.Enqueue(Backend.GameData.Get, "Weapon_Status", new Where(), callback =>            // inDate 값 로드
            {
                WeaponLVDataRowInDate = callback.GetInDate();
            });
        }
        /// <summary>
        /// 게임 데이터의 고유값이 있을 시, 테이블에 저장되어 있는 값 중 inDate 칼럼의 값과,
        /// 소유하는 유저의 owner_inDate가 일치하는 row를 검색해 수정
        /// owner_inDate는 뒤끝 콘솔에서 확인가능
        /// </summary>
        else
        {
#if UNITY_EDITOR
            Debug.Log($"{WeaponLVDataRowInDate}의 게임 정보 데이터 수정 요청");
#endif
            SendQueue.Enqueue(Backend.GameData.UpdateV2, "Weapon_Status", WeaponLVDataRowInDate, Backend.UserInDate, param_ws, callback =>         // 데이터 갱신 요청
            {
                if (callback.IsSuccess())                                                               // 수정 성공
                {
#if UNITY_EDITOR
                    Debug.Log($"데이터 수정 성공 : {callback}");
#endif
                    action?.Invoke();                                                                   // 후에 취할 행동이 있다면 진행
                }
                else                                                                                    // 수정 실패
                {
#if UNITY_EDITOR
                    Debug.LogError($"데이터 수정 실패 : {callback}");
#endif
                }
            });
        }
    }
    /// <summary>
    /// 진행도 데이터 갱신
    /// </summary>
    /// <param name="action"></param>
    public void UpdateClearDatas_(UnityAction action = null)
    {
        if (Cleardatas == null)
        {
#if UNITY_EDITOR
            Debug.LogError("데이터가 존재하지 않습니다." + "Insert or Load 필요");
#endif
        }
        Param param_p = new Param()                                                                     // 갱신한 데이터 삽입 준비
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
        // data 고유값 없을 시 에러 출력
        if (string.IsNullOrEmpty(ClearDataRowInDate))
        {
#if UNITY_EDITOR
            Debug.LogError("유저의 inDate 정보없습니다. 재로드 합니다.");
#endif
            SendQueue.Enqueue(Backend.GameData.Get, "Progress_Chart", new Where(), callback =>           // inDate 값 로드
            {
                ClearDataRowInDate = callback.GetInDate();
            });
        }
        /// <summary>
        /// 게임 데이터의 고유값이 있을 시, 테이블에 저장되어 있는 값 중 inDate 칼럼의 값과,
        /// 소유하는 유저의 owner_inDate가 일치하는 row를 검색해 수정
        /// owner_inDate는 뒤끝 콘솔에서 확인가능
        /// </summary>
        else
        {
#if UNITY_EDITOR
            Debug.Log($"{ClearDataRowInDate}의 게임 정보 데이터 수정 요청");
#endif
            SendQueue.Enqueue(Backend.GameData.UpdateV2, "Progress_Chart", ClearDataRowInDate, Backend.UserInDate, param_p, callback =>         // 데이터 갱신 요청
            {
                if (callback.IsSuccess())                                                               // 수정 성공
                {
#if UNITY_EDITOR
                    Debug.Log($"데이터 수정 성공 : {callback}");
#endif
                    action?.Invoke();                                                                   // 후에 취할 행동이 있다면 진행
                }
                else                                                                                    // 수정 실패
                {
#if UNITY_EDITOR
                    Debug.LogError($"데이터 수정 실패 : {callback}");
#endif
                }
            });
        }
    }
    /// <summary>
    /// Life 생성 Date 갱신
    /// </summary>
    /// <param name="action"></param>
    public void UpdateLifeData_(UnityAction action = null)
    {
        if (Lifedate== null)
        {
#if UNITY_EDITOR
            Debug.LogError("데이터가 존재하지 않습니다." + "Insert or Load 필요");
#endif
        }
        Lifedate.LifeDate = Lifedate.UTCDate;                                                           // 현재 시간 Life 생성 시간으로 반영

        Param param_date = new Param()                                                                  // 갱신할 데이터 삽입 준비
       {
            {"Date", Lifedate.LifeDate},
       };

        if (string.IsNullOrEmpty(DateRowInDate))                                                        // data 고유값 없을 시 에러 출력
        {
#if UNITY_EDITOR
            Debug.LogError("유저의 inDate 정보가 없습니다. 재로드 합니다.");
#endif
            SendQueue.Enqueue(Backend.GameData.Get, "Life_Date", new Where(), callback =>               // inDate 값 로드
            {
                DateRowInDate = callback.GetInDate();
            });

            GetServer_Synchronous();
            Lifedate.LifeDate = Lifedate.UTCDate;
        }
        /// <summary>
        /// 게임 데이터의 고유값이 있을 시, 테이블에 저장되어 있는 값 중 inDate 칼럼의 값과,
        /// 소유하는 유저의 owner_inDate가 일치하는 row를 검색해 수정
        /// owner_inDate는 뒤끝 콘솔에서 확인가능
        /// </summary>
        else
        {
#if UNITY_EDITOR
            Debug.Log($"{userDataRowInDate}의 게임 정보 데이터 수정 요청");
#endif
            SendQueue.Enqueue(Backend.GameData.UpdateV2, "Life_Date", DateRowInDate, Backend.UserInDate, param_date, callback =>         // 데이터 갱신 요청
            {
                if (callback.IsSuccess())                                                               // 수정 성공
                {
#if UNITY_EDITOR
                    Debug.Log($"데이터 수정 성공 : {callback}");
#endif
                    action?.Invoke();                                                                   // 후에 취할 행동이 있다면 진행
                }
                else // 수정 실패
                {
#if UNITY_EDITOR
                    Debug.LogError($"데이터 수정 실패 : {callback}");
#endif
                }
            });
        }
    }
    /// <summary>
    /// 서버 시간 불러오기(동기)
    /// </summary>
    public void GetServer_Synchronous()
    {
        BackendReturnObject servertime = Backend.Utils.GetServerTime();                                                                 // 서버에 서버 시간 요청

        string time = servertime.GetReturnValuetoJSON()["utcTime"].ToString();                                                          // 가져온 시간을 서버 시간을 위해 만든 변수에 저장
        Lifedate.UTCDate = DateTime.Parse(time);
    }
}
