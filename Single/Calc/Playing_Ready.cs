using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;

public class Playing_Ready : MonoBehaviour
{
    [SerializeField]
    private Progress progress;                                  // 진행도에 따른 Off_ 버튼 활성화 여부를 위한 스크립트
    [SerializeField]
    private SceneNames nextScene;                               // 다음 씬으로 넘어갈때 지정한 씬 이름

    [SerializeField]
    private Text Guide_text;                                    // 안내 텍스트
    [SerializeField]
    private Loadding_Monster LM;                                // 로딩 중 표시될 몬스터
    [SerializeField]
    private Button Off_;                                        // 로딩이 완료된 후 다음액션값을 입력받아 행동하기 위한 버튼

    [SerializeField]
    private float Text_blink;                                   // Text가 블링크될 속도반영 값

    [Header("Loadding_Sound")]                                  // 로딩 완료시 재생될 사운드
    [SerializeField]
    private string Loadding_Sound;

    private void Awake()
    {
        Setup_init();                                           // 초기 설정
        FrameRate_Setting();                                    // 프레임 설정
        Off_.enabled = false;                                   // 버튼 비활성화
    }
    private void Setup_init()
    {
        Application.runInBackground = true;                     // 비활성화에서도 게임 계속 진행

        Screen.sleepTimeout = SleepTimeout.NeverSleep;          // 화면이 꺼지지 않게 설정

        progress.Play(OnAfterProgress);                         // Progress의 Play함수 후 진행될 액션
    }
    private void OnAfterProgress()                              
    {
        Changed_Monster();                                      // 로딩이 완료된 후 처리 함수_1
        Next_();           

        //계정 동기화
    }
    private void Changed_Monster()
    {
        Guide_text.text = "Touch to Start!";                                        // 현재 Now Loadding... 텍스트를 변환

        LM.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Death");      // 로딩이 완료됬음을 시각적으로 표현하기 위해 표시된 몬스터 사망 처리

        StartCoroutine(Text_Blink_out());                                           // 로딩이 완료됬음을 시각적으로 표현하기 위한 텍스트 깜빡임 효과를 위한 코루틴 실행
    }
    /// <summary>
    /// 텍스트의 알파값이 0보다 클때 점차 감소
    /// </summary>
    private IEnumerator Text_Blink_out()
    {
        while (Guide_text.color.a > 0)                                              
        {
            Guide_text.color=new Color(Guide_text.color.r, Guide_text.color.g,
                Guide_text.color.b,Guide_text.color.a-(Time.deltaTime*Text_blink));

            yield return null;
        }

        StartCoroutine(Text_Blink_in());
    }
    /// <summary>
    /// 텍스트의 알파값이 1보다 작을때 점차 증가
    /// </summary>
    private IEnumerator Text_Blink_in()
    {
        while (Guide_text.color.a < 1)
        {
            Guide_text.color = new Color(Guide_text.color.r, Guide_text.color.g,
                Guide_text.color.b, Guide_text.color.a + (Time.deltaTime * Text_blink));

            yield return null;
        }

        StartCoroutine(Text_Blink_out());
    }
    private void Next_()
    {
        Off_.enabled = true;                                    // 다음 액션 진행을 위한 버튼 활성화
        Off_.onClick.AddListener(Loadding_sound);               // 버튼에 액션 추가(클릭 및 터치시 사운드 재생)
        Off_.onClick.AddListener(Scene_Change);                 // 버튼에 액션 추가(씬 변경)
    }
    private void Loadding_sound()
    {
        soundManager.Instance.PlaySoundEffect(Loadding_Sound);
    }
    public void Scene_Change()
    {
        Quit_Game.Instance.Limit = false;                                               // 뒤로가기 버튼 및 Escape 클릭 및 터치 시 예정된 이벤트 방지를 위한 방지 값 해제

        string message = string.Empty;                                                  // 서버 연결 후 안내 메시지 전송을 위한 변수 선언

        bool autoLoginEnabled = PlayerPrefs.GetInt("AutoLogin") == 1;                   // PlayerPrefs로 현재 기기에 저장된 자동로그인 결정 값 받아오기
        PlayerPrefs.Save();                                                             // PlayerPrefs 저장

        if (autoLoginEnabled)                                                           // 자동 로그인이 참이라면
        {
            SendQueue.Enqueue(Backend.BMember.LoginWithTheBackendToken, callback =>     // 서버(뒤끝) 비동기 큐로 신호 전송
            {
                if (callback.IsSuccess())                                               // 신호 송신 성공 시
                {
                    message = Backend_GameData.Instance.GetDatas();

                    if (message != "")                                                  // User의 데이터 불러오기, 파싱 검사(데이터 불러오기 확인) 오류 체크
                    {
                        Utils.Instance.LoadScene(SceneNames.Loadding);                  // 문제 발생 초기화면으로 이동
                        return;
                    }
                    else
                    {
                        Utils.Instance.LoadScene(nextScene + 1);                        // 바로 Main씬으로 이동
                    }
                }
                else
                {
                    Utils.Instance.LoadScene(nextScene);                                // 신호 송신 실패, Loggin 씬으로 이동
                }
            });
        }
        else
        {
            Utils.Instance.LoadScene(nextScene);                                        // 자동 로그인 거짓 이라면, Loggin 씬으로 이동
        }
    }
    private void FrameRate_Setting()
    {
        Application.targetFrameRate = 70;                       // 최소 프레임 속도 설정

                                                                // 최대 프레임 속도 설정
        QualitySettings.vSyncCount = 0;                         // 수직 동기화 비활성화
    }
}
