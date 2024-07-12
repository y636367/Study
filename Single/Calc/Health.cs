using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [System.Serializable]
    public class After_ : UnityEvent { };                                                       // 이벤트 적용을 위한 인스턴스 클래스 생성
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
    /// Player가 착지 시 떨어진 높이에 비례하여 받는 데미지 계산 함수
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
                float DamageWeight = Mathf.Floor(Damage * 10f) / 10f;                               // 소수점 1자리 만 남기고 덜기
                DamageValue = DamageWeight;

                var range = new List<(float, float, float)>                                         // 각 범위에 해당하는 가중치 설정 (Dictionary 사용)
                {
                    (10f,15f,1.2f),                                                                 // (최소, 최대, 가중치)
                    (15f,20f,1.4f),
                    (20f,25f,1.6f),
                    (25f,30f,1.8f),
                    (30f,35f,2.0f),
                    (35f,40f,2.2f),
                    (40f,float.PositiveInfinity,2.4f),                                              // 40 보다 크다면
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

            Event?.Invoke();                                                                        // 데미지 계산 후 등록된 이벤트 함수들 진행(UI 갱신 등)

            if (NowHealth <= 0f)                                                                    // Player 사망 진행
            {
                player.Death();
                StartCoroutine(nameof(RevivePlayer));
                return true;
            }
            else
            {
                if (DamageValue > LimitDamage)                                                           // 데미지에 비례한 경직 모션 체크(래그돌)
                    return true;
                else
                    return false;
            }
        }
        else
            return false;
    }
    /// <summary>
    ///  Player 부활 코루틴 메서드
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
