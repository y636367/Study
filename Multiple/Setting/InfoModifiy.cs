using UnityEngine;
using UnityEngine.UI;
using BackEnd;

public class InfoModifiy : Login_Base
{
    [System.Serializable]
    public class UpdateInfoEvent : UnityEngine.Events.UnityEvent { }                                            // UnityEngin�� �̺�Ʈ Ŭ���� ����
    public UpdateInfoEvent updateinfo = new UpdateInfoEvent();                                                  // �ν��Ͻ��� �ٸ� �̺�Ʈ ���� �� ����ǰԲ� ����_1
    public UpdateInfoEvent updateinfo_2 = new UpdateInfoEvent();                                                // �ν��Ͻ��� �ٸ� �̺�Ʈ ���� �� ����ǰԲ� ����_2

    public enum Value { Nickname, Password, Email }                                                             // ������ Ÿ�� ������

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
    private InputField OldPasword;                                                                              // ��й�ȣ ���� �� ������ ��й�ȣ �Է�â

    public void Get_Info(int t_value)
    {
        Reset_Field();                                                                                          // InputField �ʱ�ȭ

        value = (Value)t_value;                                                                                 // � Ÿ������ Ȯ��

        Title.text = value.ToString();

        if (value == Value.Password)                                                                            // ��й�ȣ���
        {
            OldPasword.gameObject.SetActive(true);                                                              // �Ѵ� ��й�ȣ Ÿ������ ����
            OldPasword.contentType = InputField.ContentType.Password;
            inputFields.contentType = InputField.ContentType.Password;
        }
        else                                                                                                    // �ƴ϶��
        {
            OldPasword.gameObject.SetActive(false);                                                             // �Ϲ������� ����
            inputFields.contentType = InputField.ContentType.Standard;
        }
    }
    /// <summary>
    /// ���� ���� �Լ�
    /// </summary>
    public void OnclickUpdate_data()
    {
        ResetUI(inputFields.image);                                                                             // �Ű������� �Է��� UI���� �� ���� �ʱ�ȭ

        if (IsFieldEmpty(inputFields.image, inputFields.text, Title.text))                                      // ���� üũ
            return;

        if (value == Value.Password)                                                                            // Ÿ���� ��й�ȣ���
        {
            if (IsFieldEmpty(OldPasword.image, OldPasword.text, Title.text))                                    // ���� üũ
                return;

            if (OldPasword.text.Equals(inputFields.text))                                                       // ���� ��й�ȣ�� ���� ��й�ȣ�� ���ٸ�
            {
                GuideForCorrectlyEnteredData(inputFields.image, "�ٲ��� �ϴ� ��й�ȣ�� ���� ��й�ȣ�� �����մϴ�.");
                return;
            }
        }

        if(value == Value.Email)                                                                                // Ÿ���� �̸����̶�� ���� Ȯ��
            if (!inputFields.text.Contains("@"))
            {
                GuideForCorrectlyEnteredData(inputFields.image, "���� ������ �߸� �Ǿ����ϴ�. (ex. address@xxxx.xxx)");
                return;
            }
        
        Modify_btn.interactable = false;                                                                        // ��Ÿ ������ ��Ȱ��

        Update_data();
    }
    /// <summary>
    /// ������ ����
    /// </summary>
    public void Update_data()
    {
        switch (value)
        {
            case Value.Nickname:                                                                                    // �г��� ���� �Ͻ�
                SendQueue.Enqueue(Backend.BMember.CreateNickname, inputFields.text, callback =>
                {
                    if (callback.IsSuccess())                                                                       // ���� ����
                    {
                        SetMassage($"{inputFields.text}(��)�� �г����� ����Ǿ����ϴ�.");

                        Quit_Game.Instance.Limit = true;                                                            // Quit ���� ���� �� ����

                        Invoke(nameof(Close_Panel), 0.3f);
                    }
                    else                                                                                            // ���� ����
                    {
                        Modify_btn.interactable = true;                                                             // ��ư Ȱ��ȭ

                        string message = string.Empty;

                        switch (int.Parse(callback.GetStatusCode()))
                        {
                            case 400:                                                                               // �� �г���, Ȥ�� string.empty, 20�� �̻� �г���, ��/�� ���� ���� ��
                                message = "�г����� ����ְų� | 20�� �̻��̰ų� | ��/�ڿ� ������ �ֽ��ϴ�.";
                                break;
                            case 409:                                                                               // �̹� �ߺ��� �г����� �ִ� ���
                                message = "�ߺ��� �г��� �Դϴ�.";
                                break;
                            default:
                                message = callback.GetMessage();
                                break;
                        }

                        GuideForCorrectlyEnteredData(inputFields.image, message);                                   // InputField ���� ���� �� �ؽ�Ʈ �ʱ�ȭ

                        Invoke(nameof(Reset_Text), 0.7f);
                    }
                });
                break;
            case Value.Password:                                                                                    // ��й�ȣ ����
                SendQueue.Enqueue(Backend.BMember.UpdatePassword, OldPasword.text, inputFields.text, callback =>
                {
                    if(callback.IsSuccess())                                                                        // ���� ����
                    {
                        SetMassage($"��й�ȣ�� ����Ǿ����ϴ�.");

                        Quit_Game.Instance.Limit = true;                                                            // Quit ���� ���� �� ����

                        Invoke(nameof(Close_Panel), 0.3f);
                    }
                    else                                                                                            // ���� ����
                    {
                        Modify_btn.interactable = true;                                                             // ��ư Ȱ��ȭ

                        string message = string.Empty;

                        switch (int.Parse(callback.GetStatusCode()))
                        {
                            case 400:                                                                               // �߸��� OldPassword�� �Է��� ���
                                message = "���� ��й�ȣ�� Ȯ���� �ּ���.";
                                break;
                            default:
                                message = callback.GetMessage();
                                break;
                        }

                        GuideForCorrectlyEnteredData(inputFields.image, message);                                   // InputField ���� ���� �� �ؽ�Ʈ �ʱ�ȭ

                        Invoke(nameof(Reset_Text), 0.7f);
                    }
                });
                break;
            case Value.Email:                                                                                       // �����ּ� ����
                SendQueue.Enqueue(Backend.BMember.UpdateCustomEmail, inputFields.text, callback =>
                {
                    if(callback.IsSuccess())                                                                        // ���� ����
                    {
                        SetMassage($" {inputFields.text} ���� ���� �Ϸ�.");

                        Quit_Game.Instance.Limit = true;                                                            // Quit ���� ���� �� ����

                        Invoke(nameof(Close_Panel), 0.3f);
                    }
                    else                                                                                            // ���� ����
                    {
                        Modify_btn.interactable = true;

                        string message = string.Empty;

                        message=callback.GetMessage();                                                              // callback �޽��� ���

                        SetMassage(message);

                        Invoke(nameof(Reset_Text), 0.7f);
                    }
                });
                break;
        }
    }
    /// <summary>
    /// �г� ���� �Լ�
    /// </summary>
    private void Close_Panel()
    {
        Reset_Text();                                                                                           // �ʱ�ȭ
        Modify_btn.interactable = true;                                                                         // ��ư Ȱ��ȭ
        updateinfo.Invoke();                                                                                    // ��ϵ� �̺�Ʈ ������ ����

        if (value == Value.Password)                                                                            // Ÿ���� ��й�ȣ��� 
        {
            updateinfo_2.Invoke();                                                                              // �߰��� ��ϵ� �̺�Ʈ ������ ����
        }

        Quit_Game.Instance.Limit = false;                                                                       // Quit ���� ���� �� ����
        Quit_Game.Instance.Panel_Out();                                                                         // Quit List ���� �ش� �г� ����
        gameObject.SetActive(false);
    }
    private void Reset_Field()
    {
        Reset_Text();                                                                                           // InputField text ���� �ʱ�ȭ
        ResetUI(inputFields.image, OldPasword.image);                                                           // InputField ���� �ʱ�ȭ
    }
}
