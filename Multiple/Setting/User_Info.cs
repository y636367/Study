using UnityEngine;
using BackEnd;
using LitJson;

public class User_Info : MonoBehaviour
{
    [System.Serializable]
    public class UserInfoEvent : UnityEngine.Events.UnityEvent { }                                      // 유저 정보 불러오는데 성공시 호출할 메소드 등록
    public UserInfoEvent onUserInfoEvent=new UserInfoEvent();                                           // 클래스 인스턴스 선언

    private static UserInfoData data = new UserInfoData();                                              // 현재 로그인한 유저의 정보 저장
    public static UserInfoData Data => data;                                                            // 외부에서 접근 가능하게 Get 프로퍼티 정의

    public void GetUserInfoFromBackend()
    {
        SendQueue.Enqueue(Backend.BMember.GetUserInfo, callback =>                                      // 현재 로그인한 사용자 정보 불러오기
        {
            if (callback.IsSuccess())                                                                   // 불러오기 성공
            {
                try                                                                                     // Json 데이터 파싱 성공
                {
                    JsonData json = callback.GetReturnValuetoJSON()["row"];                             // 불러온 정보 파싱
                    #region 사용자 정보
                    data.UserID = json["gamerId"].ToString();
                    data.countryCode = json["countryCode"]?.ToString();
                    data.NickName = json["nickname"]?.ToString();
                    data.inDate = json["inDate"].ToString();
                    data.emailForFindPassword = json["emailForFindPassword"]?.ToString();
                    data.subscriptionType = json["subscriptionType"].ToString();
                    data.federationID = json["federationId"]?.ToString();
                    #endregion
                }
                catch (System.Exception e)                                                              // 파싱 실패
                {
                    data.Reset();                                                                       // 유저 정보 기본 상태로 설정
#if UNITY_EDITOR
                    Debug.LogError(e);                                                                  // try-catch 에러 출력
#endif
                }
            }
            else                                                                                        // 불러오기 실패
            {
                data.Reset();                                                                           // 유저 정보 기본 상태로 설정
#if UNITY_EDITOR
                Debug.LogError(callback.GetMessage());                                                  // 오프라인 상태 대비, 기본적인 정보 저장해두고 오프라인일때 불러와서 사용 (?)
#endif
            }
            onUserInfoEvent.Invoke();                                                                   // 유저 정보 불러오기 완료시, onUserInfoEvent에 등록된 이벤트 메소드 호출
        });
    }
}
public class UserInfoData                                                                              // User의 서버에 자동으로 등록된 기본적인 데이터
{
    public string UserID;                                                                              // 유저 아이디
    public string countryCode;                                                                         // 국가코드 미 설정시 null
    public string NickName;                                                                            // 닉네임 미 설정시 유저 아이디
    public string inDate;                                                                              // 유저의 접속날짜
    public string emailForFindPassword;                                                                // 이메일 주소
    public string subscriptionType;                                                                    // 커스텀, 페더레이션 타입
    public string federationID;                                                                        // 페더레이션 ID

    /// <summary>
    /// 기본 데이터 초기화
    /// </summary>
    public void Reset()
    {
        UserID = "Offline";
        countryCode = "Unknown";
        NickName = UserID;
        inDate = string.Empty;
        emailForFindPassword = string.Empty;
        subscriptionType = string.Empty;
        federationID = string.Empty;
    }
}
