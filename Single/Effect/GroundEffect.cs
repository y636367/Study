using System.Collections;
using UnityEngine;

public class GroundEffect : MonoBehaviour
{
    [SerializeField]
    private GameObject Field;

    [Tooltip("소환할 양")]
    [SerializeField]
    private int Value;                                                                  // 소환할 양

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
    /// 이펙트 생성
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
