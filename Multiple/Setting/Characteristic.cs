using UnityEngine;

public class Characteristic : MonoBehaviour
{
    #region Variable
    [Header("�⺻ ��ġ")]
    [SerializeField]
    private int id;
    [SerializeField]
    private float Figure_1;
    [SerializeField]
    private float Figure_2;                                                     // ���� Ư���� ���� ������
    [SerializeField]
    private float Figure_3;

    [Header("��ȭ ��ġ")]
    [SerializeField]
    private float UP_figure_1;
    [SerializeField]
    private float UP_figure_2;
    [SerializeField]
    private float UP_figure_3;

    [Header("Weapon_Check")]
    [SerializeField]
    private int maxLv;
    [SerializeField]
    private int current_Lv;

    [SerializeField]
    private Player player;

    [SerializeField]
    private bool Attack;
    [SerializeField]
    private bool Shield;
    [SerializeField]
    private bool Health;
    [SerializeField]
    private bool MoveSpeed;
    [SerializeField]
    private bool CoolTime;
    [SerializeField]
    private bool AttackSpeed;

    public bool Epic_1;
    #endregion
    private void Awake()
    {
        player = GameManager.Instance.player;
    }
    private void Start()
    {
        Init();
    }
    /// <summary>
    /// ȹ���� Ư�� �ʱ�ȭ
    /// </summary>
    public void Init()
    {
        this.current_Lv = 1;

        switch (id)
        {
            case 11:
                if (Attack)
                {
                    player.Attack_power += player.t_Attack_power += this.Figure_1;
                    Backend_GameData.Instance.Userstatusdatas.Attack_Power += player.t_Attack_power;
                    UP_Status();
                }
                else if (Shield)
                {
                    player.Defensive_power += this.Figure_1;
                }
                else if (Health)
                {
                    player.Physical_strength += this.Figure_1;
                    player.current_hp += this.Figure_1;
                }
                else if (MoveSpeed)
                {
                    player.Move_speed += this.Figure_1;
                }
                break;
            case 12:
                player.Attack_speed += player.t_Attack_speed += this.Figure_1;
                Backend_GameData.Instance.Userstatusdatas.AttackSpeed += player.t_Attack_speed;
                Multiple();
                break;
            case 13:
                player.Magnet_Range(this.Figure_1);
                break;
            case 14:
                if (this.Epic_1)
                {
                    player.Attack_power += player.t_Attack_power += this.Figure_1;
                    Backend_GameData.Instance.Userstatusdatas.Attack_Power += player.t_Attack_power;
                    UP_Status();
                    player.Physical_strength += this.Figure_2;
                    player.Defensive_power += this.Figure_3;
                }
                else
                {
                    player.Move_speed += this.Figure_1;
                    player.Attack_speed += player.t_Attack_speed += (this.Figure_2 + this.Figure_3);
                    Backend_GameData.Instance.Userstatusdatas.AttackSpeed += player.t_Attack_speed;
                    Multiple();
                }
                break;
            case 15:
                player.Recovery = true;
                player.Recovery_Status = this.Figure_1;
                break;
            case 16:
                GameManager.Instance.Resurrection = true;
                this.current_Lv = maxLv;
                break;
            case 17:
                R_Multiple(this.Figure_1);
                break;
        }
    }
    /// <summary>
    /// ���� ������ �ִ� Weapon Damage ����
    /// </summary>
    private void UP_Status()
    {
        for (int index = 0; index < GameManager.Instance.weaponManager.weapons.Count; index++)
        {
            GameManager.Instance.weaponManager.weapons[index].Update_Damage();
        }
    }
    /// <summary>
    /// ���� ������ �ִ� Weapon ��Ÿ�� ����
    /// </summary>
    /// <param name="t_num"></param>
    private void Multiple()
    {
        for (int index = 0; index < GameManager.Instance.weaponManager.weapons.Count; index++)
        {
            if (GameManager.Instance.weaponManager.weapons[index].id != 0 || GameManager.Instance.weaponManager.weapons[index].id != 8)
                GameManager.Instance.weaponManager.weapons[index].Multiple_status();
        }
    }
    /// <summary>
    /// ���� ������ �ִ� Weapon ���ӽð� ����
    /// </summary>
    /// <param name="t_num"></param>
    private void R_Multiple(float t_num)
    {
        for (int index = 0; index < GameManager.Instance.weaponManager.weapons.Count; index++)
        {
            if (GameManager.Instance.weaponManager.weapons[index].id == 8)
                GameManager.Instance.weaponManager.weapons[index].R_Multiple_status(t_num);
        }
    }
    /// <summary>
    /// Ư�� LevelUP�� ȿ�� ����
    /// </summary>
    public void LevelUp()
    {
        if (this.current_Lv < maxLv)
        {
            this.current_Lv += 1;
            switch (id)
            {
                case 11:
                    if (Attack)
                    {
                        player.Attack_power += player.t_Attack_power += this.UP_figure_1;
                        Backend_GameData.Instance.Userstatusdatas.Attack_Power += this.UP_figure_1;
                        UP_Status();
                    }
                    else if (Shield)
                    {
                        player.Defensive_power += this.UP_figure_1;
                    }
                    else if (Health)
                    {
                        player.Physical_strength += this.UP_figure_1;
                        player.current_hp += this.UP_figure_1;
                    }
                    else if (MoveSpeed)
                    {
                        player.Move_speed += this.UP_figure_1;
                    }
                    break;
                case 12:
                    player.Attack_speed += player.t_Attack_speed += this.UP_figure_1;
                    Backend_GameData.Instance.Userstatusdatas.AttackSpeed += player.t_Attack_speed;
                    Multiple();
                    break;
                case 13:
                    this.Figure_1 += this.UP_figure_1;
                    player.Magnet_Range(this.Figure_1);
                    break;
                case 14:
                    if (this.Epic_1)
                    {
                        player.Attack_power += player.t_Attack_power += this.UP_figure_1;
                        Backend_GameData.Instance.Userstatusdatas.Attack_Power += this.UP_figure_1;
                        UP_Status();
                        player.Physical_strength += this.UP_figure_2;
                        player.Defensive_power += this.UP_figure_3;
                    }
                    else
                    {
                        player.Move_speed += this.UP_figure_1;
                        player.Attack_speed += player.t_Attack_speed += (this.UP_figure_2 + this.UP_figure_3);
                        Backend_GameData.Instance.Userstatusdatas.AttackSpeed += player.t_Attack_speed;
                        Multiple();
                    }
                    break;
                case 15:
                    player.Recovery_Status = this.Figure_1 += this.UP_figure_1;
                    break;
                case 17:
                    float t_figure = this.Figure_1 + this.UP_figure_1;
                    R_Multiple(t_figure);
                    break;
            }
        }
    }
    /// <summary>
    /// Ư�� ȹ�� �� ���� Ư�� ȹ�� �� Ư�� �ݿ� ��ġ �ݿ�
    /// </summary>
    public void WeapondataUpdate()
    {
        switch (id)
        {
            case 11:
                if (Attack)
                    UP_Status();
                break;
            case 12:
                Multiple();
                break;
            case 14:
                if (this.Epic_1)               
                    UP_Status();
                else
                    Multiple();
                break;
            case 17:
                float t_figure = this.Figure_1 + this.UP_figure_1;
                R_Multiple(t_figure);
                break;
        }
    }
}
