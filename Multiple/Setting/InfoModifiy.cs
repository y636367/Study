using UnityEngine;
using UnityEngine.UI;
using BackEnd;

public class InfoModifiy : Login_Base
{
    [System.Serializable]
    public class UpdateInfoEvent : UnityEngine.Events.UnityEvent { }                                            // UnityEngin의 이벤트 클래스 정의
    public UpdateInfoEvent updateinfo = new UpdateInfoEvent();                                                  // 인스턴스를 다른 이벤트 종료 후 실행되게끔 정의_1
    public UpdateInfoEvent updateinfo_2 = new UpdateInfoEvent();                                                // 인스턴스를 다른 이벤트 종료 후 실행되게끔 정의_2

    public enum Value { Nickname, Password, Email }                                                             // 수정할 타입 열거형

    [SerializeField]
    private Text Title;
    [SerializeField]
    private InputField inputFields;
    [SerializeField]
    private Value value;

    [SerializeField]
    private Button Modify_btn;

    [Header("Pasword")]
    [SerializeField]
    private InputField OldPasword;                                                                              // 비밀번호 수정 시 이전의 비밀번호 입력창

    public void Get_Info(int t_value)
    {
        Reset_Field();                                                                                          // InputField 초기화

        value = (Value)t_value;                                                                                 // 어떤 타입인지 확인

        Title.text = value.ToString();

        if (value == Value.Password)                                                                            // 비밀번호라면
        {
            OldPasword.gameObject.SetActive(true);                                                              // 둘다 비밀번호 타입으로 변경
            OldPasword.contentType = InputField.ContentType.Password;
            inputFields.contentType = InputField.ContentType.Password;
        }
        else                                                                                                    // 아니라면
        {
            OldPasword.gameObject.SetActive(false);                                                             // 일반형으로 변경
            inputFields.contentType = InputField.ContentType.Standard;
        }
    }
    /// <summary>
    /// 수정 진행 함수
    /// </summary>
    public void OnclickUpdate_data()
    {
        ResetUI(inputFields.image);                                                                             // 매개변수로 입력한 UI색상 및 내용 초기화

        if (IsFieldEmpty(inputFields.image, inputFields.text, Title.text))                                      // 공백 체크
            return;

        if (value == Value.Password)                                                                            // 타입이 비밀번호라면
        {
            if (IsFieldEmpty(OldPasword.image, OldPasword.text, Title.text))                                    // 공백 체크
                return;

            if (OldPasword.text.Equals(inputFields.text))                                                       // 이전 비밀번호와 현재 비밀번호가 같다면
            {
                GuideForCorrectlyEnteredData(inputFields.image, "바꾸자 하는 비밀번호가 현재 비밀번호와 동일합니다.");
                return;
            }
        }

        if(value == Value.Email)                                                                                // 타입이 이메일이라면 형식 확인
            if (!inputFields.text.Contains("@"))
            {
                GuideForCorrectlyEnteredData(inputFields.image, "메일 형식이 잘못 되었습니다. (ex. address@xxxx.xxx)");
                return;
            }
        
        Modify_btn.interactable = false;                                                                        // 연타 방지용 비활성

        Update_data();
    }
    /// <summary>
    /// 데이터 갱신
    /// </summary>
    public void Update_data()
    {
        switch (value)
        {
            case Value.Nickname:                                                                                    // 닉네임 수정 일시
                SendQueue.Enqueue(Backend.BMember.CreateNickname, inputFields.text, callback =>
                {
                    if (callback.IsSuccess())                                                                       // 수정 성공
                    {
                        SetMassage($"{inputFields.text}(으)로 닉네임이 변경되었습니다.");

                        Quit_Game.Instance.Limit = true;                                                            // Quit 제어 변수 값 변경

                        Invoke(nameof(Close_Panel), 0.3f);
                    }
                    else                                                                                            // 수정 실패
                    {
                        Modify_btn.interactable = true;                                                             // 버튼 활성화

                        string message = string.Empty;

                        switch (int.Parse(callback.GetStatusCode()))
                        {
                            case 400:                                                                               // 빈 닉네임, 혹은 string.empty, 20자 이상 닉네임, 앞/뒤 공백 존재 시
                                message = "닉네임이 비어있거나 | 20자 이상이거나 | 앞/뒤에 공백이 있습니다.";
                                break;
                            case 409:                                                                               // 이미 중복된 닉네임이 있는 경우
                                message = "중복된 닉네임 입니다.";
                                break;
                            default:
                                message = callback.GetMessage();
                                break;
                        }

                        GuideForCorrectlyEnteredData(inputFields.image, message);                                   // InputField 색상 변경 및 텍스트 초기화

                        Invoke(nameof(Reset_Text), 0.7f);
                    }
                });
                break;
            case Value.Password:                                                                                    // 비밀번호 수정
                SendQueue.Enqueue(Backend.BMember.UpdatePassword, OldPasword.text, inputFields.text, callback =>
                {
                    if(callback.IsSuccess())                                                                        // 수정 성공
                    {
                        SetMassage($"비밀번호가 변경되었습니다.");

                        Quit_Game.Instance.Limit = true;                                                            // Quit 제어 변수 값 변경

                        Invoke(nameof(Close_Panel), 0.3f);
                    }
                    else                                                                                            // 수정 실패
                    {
                        Modify_btn.interactable = true;                                                             // 버튼 활성화

                        string message = string.Empty;

                        switch (int.Parse(callback.GetStatusCode()))
                        {
                            case 400:                                                                               // 잘못된 OldPassword를 입력한 경우
                                message = "현재 비밀번호를 확인해 주세요.";
                                break;
                            default:
                                message = callback.GetMessage();
                                break;
                        }

                        GuideForCorrectlyEnteredData(inputFields.image, message);                                   // InputField 색상 변경 및 텍스트 초기화

                        Invoke(nameof(Reset_Text), 0.7f);
                    }
                });
                break;
            case Value.Email:                                                                                       // 메일주소 수정
                SendQueue.Enqueue(Backend.BMember.UpdateCustomEmail, inputFields.text, callback =>
                {
                    if(callback.IsSuccess())                                                                        // 수정 성공
                    {
                        SetMassage($" {inputFields.text} 메일 변경 완료.");

                        Quit_Game.Instance.Limit = true;                                                            // Quit 제어 변수 값 변경

                        Invoke(nameof(Close_Panel), 0.3f);
                    }
                    else                                                                                            // 수정 실패
                    {
                        Modify_btn.interactable = true;

                        string message = string.Empty;

                        message=callback.GetMessage();                                                              // callback 메시지 출력

                        SetMassage(message);

                        Invoke(nameof(Reset_Text), 0.7f);
                    }
                });
                break;
        }
    }
    /// <summary>
    /// 패널 닫힘 함수
    /// </summary>
    private void Close_Panel()
    {
        Reset_Text();                                                                                           // 초기화
        Modify_btn.interactable = true;                                                                         // 버튼 활성화
        updateinfo.Invoke();                                                                                    // 등록된 이벤트 있을시 진행

        if (value == Value.Password)                                                                            // 타입이 비밀번호라면 
        {
            updateinfo_2.Invoke();                                                                              // 추가로 등록된 이벤트 있을시 진행
        }

        Quit_Game.Instance.Limit = false;                                                                       // Quit 제어 변수 값 변경
        Quit_Game.Instance.Panel_Out();                                                                         // Quit List 에서 해당 패널 빼기
        gameObject.SetActive(false);
    }
    private void Reset_Field()
    {
        Reset_Text();                                                                                           // InputField text 내용 초기화
        ResetUI(inputFields.image, OldPasword.image);                                                           // InputField 색상 초기화
    }
}
