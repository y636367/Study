using UnityEngine;
using UnityEngine.Events;

public class PD_Control : MonoBehaviour
{
    [System.Serializable]
    public class UI_Update : UnityEvent { };
    public UI_Update up_update = new UI_Update();

    public StageManager StageManager_;

    [SerializeField]
    private Player_Status_Controller[] Status;                              // 에디터 외의 플랫폼에선 스크립터블 오브젝트의 값 수정이 안되기에 컨트롤러로 조절
    [SerializeField]
    private WeaponData_Controller[] Weapons;

    public bool LevelUP_LifeMax = false;
    #region 싱글톤
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
    private void ResetStatus()                                               // Status 초기화 및 값 재 설정
    {
        for(int index=0;index< Status.Length; index++)
        {
            Status[index].Reset_value();
        }
    }
    private void ResetWeapons()                                              // Weapon 초기화 및 값 재 설정
    {
        for (int index = 0; index < Weapons.Length; index++)
        {
            Weapons[index].Reset_value();
        }

        Set_W_UI();

        up_update?.Invoke();                                                // Stage Choice 페이지 에서 현재 선택된 무기 표출
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
