using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    #region Variable
    [SerializeField]
    private GameObject[] Monsters;

    private List<GameObject>[] monsters;

    [SerializeField]
    private GameObject[] Middle_Boss;

    private List<GameObject>[] middle_boss;

    [SerializeField]
    private GameObject[] Final_Boss;

    private List<GameObject>[] final_boss;

    [SerializeField]
    private GameObject[] Bullets;

    private List<GameObject>[] bullets;

    [SerializeField]
    private GameObject[] Effects;

    private List<GameObject>[] effects;

    [SerializeField]
    private GameObject Enemy;
    [SerializeField]
    private GameObject Effect;
    [SerializeField]
    private GameObject Boss;
    [SerializeField]
    public GameObject Bullet_P;

    public int EnemyCount = 0;
    #endregion
    /// <summary>
    /// List �ʱ�ȭ
    /// </summary>
    private void Awake()
    {
        monsters = new List<GameObject>[Monsters.Length];
        middle_boss = new List<GameObject>[Middle_Boss.Length];
        final_boss = new List<GameObject>[Final_Boss.Length];
        bullets=new List<GameObject>[Bullets.Length];
        effects = new List<GameObject>[Effects.Length];

        for (int index = 0; index < monsters.Length; index++)
        {
            monsters[index] = new List<GameObject>();
        }

        for (int index = 0; index < middle_boss.Length; index++)
        {
            middle_boss[index] = new List<GameObject>();
        }

        for (int index = 0; index < final_boss.Length; index++)
        {
            final_boss[index] = new List<GameObject>();
        }

        for (int index = 0; index < bullets.Length; index++)
        {
            bullets[index] = new List<GameObject>();
        }

        for (int index = 0; index < effects.Length; index++)
        {
            effects[index] = new List<GameObject>();
        }
    }
    /// <summary>
    /// �Ϲ� ���� ������Ʈ Ǯ��
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public GameObject Get(int index)
    {
        GameObject select = null;

        foreach (GameObject item in monsters[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }
        if (!select)
        {
            select = Instantiate(Monsters[index], Enemy.transform);                                                 // ������ �ν���Ʈ �ڽ�����

            monsters[index].Add(select);
        }

        EnemyCount++;

        return select;
    }
    /// <summary>
    /// �߰� ���� ������Ʈ Ǯ��
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public GameObject Middle_Get(int index)
    {
        GameObject select = null;

        foreach (GameObject item in middle_boss[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }
        if (!select)
        {
            select = Instantiate(Middle_Boss[index], Boss.transform);

            middle_boss[index].Add(select);
        }

        return select;
    }
    /// <summary>
    /// ���� ������Ʈ Ǯ��
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public GameObject Final_Get(int index)
    {
        GameObject select = null;

        foreach (GameObject item in final_boss[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }
        if (!select)
        {
            select = Instantiate(Final_Boss[index], Boss.transform);

            final_boss[index].Add(select);
        }

        return select;
    }
    /// <summary>
    /// �Ѿ� ������Ʈ Ǯ��
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public GameObject Bullet_Get(int index)
    {
        GameObject select = null;

        foreach (GameObject item in bullets[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }
        if (!select)
        {
            select = Instantiate(Bullets[index], Bullet_P.transform);

            bullets[index].Add(select);
        }

        return select;
    }
    /// <summary>
    /// ����Ʈ ������Ʈ Ǯ��
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public GameObject Effect_Get(int index)
    {
        GameObject select = null;

        foreach (GameObject item in effects[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }
        if (!select)
        {
            select = Instantiate(Effects[index], Effect.transform);

            effects[index].Add(select);
        }

        return select;
    }
    /// <summary>
    /// List�� ��ϵ� �Ϲ� ���� Player�� ��ź ������ ȹ��� ���� ��� �Լ�
    /// </summary>
    public void Bomb_Get()
    {
        for(int index = 0; index < Monsters.Length; index++)
        {
            foreach (GameObject obj in monsters[index])
            {
                if (obj.activeSelf)
                {
                    obj.GetComponent<Slime>().Death_state();
                }
            }
        }
    }
}
