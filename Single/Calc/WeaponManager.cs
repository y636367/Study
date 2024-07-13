using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    #region Variable
    [SerializeField]
    private GameObject[] Weapons;

    [SerializeField]
    private Desc_Item[] Descs;

    [SerializeField]
    public GameObject Axe_P;
    [SerializeField]
    public GameObject Shield_P;
    [SerializeField]
    public GameObject Flare_P;
    [SerializeField]
    public GameObject FireBall_P;

    bool Fill_up = false;

    public List<Item_Data> weapons_data = new List<Item_Data>();
    public List<Weapon> weapons = new List<Weapon>();
    #endregion
    /// <summary>
    /// ���� Ư�� ȹ��� ���� Ư�� ���
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public GameObject Weapon_Registration(int num)
    {
        GameObject select = null;

        select = Instantiate(Weapons[num], transform);
        select.transform.parent = GameManager.Instance.player.transform;
        select.transform.localPosition = Vector3.zero;
        select.transform.localEulerAngles = Vector3.zero;

        if(!Fill_up)
            Weapon_Count();

        return select;
    }
    /// <summary>
    /// Ư�� �������� 6���� ���� ���ϵ��� ���� �� ������ Max�Ͻ� ȹ��ó���� ������ Ư���� ���� �Լ�
    /// </summary>
    private void Weapon_Count()
    {
        if(weapons_data.Count != 6)
            return;
        else
        {
            Fill_up = true;
            GameManager.Instance.tresureui.Erasing_All_W(weapons_data);
            GameManager.Instance.uilevelup.Erasing_All_W(weapons_data);
        }
    }
    /// <summary>
    /// ���â���� Ȯ���� �� �ֵ��� Ư��ĭ�� ���
    /// </summary>
    /// <param name="t_level"></param>
    /// <param name="t_data"></param>
    /// <returns></returns>
    public int Set_Desc(int t_level, Item_Data t_data)
    {
        Descs[weapons.Count].Get_Desc(t_level, t_data);
        return weapons.Count;
    }
    /// <summary>
    /// Ư�� LevelUP�� ���ÿ� Ư��ĭ�� ��ϵ� ������ ����
    /// </summary>
    /// <param name="t_num"></param>
    /// <param name="t_level"></param>
    /// <param name="t_data"></param>
    public void Update_Desc(int t_num, int t_level, Item_Data t_data)
    {
        Descs[t_num].Get_Desc(t_level, t_data);
    }
    /// <summary>
    /// ������� ���� ���� ��ü off
    /// </summary>
    public void Off_the_Weapons()
    {
        Axe_P.SetActive(false);
        Shield_P.SetActive(false);
        Flare_P.SetActive(false);
        FireBall_P.SetActive(false);

        for(int index=0; index<weapons.Count; index++)
        {
            weapons[index].gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// ��Ȱ�� ���� ���� ��ü on
    /// </summary>
    public void On_the_Weapons()
    {
        Axe_P.SetActive(true);
        Shield_P.SetActive(true);
        Flare_P.SetActive(true);
        FireBall_P.SetActive(true);

        for (int index = 0; index < weapons.Count; index++)
        {
            weapons[index].gameObject.SetActive(true);
        }
    }
}


