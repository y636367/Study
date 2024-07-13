using UnityEngine;
using UnityEngine.Events;

public class WeaponUpgrade : MonoBehaviour
{
    [System.Serializable]
    public class UI_Update : UnityEvent { };                                                              // 후 이벤트 진행을 위한 Evnet 클래스 인스턴스 생성
    public UI_Update ui_update = new UI_Update();

    [SerializeField]
    public WeaponData_Controller data;                                                                          // 제어할 Weapon 컨트롤러
    [SerializeField]
    public Weapon_UI UI;                                                                                        // 갱신할 UI

    private Upgrade upgrade;
    private void Awake()
    {
        data = GetComponent<Upgrade>().data;
        UI = GetComponent<Upgrade>().UI;
        upgrade= GetComponent<Upgrade>();
    }
    /// <summary>
    /// 데이터 갱신
    /// </summary>
    public void Upgrade_Weapon()
    {
        if (Backend_GameData.Instance.Userdatas.Coin >= data.Upgrade_Cost && !data.Max_Level)                   // Cost 검사 한번 더
        {
            Backend_GameData.Instance.Userdatas.Coin -= data.Upgrade_Cost;                                      // Cost 및 Level 데이터 갱신
            data.Upgrade_Cost += data.Plus_Cost;

            data.Level += 1;

            switch (data.data.W_type)                                                                           // Type에 맞게 데이터 갱신
            {
                case WeaponData.WeponType.Pistol:
                    Backend_GameData.Instance.Userweaponlevel.Pistol += 1;
                    break;
                case WeaponData.WeponType.Shotgun:
                    Backend_GameData.Instance.Userweaponlevel.Shotgun += 1;
                    break;
                case WeaponData.WeponType.Sniper:
                    Backend_GameData.Instance.Userweaponlevel.Sniper += 1;
                    break;
                case WeaponData.WeponType.Submachin:
                    Backend_GameData.Instance.Userweaponlevel.Rampage += 1;
                    break;
                case WeaponData.WeponType.Mine:
                    Backend_GameData.Instance.Userweaponlevel.Mine += 1;
                    break;
                case WeaponData.WeponType.Knife:
                    Backend_GameData.Instance.Userweaponlevel.Knife += 1;
                    break;
                case WeaponData.WeponType.Rocket:
                    Backend_GameData.Instance.Userweaponlevel.RocketLauncer += 1;
                    break;
                case WeaponData.WeponType.Flare:
                    Backend_GameData.Instance.Userweaponlevel.Flare_gun += 1;
                    break;
                case WeaponData.WeponType.FireThrower:
                    Backend_GameData.Instance.Userweaponlevel.FlareThrower += 1;
                    break;
                case WeaponData.WeponType.GasShiled:
                    Backend_GameData.Instance.Userweaponlevel.GasShield += 1;
                    break;
            }

            Backend_Reflections();                                                                              // 서버 데이터 반영

            Utils.Instance.Delay_Frame(10);
            UI.Data_Up();                                                                                       // UI 갱신
            upgrade.Cost_Check(data);                                                                           // Cost 체크
            upgrade.Setting_UI();                                                                               // UI 갱신
            ui_update?.Invoke();
        }
    }
    /// <summary>
    /// 바뀐 데이터 서버 갱신
    /// </summary>
    private void Backend_Reflections()
    {
        Backend_GameData.Instance.UpdateUserDatas_();
        Backend_GameData.Instance.UpdateWeaponLVDatas_();
    }
}
