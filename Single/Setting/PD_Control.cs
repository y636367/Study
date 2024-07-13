using UnityEngine;
using UnityEngine.Events;

public class PD_Control : MonoBehaviour
{
    [System.Serializable]
    public class UI_Update : UnityEvent { };
    public UI_Update up_update = new UI_Update();

    public StageManager StageManager_;

    [SerializeField]
    private Player_Status_Controller[] Status;                              // ������ ���� �÷������� ��ũ���ͺ� ������Ʈ�� �� ������ �ȵǱ⿡ ��Ʈ�ѷ��� ����
    [SerializeField]
    private WeaponData_Controller[] Weapons;

    public bool LevelUP_LifeMax = false;
    #region �̱���
    public static PD_Control Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
    #endregion
    public void ResetDatas()
    {
        ResetStatus();
        ResetWeapons();
    }
    private void ResetStatus()                                               // Status �ʱ�ȭ �� �� �� ����
    {
        for(int index=0;index< Status.Length; index++)
        {
            Status[index].Reset_value();
        }
    }
    private void ResetWeapons()                                              // Weapon �ʱ�ȭ �� �� �� ����
    {
        for (int index = 0; index < Weapons.Length; index++)
        {
            Weapons[index].Reset_value();
        }

        Set_W_UI();

        up_update?.Invoke();                                                // Stage Choice ������ ���� ���� ���õ� ���� ǥ��
    }
    public void Set_Controller(WeaponData_Controller[] w_datas, Player_Status_Controller[] s_datas)
    {
        for(int index=0;index<w_datas.Length;index++)
        {
            Weapons[index] = w_datas[index];
        }

        for(int index = 0; index < s_datas.Length; index++)
        {
            Status[index] = s_datas[index];
        }

        ResetDatas();
    }
    private void Set_W_UI()
    {
        for (int index = 0; index < Weapons.Length; index++)
        {
            Weapons[index].ui.Data_Up();
        }
    }
}
