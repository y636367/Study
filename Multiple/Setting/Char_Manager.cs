using System.Collections.Generic;
using UnityEngine;

public class Char_Manager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Characteristics;                                                                           // ���� Ư��

    [SerializeField]
    private Desc_Item[] Descs;

    bool Fill_up = false;

    public List<Item_Data> characteristics_data = new List<Item_Data>();
    public List<Characteristic> characteristics = new List<Characteristic>();
    /// <summary>
    /// Ư�� ȹ��� ���
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
    /// ȹ���� Ư�� ���� �Լ�
    /// </summary>
    private void Char_Count()
    {
        if (characteristics_data.Count != 6)                                                                        // �������� 6���� �ƴ϶�� 
            return;                                                                                                 // return;
        else                                                                                                        // �������� 6�����
        {
            Fill_up = true;                                                                                         // �� �̻� LevelUP, Tresure���� ���� �ʰ� ȹ������ �ʾҴ� �׸� ����
            GameManager.Instance.tresureui.Erasing_All_C(characteristics_data);
            GameManager.Instance.uilevelup.Erasing_All_C(characteristics_data);
        }
    }
    /// <summary>
    /// ȹ���� Ư�� ���� Level, Desc �� Data  ���
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
    /// LevelUP���� ���� Data ����
    /// </summary>
    /// <param name="t_num"></param>
    /// <param name="t_level"></param>
    /// <param name="t_data"></param>
    public void Update_Desc(int t_num, int t_level, Item_Data t_data)
    {
        Descs[t_num].Get_Desc(t_level, t_data);
    }
    /// <summary>
    /// Player ������� ���� Ư�� Off
    /// </summary>
    public void Off_the_Characteristic()
    {
        for(int index=0;index<characteristics.Count;index++)
        {
            characteristics[index].gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Player ��Ȱ�� Ư�� On
    /// </summary>
    public void On_the_Characteristic()
    {
        for (int index = 0; index < characteristics.Count; index++)
        {
            characteristics[index].gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// Ư���� ��ġ ���� Ư���� �ݿ�
    /// </summary>
    public void Update_Weapon_Status()
    {
        for (int index = 0; index < characteristics.Count; index++)
        {
            characteristics[index].WeapondataUpdate();
        }
    }
}
