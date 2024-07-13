using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Upgrade : MonoBehaviour
{
    [System.Serializable]
    public class Choice_ : UnityEvent { };                                                              // 후 이벤트 진행을 위한 Evnet 클래스 인스턴스 생성
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
    ///  UI 표출
    /// </summary>
    public void Setting_UI()
    {
        sprite.sprite = data.data.data.ItemIcon;

        Name.text = data.data.Name;
        Lv.text = data.Level==0 ? "미해방" : "Lv." + (data.Level);
        Desc.text = data.data.Desc;

        if (data.Level < 6)
        {
            upgrade_Button.enabled = true;
        }
        else
        {
            data.Max_Level = true;
            upgrade_Button.gameObject.SetActive(false);
            Next_status.text = "더 이상 강화 할 수 없습니다.";
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
                case 0: // 도끼
                    status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level - 1], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                        data.data.data.counts[data.Level - 1], data.data.data.Cool_time[data.Level - 1]);
                    if (!data.Max_Level)
                        Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                            data.data.data.counts[data.Level], data.data.data.Cool_time[data.Level]);
                    break;
                case 1: // 권총, 권총강화, 로켓런처, 유탄발사기
                case 2: // 저격, 저격총강화
                case 3: // 산탄, 산탄총강화, 기관단총
                    status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level - 1], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                        data.data.data.counts[data.Level - 1], data.data.data.Cool_time[data.Level - 1], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    if (!data.Max_Level)
                        Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                        data.data.data.counts[data.Level], data.data.data.Cool_time[data.Level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    break;
                case 4: // 연막탄
                    status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level - 1], Backend_GameData.Instance.Userstatusdatas.Attack_Power * 0.05f,
                        data.data.data.counts[data.Level - 1], data.data.data.Cool_time[data.Level - 1], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    if (!data.Max_Level)
                        Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power * 0.05f,
                        data.data.data.counts[data.Level], data.data.data.Cool_time[data.Level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    break;
                case 5: // 섬광탄
                    status.text = string.Format(data.data.data.ItemDesc, data.data.data.counts[data.Level - 1], data.data.data.Cool_time[data.Level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    if (!data.Max_Level)
                        Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.counts[data.Level], data.data.data.Cool_time[data.Level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    break;
                case 6: // 지뢰
                    status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level - 1], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                        data.data.data.counts[data.Level - 1], data.data.data.Cool_time[data.Level - 1], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    if (!data.Max_Level)
                        Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                        data.data.data.counts[data.Level], data.data.data.Cool_time[data.Level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    break;
                case 7:// 가스쉴드, 화염쉴드
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
                case 8: // 화염방사기, 화염방사기강화
                    status.text = string.Format(data.data.data.ItemDesc, data.data.data.counts[data.Level - 1], Backend_GameData.Instance.Userstatusdatas.Attack_Power * 0.05f,
                        data.data.data.Cool_time[data.Level - 1], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    if (!data.Max_Level)
                        Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.counts[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power * 0.05f,
                            data.data.data.Cool_time[data.Level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    break;
                case 9: // 신호탄
                case 10: // 대거
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

            status.text = "미해방";

            switch (data.data.data.ItemId)
            {
                case 0: // 도끼
                    Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                        data.data.data.counts[data.Level], data.data.data.Cool_time[data.Level]);
                    break;
                case 1: // 권총, 권총강화, 로켓런처, 유탄발사기
                case 2: // 저격, 저격총강화
                case 3: // 산탄, 산탄총강화, 기관단총
                    Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                    data.data.data.counts[data.Level], data.data.data.Cool_time[data.Level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    break;
                case 4: // 연막탄
                    Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power * 0.05f,
                    data.data.data.counts[data.Level], data.data.data.Cool_time[data.Level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    break;
                case 5: // 섬광탄
                    Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.counts[data.Level], data.data.data.Cool_time[data.Level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    break;
                case 6: // 지뢰
                    Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                    data.data.data.counts[data.Level], data.data.data.Cool_time[data.Level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    break;
                case 7:// 가스쉴드, 화염쉴드
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
                case 8: // 화염방사기, 화염방사기강화
                    Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.counts[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power * 0.05f,
                        data.data.data.Cool_time[data.Level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    break;
                case 9: // 신호탄
                case 10: // 대거
                    Next_status.text = string.Format(data.data.data.ItemDesc, data.data.data.damages[data.Level], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                    data.data.data.counts[data.Level], data.data.data.Cool_time[data.Level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                    break;
            }
        }
    }
    /// <summary>
    /// 강화 하고자 하는 Weapon Data 컨트롤러 가져오기
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
    /// 강화 할 수 있는지에 대한 Cost가 충분한지 체크
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
    /// 게임에 들고 나갈 무기 선택 함수
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
    /// Stage 선택 화면에서 무기 업그레이드를 위한 현재 무기 강화 패널에 데이터 전달
    /// </summary>
    public void Hunt_Weapon_Upgrade()
    {
        data = Backend_GameData.Instance.Userdatas.Now_Weapon;
        weaponupgrade.data = data;

        Cost_Check(data);
    }
}
