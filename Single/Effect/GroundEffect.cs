using System.Collections;
using UnityEngine;

public class GroundEffect : MonoBehaviour
{
    [SerializeField]
    private GameObject Field;

    [Tooltip("��ȯ�� ��")]
    [SerializeField]
    private int Value;                                                                  // ��ȯ�� ��

    public int Count = 0;

    private float x_;
    private float z_;
    private Vector3 offset;
    private void Awake()
    {
        x_ = Field.transform.localScale.x;
        z_ = Field.transform.localScale.z;

        offset = Field.transform.position;
    }
    private void Start()
    {
        GameManager.instance.effectpool.GE = this;

        StartCoroutine(nameof(SpawnEffect));
    }
    /// <summary>
    /// ����Ʈ ����
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnEffect()
    {
        while (true)
        {
            if (Count < Value)
            {
                GameObject bubble = GameManager.instance.effectpool.SpawnEffect(1, GameManager.instance.effectpool.Bubbles_P.transform);

                Vector3 RandomPostion = new Vector3(Random.Range(-x_ / 2, x_ / 2), 0, Random.Range(-z_ / 2, z_ / 2));
                
                bubble.transform.position = RandomPostion + offset;
                Count++;
            }

            yield return null;
        }
    }
}
