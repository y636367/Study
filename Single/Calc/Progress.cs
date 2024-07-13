using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Progress : MonoBehaviour
{
    [SerializeField]
    private Slider Loadding_bar;                                            // 현재 로딩 진행 상황을 표현할 Slider
    [SerializeField]
    private Text Progress_text;                                             // 현재 로딩 진행 상황을 표현할 Text
    [SerializeField]
    private float Loadding_time;                                            // 로딩 항목 수(로딩 시간)

    [Header("Complite_Sound")]
    [SerializeField]
    private string Complite_Sound;                                          // 로딩 완료를 알리는 소리 이름    

    public void Play(UnityAction action=null)                               // 재생 완료시 원하는 메소드 실행하기 위해 Aciton 받음)
    {
        StartCoroutine(OnProgress(action));                                 // 코루틴 실행
    }
    private IEnumerator OnProgress(UnityAction action)
    {
        float percent = 0;                                                  // Slider, Text 값 표현을 위한 변수 선언
        float current = 0;

        while(percent < 1)
        {
            current += Time.deltaTime;                                      // 로딩 항목들이 완료 될때마다 current 가 증가 하도록 설정 변환 필요
            percent = current / Loadding_time;                              // 로딩 시간(전체 항목)동안 반복되게 나눈 값을 percent에 저장

            Progress_text.text = $"{Loadding_bar.value * 100:F0}%";         // text에 진행률 퍼센트 표출    
            Loadding_bar.value = Mathf.Lerp(0, 1, percent);                 // 게이지 이동

            yield return null;
        }
        Progress_text.text = "100%";                                        // 완료시 Loading_bar.value가 완벽하게 100에서 멈추지 않기에 시각적 표현을 위한 텍스트 변경
        soundManager.Instance.PlaySoundEffect(Complite_Sound);              // 소리 재생

        action?.Invoke();                                                   // action이 null이 아니면 action 메소드 실행
    }

    // unityAction : 반환 값이 없는 메서드 등록 가능
}
