using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Upgrade : MonoBehaviour
{
    [System.Serializable]
    public class Choice_ : UnityEvent { };                                                              // �� �̺�Ʈ ������ ���� Evnet Ŭ���� �ν��Ͻ� ����
    public Choice_ choice = new Choice_ ();

    #region Variable
    [SerializeField]
    public WeaponData_Controller data;
    [SerializeField]
    public Weapon_UI UI;

    [SerializeField]
    public Image sprite;
    [SerializeField]
    public Text Name;
    [SerializeField]
    public Text Lv;
    [SerializeField]
    public Text Desc;
    [SerializeField]
    public Text status;

    [SerializeField]
    private Text Next_status;

    [SerializeField]
    private Button upgrade_Button;
    [SerializeField]
    private Button Choice_Button;

    [SerializeField]
    private Text Button_Text;

    [SerializeField]
    private WeaponUpgrade weaponupgrade;
    #endregion
    private void Awake()
    {
        weaponupgrade = GetComponent<WeaponUpgrade>();
    }
    private void OnEnable()
    {
        Setting_UI();
    }
    /// <summary>
    ///  UI ǥ��
    /// </summary>
    public void Setting_UI()
    {
        sprite.sprite = data.data.data.ItemIcon;

        Name.text = data.data.Name;
        Lv.text = data.Level==0 ? "���ع�" : "Lv." + (data.Level);
        Desc.text = data.data.Desc;

        if (data.Level < 6)
        {
            upgrade_Button.enabled = true;
        }
        else
        {
            data.Max_Level = true;
            upgrade_Button.gameObject.SetActive(false);
            Next_status.text = "�� �̻� ��ȭ �� �� �����ϴ�.";
            Lv.text = "Lv.MAX";
        }

        if (data.Level != 0)
        {
            try
            {
                Choice_Button.gameObject.SetActive(true);
            }
            catch (NullReferenceException) { }

            switch (data.data.data.ItemId)
            {
                case 0: // ����
                    status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level - 1], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                        data.data.data.counts[data.Level - 1], data.data.data.Cool_time[data.Level - 1]);
                    if (!data.Max_Level)
                        Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                            data.data.data.counts[data.Level], data.data.data.Cool_time[data.Level]);
                    break;
                case 1: // ����, ���Ѱ�ȭ, ���Ϸ�ó, ��ź�߻��
                case 2: // ����, �����Ѱ�ȭ
                case 3: // ��ź, ��ź�Ѱ�ȭ, �������
                    status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level - 1], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                        data.data.data.counts[data.Level - 1], data.data.data.Cool_time[data.Level - 1], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    if (!data.Max_Level)
                        Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                        data.data.data.counts[data.Level], data.data.data.Cool_time[data.Level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    break;
                case 4: // ����ź
                    status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level - 1], Backend_GameData.Instance.Userstatusdatas.Attack_Power * 0.05f,
                        data.data.data.counts[data.Level - 1], data.data.data.Cool_time[data.Level - 1], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    if (!data.Max_Level)
                        Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power * 0.05f,
                        data.data.data.counts[data.Level], data.data.data.Cool_time[data.Level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    break;
                case 5: // ����ź
                    status.text = string.Format(data.data.data.ItemDesc, data.data.data.counts[data.Level - 1], data.data.data.Cool_time[data.Level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    if (!data.Max_Level)
                        Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.counts[data.Level], data.data.data.Cool_time[data.Level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    break;
                case 6: // ����
                    status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level - 1], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                        data.data.data.counts[data.Level - 1], data.data.data.Cool_time[data.Level - 1], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    if (!data.Max_Level)
                        Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                        data.data.data.counts[data.Level], data.data.data.Cool_time[data.Level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    break;
                case 7:// ��������, ȭ������
                    switch (data.data.data.Type)
                    {
                        case Item_Data.ItemType.nomal_weapon:
                            status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level - 1], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                                data.data.data.Cool_time[data.Level - 1]);
                            if (!data.Max_Level)
                                Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                                    data.data.data.Cool_time[data.Level]);
                            break;
                        case Item_Data.ItemType.reinforced_weapon:
                            status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level - 1], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                                data.data.data.counts[data.Level - 1], data.data.data.Cool_time[data.Level - 1]);
                            if (!data.Max_Level)
                                Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                                    data.data.data.counts[data.Level], data.data.data.Cool_time[data.Level]);
                            break;
                    }
                    break;
                case 8: // ȭ������, ȭ�����Ⱝȭ
                    status.text = string.Format(data.data.data.ItemDesc, data.data.data.counts[data.Level - 1], Backend_GameData.Instance.Userstatusdatas.Attack_Power * 0.05f,
                        data.data.data.Cool_time[data.Level - 1], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    if (!data.Max_Level)
                        Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.counts[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power * 0.05f,
                            data.data.data.Cool_time[data.Level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    break;
                case 9: // ��ȣź
                case 10: // ���
                    status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level - 1], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                        data.data.data.counts[data.Level - 1], data.data.data.Cool_time[data.Level - 1], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    if (!data.Max_Level)
                        Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                        data.data.data.counts[data.Level], data.data.data.Cool_time[data.Level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    break;
            }
        }
        else
        {
            try
            {
                Choice_Button.gameObject.SetActive(false);
            }
            catch (NullReferenceException) { }

            status.text = "���ع�";

            switch (data.data.data.ItemId)
            {
                case 0: // ����
                    Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                        data.data.data.counts[data.Level], data.data.data.Cool_time[data.Level]);
                    break;
                case 1: // ����, ���Ѱ�ȭ, ���Ϸ�ó, ��ź�߻��
                case 2: // ����, �����Ѱ�ȭ
                case 3: // ��ź, ��ź�Ѱ�ȭ, �������
                    Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                    data.data.data.counts[data.Level], data.data.data.Cool_time[data.Level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    break;
                case 4: // ����ź
                    Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power * 0.05f,
                    data.data.data.counts[data.Level], data.data.data.Cool_time[data.Level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    break;
                case 5: // ����ź
                    Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.counts[data.Level], data.data.data.Cool_time[data.Level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    break;
                case 6: // ����
                    Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                    data.data.data.counts[data.Level], data.data.data.Cool_time[data.Level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    break;
                case 7:// ��������, ȭ������
                    switch (data.data.data.Type)
                    {
                        case Item_Data.ItemType.nomal_weapon:
                            Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                                data.data.data.Cool_time[data.Level]);
                            break;
                        case Item_Data.ItemType.reinforced_weapon:
                            Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                                data.data.data.counts[data.Level], data.data.data.Cool_time[data.Level]);
                            break;
                    }
                    break;
                case 8: // ȭ������, ȭ�����Ⱝȭ
                    Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.counts[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power * 0.05f,
                        data.data.data.Cool_time[data.Level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    break;
                case 9: // ��ȣź
                case 10: // ���
                    Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                    data.data.data.counts[data.Level], data.data.data.Cool_time[data.Level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    break;
            }
        }
    }
    /// <summary>
    /// ��ȭ �ϰ��� �ϴ� Weapon Data ��Ʈ�ѷ� ��������
    /// </summary>
    /// <param name="t_data"></param>
    /// <param name="t_Weapon"></param>
    public void Get_Data(WeaponData_Controller t_data)
    {
        data = t_data;
        weaponupgrade.data = data;

        Cost_Check(data);
    }
    public void Get_WeaponUI(Weapon_UI t_Weapon)
    {
        UI = t_Weapon;
    }
    /// <summary>
    /// ��ȭ �� �� �ִ����� ���� Cost�� ������� üũ
    /// </summary>
    /// <param name="data"></param>
    public void Cost_Check(WeaponData_Controller data)
    {
        if (Backend_GameData.Instance.Userdatas.Coin < data.Upgrade_Cost || data.Max_Level)
        {
            upgrade_Button.gameObject.SetActive(false);
        }
        else
        {
            upgrade_Button.gameObject.SetActive(true);
            upgrade_Button.interactable = true;
            Button_Text.text = data.Upgrade_Cost.ToString();
        }
    }
    /// <summary>
    /// ���ӿ� ��� ���� ���� ���� �Լ�
    /// </summary>
    public void Choice_Weapon()
    {
        Main_UIManager.Instance.NowWeapons = Backend_GameData.Instance.Userdatas.Now_Weapon = data;
        Main_UIManager.Instance.Weapon_UI = data.GetComponent<Weapon_UI>();
        Backend_GameData.Instance.Userdatas.WeaponNumber = Backend_GameData.Instance.Userdatas.Now_Weapon.data.Numbering;
        Backend_GameData.Instance.UpdateUserDatas_();
        choice?.Invoke();
    }
    /// <summary>
    /// Stage ���� ȭ�鿡�� ���� ���׷��̵带 ���� ���� ���� ��ȭ �гο� ������ ����
    /// </summary>
    public void Hunt_Weapon_Upgrade()
    {
        data = Backend_GameData.Instance.Userdatas.Now_Weapon;
        weaponupgrade.data = data;

        Cost_Check(data);
    }
}
