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
    private void Awake()                                                                                    // �� �׸� �ʱ�ȭ
    {
        stage_Num= 0;
        Now_Stgae.sprite = Stage_Sprite[stage_Num];

        Left.interactable = false;
        ColorBlock colorBlock = Left.colors;
        colorBlock.normalColor = new Color(colorBlock.normalColor.r, colorBlock.normalColor.g, colorBlock.normalColor.b, colorBlock.normalColor.a / 2);

        foreach (var di_c in Difficult_Choice)
        {
            di_c.GetComponent<Toggle>().enabled = false;
            di_c.SetIsOnWithoutNotify(false);                                                                // ���� ����
        }
        Difficult_Choice[0].GetComponent<Toggle>().enabled = true;

        Lock_Image_P.gameObject.SetActive(false);
    }
    #region Stage ���� �Լ�
    public void Next_Stage()
    {
        if (stage_Num == 4)
            return;
        else
        {
            stage_Num += 1;
            Now_Stgae.sprite = Stage_Sprite[stage_Num];

            Stage_Image();

            if (stage_Num == 4)                                                                                                                                 // ������ ���� �����ϸ�
            {
                Right.interactable = false;                                                                                                                     // Next Button ��ȣ�ۿ� ��Ȱ��ȭ
                ColorBlock colorBlock = Right.colors;
                colorBlock.normalColor = new Color(colorBlock.normalColor.r, colorBlock.normalColor.g, colorBlock.normalColor.b, colorBlock.normalColor.a / 2); // �� �̻� �� ����Ѵٴ� �ð��� ǥ���� ���� ���İ� ����
            }
            else if (!Left.interactable)                                                                                                                        // Prev Button�� ��ȣ�ۿ��� ��Ȱ��ȭ ���¶��(���� ���� ������ ���¿��ٸ�)
            {
                Left.interactable = true;                                                                                                                       // �ٽ� ��� �� �� �ְ� Prev Button ��ȣ�ۿ� Ȱ��ȭ
                ColorBlock colorBlock = Left.colors;
                colorBlock.normalColor = new Color(colorBlock.normalColor.r, colorBlock.normalColor.g, colorBlock.normalColor.b, 1f);                           // ��� �� �� �ִٴ� �ð��� ǥ���� ���� ���İ� 1
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

            if (stage_Num == 0)                                                                                                                                 // ���� ���� �����ϸ�
            {
                Left.interactable = false;                                                                                                                      // Prev Button ��ȣ�ۿ� ��Ȱ��ȭ
                ColorBlock colorBlock = Left.colors;
                colorBlock.normalColor = new Color(colorBlock.normalColor.r, colorBlock.normalColor.g, colorBlock.normalColor.b, colorBlock.normalColor.a / 2); // �� �̻� �� ����Ѵٴ� �ð��� ǥ���� ���� ���İ� ����
            }
            else if(!Right.interactable)                                                                                                                        // Next Button�� ��ȣ�ۿ��� ��Ȱ��ȭ ���¶��(������ ���� ������ ���¿��ٸ�)
            {
                Right.interactable = true;                                                                                                                      // �ٽ� ��� �� �� �ְ� Next Button ��ȣ�ۿ� Ȱ��ȭ
                ColorBlock colorBlock = Right.colors;
                colorBlock.normalColor = new Color(colorBlock.normalColor.r, colorBlock.normalColor.g, colorBlock.normalColor.b, 1f);                           // ��� �� �� �ִٴ� �ð��� ǥ���� ���� ���İ� 1
            }
        }

        PD_Control.Instance.StageManager_.Stage_num = stage_Num;
    }
    #endregion
    /// <summary>
    /// Clear �� �������� ��� ������ �������� �� ��� ����
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
    /// ���õ� �������� ��
    /// </summary>
    public void Choice_stage()
    {
        for (int index = 0; index < (Difficult_Choice.Length); index++)                                      // ��� ���̵� ��� ����(Reset)
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

        Ready_Stage.sprite = Now_Stgae.sprite;                                                               // ���õ� �������� �̹��� ��������

        switch (stage_Num)                                                                                   // Cleardata ��� ���� ���� ���� ����
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

        for (int index = 0; index < (Difficult_Choice.Length - length); index++)                             // ���̰��� ������ ������� Clear�Ǿ� �̿� �� �� �ְԵ� ���̵� ���
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
    /// ���̵� ���� UI �� ����
    /// </summary>
    /// <param name="diffcult_num"></param>
    public void Difficult_Button(int diffcult_num)
    {
        switch(diffcult_num)
        {
            case 0: // ���̵� 1
                D_TIme_text.text = "Time\n10:00";
                Diffcult_num = 0;
                break;
            case 1: // ���̵� 2
                D_TIme_text.text = "Time\n16:00";
                Diffcult_num = 1;
                break;
            case 2: // ���̵� 3
                D_TIme_text.text = "Time\n32:00";
                Diffcult_num = 2;
                break;
            case 3: // ���̵� ����
                D_TIme_text.text = "Time\n00:00";
                Diffcult_num = 3;
                break;
        }
        Activation_Button();
    }
    /// <summary>
    /// Default �� ���̵� : 1
    /// </summary>
    public void OnReady()
    {
        Difficult_Button(0);
        Select_Difficult_Button();
    }
    /// <summary>
    /// Stage �̵� �� �� ���� ������ ����
    /// </summary>
    public void Go_Stage()
    {
        Start_btn.interactable = false;                                                                  // ��ư ��Ȱ��ȭ

        PD_Control.Instance.StageManager_.Stage_num = stage_Num;                                         // Stage Data ����
        PD_Control.Instance.StageManager_.Difficult = Diffcult_num;

        soundManager.Instance.Save_prview_SliderVale();                                                  // ���� Slider�� Value �� ����
        Utils.Instance.Delay_Frame(25);
        Utils.Instance.LoadScene(_StageNames + stage_Num);                                               // �� �̵�
    }
    /// <summary>
    /// ������ �����Կ� �־� Life�� ������� Ȯ��
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
    /// ���� ���õ� ���̵� ǥ�ø� ���� Slelec
    /// </summary>
    public void Select_Difficult_Button()
    {
        foreach (var di_c in Difficult_Choice)
        {
            di_c.SetIsOnWithoutNotify(false);                                                                // ���� ����
        }
        Difficult_Choice[Diffcult_num].SetIsOnWithoutNotify(true);                                           // Player �� ������ Difficult�� ����
    }
}
// di_c.isOn = false; StackOverflow�� �߱��Ѵ�, ������ ����� ���°� ����ǰ� �� ������ �ٽ� �Ӽ� ������ ȣ���ϰ� �� �� �ֱ� �����̴�.
// �̶� ������ �Ӽ� ������ �߻��Ͽ� Overflow�� �Ͼ��.
// �׷��⿡ SetIsOnWithoutNotify�޼���� �̺�Ʈ�� �߻���Ű�� �ʰ� �����Ѵ�.