using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[System.Serializable]
public class Sound
{
    public string name;                                                         //재생할 곡 이름
    public AudioClip clip;                                                      //곡
}
public class SoundManager : MonoBehaviour
{
    #region 싱글톤
    static public SoundManager Instance;
    void Awake()
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
    #endregion Singleton
    #region Variable
    public AudioSource[] audioSourcesEffect;                                    // 재생될 오디오 소스
    public AudioSource[] audioSourceBgm;

    public string[] playSoundName;                                              // 재생중인 오디오 이름

    public Sound[] EffectSound;                                                 // 재생할 오디오 미리 등록
    public Sound[] BgmSound;

    public AudioMixer audioMixer;                                               // 오디오 믹서

    public Slider BGMSlider;                                                    // 오디오 값 조절을 위한 Slider
    public Slider SFXSlider;

    float bgm_value = 0.55f;                                                    // 오디오 Slider 기본 값
    float sfx_value = 0.55f;

    public float prv_bgm_value = -1.0f;                                         // 씬 옮기기 전 현재의 Slider 값 저장할 변수
    public float prv_sfx_value = -1.0f;

    public bool TheFirst = true;
    public bool now_Set_possible = false;
    #endregion
    void Start()
    {
        playSoundName = new string[audioSourcesEffect.Length];                  // 재생중인 오디오 확인을 위한 string 배열 할당
    }
    void LateUpdate()
    {
        if (now_Set_possible)                                                   // 오디오 값 변경 가능 여부 확인
        {
            SetBgm();
            SetSfx();
        }
    }
    #region SoundEffect
    /// <summary>
    /// string 값을 받아서 미리 컴포넌트에 적용한 오디오 클립의 이름과 동일하다면 재생
    /// </summary>
    /// <param name="name"></param>
    public void PlaySoundEffect(string name)
    {
        for (int i = 0; i < EffectSound.Length; i++)
        {
            if (name == EffectSound[i].name)
            {
                for (int j = 0; j < audioSourcesEffect.Length; j++)
                {
                    if (!audioSourcesEffect[j].isPlaying)
                    {
                        playSoundName[j] = EffectSound[i].name;
                        audioSourcesEffect[j].clip = EffectSound[i].clip;
                        audioSourcesEffect[j].Play();
                        return;
                    }
                }
                return;
            }
        }
    }
    /// <summary>
    /// 현재 재생되고 있는 모든 효과음 정지
    /// </summary>
    public void StopAllSoundEffect()
    {
        for (int i = 0; i < audioSourcesEffect.Length; i++)
        {
            audioSourcesEffect[i].Stop();
        }
    }
    /// <summary>
    /// string 값을 받아 특정한 하나의 효과음 정지
    /// </summary>
    /// <param name="name"></param>
    public void StopSoundEffect(string name)
    {
        for (int i = 0; i < audioSourcesEffect.Length; i++)
        {
            if (playSoundName[i] == name)
            {
                audioSourcesEffect[i].Stop();
                return;
            }
        }
    }
    #endregion SoundEffect
    #region Bgm
    /// <summary>
    /// 효과음과 구동방식 동일
    /// </summary>
    /// <param name="name"></param>
    public void PlaySoundBGM(string name)
    {
        for (int i = 0; i < BgmSound.Length; i++)
        {
            if (name == BgmSound[i].name)
            {
                for (int j = 0; j < audioSourceBgm.Length; j++)
                {
                    if (!audioSourceBgm[j].isPlaying)
                    {
                        playSoundName[j] = BgmSound[i].name;
                        audioSourceBgm[j].clip = BgmSound[i].clip;
                        audioSourceBgm[j].Play();
                        audioSourceBgm[j].loop = true;                                      // 반복재생 체크
                        return;
                    }
                }
                return;
            }
        }
    }
    public void StopAllSoundBGM()
    {
        for (int i = 0; i < audioSourceBgm.Length; i++)
        {
            audioSourceBgm[i].Stop();
        }
    }
    public void StopSoundBGM(string name)
    {
        for (int i = 0; i < audioSourceBgm.Length; i++)
        {
            if (playSoundName[i] == name)
            {
                audioSourceBgm[i].Stop();
                return;
            }
        }
    }
    #endregion Bgm
    /// <summary>
    /// Slider의 로그 연산 값을 전달하여 해당 오디오 믹서 조절
    /// </summary>
    public void SetBgm()
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(BGMSlider.value) * 20);
    }
    public void SetSfx()
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(SFXSlider.value) * 20);
    }
    /// <summary>
    /// 해당 씬에서 오디오 믹서 조절하는 Slider를 찾고 Slider를 오디오 믹서와 연결, 기존에 설정된 값 동일하게 반영
    /// </summary>
    public void FInd_slider()
    {
        SetFunction_UI_InGame();
        SetValue_UI();
    }
    #region First_Set_Sound_Slder
    /// <summary>
    /// 최초 Slider 할당시
    /// </summary>
    public void SetFunction_UI()
    {
        BGMSlider.onValueChanged.AddListener(Function_Slider_BGM);                          //BGM 슬라이더에 Function_Slider_BGM 이벤트 추가
        SFXSlider.onValueChanged.AddListener(Function_Slider_SFX);                          //SFX 슬라이더에 Function_Slider_SFX 이벤트 추가

        ResetFunction_UI();
    }
    private void SetFunction_UI_InGame()
    {
        BGMSlider.onValueChanged.AddListener(Function_Slider_BGM);
        SFXSlider.onValueChanged.AddListener(Function_Slider_SFX);
    }
    /// <summary>
    /// 입력받은(인 게임 내부에서 슬라이더의 변경된 값)을 Slider에 반영하기 위한 함수
    /// </summary>
    /// <param name="_value"></param>
    private void Function_Slider_BGM(float _value)
    {
        bgm_value = _value;
    }
    private void Function_Slider_SFX(float _value)
    {
        sfx_value = _value;
    }
    /// <summary>
    /// 오디오 믹서를 조절할 Slider 기본 값으로 설정, 오디오 믹서 또한 기본 값으로 설정
    /// </summary>
    private void ResetFunction_UI()
    {
        BGMSlider.maxValue = 1.0f;
        SFXSlider.maxValue = 1.0f;

        BGMSlider.minValue = 0.0001f;
        SFXSlider.minValue = 0.0001f;

        BGMSlider.value = 0.55f;
        SFXSlider.value = 0.55f;

        BGMSlider.wholeNumbers = false;                                                     // 정수 값 제한 풀기
        SFXSlider.wholeNumbers = false;

        audioMixer.SetFloat("BGM", Mathf.Log10(BGMSlider.value) * 20);
        audioMixer.SetFloat("SFX", Mathf.Log10(SFXSlider.value) * 20);

        now_Set_possible = true;
        TheFirst = false;
    }
    #endregion
    private void SetValue_UI()
    {
        BGMSlider.maxValue = 1.0f;
        SFXSlider.maxValue = 1.0f;

        BGMSlider.minValue = 0.0001f;
        SFXSlider.minValue = 0.0001f;
        //값 재설정 필요함

        if (prv_bgm_value == -1.0f)
            BGMSlider.value = 0.55f;
        else
            BGMSlider.value = prv_bgm_value;
        if (prv_sfx_value == -1.0f)
            SFXSlider.value = 0.55f;
        else
            SFXSlider.value = prv_sfx_value;

        BGMSlider.wholeNumbers = false;
        SFXSlider.wholeNumbers = false;

        audioMixer.SetFloat("BGM", Mathf.Log10(BGMSlider.value) * 20);
        audioMixer.SetFloat("SFX", Mathf.Log10(SFXSlider.value) * 20);

        now_Set_possible = true;
    }
    /// <summary>
    /// 새로운 씬이 시작될때마다 해당 씬의 Slider 들을 받아 반영
    /// </summary>
    /// <param name="t_BGM"></param>
    /// <param name="t_SFX"></param>
    public void GetSliders(Slider t_BGM, Slider t_SFX)
    {
        BGMSlider = t_BGM;
        SFXSlider = t_SFX;

        if (TheFirst)
            SetFunction_UI();
        else
            FInd_slider();
    }
    /// <summary>
    /// 재생중인 BGM을 FadeOut 하여 변환 요소 추가
    /// </summary>
    public void Sounds_BGM_Fade_Out()
    {
        for (int i = 0; i < audioSourceBgm.Length; i++)
        {
            StartCoroutine(SBF(audioSourceBgm[i]));
        }
    }
    public IEnumerator SBF(AudioSource AS)
    {
        while (AS.volume > 0f)
        {
            AS.volume -= Time.deltaTime * 0.9f;
            yield return null;
        }
    }
    /// <summary>
    /// 새로운 씬이 시작될때마다 Fade_Out 했던 오디오 소스를 다시 기본값으로 설정
    /// </summary>
    /// <param name="AS"></param>
    /// <returns></returns>
    public void Reset_BGM_Fade()
    {
        for (int i = 0; i < audioSourceBgm.Length; i++)
        {
            StopCoroutine(SBF(audioSourceBgm[i]));
            audioSourceBgm[i].volume = 1f;
        }
    }
    //public void return_BGM_Volume()
    //{
    //    for (int i = 0; i < audioSourceBgm.Length; i++)
    //    {
    //        audioSourceBgm[i].volume = BGMSlider.value;
    //    }
    //}
    //public void return_SFX_Volume()
    //{
    //    for (int i = 0; i < audioSourcesEffect.Length; i++)
    //    {
    //        audioSourcesEffect[i].volume = SFXSlider.value;
    //    }
    //}
    //public void Pasue()
    //{
    //    for (int i = 0; i < audioSourceBgm.Length; i++)
    //    {
    //        audioSourceBgm[i].Pause();
    //    }
    //    for (int i = 0; i < audioSourcesEffect.Length; i++)
    //    {
    //        audioSourcesEffect[i].Pause();
    //    }
    //}

    /// <summary>
    ///  현재 재생중인 모든 효과음 일시정지
    /// </summary>
    public void Pause_Sfx()
    {
        for (int i = 0; i < audioSourcesEffect.Length; i++)
        {
            audioSourcesEffect[i].Pause();
        }
    }
    /// <summary>
    /// 모든 효과음 일시정지 해제
    /// </summary>
    public void UnPause_Sfx()
    {
        for (int i = 0; i < audioSourcesEffect.Length; i++)
        {
            audioSourcesEffect[i].UnPause();
        }
    }
    //public void Play_Re()
    //{
    //    for (int i = 0; i < audioSourceBgm.Length; i++)
    //    {
    //        audioSourceBgm[i].UnPause();
    //    }
    //    for (int i = 0; i < audioSourcesEffect.Length; i++)
    //    {
    //        audioSourcesEffect[i].UnPause();
    //    }
    //}

    /// <summary>
    /// 현재 재생중인 모든 배경음 일시정지
    /// </summary>
    public void Pause_Bgm()
    {
        for (int i = 0; i < audioSourceBgm.Length; i++)
        {
            audioSourceBgm[i].Pause();
        }
    }
    /// <summary>
    /// 모든 배경음 일시정지 해제
    /// </summary>
    public void UnPause_Bgm()
    {
        for (int i = 0; i < audioSourceBgm.Length; i++)
        {
            audioSourceBgm[i].UnPause();
        }
    }
    /// <summary>
    /// Slider Value 값 저장
    /// </summary>
    public void Save_prview_SliderVale()
    {
        prv_bgm_value = BGMSlider.value;
        prv_sfx_value = SFXSlider.value;
    }
}
