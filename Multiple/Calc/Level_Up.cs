using UnityEngine;
using static Item_Data;
using System.Linq;
using System.Collections.Generic;

public class Level_Up : MonoBehaviour
{
    #region Variable
    RectTransform rect;

    [SerializeField]
    private Item_Control coin;

    [SerializeField]
    private GameObject _parent;

    bool New_ins = false;
    bool Item_Pass = false;
    bool Fill_Up = false;

    [Header("Item_List")]
    [SerializeField]
    public List<Item_Control> items;

    [Header("Nomal_Weapon")]
    List<Item_Control> nomal = new List<Item_Control>();
    [Header("Reinforced_Weapon")]
    List<Item_Control> Reinforced = new List<Item_Control>();
    [Header("Epic_Weapon")]
    List<Item_Control> Epic = new List<Item_Control>();
    [Header("Characteristic_Weapon")]
    List<Item_Control> Characteristic = new List<Item_Control>();
    [Header("Epic_Characteristic_Weapon")]
    List<Item_Control> Epic_Charateristic = new List<Item_Control>();
    [Header("One_Time_performance")]
    List<Item_Control> One_time_performance = new List<Item_Control>();

    public Dictionary<ItemType, float> itemProbabilities = new Dictionary<ItemType, float>
    {
        { ItemType.nomal_weapon, 12f },
        { ItemType.reinforced_weapon, 3f },
        { ItemType.epic_weapon, 0.8f },
        { ItemType.Characteristic, 10f },
        { ItemType.epic_Characteristic, 1.5f },
        { ItemType.one_time_performance, 0.3f }
    };
    #endregion
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        items= new List<Item_Control>(GetComponentsInChildren<Item_Control>(true));

        foreach (Item_Control item in items)
        {
            if (item == coin)
            {
                items.Remove(item);                                                                                         // ���� ���� �� ���� ����� ���� �߻��ϱ⿡ ���� �ϰ��� �ϴ� �׸��� �ϳ��Ͻ�, �������ڸ��� �ٷ� Ż��
                break;
            }
        }

        sort();                                                                                                             // type�� �з�
    }
    /// <summary>
    /// �Ϲ��� �Ǵ� ��ȭ�� ���ý� �ٸ� �׸� ��� ���� ��, Level Max, ����Ʈ ���� 6�� ���� á�� �� �� ��� ���� �Լ�
    /// </summary>
    /// <param name="t_data"></param>
    /// <returns></returns>
    private List<Item_Control> Activates(List<Item_Control> t_data)
    {
        for(int index=t_data.Count-1; index>=0; index--)                                                                                    // �߰��� �����ع����� list ����� ����Ǽ� �߸��� �ε��� ������ �� �ֱ⿡ ������ ����
        {
            if (!t_data[index].Activate_Check)
            {
                t_data.Remove(t_data[index]);
            }
        }

        if (items.Count == 0)
            Fill_Up = true;

        return items = t_data;
    }
    /// <summary>
    /// �� �׸� Ư�� �����ؼ� ���� �Լ�
    /// </summary>
    private void sort()
    {
        for(int index=0; index<items.Count; index++)
        {
            if (items[index].data.Type == ItemType.nomal_weapon)
            {
                nomal.Add(items[index]);
            }
            else if (items[index].data.Type == ItemType.reinforced_weapon)
            {
                Reinforced.Add(items[index]);
            }
            else if (items[index].data.Type == ItemType.epic_weapon)
            {
                Epic.Add(items[index]);
            }
            else if (items[index].data.Type == ItemType.Characteristic)
            {
                Characteristic.Add(items[index]);
            }
            else if (items[index].data.Type == ItemType.epic_Characteristic)
            {
                Epic_Charateristic.Add(items[index]);
            }
            else if (items[index].data.Type == ItemType.one_time_performance)
            {
                One_time_performance.Add(items[index]);
            }
        }
    }
    /// <summary>
    /// LevelUP �� ���� �׸� ǥ��
    /// </summary>
    public void Show()
    {
        Next();
        rect.localScale = Vector3.one;
        GameManager.Instance.Stop();                                                                                                        // �Ͻ�����
    }
    /// <summary>
    /// ���� �Ϸ� �� ����
    /// </summary>
    public void Hide()
    {
        rect.localScale = Vector3.zero;
        if (New_ins)
            list_Init();
        New_ins = false;
        GameManager.Instance.Resume();                                                                                                      // ���
    }
    /// <summary>
    /// �ν��Ͻ��� ������ �ߺ� �׸� ���� �Լ�
    /// </summary>
    private void list_Init()
    {
        List<Item_Control> Init_list = new List<Item_Control>(GetComponentsInChildren<Item_Control>(true));

        foreach (Item_Control child in Init_list)
        {
            if (child.Instance)
                Destroy(child.gameObject);
        }
    }
    /// <summary>
    /// ���õ� �׸�(����) Ȱ��ȭ �Լ�
    /// </summary>
    /// <param name="index"></param>
    public void Select(int index)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].data.Type == ItemType.nomal_weapon)
            {
                if (index == items[i].data.Weapon_num)
                {
                    items[i].OnClick();
                    Level_Setting(items[i]);
                }
            }
        }
    }
    /// <summary>
    /// ���õ� ���� �׸� Ȱ��ȭ �Լ�
    /// </summary>
    /// <param name="t_weapon"></param>
    public void Level_Setting(Item_Control t_weapon)
    {
        switch (Backend_GameData.Instance.Userdatas.WeaponNumber)
        {
            case 1: // ����
                while(t_weapon.level!=Backend_GameData.Instance.Userweaponlevel.Pistol)
                {
                    t_weapon.OnClick();
                }
                break;
            case 3: // ���Ϸ�ó
                while (t_weapon.level != Backend_GameData.Instance.Userweaponlevel.RocketLauncer)
                {
                    t_weapon.OnClick();
                }
                break;
            case 4: // ������
                while (t_weapon.level != Backend_GameData.Instance.Userweaponlevel.Sniper)
                {
                    t_weapon.OnClick();
                }
                break;
            case 6: // �������
                while (t_weapon.level != Backend_GameData.Instance.Userweaponlevel.Rampage)
                {
                    t_weapon.OnClick();
                }
                break;
            case 7: // ��ź��
                while (t_weapon.level != Backend_GameData.Instance.Userweaponlevel.Shotgun)
                {
                    t_weapon.OnClick();
                }
                break;
            case 12: // ����
                while (t_weapon.level != Backend_GameData.Instance.Userweaponlevel.Mine)
                {
                    t_weapon.OnClick();
                }
                break;
            case 13: // ��������
                while (t_weapon.level != Backend_GameData.Instance.Userweaponlevel.GasShield)
                {
                    t_weapon.OnClick();
                }
                break;
            case 15: // ȭ������
                while (t_weapon.level != Backend_GameData.Instance.Userweaponlevel.FlareThrower)
                {
                    t_weapon.OnClick();
                }
                break;
            case 17: // ��ȣź
                while (t_weapon.level != Backend_GameData.Instance.Userweaponlevel.Flare_gun)
                {
                    t_weapon.OnClick();
                }
                break;
            case 18: // ���
                while (t_weapon.level != Backend_GameData.Instance.Userweaponlevel.Knife)
                {
                    t_weapon.OnClick();
                }
                break;
        }
    }
    /// <summary>
    /// �������� ������ Ư�� 3���� ǥ�� �Լ�
    /// </summary>
    private void Next()
    {
        List<Item_Control>  t_items = new List<Item_Control>(GetComponentsInChildren<Item_Control>(true));                                          // ���� ��ϵ� ��� �׸� �ʱ�ȭ

        foreach (Item_Control item in t_items)
        {
            item.gameObject.SetActive(false);
        }

        Item_Control[] ran = new Item_Control[3];                                                                                                   // ��ϵ� �׸� �� 3���� ��� ���� �迭 ����

        if (!Fill_Up)
        {
            while (!Item_Pass)
            {
                Random_Setting(ran);                                                                                                                // ������ �� ����
                Check_Activate(ran);                                                                                                                // ������ Ȱ��ȭ ���� Ȯ�� �� ��ü �ʿ�� ���� Ÿ�� ������ ��ȯ, ���� �� �������� ��ü

                Item_Pass = Activate_Check(ran);                                                                                                    // Ȱ��ȭ ���� Ȯ�� �ϳ��� ��Ȱ��ȭ ���� �� ��ü �� �缳��
            }
        }
        else
        {
            for (int index = 0; index < ran.Length; index++)                                                                                        // �� �̻� �׸� ���� �Ұ� �ϱ⿡ ��� Coin ���� ��ü
            {
                ran[index] = coin;
            }
        }
        Item_Pass = false;

        for(int index=0;index<ran.Length;index++)
        {
            Item_Control ranitem = ran[index];                                                                                                      // ���õ� ������ �ߺ� ����(Coin �� ��� �ߺ� ���� �ʿ�)

            if(ranitem.gameObject.activeSelf)                                                                                                       // �̹� Ȱ��ȭ ���¶�� ���� ���õ� �׸� �ν��ͽ� �����ؼ� ����
            {
                New_ins = true;
                ranitem = Instantiate(ranitem, _parent.transform);
                ranitem.Instance = true;
            }
            ranitem.gameObject.SetActive(true);                                                                                                     // ���� ������ 3���� �׸� ǥ��
        }
    }
    /// <summary>
    /// Ȱ��ȭ�� �׸�� �� �ߺ��� �ְų� �� �̻� ��ü�� �׸��� ���� ��� Coin ��ü �Լ�
    /// </summary>
    /// <param name="ran"></param>
    /// <returns></returns>
    private Item_Control[] Check_Activate(Item_Control[] ran)
    {
        Item_Control Check_data;

        for (int index = 0; index < ran.Length; index++)
        {
            if (!ran[index].Activate_Check)
            {
                Check_data = ran[index];
                ran[index] = Spin_Result(ran[index].data.Type);

                if (Check_data == ran[index])                                                                                           // ���� ȣ�� 
                {
                    ran[index] = coin;
                }
            }
        }
        return ran;
    }
    /// <summary>
    /// ���� ���� �Լ�(sort) ���� �� �� �׸� ���� �ݿ� �Ǵ� ���� �Լ�
    /// </summary>
    /// <param name="ran"></param>
    /// <returns></returns>
    private bool Activate_Check(Item_Control[] ran)
    {
        bool return_value = false;

        for (int index = 0; index < ran.Length; index++)
        {
            if (!ran[index].Activate_Check)
            {
                return_value = false;
                break;
            }

            if (index == ran.Length - 1)
                return_value = true;
        }

        return return_value;
    }
    /// <summary>
    /// LevelUP�� ������ �� ���� �Լ�
    /// </summary>
    /// <param name="ran"></param>
    /// <returns></returns>
    private Item_Control[] Random_Setting(Item_Control[] ran)
    {
        while (true)
        {
            while (true)
            {
                Item_Control ranitem;
                ItemType type_data = SpinGacha();

                ranitem = Spin_Result(type_data);

                if (ranitem.level == ranitem.data.damages.Length)
                {
                    type_data = SpinGacha();
                    ranitem = Spin_Result(type_data);
                }
                else
                {
                    ran[0] = ranitem;
                    break;
                }
            }
            while (true)
            {
                Item_Control ranitem;
                ItemType type_data = SpinGacha();

                ranitem = Spin_Result(type_data);

                if (ranitem.level == ranitem.data.damages.Length || ran[0]==ranitem)
                {
                    type_data = SpinGacha();
                    ranitem = Spin_Result(type_data);
                }
                else
                {
                    ran[1] = ranitem;
                    break;
                }
            }
            while (true)
            {
                Item_Control ranitem;
                ItemType type_data = SpinGacha();

                ranitem = Spin_Result(type_data);

                if (ranitem.level == ranitem.data.damages.Length || ran[0] == ranitem || ran[1] == ranitem)
                {
                    type_data = SpinGacha();
                    ranitem = Spin_Result(type_data);
                }
                else
                {
                    ran[2] = ranitem;
                    break;
                }
            }

            if ((ran[0] != ran[1] && ran[0] != ran[2] && ran[1] != ran[2]) || 
                ((ran[0] == ran[1] && ran[0] == coin) && (ran[0] == ran[2] && ran[2] == coin) && (ran[1] == ran[2] && ran[1] == coin)))                                         // �ߺ� ���� Ȯ�� �Լ�
                break;       
        }
        return ran;
    }
    /// <summary>
    /// �׸� ���� �Լ�
    /// </summary>
    /// <returns></returns>
    private ItemType SpinGacha()
    {
        float totalWeight = 0f;

        foreach (var probability in itemProbabilities.Values)                                                                   // ��ü ����ġ ���
        {
            totalWeight += probability;
        }

        float randomValue = Random.Range(0f, totalWeight);                                                                      // ������ �� ����

        float cumulativeWeight = 0f;
        foreach (var kvp in itemProbabilities)                                                                                  // �� �������� Ȯ���� ���� ��� ����
        {
            cumulativeWeight += kvp.Value;
            if (randomValue <= cumulativeWeight)
            {
                return kvp.Key;
            }
        }

        return ItemType.Coin;                                                                                                   // ������� ���� ������ �߻��� ���̹Ƿ� ����Ʈ �� ��ȯ
    }
    /// <summary>
    /// �ߺ�, LevelMax ������ ���� ��ü ������ �׸� ��ü �Լ�
    /// </summary>
    /// <param name="t_type"></param>
    /// <returns></returns>
    private Item_Control Spin_Result(ItemType t_type)
    {
        int num;
        Item_Control t_data = coin;

        switch (t_type)
        {
            case Item_Data.ItemType.nomal_weapon:
                num = Random.Range(0, nomal.Count);
                if (nomal[num] == null)
                    t_data = coin;
                else
                    t_data = nomal[num];
                break;
            case Item_Data.ItemType.reinforced_weapon:
                num = Random.Range(0, Reinforced.Count);
                if (Reinforced[num] == null)
                    t_data = coin;
                else
                    t_data = Reinforced[num];
                break;
            case Item_Data.ItemType.epic_weapon:
                num = Random.Range(0, Epic.Count);
                if (Epic[num] == null)
                    t_data = coin;
                else
                    t_data = Epic[num];
                break;
            case Item_Data.ItemType.Characteristic:
                num = Random.Range(0, Characteristic.Count);
                if (Characteristic[num] == null)
                    t_data = coin;
                else
                    t_data = Characteristic[num];
                break;
            case Item_Data.ItemType.epic_Characteristic:
                num = Random.Range(0, Epic_Charateristic.Count);
                if (Epic_Charateristic[num] == null)
                    t_data = coin;
                else
                    t_data = Epic_Charateristic[num];
                break;
            case Item_Data.ItemType.one_time_performance:
                num = Random.Range(0, One_time_performance.Count);
                if (One_time_performance[num] == null)
                    t_data = coin;
                else
                    t_data = One_time_performance[num];
                break;
            case Item_Data.ItemType.Coin:
                t_data = coin;
                break;
        }
        return t_data;
    }
    /// <summary>
    /// ���� ���� �� �׸� �� ������ ���� �� ���� �Լ�
    /// </summary>
    /// <param name="t_data"></param>
    public void Erasing(Item_Control t_data)
    {
        t_data.Activate_Check = false;

        Activates(items);
        sort();                                                                                                                             // type�� �з�
    }
    /// <summary>
    /// Ư�� ��� 6�� �׸� ä���⿡ ��� �׸� ���� �Լ�
    /// </summary>
    /// <param name="t_data"></param>
    public void Erasing_All_C(List<Item_Data> t_data)
    {
        List<Item_Control> t_List = new List<Item_Control>();
        List<Item_Control> c_List = new List<Item_Control>();
        List<Item_Control> w_List = new List<Item_Control>();

        foreach(Item_Control item in items)
        {
            if (item.data.Weapon)
                w_List.Add(item);
            else
                c_List.Add(item);
        }

        for (int index=0; index<c_List.Count; index++)
        {
            for(int j=0; j < t_data.Count; j++)
            {
                if (c_List[index].data == t_data[j])
                {
                    if(!c_List[index].Max_Level)
                        c_List[index].Activate_Check = true;
                    else
                        c_List[index].Activate_Check = false;

                    break;
                }
                c_List[index].Activate_Check = false;
            }
        }
        t_List=c_List.Concat(w_List).ToList();

        items=t_List;
        Activates(items);
        sort();
    }
    /// <summary>
    /// ���� ��� 6�� �׸� ä���⿡ ��� �׸� ���� �Լ�
    /// </summary>
    /// <param name="t_data"></param>
    public void Erasing_All_W(List<Item_Data> t_data)
    {
        List<Item_Control> t_List = new List<Item_Control>();
        List<Item_Control> c_List = new List<Item_Control>();
        List<Item_Control> w_List = new List<Item_Control>();

        foreach (Item_Control item in items)
        {
            if (item.data.Weapon)
                w_List.Add(item);
            else
                c_List.Add(item);
        }

        for (int index = 0; index < w_List.Count; index++)
        {
            for (int j = 0; j < t_data.Count; j++)
            {
                if (w_List[index].data == t_data[j])
                {
                    if (!w_List[index].Max_Level)
                        w_List[index].Activate_Check = true;
                    else
                        w_List[index].Activate_Check = false;

                    break;
                }
                w_List[index].Activate_Check = false;
            }
        }
        t_List = c_List.Concat(w_List).ToList();

        items = t_List;
        Activates(items);
        sort();
    }
}
