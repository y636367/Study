using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Item_Data;

public class Tresure_UI : MonoBehaviour
{
    #region Variable
    [SerializeField]
    private Image Hall;
    [SerializeField]
    private Image Hall_2;

    [SerializeField]
    private Animator hall_animator;

    [SerializeField]
    float s_speed;
    [SerializeField]
    float c_speed;

    [SerializeField]
    float shake_time;

    RectTransform rect;

    bool first_Change;

    bool R = false;
    bool G = false;
    bool B = false;

    float hall_shakingtime = 0f;

    int weapons_count = 0;
    int characteristic_count = 0;

    bool Tresure_Pass = false;
    bool New_ins = false;
    bool Fill_Item = false;

    public int Tresure_Count;

    public bool hall_bomb = false;

    [SerializeField]
    private Tresure_Control Coin;

    private Tresure_Control[] GetTresure;

    [SerializeField]
    private GameObject _parent;

    [Header("Tresure_List")]
    [SerializeField]
    public List<Tresure_Control> tresures = new List<Tresure_Control>();

    [Header("Nomal_Weapon")]
    List<Tresure_Control> nomal = new List<Tresure_Control>();
    [Header("Reinforced_Weapon")]
    List<Tresure_Control> Reinforced = new List<Tresure_Control>();
    [Header("Epic_Weapon")]
    List<Tresure_Control> Epic = new List<Tresure_Control>();
    [Header("Characteristic_Weapon")]
    List<Tresure_Control> Characteristic = new List<Tresure_Control>();
    [Header("Epic_Characteristic_Weapon")]
    List<Tresure_Control> Epic_Charateristic = new List<Tresure_Control>();
    [Header("One_Time_performance")]
    List<Tresure_Control> One_time_performance = new List<Tresure_Control>();

    [Header("Tresure_Sounds")]
    [SerializeField]
    private string Tresure_s;
    [SerializeField]
    private string Tresure_Bomb_s;

    enum Tresure
    {
        Nomal,
        Rare,
        Epic
    }

    private Dictionary<Tresure, float> Tresurelist = new Dictionary<Tresure, float>
    {
        { Tresure.Nomal, 10.0f},
        { Tresure.Rare, 3.0f},
        { Tresure.Epic, 0.8f}
    };
    #endregion
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        tresures = new List<Tresure_Control>(GetComponentsInChildren<Tresure_Control>(true));

        foreach (Tresure_Control item in tresures)
        {
            if (item == Coin)
            {
                tresures.Remove(item);
                break;
            }
        }
        sort();
    }
    private void Update()
    {
        if (hall_bomb)
        {
            hall_anima_false();
        }
    }
    private IEnumerator ShakeHall()
    {
        while (hall_shakingtime <= shake_time)
        {
            hall_shakingtime += Time.deltaTime;
            Hall.rectTransform.anchoredPosition = new Vector2(Random.Range(-30, 30), Random.Range(-30, 30));

            yield return null;
        }
        hall_shakingtime = 0f;
    }
    /// <summary>
    /// 현재 활성화 되어있는 특성들 가져와 정렬
    /// </summary>
    /// <param name="t_data"></param>
    /// <returns></returns>
    private List<Tresure_Control> Activates(List<Tresure_Control> t_data)
    {
        for (int index = t_data.Count - 1; index >= 0; index--)
        {
            if (!t_data[index].control.Activate_Check)
                t_data.Remove(t_data[index]);
        }

        if (tresures.Count == 0)
            Fill_Item = true;

        return tresures = t_data;
    }
    #region 중앙 구체 효과
    /// <summary>
    /// 중앙 구체 크기 이미지
    /// </summary>
    /// <returns></returns>
    IEnumerator Hall_Size()
    {
        Hall.rectTransform.sizeDelta = Vector2.zero;
        Hall_2.rectTransform.sizeDelta = Vector2.zero;

        while (Hall.rectTransform.sizeDelta.x < 650f)
        {
            Hall.rectTransform.sizeDelta = new Vector2(Hall.rectTransform.sizeDelta.x + (Time.deltaTime / s_speed), Hall.rectTransform.sizeDelta.y + (Time.deltaTime / s_speed));
            Hall_2.rectTransform.sizeDelta = new Vector2(Hall.rectTransform.sizeDelta.x - 50 + (Time.deltaTime / s_speed), Hall.rectTransform.sizeDelta.y - 50 + (Time.deltaTime / s_speed));
            yield return null;
        }
        Hall.rectTransform.sizeDelta = new Vector2(650, 650);
        Hall_2.rectTransform.sizeDelta = new Vector2(600, 600);
        StartCoroutine(Hall_R());
    }
    /// <summary>
    /// 색상 변경 R
    /// </summary>
    /// <returns></returns>
    IEnumerator Hall_R()
    {
        if (!R)
        {
            while (Hall.color.r > 0.0f)
            {
                Hall.color = new Color(Hall.color.r - (Time.deltaTime / c_speed), Hall.color.g, Hall.color.b);
                yield return null;
            }
            Hall.color = new Color(0.0f, Hall.color.g, Hall.color.b);
            R = true;
        }
        else
        {
            while (Hall.color.r < 1.0f)
            {
                Hall.color = new Color(Hall.color.r + (Time.deltaTime / c_speed), Hall.color.g, Hall.color.b);
                yield return null;
            }
            Hall.color = new Color(1.0f, Hall.color.g, Hall.color.b);
            R = false;
        }

        if (first_Change)
            StartCoroutine(Hall_G());
        else
        {
            first_Change = true;
            StartCoroutine(Hall_B());
        }
    }
    /// <summary>
    /// 색상변경 G
    /// </summary>
    /// <returns></returns>
    IEnumerator Hall_G()
    {
        if (!G)
        {
            while (Hall.color.g > 0.0f)
            {
                Hall.color = new Color(Hall.color.r, Hall.color.g - (Time.deltaTime / c_speed), Hall.color.b);
                yield return null;
            }
            Hall.color = new Color(Hall.color.r, 0.0f, Hall.color.b);
            G = true;
        }
        else
        {
            while (Hall.color.g < 1.0f)
            {
                Hall.color = new Color(Hall.color.r, Hall.color.g + (Time.deltaTime / c_speed), Hall.color.b);
                yield return null;
            }
            Hall.color = new Color(Hall.color.r, 1.0f, Hall.color.b);
            G = false;
        }
        StartCoroutine(Hall_B());
    }
    /// <summary>
    /// 색상변경 B
    /// </summary>
    /// <returns></returns>
    IEnumerator Hall_B()
    {
        if (!B)
        {
            while (Hall.color.b > 0.0f)
            {
                Hall.color = new Color(Hall.color.r, Hall.color.g, Hall.color.b - (Time.deltaTime / c_speed));
                yield return null;
            }
            Hall.color = new Color(Hall.color.r, Hall.color.g, 0.0f);
            B = true;
        }
        else
        {
            while (Hall.color.b < 1.0f)
            {
                Hall.color = new Color(Hall.color.r, Hall.color.g, Hall.color.b + (Time.deltaTime / c_speed));
                yield return null;
            }
            Hall.color = new Color(Hall.color.r, Hall.color.g, 1.0f);
            B = false;
        }
        StartCoroutine(Hall_R());
    }
#endregion
    /// <summary>
    /// Tresure 획득 시 랜덤 아이템 On
    /// </summary>
    /// <returns></returns>
    private Tresure Tresure_Spin()
    {
        float totalWeight = 0f;

        foreach (var probability in Tresurelist.Values)                                                 // 전체 가중치 계산
        {
            totalWeight += probability;
        }

        float randomValue = Random.Range(0f, totalWeight);                                              // 랜덤한 값 생성

        float cumulativeWeight = 0f;                                                                    // 각 아이템의 확률에 따라 결과 결정
        foreach (var kvp in Tresurelist)
        {
            cumulativeWeight += kvp.Value;
            if (randomValue <= cumulativeWeight)
            {
                return kvp.Key;
            }
        }

        return Tresure.Nomal;                                                                           // 여기까지 오면 오류가 발생한 것이므로 디폴트 값 반환
    }
    /// <summary>
    /// Tresure 해서 나온 아이템 현재 Player 가 가지고 있는 가지수 검사 확인 및 재 Spin 및 최종 결정 사안 시각적으로 보여주기
    /// </summary>
    private void Show()
    {
        rect.localScale = Vector3.one;

        Tresure_Control[] ran = new Tresure_Control[Tresure_Count];
        GetTresure = new Tresure_Control[Tresure_Count];

        if (!Fill_Item)
        {
            while (!Tresure_Pass)
            {
                Random_Setting(ran);                                                                                    // 무작위 값 설정
                Tresure_Pass = Weapons_Counts(ran);                                                                     // 무기 및 특성의 가지 수 확인

                weapons_count = 0;
                characteristic_count = 0;
            }

            while (Tresure_Pass)
            {
                reinforced_check(ran);                                                                                  // 설정 된 값들끼리의 강화형이 있는지 확인
                Multiple_Check(ran);                                                                                    // 중복된 값들이 있는지 확인

                Tresure_Pass = Check_Clear(ran);

                Change_Type_Tresure(ran);                                                                               // Not_Clear 부분 동일 타입으로 교환, 없을 시 코인으로
            }
        }
        else
        {
            for(int index=0; index<ran.Length;index++)
            {
                ran[index] = Coin;
            }
        }
        for (int index=0; index < ran.Length; index++)
        {
            Tresure_Control ranitem = ran[index];

            if (!ranitem.control.Activate_Check)                                                                        // 마지막으로 나와선 안될 아이템 등장시 Acivate_Check로 나와도 되는 오브젝트인지 확인, 아닐 시 코인으로 교체
            {
                ranitem = Coin;
            }

            if (ranitem.gameObject.activeSelf)                                                                          // 이미 나왔던 특성이 또 나왔다면 중복 특성이 나왔다는 걸 보이기 위해 인스턴스 생성
            {
                New_ins = true;
                ranitem = Instantiate(ranitem, _parent.transform);
                ranitem.instance = true;
            }
            ranitem.gameObject.SetActive(true);
            GetTresure[index]= ranitem;
        }
        Reset_Pass();
    }
    /// <summary>
    /// 다음 보물 획득 후 계산을 위한 초기화
    /// </summary>
    private void Reset_Pass()
    {
        Tresure_Pass = false;
        weapons_count = 0;
        characteristic_count = 0;
    }
    /// <summary>
    /// 중복 체크
    /// </summary>
    /// <param name="ran"></param>
    /// <returns></returns>
    private Tresure_Control[] Multiple_Check(Tresure_Control[] ran)
    {
        List<Tresure_Control> t_list = new List<Tresure_Control>();

        foreach(Tresure_Control tresure in ran)
        {
            if(!tresure.Not_Clear)
                t_list.Add(tresure);
        }

        int count = 1;                                                                                                      // 본인 이기에 1부터
        bool change;

        for(int index=0; index < t_list.Count; index++)
        {
            for(int j=index+1; j<t_list.Count; j++)
            {
                if (t_list[index] == t_list[j])
                {
                    count += 1;
                }
            }

            if (count != 1)
            {
                change = Multiple_Level_check(count, t_list[index]).Item2;
                count = Multiple_Level_check(count, t_list[index]).Item1;

                if (change)                                                                                                // 변경 요망일시
                {
                    for (int j = index + 1; j < t_list.Count; j++)
                    {
                        if (t_list[index] == t_list[j])
                        {
                            if (count != 0)
                            {
                                t_list[j].Not_Clear = true;                                                                // 재검을 위해 값 변경
                                count -= 1;
                            }                          
                        }
                    }
                }
            }
        }

        return ran;
    }
    /// <summary>
    /// 획득한 특성의 가지수가 중복으로 인해 LevelUP을 통해 개수 초과여부 판단
    /// </summary>
    /// <param name="count"></param>
    /// <param name="t_data"></param>
    /// <returns></returns>
    private (int, bool) Multiple_Level_check(int count, Tresure_Control t_data) // 다중 값 받아서 리턴
    {
        int minus_count = 0;
        bool change = false;

        if (t_data.control.level + count > 6)
        {
            minus_count = (t_data.control.level + count) - 6;
            change = true;                                                                                      // 변경 요망
        }

        return (minus_count,change);
    }
    /// <summary>
    /// 획득한 특성들중 서로의 강화형 관계가 있는지 확인
    /// </summary>
    /// <param name="ran"></param>
    /// <returns></returns>
    private Tresure_Control[] reinforced_check(Tresure_Control[] ran)
    {
        Item_Data r_data;

        for(int index=0;index<ran.Length; index++)
        {
            if (ran[index].control.data.ReinforcedMode != null)
            {
                r_data = ran[index].control.data.ReinforcedMode;

                for(int j = 0; j < ran.Length; j++)
                {
                    if (r_data == ran[j].control.data)
                    {
                        if (index > j)
                            ran[index].Not_Clear = true;                                                            // 확인
                        else
                            ran[j].Not_Clear = true;
                    }
                }
            }
        }

        return ran;
    }
    /// <summary>
    /// 보물획득시 무작위 값 설정
    /// </summary>
    /// <param name="ran"></param>
    /// <returns></returns>
    private Tresure_Control[] Random_Setting(Tresure_Control[] ran)
    {
        Tresure_Control ranitem;

        for (int index = 0; index < ran.Length; index++)
        {
            while (true)
            {
                ItemType type_data = SpinGacha();
                ranitem = Spin_Result(type_data);

                if (ranitem.control.level == ranitem.control.data.damages.Length)
                {
                    type_data = SpinGacha();
                    ranitem = Spin_Result(type_data);
                }
                else
                {
                    ran[index] = ranitem;
                    break;
                }
            }
        }
        return ran;
    }
    /// <summary>
    /// 랜덤으로 뽑기된 무기 특성 및 획득한 특성, 무기 수 와 현재 지니고 있는 특성, 무기 수의 합이 최대 값을 넘는지에 대한 확인 및 중복시 최대 Level을 넘는지에 대한 확인
    /// </summary>
    /// <param name="ran"></param>
    /// <returns></returns>
    private bool Weapons_Counts(Tresure_Control[] ran)
    {
        Item_Data t_data;
        bool return_value = false;

        for(int j=0; j<ran.Length; j++)
        {
            if (ran[j].control.data.Type != ItemType.Coin)                                                                                              // 획득한 값이 Coin이 아니고
            {
                if (ran[j].control.data.Weapon)                                                                                                         // 획득한 값이 무기일시
                {
                    for (int index = 0; index < GameManager.Instance.weaponManager.weapons_data.Count; index++)
                    {
                        t_data = GameManager.Instance.weaponManager.weapons_data[index];
                        if (ran[j].control.data == t_data || ran[j].Multiple_Check)                                                                     // 이미 획득한 무기 특성과 동일한 값이라면
                        {
                            weapons_count -= 1;                                                                                                         // WeaponCount -1
                            break;
                        }

                        if (index == GameManager.Instance.weaponManager.weapons_data.Count - 1 && !ran[j].Multiple_Check)                               // 이미 획득한 무기 중 마지막 항목까지 중복값 검사가 끝났다면
                        {
                            for (int i = j + 1; i < ran.Length; i++)
                            {
                                if (ran[j] == ran[i])                                                                                                   // 이미 확인한 항목이기에 판단 변수 값 변경
                                    ran[i].Multiple_Check = true;
                            }
                        }
                    }
                    weapons_count += 1;                                                                                                                 // 나온 무기 특성 만큼 Count UP
                }
                else
                {
                    for (int index = 0; index < GameManager.Instance.charManager.characteristics_data.Count; index++)                                   // 획득한 값이 특성일시
                    {
                        t_data = GameManager.Instance.charManager.characteristics_data[index];                                                          // 이미 획득한 특성과 동일한 값이라면
                        if (ran[j].control.data == t_data || ran[j].Multiple_Check)
                        {  
                            characteristic_count -= 1;                                                                                                  // CharacterCOunt -1
                            break;
                        }

                        if (index == GameManager.Instance.charManager.characteristics_data.Count - 1 && !ran[j].Multiple_Check)                         // 이미 획득한 무기 중 마지막 항목까지 중복값 검사가 끝났다면
                        {
                            for (int i = j + 1; i < ran.Length; i++)
                            {
                                if (ran[j] == ran[i])
                                    ran[i].Multiple_Check = true;                                                                                       // 이미 확인한 항목이기에 판단 변수 값 변경
                            }
                        }
                    }
                    characteristic_count += 1;                                                                                                          // 나온 특성 만큼 Count UP
                }
            }
        }

        foreach (Tresure_Control t_d in ran)                                                                                                            // 후 다른 검사들을 통해 재 검사 실시를 위한 판단 변수 값 초기화
        {
            t_d.Multiple_Check = false;
        }

        if ((6 - GameManager.Instance.weaponManager.weapons_data.Count) < weapons_count
            || (6 - GameManager.Instance.charManager.characteristics_data.Count) < characteristic_count)
        {
            return_value = false;                                                                                                                       // 재 실시
        }
        else
        {
            return_value = true;                                                                                                                        // 다음 검사 이동
        }
        return return_value;                                                                                                                            // 검사 결과 값 반환
    }
    /// <summary>
    /// 중복된 값으로 인해 특성 변환을 시도 하는 함수
    /// </summary>
    /// <param name="ran"></param>
    /// <returns></returns>
    private Tresure_Control[] Change_Type_Tresure(Tresure_Control[] ran)
    {
        Tresure_Control Check_data;

        for (int index = 0; index < ran.Length; index++)
        {
            if (ran[index].Not_Clear)
            {
                ran[index].Not_Clear = false;
                Check_data = ran[index];
                ran[index] = Spin_Result(ran[index].control.data.Type);                                                                 // 특성 변환

                if (Check_data == ran[index])                                                                                           // 코인 호출 
                {
                    ran[index] = Coin;                                                                                                  // 변환 했지만 동일 할 시 이미 모든 검사 가지 수 소멸이므로 Coin으로 교체
                }
            }
        }
        return ran;
    }
    /// <summary>
    /// 획득된 특성들이 무결성 검사 함수
    /// </summary>
    /// <param name="ran"></param>
    /// <returns></returns>
    private bool Check_Clear(Tresure_Control[] ran)
    {
        bool Check = false;

        for(int index=0; index < ran.Length; index++)
        {
            if (ran[index].Not_Clear)
            {
                return true;
            }

            if (index == ran.Length - 1)
                return false;
        }

        return Check;
    }
    /// <summary>
    /// ItemType에 대한 가중치 별 랜덤 특성 뽑기
    /// </summary>
    /// <returns></returns>
    private ItemType SpinGacha()
    {
        float totalWeight = 0f;

        foreach (var probability in GameManager.Instance.uilevelup.itemProbabilities.Values)                                        // 전체 가중치 계산
        {
            totalWeight += probability;
        }

        float randomValue = Random.Range(0f, totalWeight);                                                                          // 랜덤한 값 생성

        float cumulativeWeight = 0f;                                                                                                // 각 아이템의 확률에 따라 결과 결정
        foreach (var kvp in GameManager.Instance.uilevelup.itemProbabilities)
        {
            cumulativeWeight += kvp.Value;
            if (randomValue <= cumulativeWeight)
            {
                return kvp.Key;
            }
        }

        return ItemType.Coin;                                                                                                       // 여기까지 오면 오류가 발생한 것이므로 디폴트 값 반환
    }
    /// <summary>
    /// 이미 획득한 특성이거나 중복 선택으로 인한 Level Max로 인해 일부 특성 교환 함수(동일 등급 내에서만 교환 시도, 적당한 교체특성이 존재 하지 않다면 Coin으로 설정)
    /// </summary>
    /// <param name="t_type"></param>
    /// <returns></returns>
    private Tresure_Control Spin_Result(ItemType t_type)
    {
        int num;
        Tresure_Control t_data = Coin;

        switch (t_type)
        {
            case Item_Data.ItemType.nomal_weapon:
                num = Random.Range(0, nomal.Count);
                if (nomal[num] == null)
                    t_data = Coin;
                else
                    t_data = nomal[num];
                break;
            case Item_Data.ItemType.reinforced_weapon:
                num = Random.Range(0, Reinforced.Count);
                if (Reinforced[num] == null)
                    t_data = Coin;
                else
                    t_data = Reinforced[num];
                break;
            case Item_Data.ItemType.epic_weapon:
                num = Random.Range(0, Epic.Count);
                if (Epic[num] == null)
                    t_data = Coin;
                else
                    t_data = Epic[num];
                break;
            case Item_Data.ItemType.Characteristic:
                num = Random.Range(0, Characteristic.Count);
                if (Characteristic[num] == null)
                    t_data = Coin;
                else
                    t_data = Characteristic[num];
                break;
            case Item_Data.ItemType.epic_Characteristic:
                num = Random.Range(0, Epic_Charateristic.Count);
                if (Epic_Charateristic[num] == null)
                    t_data = Coin;
                else
                    t_data = Epic_Charateristic[num];
                break;
            case Item_Data.ItemType.one_time_performance:
                num = Random.Range(0, One_time_performance.Count);
                if (One_time_performance[num] == null)
                    t_data = Coin;
                else
                    t_data = One_time_performance[num];
                break;
            case Item_Data.ItemType.Coin:
                t_data = Coin;
                break;
        }
        return t_data;
    }
    /// <summary>
    /// Tresure 획득 시 효과 재생을 위하 Panel 활성화 및 각 bool 값 초기화
    /// </summary>
    public void Tresure_On()
    {
        GameManager.Instance.Stop_2();
        soundManager.Instance.PlaySoundEffect(Tresure_s);

        this.GetComponent<Button>().enabled = true;

        List<Tresure_Control> t_tresures = new List<Tresure_Control>(GetComponentsInChildren<Tresure_Control>(true));

        foreach (Tresure_Control tresure in t_tresures)
        {
            tresure.gameObject.SetActive(false);
        }
        Coin.gameObject.SetActive(false);

        rect.localScale = Vector3.one;

        first_Change = false;
        StartCoroutine(nameof(ShakeHall));
        hall_animator.enabled = false;
        Bomb_animation_();

        hall_animator.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        hall_animator.GetComponent<Button>().enabled = false;

        switch (Tresure_Spin())                                                                                 // 획득 할 수 있는 특성 가지수 결정
        {
            case Tresure.Nomal:
                Tresure_Count = 1;
                break;
            case Tresure.Rare:
                Tresure_Count = 3;
                break;
            case Tresure.Epic:
                Tresure_Count = 5;
                break;
        }
        StartCoroutine(Hall_Size());
    }
    /// <summary>
    /// Tresure 획득 적용 및 패널 숨기기
    /// </summary>
    public void Hide()
    {
        foreach(Tresure_Control tresure in GetTresure)
        {
            tresure.TresureOn();                                                                                    // Ingame적용
        }

        rect.localScale = Vector3.zero;
        sort();
        hall_animator.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        hall_animator.enabled = false;
        StopAllCoroutines();
        if (New_ins)
            list_Init();
        New_ins = false;
        GameManager.Instance.player.invincibile = false;
        GameManager.Instance.Resume_2();
    }
    /// <summary>
    /// Tresure 획득 효과 재생 도중 User가 화면 터치 시 스킵
    /// </summary>
    public void Skip()
    {
        StopAllCoroutines();
        CancelInvoke(nameof(Bomb_animation_));
        CancelInvoke(nameof(hall_animation));
        this.GetComponent<Button>().enabled = false;
        Hall.rectTransform.sizeDelta = new Vector2(650, 650);
        Hall_2.rectTransform.sizeDelta = new Vector2(600, 600);
        soundManager.Instance.StopSoundEffect(Tresure_s);

        hall_animation();
        StartCoroutine(Hall_R());
    }
    /// <summary>
    /// Hall 애니메이션을 위해 사이즈 0를 1로 변환
    /// </summary>
    public void hall_animation()
    {
        hall_animator.GetComponent<RectTransform>().sizeDelta = Vector2.one;
        hall_bomb = true;
        hall_animator.enabled = true;
    }
    /// <summary>
    /// 폭발 효과
    /// </summary>
    public void Bomb_animation_()
    {
        Invoke(nameof(hall_animation),4.7f);
    }
    /// <summary>
    /// 폭발 애니메이션 종료 확인 후 획득한 보물 특성 결과창 보이기
    /// </summary>
    private void hall_anima_false()
    {
        if(hall_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && hall_animator.GetCurrentAnimatorStateInfo(0).IsName("Bomb"))
        {
            StopCoroutine(nameof(ShakeHall));
            Hall.rectTransform.anchoredPosition = new Vector2(0, 0);
            hall_bomb = false;
            Show();
            StartCoroutine(Bomb_off());
        }
    }
    /// <summary>
    /// 폭발 애니메이션
    /// </summary>
    /// <returns></returns>
    IEnumerator Bomb_off()
    {
        soundManager.Instance.PlaySoundEffect(Tresure_Bomb_s);

        while (hall_animator.GetComponent<Image>().color.a > 0.0f)
        {
            hall_animator.GetComponent<Image>().color = new Color(hall_animator.GetComponent<Image>().color.r, hall_animator.GetComponent<Image>().color.g, hall_animator.GetComponent<Image>().color.b, hall_animator.GetComponent<Image>().color.a - (Time.deltaTime / (s_speed+0.399f)));
            yield return null;
        }
        hall_animator.GetComponent<Button>().enabled = true;
    }
    /// <summary>
    /// 획득한 특성들로 인해 최대 가지수에 도달하였거나 LevelMax가 되어 더 이상 나오면 안되는 특성들 제외 후 다시 정렬
    /// </summary>
    /// <param name="t_data"></param>
    public void Erasing(Item_Control t_data)
    {
        t_data.Activate_Check = false;

        Activates(tresures);
        sort();
    }
    /// <summary>
    /// Player 가 특성을 6개 모두 가졌을 시 나머지 특성 제거
    /// </summary>
    /// <param name="t_data"></param>
    public void Erasing_All_C(List<Item_Data> t_data)
    {
        List<Tresure_Control> t_List = new List<Tresure_Control>();
        List<Tresure_Control> c_List = new List<Tresure_Control>();
        List<Tresure_Control> w_List = new List<Tresure_Control>();

        foreach (Tresure_Control item in tresures)                                                                  // 무기, 특성 구분
        {
            if (item.control.data.Weapon)
                w_List.Add(item);
            else
                c_List.Add(item);
        }

        for (int index = 0; index < c_List.Count; index++)
        {
            for (int j = 0; j < t_data.Count; j++)
            {
                if (c_List[index].control.data == t_data[j])
                {
                    if (!c_List[index].control.Max_Level)
                        c_List[index].control.Activate_Check = true;                                                // 가지고 있는 특성들(MaxLevel이 아닌)
                    else
                        c_List[index].control.Activate_Check = false;                                               // 가지지 못한 특성들

                    break;
                }
                c_List[index].control.Activate_Check = false;
            }
        }
        t_List = c_List.Concat(w_List).ToList();

        tresures = t_List;
        Activates(tresures);
        sort();
    }
    /// <summary>
    /// Player 가 무기 특성을 6개 모두 가졌을 시 나머지 무기 특성 제거
    /// </summary>
    /// <param name="t_data"></param>
    public void Erasing_All_W(List<Item_Data> t_data)
    {
        List<Tresure_Control> t_List = new List<Tresure_Control>();
        List<Tresure_Control> c_List = new List<Tresure_Control>();
        List<Tresure_Control> w_List = new List<Tresure_Control>();

        foreach (Tresure_Control item in tresures)                                                                          // 무기, 특성 구분
        {
            if (item.control.data.Weapon)
                w_List.Add(item);
            else
                c_List.Add(item);
        }

        for (int index = 0; index < w_List.Count; index++)
        {
            for (int j = 0; j < t_data.Count; j++)
            {
                if (w_List[index].control.data == t_data[j])
                {
                    if (!w_List[index].control.Max_Level)
                        w_List[index].control.Activate_Check = true;                                                        // 획득한 무기(MaxLevel이 되지 않은)
                    else
                        w_List[index].control.Activate_Check = false;                                                       // 획득하지 못한 무기

                    break;
                }
                w_List[index].control.Activate_Check = false;                
            }
        }
        t_List = w_List.Concat(c_List).ToList();

        tresures = t_List;
        Activates(tresures);
        sort();
    }
    /// <summary>
    /// 현재 등록된 Tresure의 특성에 맞게 분류
    /// </summary>
    private void sort()
    {
        for (int index = 0; index < tresures.Count; index++)
        {
            if (tresures[index].control.data.Type == ItemType.nomal_weapon)
            {
                nomal.Add(tresures[index]);
            }
            else if (tresures[index].control.data.Type == ItemType.reinforced_weapon)
            {
                Reinforced.Add(tresures[index]);
            }
            else if (tresures[index].control.data.Type == ItemType.epic_weapon)
            {
                Epic.Add(tresures[index]);
            }
            else if (tresures[index].control.data.Type == ItemType.Characteristic)
            {
                Characteristic.Add(tresures[index]);
            }
            else if (tresures[index].control.data.Type == ItemType.epic_Characteristic)
            {
                Epic_Charateristic.Add(tresures[index]);
            }
            else if (tresures[index].control.data.Type == ItemType.one_time_performance)
            {
                One_time_performance.Add(tresures[index]);
            }
        }
    }
    /// <summary>
    /// 중복 획득을 보이기 위해 생성한 특성 제거
    /// </summary>
    private void list_Init()
    {
        List<Tresure_Control> Init_list = new List<Tresure_Control>(GetComponentsInChildren<Tresure_Control>(true));

        foreach (Tresure_Control child in Init_list)
        {
            if (child.instance)
                Destroy(child.gameObject);
        }
    }
}
