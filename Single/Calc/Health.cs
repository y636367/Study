using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [System.Serializable]
    public class After_ : UnityEvent { };                                                       // �̺�Ʈ ������ ���� �ν��Ͻ� Ŭ���� ����
    public After_ Event;

    [SerializeField]
    public float NowHealth;
    [SerializeField]
    public float DefalutHealth;
    [SerializeField]
    private float LimitDamage;
    [SerializeField]
    private float ReviveTime;

    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }
    private void Start()
    {
        Init();
    }
    public void Init()
    {
        NowHealth = DefalutHealth;
        Event?.Invoke();
    }
    /// <summary>
    /// Player�� ���� �� ������ ���̿� ����Ͽ� �޴� ������ ��� �Լ�
    /// </summary>
    /// <param name="Damage"></param>
    /// <returns></returns>
    public bool CalcDamage(float Damage = 0.0f)
    {
        float DamageValue = 0;

        if (Damage > 0)
        {
            if (GameManager.instance.DamageOn)
            {
                float DamageWeight = Mathf.Floor(Damage * 10f) / 10f;                               // �Ҽ��� 1�ڸ� �� ����� ����
                DamageValue = DamageWeight;

                var range = new List<(float, float, float)>                                         // �� ������ �ش��ϴ� ����ġ ���� (Dictionary ���)
                {
                    (10f,15f,1.2f),                                                                 // (�ּ�, �ִ�, ����ġ)
                    (15f,20f,1.4f),
                    (20f,25f,1.6f),
                    (25f,30f,1.8f),
                    (30f,35f,2.0f),
                    (35f,40f,2.2f),
                    (40f,float.PositiveInfinity,2.4f),                                              // 40 ���� ũ�ٸ�
                };

                float multiplier = 1f;

                foreach (var (min, max, weight) in range)
                {
                    if (DamageWeight > min && DamageWeight <= max)
                    {
                        multiplier = weight;
                        break;
                    }
                }

                DamageValue *= multiplier;
                Debug.Log(DamageValue);
                NowHealth -= DamageValue;
            }

            Event?.Invoke();                                                                        // ������ ��� �� ��ϵ� �̺�Ʈ �Լ��� ����(UI ���� ��)

            if (NowHealth <= 0f)                                                                    // Player ��� ����
            {
                player.Death();
                StartCoroutine(nameof(RevivePlayer));
                return true;
            }
            else
            {
                if (DamageValue > LimitDamage)                                                           // �������� ����� ���� ��� üũ(���׵�)
                    return true;
                else
                    return false;
            }
        }
        else
            return false;
    }
    /// <summary>
    ///  Player ��Ȱ �ڷ�ƾ �޼���
    /// </summary>
    /// <returns></returns>
    public IEnumerator RevivePlayer()
    {
        int count = 0;

        while (count < ReviveTime)
        {
            count++;
            yield return new WaitForSeconds(1f);
        }

        Init();
        if(!GameManager.instance.isForcedDeath)
            GameManager.instance.rm.Transmission_Player(player);
        player.Revive();
    }
}
