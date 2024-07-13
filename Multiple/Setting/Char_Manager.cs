using System.Collections.Generic;
using UnityEngine;

public class Char_Manager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Characteristics;                                                                           // 보유 특성

    [SerializeField]
    private Desc_Item[] Descs;

    bool Fill_up = false;

    public List<Item_Data> characteristics_data = new List<Item_Data>();
    public List<Characteristic> characteristics = new List<Characteristic>();
    /// <summary>
    /// 특성 획득시 등록
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public GameObject Characteristic_Registration(int num)
    {
        GameObject select = null;

        select = Instantiate(Characteristics[num], transform);
        select.transform.parent = GameManager.Instance.player.transform;
        select.transform.localPosition = Vector3.zero;

        if (!Fill_up)
            Char_Count();

        return select;
    }
    /// <summary>
    /// 획득한 특성 관리 함수
    /// </summary>
    private void Char_Count()
    {
        if (characteristics_data.Count != 6)                                                                        // 가짓수가 6개가 아니라면 
            return;                                                                                                 // return;
        else                                                                                                        // 가짓수가 6개라면
        {
            Fill_up = true;                                                                                         // 더 이상 LevelUP, Tresure에서 뜨지 않게 획득하지 않았던 항목 제거
            GameManager.Instance.tresureui.Erasing_All_C(characteristics_data);
            GameManager.Instance.uilevelup.Erasing_All_C(characteristics_data);
        }
    }
    /// <summary>
    /// 획득한 특성 설명 Level, Desc 등 Data  등록
    /// </summary>
    /// <param name="t_level"></param>
    /// <param name="t_data"></param>
    /// <returns></returns>
    public int Set_Desc(int t_level, Item_Data t_data)
    {
        Descs[characteristics.Count].Get_Desc(t_level, t_data);
        return characteristics.Count;
    }
    /// <summary>
    /// LevelUP으로 인한 Data 갱신
    /// </summary>
    /// <param name="t_num"></param>
    /// <param name="t_level"></param>
    /// <param name="t_data"></param>
    public void Update_Desc(int t_num, int t_level, Item_Data t_data)
    {
        Descs[t_num].Get_Desc(t_level, t_data);
    }
    /// <summary>
    /// Player 사망으로 인한 특성 Off
    /// </summary>
    public void Off_the_Characteristic()
    {
        for(int index=0;index<characteristics.Count;index++)
        {
            characteristics[index].gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Player 부활시 특성 On
    /// </summary>
    public void On_the_Characteristic()
    {
        for (int index = 0; index < characteristics.Count; index++)
        {
            characteristics[index].gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// 특성들 수치 무기 특성에 반영
    /// </summary>
    public void Update_Weapon_Status()
    {
        for (int index = 0; index < characteristics.Count; index++)
        {
            characteristics[index].WeapondataUpdate();
        }
    }
}
