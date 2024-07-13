using System.Collections.Generic;
using UnityEngine;

public class DropManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Items;

    private List<GameObject>[] items;

    private Transform t_transform;
    private void Awake()
    {
        items=new List<GameObject>[Items.Length];

        for(int index=0; index<items.Length; index++)
        {
            items[index]=new List<GameObject>();
        }
    }
    /// <summary>
    /// Drop 되는 아이템 오브젝트 풀링으로 필드 생성
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private GameObject Drop_Item(int index)
    {
        GameObject select = null;

        foreach (GameObject item in items[index])
        {
            if (!item.activeSelf)                                                                           // 이미 생성되었고 비활성화 상태라면
            {
                select = item;
                select.SetActive(true);                                                                     // 활성화
                break;
            }
        }
        if (!select)                                                                                        // 생성된적 없고 추가적으로 더 필요하다면
        {
            select = Instantiate(Items[index], transform);                                                  // 인스턴스 생성
            items[index].Add(select);
        }
        select.transform.position = t_transform.position;
        return select;
    }
    /// <summary>
    /// 몬스터 및 보스 사망 시 확률에 따른 코인, 경험치, 보물 생성
    /// </summary>
    /// <param name="Get_num"></param>
    /// <param name="exp"></param>
    /// <param name="t_position"></param>
    public void Category(int Get_num, int exp, Transform t_position)
    {
        int t_num;
        this.t_transform = t_position;
        // 0 : coin 1, 2, 3: exp 4 : bomb 5 : heal 6 : magnet 7 : tresure
        switch (Get_num)
        {
            case 0:
                if (Random.value < 2f / 3f)
                {
                    Drop_Item(0);                                                                           // 코인 생성
                }
                else
                    return;
                break;
            case 1:
            case 2:
            case 3:
                switch (exp)
                {
                    case 0:
                        Drop_Item(1);                                                                       // 경험치 생성
                        return;
                    case 4:
                        Drop_Item(2);
                        return;
                    case 8:
                        Drop_Item(3);
                        return;
                    default:
                        return;
                }
            case 4:                
            case 5:                
            case 6:
                if (Random.value < 1f / 50f)
                {
                    t_num = Random.Range(0, 3);
                    Drop_Item(t_num + 4);                                                                   // Item 중 한가지 랜덤 생성
                }
                else
                    return;
                break;
            case 7:
                Drop_Item(7);                                                                               // 보물 상자 생성
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// Magnet 아이템 획득 시 모든 코인, 경험치 Player에게로 이동
    /// </summary>
    public void Magnet_Get()
    {
        Magnet_Coin(0);                                                 // coin
        Magnet_Exp(1);                                                  // exp_1
        Magnet_Exp(2);                                                  // exp_2
        Magnet_Exp(3);                                                  // exp_3
    }
    /// <summary>
    /// 자석 효과로 인한 Coin들 이동제한 변수값 변경
    /// </summary>
    /// <param name="index"></param>
    private void Magnet_Coin(int index)
    {
        foreach (GameObject obj in items[index])
        {
            if (obj.activeSelf)
            {
                obj.GetComponent<Item>().Go = true;
            }
        }
    }
    /// <summary>
    /// 자석 효과로 인한 Exp들 이동제한 변수값 변경
    /// </summary>
    /// <param name="index"></param>
    private void Magnet_Exp(int index)
    {
        foreach (GameObject obj in items[index])
        {
            if (obj.activeSelf)
            {
                obj.GetComponent<Item>().Go = true;
            }
        }
    }
}
