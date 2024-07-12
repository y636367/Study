using System;
using UnityEngine;
using UnityEngine.UI;

public class BindingPanel : MonoBehaviour
{
    #region Variable
    public static BindingPanel instance;

    [Header("Button_Move")]
    [SerializeField]
    private Button MoveForward;
    [SerializeField]
    private Button MoveBackward;
    [SerializeField]
    private Button MoveLeft;
    [SerializeField]
    private Button MoveRight;
    [SerializeField]
    private Button Option;

    [Header("Button_Control")]
    [SerializeField]
    private Button Dash;
    [SerializeField]
    private Button Jump;
    [SerializeField]
    private Button RotateCamera;
    [SerializeField]
    private Button RagDoll;
    [SerializeField]
    private Button ForcedEscape;

    [SerializeField]
    private Button[] Buttons;

    [SerializeField]
    private GameObject prevention;
    [SerializeField]
    private Button Apply_b;

    private bool Setting;

    public KeyBinding _binding = new KeyBinding();                                                          // 바인딩을 위핸 클래스 생성
    public SerializableInputBinding _sib;                                                                   // 생성한 바인딩 관리

    [SerializeField]
    private Action action;
    #endregion
    private void Awake()
    {
        instance = this;
    }
    private void LateUpdate()
    {
        if (!Setting)
            return;
        else
        {
            if (Input.anyKeyDown)
            {
                Setting = false;
                ChangeKey();
            }
        }
    }
    /// <summary>
    /// 바인딩 초기화
    /// </summary>
    public void Init()
    {
        _binding.ResetAll(_binding);                                                            // 바인딩 초기화
        _sib = new SerializableInputBinding(_binding);                                          // 생성되고 초기화된 바인딩 클래스 할당, 바인딩 관리할 클래스 생성
        _binding.LoadFromFile(_binding, _sib);                                                  // 이전 바인딩 기록 남아있다면 Load
        Save_bindForm();                                                                        // 현 바인딩 상태 저장
        prevention.SetActive(false);                                                            // 바인딩 시 입력 화면 입력 예방 패널 Off

        UI_Reset();
    }
    /// <summary>
    /// 바인딩 버튼 모두 미 선택상태로 초기화 및 바인딩된 키 텍스트 표시
    /// </summary>
    public void UI_Reset()
    {
        for (int index = 0; index < Buttons.Length; index++)
        {
            Buttons[index].interactable = true;
            Buttons[index].GetComponent<Image>().color = Color.gray;
            Binding(Buttons[index].GetComponent<BindPairing>());
            Update_Text(Buttons[index]);
        }

        try
        {
            Utils.Instance.binding = _binding;
        }
        catch (NullReferenceException e) { }

        prevention.SetActive(false);
        GameManager.instance.isBinding = false;
    }
    /// <summary>
    /// 키 바인딩 을 위한 UI 전환 함수
    /// </summary>
    /// <param name="t_button"></param>
    public void Choice_Key(Button t_button)
    {
        GameManager.instance.isBinding = true;

        action = t_button.GetComponent<BindPairing>().action;
        t_button.GetComponent<Image>().color = Color.green;                                                     // 선택되었음을 색으로 알려줌

        foreach (var bt in Buttons)
        {
            bt.interactable = false;                                                                            // 나머지 버튼 비활성화
        }
        prevention.SetActive(true);                                                                             // 화면 클릭 예방 패널 On
        
        Setting = true;
    }
    /// <summary>
    /// 바인딩 상태일시 입력된 키로 바인딩 재 설정
    /// </summary>
    /// <param name="mousenum"></param>
    private void ChangeKey(int mousenum = -1)
    {
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                _binding.Bind(_binding, action, keyCode);
                for (int index = 0; index < _sib.bindPairs.Length; index++)
                {
                    if (_sib.bindPairs[index].key == action)
                    {
                        _sib.bindPairs[index].value = keyCode;
                        break;
                    }
                }
                break;
            }
        }

        UI_Reset();
    }
    /// <summary>
    /// 바인딩 된 키 텍스트 표출
    /// </summary>
    /// <param name="t_button"></param>
    private void Update_Text(Button t_button)
    {
        Text code_text = t_button.GetComponentInChildren<Text>();
        KeyCode code = t_button.GetComponent<BindPairing>().keycode;

        switch (code)                                                                                               // 마우스 입력의 경우 Mouse0, Mouse1이 아닌 임의 텍스트로 대체
        {
            case KeyCode.Mouse0:
                code_text.text = $"LeftButton";
                break;
            case KeyCode.Mouse1:
                code_text.text = $"RightButton";
                break;
            case KeyCode.Mouse2:
                code_text.text = $"MiddleButton";
                break;
            default:
                code_text.text = $"{code}";
                break;
        }
    }
    /// <summary>
    /// 할당된 바인드 값 적용
    /// </summary>
    /// <param name="_bind"></param>
    private void Binding(BindPairing _bind)
    {
        for (int index = 0; index < _sib.bindPairs.Length; index++)
        {
            if (_sib.bindPairs[index].key == _bind.action)
            {
                _bind.keycode = _sib.bindPairs[index].value;
            }
        }
    }
    /// <summary>
    /// Apply 버튼에 바인드 설정 저장 함수 할당
    /// </summary>
    public void Save_bindForm()
    {
        Apply_b.onClick.AddListener(Set_sib);
    }
    /// <summary>
    /// 바인딩 된 키 값 저장
    /// </summary>
    private void Set_sib()
    {
        _binding.SaveFile(_sib);
    }
    private void OnEnable()
    {
        _binding.LoadFromFile(_binding, _sib);
        UI_Reset();
    }
}
