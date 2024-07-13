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
    /// Drop �Ǵ� ������ ������Ʈ Ǯ������ �ʵ� ����
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private GameObject Drop_Item(int index)
    {
        GameObject select = null;

        foreach (GameObject item in items[index])
        {
            if (!item.activeSelf)                                                                           // �̹� �����Ǿ��� ��Ȱ��ȭ ���¶��
            {
                select = item;
                select.SetActive(true);                                                                     // Ȱ��ȭ
                break;
            }
        }
        if (!select)                                                                                        // �������� ���� �߰������� �� �ʿ��ϴٸ�
        {
            select = Instantiate(Items[index], transform);                                                  // �ν��Ͻ� ����
            items[index].Add(select);
        }
        select.transform.position = t_transform.position;
        return select;
    }
    /// <summary>
    /// ���� �� ���� ��� �� Ȯ���� ���� ����, ����ġ, ���� ����
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
                    Drop_Item(0);                                                                           // ���� ����
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
                        Drop_Item(1);                                                                       // ����ġ ����
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
                    Drop_Item(t_num + 4);                                                                   // Item �� �Ѱ��� ���� ����
                }
                else
                    return;
                break;
            case 7:
                Drop_Item(7);                                                                               // ���� ���� ����
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// Magnet ������ ȹ�� �� ��� ����, ����ġ Player���Է� �̵�
    /// </summary>
    public void Magnet_Get()
    {
        Magnet_Coin(0);                                                 // coin
        Magnet_Exp(1);                                                  // exp_1
        Magnet_Exp(2);                                                  // exp_2
        Magnet_Exp(3);                                                  // exp_3
    }
    /// <summary>
    /// �ڼ� ȿ���� ���� Coin�� �̵����� ������ ����
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
    /// �ڼ� ȿ���� ���� Exp�� �̵����� ������ ����
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
