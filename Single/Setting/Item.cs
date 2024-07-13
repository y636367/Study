using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Item_Data;

public class Item : MonoBehaviour
{
    enum Item_Type                                                                                                          // ������ Ÿ�� ������
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
    enum Tresure_Grade                                                                                                      // ���� Ÿ�� ������
    {
        Nomal,
        Rare,
        Epic
    }

    private Dictionary<Tresure_Grade, float> T_Probabilities = new Dictionary<Tresure_Grade, float>                         // ����ġ�� ���� ���� ��� Dictionary��
    {
        { Tresure_Grade.Nomal, 10f },
        { Tresure_Grade.Rare, 5f },
        { Tresure_Grade.Epic, 0.5f },
    };

    #region Variable
    [Header("��")]
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
    #region ����
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
    /// �������� 1�� �� �Ҹ�
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
        if (!GameManager.Instance.Start_)                                                                                              // �Ͻ������� return
        {
            return;
        }

        Move_the_Objcet();
    }
   /// <summary>
   /// �� ������ Ÿ�Կ� ���� Player �� PlayerMagnet�� ����� �� ����� �Լ�
   /// </summary>
   /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerMagnet"))
        {
            if (this.Type == Item_Type.Exp_1 || this.Type == Item_Type.Exp_2 || this.Type == Item_Type.Exp_3 || this.Type == Item_Type.Coin)            // Exp, Coin �� Magnet ����
                this.Go = true;
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            if (this.Type == Item_Type.Bomb)                                                                                                            // Type ��ź�Ͻ�
            {
                soundManager.Instance.PlaySoundEffect(Bomb_s);                                                                                          
                GameManager.Instance.pool.Bomb_Get();                                                                                                   // ��ź ȿ��(�Ϲ� ���� ��� ����)
                GameManager.Instance.player.Motion_Item();
            }
            else if (this.Type == Item_Type.Magnet)                                                                                                     // Type �ڼ��Ͻ�
            {
                soundManager.Instance.PlaySoundEffect(Magnet_s);
                GameManager.Instance.drop.Magnet_Get();                                                                                                 // �ڼ� ȿ��(����ġ, ���� ��� ���� ��ġ ���� Player���Է�)
                GameManager.Instance.player.Motion_Item();
            }
            else if (this.Type == Item_Type.Coin)                                                                                                       // Type �����Ͻ�
            {
                soundManager.Instance.PlaySoundEffect(Coin_s);  
                GameManager.Instance.Get_Coin((int)this.Value);                                                                                         // ���� �� ����
            }
            else if (this.Type == Item_Type.Exp_1 || this.Type == Item_Type.Exp_2 || this.Type == Item_Type.Exp_3)                                      // Type ����ġ �Ͻ�
            {
                soundManager.Instance.PlaySoundEffect(EXP_s);
                GameManager.Instance.Get_Exp((int)this.Value);                                                                                          // ����ġ �� ����
            }
            else if (this.Type == Item_Type.Heal)                                                                                                       // Type �� �Ͻ�
            {
                soundManager.Instance.PlaySoundEffect(Heal_s);
                GameManager.Instance.player.Heal();                                                                                                     // Player ü�� ��� ü�� ȸ��
                GameManager.Instance.player.Motion_Item();
            }
            else if (this.Type == Item_Type.Tresure)                                                                                                    // Type ���� �Ͻ�
            {
                GameManager.Instance.player.invincibile = true;
                GameManager.Instance.tresureui.Tresure_On();                                                                                            // ���� ȹ�� �Լ� ����
            }

            this.Go = false;
            this.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Magnet ȿ���� ���� Player���Է� �̵�
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

            if (Type == Item_Type.Coin)                                                                                                 // Coin�� ���� ũ�� ����
            {
                int Current_Difficult = PD_Control.Instance.StageManager_.Difficult;                                                    // ���̵���
                float Current_Time = GameManager.Instance.GameTime;                                                                     // �����ð� �����ؼ� Coin �� ����

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
