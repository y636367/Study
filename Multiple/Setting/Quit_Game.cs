using System;
using System.Collections.Generic;
using UnityEngine;

public class Quit_Game : MonoBehaviour
{
    public Quit_Canvas_Registration QCR;                                // Quit_Canavas �Լ� ������ ���� ���� ���� ����

    /// <summary>
    /// QuitPanel �� �׸� ����
    /// </summary>
    public GameObject Quit_Panel;
    public GameObject Null_Box;
    public GameObject Warning_Text;

    [SerializeField]
    private List<GameObject> On_Panels = new List<GameObject>();        // �����Ǵ� �гε��� �����ϱ� ���� List ����

    public bool InGame = false;
    public bool Limit;

    #region �̱���
    public static Quit_Game Instance;
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
    private void Start()
    {
        Limit = true;                                                   // �ƹ����� Quit_Panel�� On �Ǵ°� �����ϱ� ���� ���� ���� �� ����

        QCR = GetComponent<Quit_Canvas_Registration>();                 // �� ��ũ��Ʈ�� ������Ʈ�� ��ϵ� ������Ʈ���� ���� ��ϵ� ������Ʈ �� �޾ƿ���
    }
    private void FixedUpdate()                                          // Quit_Panel �� ��� üũ�� ���� Update ���
    {
        if (Limit)                                                      // ���� �� ���� ���� ���� ���� ����
            return;

#if UNITY_EDITOR                                                        // ������ �� ���� ���� ��
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Panel_Out())                                            // ���� On_Panels�� ��ϵ� �г��� ���ٸ�
            {
                if (InGame)                                             // �� ����(�������� ��)�Ͻ�
                {
                    Null_Box.SetActive(false);
                    Warning_Text.SetActive(true);                       // �ߵ� �����̱� ������ ���� ��Ȳ �ݿ� �ȵȴٴ� ��� ���� ���
                }
                else                                                    // �Ϲ� �� �Ͻ�
                {
                    Null_Box.SetActive(true);
                    Warning_Text.SetActive(false);                  
                }
                Quit_Panel.SetActive(true);                             // Quit_Panel On
            }
        }
#elif UNITY_ANDROID || UNITY_IOS // IOS ���� Ȯ�� �� ���� �ʿ�
        if (Input.GetKey(KeyCode.Escape))                             // �ȵ���̵� �� Ios �÷������� �����
        {
            if (Panel_Out())
            {
                if (InGame)
                {
                    Null_Box.SetActive(false);
                    Warning_Text.SetActive(true);

                }
                else
                {
                    Null_Box.SetActive(true);
                    Warning_Text.SetActive(false);
                }
                Quit_Panel.SetActive(true);
            }
        }
#endif
    }
    /// <summary>
    /// ���� ���� �Լ�
    /// </summary>
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_ANDROID || UNITY_IOS // IOS ���� Ȯ�� �� ���� �ʿ�
    Application.Quit();
#endif
    }
    /// <summary>
    /// Ȱ��ȭ �� �г� ���
    /// </summary>
    /// <param name="t_panel"></param>
    public void Panel_In(GameObject t_panel)
    {
        On_Panels.Add(t_panel);
    }
    /// <summary>
    /// ��ϵ� �г� ��ϵ� ������� ����
    /// </summary>
    /// <returns></returns>
    public bool Panel_Out()
    {
        Sounds.instance.WindowOpen_Sound();                             // ���� �� �Ҹ� ��� 

        try                                                             // �����ִ� �г��� �ִ� �� �ϴ� try
        {
            GameObject Last_Panel = On_Panels[On_Panels.Count - 1];
            Last_Panel.SetActive(false);
            On_Panels.RemoveAt(On_Panels.Count - 1);
            return false;                                               // �־��ٸ� false ��ȯ
        }
        catch (ArgumentOutOfRangeException)                             // �����ִ� �г��� ���ٸ� RangeOut ���� �߻�
        {
            return true;                                                // �߻��� ������ ���� �Ͽ� true ��ȯ
        }
    }
    /// <summary>
    ///  ���� �����ִ�(��ϵ�) ��� �г� ���ֱ�
    /// </summary>
    public void All_Panel_Out()
    {
        On_Panels.Clear();
    }
}
