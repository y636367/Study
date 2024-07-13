using UnityEngine;
using UnityEngine.Events;

public class WeaponUpgrade : MonoBehaviour
{
    [System.Serializable]
    public class UI_Update : UnityEvent { };                                                              // �� �̺�Ʈ ������ ���� Evnet Ŭ���� �ν��Ͻ� ����
    public UI_Update ui_update = new UI_Update();

    [SerializeField]
    public WeaponData_Controller data;                                                                          // ������ Weapon ��Ʈ�ѷ�
    [SerializeField]
    public Weapon_UI UI;                                                                                        // ������ UI

    private Upgrade upgrade;
    private void Awake()
    {
        data = GetComponent<Upgrade>().data;
        UI = GetComponent<Upgrade>().UI;
        upgrade= GetComponent<Upgrade>();
    }
    /// <summary>
    /// ������ ����
    /// </summary>
    public void Upgrade_Weapon()
    {
        if (Backend_GameData.Instance.Userdatas.Coin >= data.Upgrade_Cost && !data.Max_Level)                   // Cost �˻� �ѹ� ��
        {
            Backend_GameData.Instance.Userdatas.Coin -= data.Upgrade_Cost;                                      // Cost �� Level ������ ����
            data.Upgrade_Cost += data.Plus_Cost;

            data.Level += 1;

            switch (data.data.W_type)                                                                           // Type�� �°� ������ ����
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

            Backend_Reflections();                                                                              // ���� ������ �ݿ�

            Utils.Instance.Delay_Frame(10);
            UI.Data_Up();                                                                                       // UI ����
            upgrade.Cost_Check(data);                                                                           // Cost üũ
            upgrade.Setting_UI();                                                                               // UI ����
            ui_update?.Invoke();
        }
    }
    /// <summary>
    /// �ٲ� ������ ���� ����
    /// </summary>
    private void Backend_Reflections()
    {
        Backend_GameData.Instance.UpdateUserDatas_();
        Backend_GameData.Instance.UpdateWeaponLVDatas_();
    }
}
