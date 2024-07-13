using UnityEngine;
using UnityEngine.UI;

public class Stage_Choice : MonoBehaviour
{
    #region Variable
    [SerializeField]
    private Sprite[] Stage_Sprite;
    [SerializeField]
    private Image Lock_Image_P;
    [SerializeField]
    private Sprite Lock_Sprite;
    [SerializeField]
    private Sprite Nomal_Sprite;
    [SerializeField]
    private Image Now_Stgae;
    [SerializeField]
    private Image Ready_Stage;

    private StageNames _StageNames;

    [SerializeField]
    private Button Left;
    [SerializeField]
    private Button Right;
    [SerializeField]
    private Button Ready_Button;

    [SerializeField]
    private Toggle[] Difficult_Choice;
    [SerializeField]
    private Text D_TIme_text;

    public int stage_Num;

    int Diffcult_num;

    [SerializeField]
    private Button Start_btn;
    [SerializeField]
    private Text Consum_life_text;
    #endregion
    private void Awake()                                                                                    // 각 항목 초기화
    {
        stage_Num= 0;
        Now_Stgae.sprite = Stage_Sprite[stage_Num];

        Left.interactable = false;
        ColorBlock colorBlock = Left.colors;
        colorBlock.normalColor = new Color(colorBlock.normalColor.r, colorBlock.normalColor.g, colorBlock.normalColor.b, colorBlock.normalColor.a / 2);

        foreach (var di_c in Difficult_Choice)
        {
            di_c.GetComponent<Toggle>().enabled = false;
            di_c.SetIsOnWithoutNotify(false);                                                                // 선택 해제
        }
        Difficult_Choice[0].GetComponent<Toggle>().enabled = true;

        Lock_Image_P.gameObject.SetActive(false);
    }
    #region Stage 선택 함수
    public void Next_Stage()
    {
        if (stage_Num == 4)
            return;
        else
        {
            stage_Num += 1;
            Now_Stgae.sprite = Stage_Sprite[stage_Num];

            Stage_Image();

            if (stage_Num == 4)                                                                                                                                 // 오른쪽 끝에 도달하면
            {
                Right.interactable = false;                                                                                                                     // Next Button 상호작용 비활성화
                ColorBlock colorBlock = Right.colors;
                colorBlock.normalColor = new Color(colorBlock.normalColor.r, colorBlock.normalColor.g, colorBlock.normalColor.b, colorBlock.normalColor.a / 2); // 더 이상 못 사용한다는 시각적 표현을 위해 알파값 절반
            }
            else if (!Left.interactable)                                                                                                                        // Prev Button의 상호작용이 비활성화 상태라면(왼쪽 끝에 도달한 상태였다면)
            {
                Left.interactable = true;                                                                                                                       // 다시 사용 할 수 있게 Prev Button 상호작용 활성화
                ColorBlock colorBlock = Left.colors;
                colorBlock.normalColor = new Color(colorBlock.normalColor.r, colorBlock.normalColor.g, colorBlock.normalColor.b, 1f);                           // 사용 할 수 있다는 시각적 표현을 위해 알파값 1
            }
        }

        PD_Control.Instance.StageManager_.Stage_num = stage_Num;
    }
    public void Prev_Stage()
    {
        if (stage_Num == 0)
            return;
        else
        {
            stage_Num -= 1;
            Now_Stgae.sprite = Stage_Sprite[stage_Num];

            Stage_Image();

            if (stage_Num == 0)                                                                                                                                 // 왼쪽 끝에 도달하면
            {
                Left.interactable = false;                                                                                                                      // Prev Button 상호작용 비활성화
                ColorBlock colorBlock = Left.colors;
                colorBlock.normalColor = new Color(colorBlock.normalColor.r, colorBlock.normalColor.g, colorBlock.normalColor.b, colorBlock.normalColor.a / 2); // 더 이상 못 사용한다는 시각적 표현을 위해 알파값 절반
            }
            else if(!Right.interactable)                                                                                                                        // Next Button의 상호작용이 비활성화 상태라면(오른쪽 끝에 도달한 상태였다면)
            {
                Right.interactable = true;                                                                                                                      // 다시 사용 할 수 있게 Next Button 상호작용 활성화
                ColorBlock colorBlock = Right.colors;
                colorBlock.normalColor = new Color(colorBlock.normalColor.r, colorBlock.normalColor.g, colorBlock.normalColor.b, 1f);                           // 사용 할 수 있다는 시각적 표현을 위해 알파값 1
            }
        }

        PD_Control.Instance.StageManager_.Stage_num = stage_Num;
    }
    #endregion
    /// <summary>
    /// Clear 된 스테이지 기반 나머지 스테이지 락 요소 결정
    /// </summary>
    private void Stage_Image()
    {
        if (Backend_GameData.Instance.Cleardatas.High_Stage >= stage_Num)
        {
            Lock_Image_P.gameObject.SetActive(false);
            Ready_Button.gameObject.SetActive(true);
        }
        else
        {
            Lock_Image_P.gameObject.SetActive(true);
            Ready_Button.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 선택된 스테이지 상세
    /// </summary>
    public void Choice_stage()
    {
        for (int index = 0; index < (Difficult_Choice.Length); index++)                                      // 모든 난이도 잠금 설정(Reset)
        {
            Difficult_Choice[index].GetComponent<Toggle>().enabled = true;
            Difficult_Choice[index].GetComponent<Toggle>().transition = Selectable.Transition.None;
            Difficult_Choice[index].transform.GetChild(1).GetComponent<Image>().sprite = Lock_Sprite;
            Difficult_Choice[index].transform.GetChild(1).GetComponent<Image>().enabled = true;
            Difficult_Choice[index].transform.GetChild(1).GetComponent<Image>().color = new Color(Difficult_Choice[index].transform.GetChild(0).GetComponent<Image>().color.r,
                Difficult_Choice[index].transform.GetChild(1).GetComponent<Image>().color.g,
                Difficult_Choice[index].transform.GetChild(1).GetComponent<Image>().color.b, 1);
            Difficult_Choice[index].GetComponent<Toggle>().enabled = false;
        }

        int length = 0;

        Ready_Stage.sprite = Now_Stgae.sprite;                                                               // 선택된 스테이지 이미지 가져오기

        switch (stage_Num)                                                                                   // Cleardata 기반 게임 진행 길이 지정
        {
            case 0:                                                                                          // stage 1
                if (Backend_GameData.Instance.Cleardatas.S1_Difficult_3)
                    length = 0;
                else if (Backend_GameData.Instance.Cleardatas.S1_Difficult_2)
                    length = 1;
                else if (Backend_GameData.Instance.Cleardatas.S1_Difficult_1)
                    length = 2;
                else
                    length = 3;
                break;
            case 1:
                if (Backend_GameData.Instance.Cleardatas.S2_Difficult_3)
                    length = 0;
                else if (Backend_GameData.Instance.Cleardatas.S2_Difficult_2)
                    length = 1;
                else if (Backend_GameData.Instance.Cleardatas.S2_Difficult_1)
                    length = 2;
                else
                    length = 3;
                break;
            case 2:
                if (Backend_GameData.Instance.Cleardatas.S3_Difficult_3)
                    length = 0;
                else if (Backend_GameData.Instance.Cleardatas.S3_Difficult_2)
                    length = 1;
                else if (Backend_GameData.Instance.Cleardatas.S3_Difficult_1)
                    length = 2;
                else
                    length = 3;
                break;
            case 3:
                if (Backend_GameData.Instance.Cleardatas.S4_Difficult_3)
                    length = 0;
                else if (Backend_GameData.Instance.Cleardatas.S4_Difficult_2)
                    length = 1;
                else if (Backend_GameData.Instance.Cleardatas.S4_Difficult_1)
                    length = 2;
                else
                    length = 3;
                break;
            case 4:
                if (Backend_GameData.Instance.Cleardatas.S5_Difficult_3)
                    length = 0;
                else if (Backend_GameData.Instance.Cleardatas.S5_Difficult_2)
                    length = 1;
                else if (Backend_GameData.Instance.Cleardatas.S5_Difficult_1)
                    length = 2;
                else
                    length = 3;
                break;
        }

        for (int index = 0; index < (Difficult_Choice.Length - length); index++)                             // 길이값을 가져와 현재까지 Clear되어 이용 할 수 있게된 난이도 언락
        {
            Difficult_Choice[index].GetComponent<Toggle>().transition = Selectable.Transition.ColorTint;
            Difficult_Choice[index].transform.GetChild(1).GetComponent<Image>().enabled = false;
            Difficult_Choice[index].transform.GetChild(1).GetComponent<Image>().color = new Color(Difficult_Choice[index].transform.GetChild(0).GetComponent<Image>().color.r,
                Difficult_Choice[index].transform.GetChild(1).GetComponent<Image>().color.g,
                Difficult_Choice[index].transform.GetChild(1).GetComponent<Image>().color.b, 0);
            Difficult_Choice[index].GetComponent<Toggle>().enabled = true;
        }
    }
    /// <summary>
    /// 난이도 구분 UI 값 설정
    /// </summary>
    /// <param name="diffcult_num"></param>
    public void Difficult_Button(int diffcult_num)
    {
        switch(diffcult_num)
        {
            case 0: // 난이도 1
                D_TIme_text.text = "Time\n10:00";
                Diffcult_num = 0;
                break;
            case 1: // 난이도 2
                D_TIme_text.text = "Time\n16:00";
                Diffcult_num = 1;
                break;
            case 2: // 난이도 3
                D_TIme_text.text = "Time\n32:00";
                Diffcult_num = 2;
                break;
            case 3: // 난이도 무한
                D_TIme_text.text = "Time\n00:00";
                Diffcult_num = 3;
                break;
        }
        Activation_Button();
    }
    /// <summary>
    /// Default 값 난이도 : 1
    /// </summary>
    public void OnReady()
    {
        Difficult_Button(0);
        Select_Difficult_Button();
    }
    /// <summary>
    /// Stage 이동 및 각 연관 데이터 갱신
    /// </summary>
    public void Go_Stage()
    {
        Start_btn.interactable = false;                                                                  // 버튼 비활성화

        PD_Control.Instance.StageManager_.Stage_num = stage_Num;                                         // Stage Data 갱신
        PD_Control.Instance.StageManager_.Difficult = Diffcult_num;

        soundManager.Instance.Save_prview_SliderVale();                                                  // 현재 Slider의 Value 값 저장
        Utils.Instance.Delay_Frame(25);
        Utils.Instance.LoadScene(_StageNames + stage_Num);                                               // 씬 이동
    }
    /// <summary>
    /// 게임을 진행함에 있어 Life가 충분한지 확인
    /// </summary>
    private void Activation_Button()
    {
        PD_Control.Instance.StageManager_.Difficult = Diffcult_num;
        PD_Control.Instance.StageManager_.Consumed_Life();
        Consum_life_text.text = $"{PD_Control.Instance.StageManager_.Life_Delete}";

        if (PD_Control.Instance.StageManager_.Life_Delete <= Backend_GameData.Instance.Userdatas.Life)
        {
            Start_btn.interactable = true;
        }
        else
            Start_btn.interactable = false;
    }
    /// <summary>
    /// 현재 선택된 난이도 표시를 위한 Slelec
    /// </summary>
    public void Select_Difficult_Button()
    {
        foreach (var di_c in Difficult_Choice)
        {
            di_c.SetIsOnWithoutNotify(false);                                                                // 선택 해제
        }
        Difficult_Choice[Diffcult_num].SetIsOnWithoutNotify(true);                                           // Player 가 선택한 Difficult만 선택
    }
}
// di_c.isOn = false; StackOverflow를 야기한다, 이유는 토글의 상태가 변경되고 이 변경이 다시 속성 설정을 호출하게 될 수 있기 때문이다.
// 이때 무한한 속성 설정이 발생하여 Overflow가 일어난다.
// 그렇기에 SetIsOnWithoutNotify메서드로 이벤트를 발생시키지 않게 설정한다.