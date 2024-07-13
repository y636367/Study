using UnityEngine;
using BackEnd;
using LitJson;

public class User_Info : MonoBehaviour
{
    [System.Serializable]
    public class UserInfoEvent : UnityEngine.Events.UnityEvent { }                                      // ���� ���� �ҷ����µ� ������ ȣ���� �޼ҵ� ���
    public UserInfoEvent onUserInfoEvent=new UserInfoEvent();                                           // Ŭ���� �ν��Ͻ� ����

    private static UserInfoData data = new UserInfoData();                                              // ���� �α����� ������ ���� ����
    public static UserInfoData Data => data;                                                            // �ܺο��� ���� �����ϰ� Get ������Ƽ ����

    public void GetUserInfoFromBackend()
    {
        SendQueue.Enqueue(Backend.BMember.GetUserInfo, callback =>                                      // ���� �α����� ����� ���� �ҷ�����
        {
            if (callback.IsSuccess())                                                                   // �ҷ����� ����
            {
                try                                                                                     // Json ������ �Ľ� ����
                {
                    JsonData json = callback.GetReturnValuetoJSON()["row"];                             // �ҷ��� ���� �Ľ�
                    #region ����� ����
                    data.UserID = json["gamerId"].ToString();
                    data.countryCode = json["countryCode"]?.ToString();
                    data.NickName = json["nickname"]?.ToString();
                    data.inDate = json["inDate"].ToString();
                    data.emailForFindPassword = json["emailForFindPassword"]?.ToString();
                    data.subscriptionType = json["subscriptionType"].ToString();
                    data.federationID = json["federationId"]?.ToString();
                    #endregion
                }
                catch (System.Exception e)                                                              // �Ľ� ����
                {
                    data.Reset();                                                                       // ���� ���� �⺻ ���·� ����
#if UNITY_EDITOR
                    Debug.LogError(e);                                                                  // try-catch ���� ���
#endif
                }
            }
            else                                                                                        // �ҷ����� ����
            {
                data.Reset();                                                                           // ���� ���� �⺻ ���·� ����
#if UNITY_EDITOR
                Debug.LogError(callback.GetMessage());                                                  // �������� ���� ���, �⺻���� ���� �����صΰ� ���������϶� �ҷ��ͼ� ��� (?)
#endif
            }
            onUserInfoEvent.Invoke();                                                                   // ���� ���� �ҷ����� �Ϸ��, onUserInfoEvent�� ��ϵ� �̺�Ʈ �޼ҵ� ȣ��
        });
    }
}
public class UserInfoData                                                                              // User�� ������ �ڵ����� ��ϵ� �⺻���� ������
{
    public string UserID;                                                                              // ���� ���̵�
    public string countryCode;                                                                         // �����ڵ� �� ������ null
    public string NickName;                                                                            // �г��� �� ������ ���� ���̵�
    public string inDate;                                                                              // ������ ���ӳ�¥
    public string emailForFindPassword;                                                                // �̸��� �ּ�
    public string subscriptionType;                                                                    // Ŀ����, ������̼� Ÿ��
    public string federationID;                                                                        // ������̼� ID

    /// <summary>
    /// �⺻ ������ �ʱ�ȭ
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
