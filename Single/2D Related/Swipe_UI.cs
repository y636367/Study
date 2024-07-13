using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Swipe_UI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    #region Variable
    [SerializeField]
    private Scrollbar scrollbar;
    [SerializeField]
    private Slider tabSlider;
    [SerializeField]
    private Image[] ImageContents;
    [SerializeField]
    private Button[] Bottom_Buttons;
    [SerializeField]
    private float swipeTime;
    [SerializeField]
    private float s_Distance;                                                                                                   // swipe �Ǳ� ���� �����̴� �ּ� �Ÿ�
    [SerializeField]
    private float currentPageImageScale;

    private int currentPage;
    private int MaxPage;

    private float[] PageValues;
    private float PageDistance;

    [SerializeField]
    private float CurPos;
    [SerializeField]
    private float TargetPos;

    [SerializeField]
    private bool isSwipe;

    [SerializeField]
    private float Content_Size;
    [SerializeField]
    private GameObject Content;

    [SerializeField]
    private GameObject tabSlider_Image;
    [SerializeField]
    private float tabSlider_Width;

    [SerializeField]
    private GameObject Weapon_List_scrollview;

    [SerializeField]
    private GameObject Weapon_Scroll;

    [SerializeField]
    private int page_num;

    [SerializeField]
    private GameObject W_Upgrade_Panel;
    [SerializeField]
    private GameObject S_Upgrade_Panel;
    [SerializeField]
    private GameObject Ready_Panel;
    [SerializeField]
    private GameObject Ready_W_Upgrade_Panel;

    bool Main_First_On;
    #endregion
    private void Awake()
    {
        Get_Buttons_Data();

        Update_Grid();
        SetScrollBarValue(page_num);                                                                            // ���� ���� ������ 0 : playerUpgrade 1 : Game Ready 2 : Weapon Upgrade
    }
    /// <summary>
    /// �г� ���� �� Quit List ����
    /// </summary>
    private void Off_Panels()
    {
        if (W_Upgrade_Panel.activeSelf)
        {
            Quit_Game.Instance.Panel_Out();
            W_Upgrade_Panel.SetActive(false);
        }
        else if (S_Upgrade_Panel.activeSelf)
        {
            Quit_Game.Instance.Panel_Out();
            S_Upgrade_Panel.SetActive(false);
        }
        else if (Ready_Panel.activeSelf)
        {
            Quit_Game.Instance.Panel_Out();
            Ready_Panel.SetActive(false);
        }
        else if(Ready_W_Upgrade_Panel.activeSelf)
        {
            Quit_Game.Instance.Panel_Out();
            Ready_W_Upgrade_Panel.SetActive(false);
        }
    }
    public void OnDrag(PointerEventData eventData) => isSwipe = true;                                                               // �޼��� ����̺�(����)
    /// <summary>
    /// eventData.Delta(���� ���� �������� �� ����) = ���� ��ȭ��
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        Main_First_On = false;
        CurPos = SetPos();
    }
    /// <summary>
    /// Drag ���� �Ǵ� �Լ�(Drag ���̸� ���� �� �̺�Ʈ���� ���� �� ���)
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        // �� �÷����� ���� ��ġ ���� �ʿ�
        isSwipe = false;

        TargetPos = SetPos();                                                                                                       // ���� Page ��ȣ(0�̶�� ����, �ƴ϶�� ��, ������ ������)

        if (CurPos == TargetPos)                                                                                                    // ���� �Ÿ��� ���� �ʾƵ� ���콺�� ������ �̵��ϸ�
        {
            if (eventData.delta.x > 18 && CurPos - PageDistance >= 0)                                                               // ��ũ���� �������� ������ �̵� ��
            {
                --currentPage;
                TargetPos = CurPos - PageDistance;
            }
            else if (eventData.delta.x < -18 && CurPos + PageDistance <= 1.01f)                                                     // ��ũ���� ���������� ������ �̵��� (�ݿø� �ؼ� ������ �߻��ϱ⿡ 1.01)
            {
                ++currentPage;
                TargetPos = CurPos + PageDistance;
            }
        }

        #region ���� ���â�� ���� ��ũ���� �����ϱ⿡ �ٸ�â�� �ٳ���� �Ǹ� ������ �ʱ� ������ ���� (Only Top)
        if (CurPos != PageValues[currentPage] && TargetPos == PageValues[currentPage])
        {
            Off_Panels();
            if (currentPage == 2)
            {
                Weapon_Scroll.GetComponent<Scrollbar>().value = 1;
            }
        }
        #endregion
    }
    /// <summary>
    /// Page �ѱ��� �Ǵ� ���� �Լ� (Page ���� �Ÿ��� �������� ����� ��ġ�� ��ȯ)
    /// </summary>
    /// <returns></returns>
    private float SetPos()
    {
        for (int index = 0; index < PageValues.Length; index++)
        {
            if ((scrollbar.value) < PageValues[index] + PageDistance * 0.5f && (scrollbar.value) > PageValues[index] - PageDistance * 0.5f)
            {
                currentPage = index;
                return PageValues[index];
            }
        }
        return 0;
    }
    /// <summary>
    /// Ư�� ���� �ȿ����� �����ϰ� �ϱ� ���� ������ ����(���� ��ũ��)
    /// </summary>
    private void Grid_OnValue()
    {
        float weapon_size = this.GetComponent<RectTransform>().rect.height/3;                                                   // rect.size ����

        Content_Size = this.GetComponent<RectTransform>().rect.width;

        Weapon_List_scrollview.GetComponent<RectTransform>().sizeDelta = new Vector2(0, weapon_size);                           // ���� ��� ��ũ�� ������
    }
    /// <summary>
    /// Scroll�� ���� �� ������ Value �� �Ÿ� ���� �� ����
    /// </summary>
    private void Set_Value()
    {
        Content.GetComponent<RectTransform>().sizeDelta = new Vector2(Content_Size * 2, 0);                                 // rect.size ����
        tabSlider_Image.GetComponent<RectTransform>().sizeDelta = new Vector2(Content_Size / 2, 0);

        PageValues = new float[Bottom_Buttons.Length];                                                                      // ��ũ�� �Ǵ� �� ������ value ����

        PageDistance = 1f / (PageValues.Length - 1f);                                                                       // ��ũ�� �Ǵ� ������ ���� �Ÿ�

        for (int i = 0; i < PageValues.Length; ++i)                                                                         // ��ũ�� �Ǵ� �������� �� value ��ġ ���� [0 <= value <= 1]
        {
            PageValues[i] = PageDistance * i;
        }

        MaxPage = transform.childCount;
    }
    /// <summary>
    /// ScrollBarValue�� ���� ������ ����
    /// </summary>
    /// <param name="index"></param>
    public void SetScrollBarValue(int index)
    {
        currentPage = index;
        scrollbar.value = PageValues[index];
    }
    private void OnEnable()
    {
        Main_First_On = true;
    }
    private void Update()
    {
        Tab_Slider_Pos();

        if (!Main_First_On)
            if (!isSwipe)
                scrollbar.value = Mathf.Lerp(scrollbar.value, TargetPos, swipeTime);                                                  // ��ġ ��ũ�� ���� �� SetPos�� ���� Page �̵�
    }
    /// <summary>
    /// �� ������ �� ������ ���� Scroll ������ ���� Gird Size ����
    /// </summary>
    private void Update_Grid()
    {
        Grid_OnValue();
        Set_Value();
    }
    /// <summary>
    /// ��ư���� Page �̵� �Լ�
    /// </summary>
    /// <param name="n"></param>
    public void TabClick(int n)
    {
        Main_First_On = false;
        currentPage = n;
        TargetPos = PageValues[n];
        Off_Panels();
    }
    /// <summary>
    ///  ���� ���õ� Page ������ ���� Slider
    /// </summary>
    private void Tab_Slider_Pos()
    {
        tabSlider.value = scrollbar.value;

        tabSlider_Width = -(Content_Size / 2) * scrollbar.value;
        tabSlider_Image.GetComponent<RectTransform>().anchoredPosition = new Vector2(tabSlider_Width, 0);

        Emphasis_Button();
    }
    /// <summary>
    /// ���� �����ִ� Page ������ ���� ��ư �� Contents(�̹���, Text) ������ ����
    /// </summary>
    private void Emphasis_Button()
    {
        for (int index = 0; index < Bottom_Buttons.Length; index++)
        {
            if (currentPage == index)
            {
                Bottom_Buttons[index].GetComponent<LayoutElement>().flexibleWidth = 6;
                ImageContents[index].GetComponent<RectTransform>().offsetMax = new Vector2(0, 100);
                ImageContents[index].GetComponent<RectTransform>().offsetMin = new Vector2(0, 70);
            }
            else
            {
                Bottom_Buttons[index].GetComponent<LayoutElement>().flexibleWidth = 1;
                ImageContents[index].GetComponent<RectTransform>().offsetMax = new Vector2(0, 50);
                ImageContents[index].GetComponent<RectTransform>().offsetMin = new Vector2(0, 50);
            }
        }
    }
    /// <summary>
    /// ScrollBar�� �����̵� ���� ��ư ��ġ(Ŭ��)�� ���ϴ� �������� �̵��ϱ� ���� Page ������ �ޱ�
    /// </summary>
    private void Get_Buttons_Data()
    {
        ImageContents=new Image[Bottom_Buttons.Length];

        for (int index = 0; index < Bottom_Buttons.Length; index++)
        {
            ImageContents[index] = Bottom_Buttons[index].GetComponentInChildren<Image>();
        }
    }
}