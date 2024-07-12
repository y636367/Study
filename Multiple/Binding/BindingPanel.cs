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

    public KeyBinding _binding = new KeyBinding();                                                          // ���ε��� ���� Ŭ���� ����
    public SerializableInputBinding _sib;                                                                   // ������ ���ε� ����

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
    /// ���ε� �ʱ�ȭ
    /// </summary>
    public void Init()
    {
        _binding.ResetAll(_binding);                                                            // ���ε� �ʱ�ȭ
        _sib = new SerializableInputBinding(_binding);                                          // �����ǰ� �ʱ�ȭ�� ���ε� Ŭ���� �Ҵ�, ���ε� ������ Ŭ���� ����
        _binding.LoadFromFile(_binding, _sib);                                                  // ���� ���ε� ��� �����ִٸ� Load
        Save_bindForm();                                                                        // �� ���ε� ���� ����
        prevention.SetActive(false);                                                            // ���ε� �� �Է� ȭ�� �Է� ���� �г� Off

        UI_Reset();
    }
    /// <summary>
    /// ���ε� ��ư ��� �� ���û��·� �ʱ�ȭ �� ���ε��� Ű �ؽ�Ʈ ǥ��
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
    /// Ű ���ε� �� ���� UI ��ȯ �Լ�
    /// </summary>
    /// <param name="t_button"></param>
    public void Choice_Key(Button t_button)
    {
        GameManager.instance.isBinding = true;

        action = t_button.GetComponent<BindPairing>().action;
        t_button.GetComponent<Image>().color = Color.green;                                                     // ���õǾ����� ������ �˷���

        foreach (var bt in Buttons)
        {
            bt.interactable = false;                                                                            // ������ ��ư ��Ȱ��ȭ
        }
        prevention.SetActive(true);                                                                             // ȭ�� Ŭ�� ���� �г� On
        
        Setting = true;
    }
    /// <summary>
    /// ���ε� �����Ͻ� �Էµ� Ű�� ���ε� �� ����
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
    /// ���ε� �� Ű �ؽ�Ʈ ǥ��
    /// </summary>
    /// <param name="t_button"></param>
    private void Update_Text(Button t_button)
    {
        Text code_text = t_button.GetComponentInChildren<Text>();
        KeyCode code = t_button.GetComponent<BindPairing>().keycode;

        switch (code)                                                                                               // ���콺 �Է��� ��� Mouse0, Mouse1�� �ƴ� ���� �ؽ�Ʈ�� ��ü
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
    /// �Ҵ�� ���ε� �� ����
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
    /// Apply ��ư�� ���ε� ���� ���� �Լ� �Ҵ�
    /// </summary>
    public void Save_bindForm()
    {
        Apply_b.onClick.AddListener(Set_sib);
    }
    /// <summary>
    /// ���ε� �� Ű �� ����
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
