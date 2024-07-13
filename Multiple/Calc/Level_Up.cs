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
                items.Remove(item);                                                                                         // 순차 도는 중 삭제 진행시 오류 발생하기에 삭제 하고자 하는 항목이 하나일시, 삭제하자마자 바로 탈출
                break;
            }
        }

        sort();                                                                                                             // type별 분류
    }
    /// <summary>
    /// 일반형 또는 강화형 선택시 다른 항목 목록 삭제 등, Level Max, 리스트 소지 6개 가득 찼을 시 등 목록 제거 함수
    /// </summary>
    /// <param name="t_data"></param>
    /// <returns></returns>
    private List<Item_Control> Activates(List<Item_Control> t_data)
    {
        for(int index=t_data.Count-1; index>=0; index--)                                                                                    // 중간에 삭제해버려면 list 사이즈가 변경되서 잘못된 인덱스 접근할 수 있기에 역으로 접근
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
    /// 각 항목 특성 구분해서 정렬 함수
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
    /// LevelUP 시 선택 항목 표출
    /// </summary>
    public void Show()
    {
        Next();
        rect.localScale = Vector3.one;
        GameManager.Instance.Stop();                                                                                                        // 일시정지
    }
    /// <summary>
    /// 선택 완료 시 가림
    /// </summary>
    public void Hide()
    {
        rect.localScale = Vector3.zero;
        if (New_ins)
            list_Init();
        New_ins = false;
        GameManager.Instance.Resume();                                                                                                      // 재게
    }
    /// <summary>
    /// 인스턴스로 생성된 중복 항목 삭제 함수
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
    /// 선택된 항목(무기) 활성화 함수
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
    /// 선택된 무기 항목 활성화 함수
    /// </summary>
    /// <param name="t_weapon"></param>
    public void Level_Setting(Item_Control t_weapon)
    {
        switch (Backend_GameData.Instance.Userdatas.WeaponNumber)
        {
            case 1: // 권총
                while(t_weapon.level!=Backend_GameData.Instance.Userweaponlevel.Pistol)
                {
                    t_weapon.OnClick();
                }
                break;
            case 3: // 로켓런처
                while (t_weapon.level != Backend_GameData.Instance.Userweaponlevel.RocketLauncer)
                {
                    t_weapon.OnClick();
                }
                break;
            case 4: // 저격총
                while (t_weapon.level != Backend_GameData.Instance.Userweaponlevel.Sniper)
                {
                    t_weapon.OnClick();
                }
                break;
            case 6: // 기관단총
                while (t_weapon.level != Backend_GameData.Instance.Userweaponlevel.Rampage)
                {
                    t_weapon.OnClick();
                }
                break;
            case 7: // 산탄총
                while (t_weapon.level != Backend_GameData.Instance.Userweaponlevel.Shotgun)
                {
                    t_weapon.OnClick();
                }
                break;
            case 12: // 지뢰
                while (t_weapon.level != Backend_GameData.Instance.Userweaponlevel.Mine)
                {
                    t_weapon.OnClick();
                }
                break;
            case 13: // 가스쉴드
                while (t_weapon.level != Backend_GameData.Instance.Userweaponlevel.GasShield)
                {
                    t_weapon.OnClick();
                }
                break;
            case 15: // 화염방사기
                while (t_weapon.level != Backend_GameData.Instance.Userweaponlevel.FlareThrower)
                {
                    t_weapon.OnClick();
                }
                break;
            case 17: // 신호탄
                while (t_weapon.level != Backend_GameData.Instance.Userweaponlevel.Flare_gun)
                {
                    t_weapon.OnClick();
                }
                break;
            case 18: // 대검
                while (t_weapon.level != Backend_GameData.Instance.Userweaponlevel.Knife)
                {
                    t_weapon.OnClick();
                }
                break;
        }
    }
    /// <summary>
    /// 랜덤으로 선별된 특성 3가지 표출 함수
    /// </summary>
    private void Next()
    {
        List<Item_Control>  t_items = new List<Item_Control>(GetComponentsInChildren<Item_Control>(true));                                          // 현재 등록된 모든 항목 초기화

        foreach (Item_Control item in t_items)
        {
            item.gameObject.SetActive(false);
        }

        Item_Control[] ran = new Item_Control[3];                                                                                                   // 등록된 항목 중 3가지 담기 위한 배열 생성

        if (!Fill_Up)
        {
            while (!Item_Pass)
            {
                Random_Setting(ran);                                                                                                                // 무작위 값 설정
                Check_Activate(ran);                                                                                                                // 아이템 활성화 여부 확인 후 교체 필요시 동일 타입 값으로 교환, 없을 시 코인으로 교체

                Item_Pass = Activate_Check(ran);                                                                                                    // 활성화 여부 확인 하나라도 비활성화 존재 시 전체 값 재설정
            }
        }
        else
        {
            for (int index = 0; index < ran.Length; index++)                                                                                        // 더 이상 항목 선택 불가 하기에 모두 Coin 으로 교체
            {
                ran[index] = coin;
            }
        }
        Item_Pass = false;

        for(int index=0;index<ran.Length;index++)
        {
            Item_Control ranitem = ran[index];                                                                                                      // 선택된 아이템 중복 여부(Coin 의 경우 중복 생성 필요)

            if(ranitem.gameObject.activeSelf)                                                                                                       // 이미 활성화 상태라면 현재 선택된 항목 인스터스 생성해서 삽입
            {
                New_ins = true;
                ranitem = Instantiate(ranitem, _parent.transform);
                ranitem.Instance = true;
            }
            ranitem.gameObject.SetActive(true);                                                                                                     // 최종 결정된 3가지 항목 표출
        }
    }
    /// <summary>
    /// 활성화된 항목들 중 중복이 있거나 더 이상 교체할 항목이 없을 경우 Coin 교체 함수
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

                if (Check_data == ran[index])                                                                                           // 코인 호출 
                {
                    ran[index] = coin;
                }
            }
        }
        return ran;
    }
    /// <summary>
    /// 다음 정렬 함수(sort) 실행 전 각 항목 정렬 반영 판단 여부 함수
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
    /// LevelUP시 무작위 값 선출 함수
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
                ((ran[0] == ran[1] && ran[0] == coin) && (ran[0] == ran[2] && ran[2] == coin) && (ran[1] == ran[2] && ran[1] == coin)))                                         // 중복 여부 확인 함수
                break;       
        }
        return ran;
    }
    /// <summary>
    /// 항목 선출 함수
    /// </summary>
    /// <returns></returns>
    private ItemType SpinGacha()
    {
        float totalWeight = 0f;

        foreach (var probability in itemProbabilities.Values)                                                                   // 전체 가중치 계산
        {
            totalWeight += probability;
        }

        float randomValue = Random.Range(0f, totalWeight);                                                                      // 랜덤한 값 생성

        float cumulativeWeight = 0f;
        foreach (var kvp in itemProbabilities)                                                                                  // 각 아이템의 확률에 따라 결과 결정
        {
            cumulativeWeight += kvp.Value;
            if (randomValue <= cumulativeWeight)
            {
                return kvp.Key;
            }
        }

        return ItemType.Coin;                                                                                                   // 여기까지 오면 오류가 발생한 것이므로 디폴트 값 반환
    }
    /// <summary>
    /// 중복, LevelMax 등으로 인한 교체 사유로 항목 교체 함수
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
    /// 다음 정렬 시 항목 미 포함을 위한 값 변경 함수
    /// </summary>
    /// <param name="t_data"></param>
    public void Erasing(Item_Control t_data)
    {
        t_data.Activate_Check = false;

        Activates(items);
        sort();                                                                                                                             // type별 분류
    }
    /// <summary>
    /// 특성 모두 6개 항목 채웠기에 모든 항목 제거 함수
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
    /// 무기 모두 6개 항목 채웠기에 모든 항목 제거 함수
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
