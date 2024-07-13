using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Item_Data;

public class Item : MonoBehaviour
{
    enum Item_Type                                                                                                          // 아이템 타입 열거형
    {
        Exp_1,
        Exp_2,
        Exp_3,
        Heal,
        Bomb,
        Coin,
        Magnet,
        Tresure
    }
    enum Tresure_Grade                                                                                                      // 보물 타입 열거형
    {
        Nomal,
        Rare,
        Epic
    }

    private Dictionary<Tresure_Grade, float> T_Probabilities = new Dictionary<Tresure_Grade, float>                         // 가중치에 따른 보물 등급 Dictionary형
    {
        { Tresure_Grade.Nomal, 10f },
        { Tresure_Grade.Rare, 5f },
        { Tresure_Grade.Epic, 0.5f },
    };

    #region Variable
    [Header("값")]
    [SerializeField]
    private Item_Type Type;

    [SerializeField]
    public float Value;

    [SerializeField]
    private Transform Player;

    private Vector3 player_Position;

    [SerializeField]
    public bool Go = false;

    [SerializeField]
    private float Speed;

    [SerializeField]
    private float Timer;

    [SerializeField]
    private float Fixed_value;
    #endregion
    #region 사운드
    [Header("Coin_Sounds")]
    [SerializeField]
    private string Coin_s;

    [Header("EXP_Sounds")]
    [SerializeField]
    private string EXP_s;

    [Header("Heal_Sounds")]
    [SerializeField]
    private string Heal_s;

    [Header("Bomb_Sounds")]
    [SerializeField]
    private string Bomb_s;

    [Header("Magnet_Sounds")]
    [SerializeField]
    private string Magnet_s;
    #endregion
    /// <summary>
    /// 생성된지 1분 후 소멸
    /// </summary>
    private IEnumerator Life_time()
    {
        int Timer = 0;

        while (Timer < 60)
        {
            Timer++;
            yield return new WaitForSeconds(1f);
        }
        gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
        if (!GameManager.Instance.Start_)                                                                                              // 일시정지시 return
        {
            return;
        }

        Move_the_Objcet();
    }
   /// <summary>
   /// 각 아이템 타입에 따른 Player 및 PlayerMagnet에 닿았을 시 실행될 함수
   /// </summary>
   /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerMagnet"))
        {
            if (this.Type == Item_Type.Exp_1 || this.Type == Item_Type.Exp_2 || this.Type == Item_Type.Exp_3 || this.Type == Item_Type.Coin)            // Exp, Coin 만 Magnet 동작
                this.Go = true;
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            if (this.Type == Item_Type.Bomb)                                                                                                            // Type 폭탄일시
            {
                soundManager.Instance.PlaySoundEffect(Bomb_s);                                                                                          
                GameManager.Instance.pool.Bomb_Get();                                                                                                   // 폭탄 효과(일반 몬스터 모두 제거)
                GameManager.Instance.player.Motion_Item();
            }
            else if (this.Type == Item_Type.Magnet)                                                                                                     // Type 자석일시
            {
                soundManager.Instance.PlaySoundEffect(Magnet_s);
                GameManager.Instance.drop.Magnet_Get();                                                                                                 // 자석 효과(경험치, 코인 모두 현재 위치 무관 Player에게로)
                GameManager.Instance.player.Motion_Item();
            }
            else if (this.Type == Item_Type.Coin)                                                                                                       // Type 코인일시
            {
                soundManager.Instance.PlaySoundEffect(Coin_s);  
                GameManager.Instance.Get_Coin((int)this.Value);                                                                                         // 코인 값 가중
            }
            else if (this.Type == Item_Type.Exp_1 || this.Type == Item_Type.Exp_2 || this.Type == Item_Type.Exp_3)                                      // Type 경험치 일시
            {
                soundManager.Instance.PlaySoundEffect(EXP_s);
                GameManager.Instance.Get_Exp((int)this.Value);                                                                                          // 경험치 값 가중
            }
            else if (this.Type == Item_Type.Heal)                                                                                                       // Type 힐 일시
            {
                soundManager.Instance.PlaySoundEffect(Heal_s);
                GameManager.Instance.player.Heal();                                                                                                     // Player 체력 비례 체력 회복
                GameManager.Instance.player.Motion_Item();
            }
            else if (this.Type == Item_Type.Tresure)                                                                                                    // Type 보물 일시
            {
                GameManager.Instance.player.invincibile = true;
                GameManager.Instance.tresureui.Tresure_On();                                                                                            // 보물 획득 함수 실행
            }

            this.Go = false;
            this.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Magnet 효과를 위한 Player에게로 이동
    /// </summary>
    private void Move_the_Objcet()
    {
        if (this.Go)
        {
            player_Position = GameManager.Instance.player.transform.position;

            float distance=Vector3.Distance(transform.position, player_Position);

            Fixed_value = Speed * (distance * 0.01f);

            if (Fixed_value < 0.07f)
                Fixed_value = 0.07f;

            Vector3 pos = Vector3.MoveTowards(transform.position, player_Position, Fixed_value);
            transform.position = pos;
        }
    }
    private void OnEnable()
    {
        if (Type == Item_Type.Tresure)
            return;
        else
        {
            if (this.Type == Item_Type.Exp_1 || this.Type == Item_Type.Exp_2 || this.Type == Item_Type.Exp_3 || this.Type == Item_Type.Coin)
                StartCoroutine(nameof(Life_time));

            if (Type == Item_Type.Coin)                                                                                                 // Coin의 값의 크기 결정
            {
                int Current_Difficult = PD_Control.Instance.StageManager_.Difficult;                                                    // 난이도와
                float Current_Time = GameManager.Instance.GameTime;                                                                     // 생존시간 종합해서 Coin 값 결정

                if (Current_Difficult == 0)
                {
                    if (Current_Time <= 3.0f * 60f)
                    {
                        Value = 1;
                    }
                    else if (Current_Time <= 6.0f * 60f)
                    {
                        Value = 3;
                    }
                    else
                    {
                        Value = 7;
                    }
                }
                else if (Current_Difficult == 1)
                {
                    if (Current_Time <= 3.0f * 60f)
                    {
                        Value = 7;
                    }
                    else if (Current_Time <= 6.0f * 60f)
                    {
                        Value = 11;
                    }
                    else
                    {
                        Value = 17;
                    }
                }
                else if (Current_Difficult == 2)
                {
                    if (Current_Time <= 6.0f * 60f)
                    {
                        Value = 17;
                    }
                    else if (Current_Time <= 15.0f * 60f)
                    {
                        Value = 25;
                    }
                    else
                    {
                        Value = 35;
                    }
                }
                else
                {
                    if (Current_Time <= 6.0f * 60f)
                    {
                        Value = 35;
                    }
                    else if (Current_Time <= 15.0f * 60f)
                    {
                        Value = 45;
                    }
                    else
                    {
                        Value = 55;
                    }
                }
                Value += PD_Control.Instance.StageManager_.Stage_num * 5f;
            }
        }
    }
}
