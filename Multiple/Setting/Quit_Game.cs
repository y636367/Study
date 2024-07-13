using System;
using System.Collections.Generic;
using UnityEngine;

public class Quit_Game : MonoBehaviour
{
    public Quit_Canvas_Registration QCR;                                // Quit_Canavas 함수 실행을 위한 전역 변수 선언

    /// <summary>
    /// QuitPanel 각 항목 변수
    /// </summary>
    public GameObject Quit_Panel;
    public GameObject Null_Box;
    public GameObject Warning_Text;

    [SerializeField]
    private List<GameObject> On_Panels = new List<GameObject>();        // 생성되는 패널들을 관리하기 위한 List 변수

    public bool InGame = false;
    public bool Limit;

    #region 싱글톤
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
        Limit = true;                                                   // 아무때나 Quit_Panel이 On 되는걸 방지하기 위한 방지 변수 값 선언

        QCR = GetComponent<Quit_Canvas_Registration>();                 // 이 스크립트가 컴포넌트로 등록된 오브젝트에서 같이 등록된 컴포넌트 값 받아오기
    }
    private void FixedUpdate()                                          // Quit_Panel 은 상시 체크를 위한 Update 사용
    {
        if (Limit)                                                      // 방지 값 으로 인한 로직 실행 방지
            return;

#if UNITY_EDITOR                                                        // 에디터 내 에서 실행 시
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Panel_Out())                                            // 현재 On_Panels에 등록된 패널이 없다면
            {
                if (InGame)                                             // 인 게임(스테이지 씬)일시
                {
                    Null_Box.SetActive(false);
                    Warning_Text.SetActive(true);                       // 중도 종료이기 때문에 진행 상황 반영 안된다는 경고 문구 출력
                }
                else                                                    // 일반 씬 일시
                {
                    Null_Box.SetActive(true);
                    Warning_Text.SetActive(false);                  
                }
                Quit_Panel.SetActive(true);                             // Quit_Panel On
            }
        }
#elif UNITY_ANDROID || UNITY_IOS // IOS 대응 확인 후 수정 필요
        if (Input.GetKey(KeyCode.Escape))                             // 안드로이드 및 Ios 플랫폼에서 실행시
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
    /// 게임 종료 함수
    /// </summary>
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_ANDROID || UNITY_IOS // IOS 대응 확인 후 수정 필요
    Application.Quit();
#endif
    }
    /// <summary>
    /// 활성화 된 패널 등록
    /// </summary>
    /// <param name="t_panel"></param>
    public void Panel_In(GameObject t_panel)
    {
        On_Panels.Add(t_panel);
    }
    /// <summary>
    /// 등록된 패널 등록된 순서대로 배출
    /// </summary>
    /// <returns></returns>
    public bool Panel_Out()
    {
        Sounds.instance.WindowOpen_Sound();                             // 닫을 때 소리 재생 

        try                                                             // 열려있는 패널이 있는 지 일단 try
        {
            GameObject Last_Panel = On_Panels[On_Panels.Count - 1];
            Last_Panel.SetActive(false);
            On_Panels.RemoveAt(On_Panels.Count - 1);
            return false;                                               // 있었다면 false 반환
        }
        catch (ArgumentOutOfRangeException)                             // 열려있는 패널이 없다면 RangeOut 오류 발생
        {
            return true;                                                // 발생할 오류에 대응 하여 true 반환
        }
    }
    /// <summary>
    ///  현재 열려있는(등록된) 모든 패널 없애기
    /// </summary>
    public void All_Panel_Out()
    {
        On_Panels.Clear();
    }
}
