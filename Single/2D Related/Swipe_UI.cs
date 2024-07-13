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
    private float s_Distance;                                                                                                   // swipe 되기 위해 움직이는 최소 거리
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
        SetScrollBarValue(page_num);                                                                            // 최초 시작 페이지 0 : playerUpgrade 1 : Game Ready 2 : Weapon Upgrade
    }
    /// <summary>
    /// 패널 종료 및 Quit List 조정
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
    public void OnDrag(PointerEventData eventData) => isSwipe = true;                                                               // 메서드 드라이브(단일)
    /// <summary>
    /// eventData.Delta(순간 적인 움직임의 값 포착) = 순간 변화율
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        Main_First_On = false;
        CurPos = SetPos();
    }
    /// <summary>
    /// Drag 종료 판단 함수(Drag 길이를 통해 후 이벤트들을 위한 값 계산)
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        // 각 플랫폼에 따른 수치 변경 필요
        isSwipe = false;

        TargetPos = SetPos();                                                                                                       // 다음 Page 번호(0이라면 현재, 아니라면 왼, 오른쪽 페이지)

        if (CurPos == TargetPos)                                                                                                    // 절반 거리를 넘지 않아도 마우스를 빠르게 이동하면
        {
            if (eventData.delta.x > 18 && CurPos - PageDistance >= 0)                                                               // 스크롤이 왼쪽으로 빠르게 이동 시
            {
                --currentPage;
                TargetPos = CurPos - PageDistance;
            }
            else if (eventData.delta.x < -18 && CurPos + PageDistance <= 1.01f)                                                     // 스크롤이 오른쪽으로 빠르게 이동시 (반올림 해서 오차가 발생하기에 1.01)
            {
                ++currentPage;
                TargetPos = CurPos + PageDistance;
            }
        }

        #region 무기 목록창은 수직 스크롤이 존재하기에 다른창에 다녀오게 되면 무조건 초기 값으로 설정 (Only Top)
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
    /// Page 넘길지 판단 여부 함수 (Page 절반 거리를 기준으로 가까운 위치를 반환)
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
    /// 특정 구역 안에서만 동작하게 하기 위해 사이즈 조정(무기 스크롤)
    /// </summary>
    private void Grid_OnValue()
    {
        float weapon_size = this.GetComponent<RectTransform>().rect.height/3;                                                   // rect.size 조정

        Content_Size = this.GetComponent<RectTransform>().rect.width;

        Weapon_List_scrollview.GetComponent<RectTransform>().sizeDelta = new Vector2(0, weapon_size);                           // 무기 목록 스크롤 사이즈
    }
    /// <summary>
    /// Scroll을 위한 각 페이지 Value 및 거리 등의 값 설정
    /// </summary>
    private void Set_Value()
    {
        Content.GetComponent<RectTransform>().sizeDelta = new Vector2(Content_Size * 2, 0);                                 // rect.size 조정
        tabSlider_Image.GetComponent<RectTransform>().sizeDelta = new Vector2(Content_Size / 2, 0);

        PageValues = new float[Bottom_Buttons.Length];                                                                      // 스크롤 되는 각 페이지 value 저장

        PageDistance = 1f / (PageValues.Length - 1f);                                                                       // 스크롤 되는 페이지 사이 거리

        for (int i = 0; i < PageValues.Length; ++i)                                                                         // 스크롤 되는 페이지의 각 value 위치 설정 [0 <= value <= 1]
        {
            PageValues[i] = PageDistance * i;
        }

        MaxPage = transform.childCount;
    }
    /// <summary>
    /// ScrollBarValue에 따른 페이지 선택
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
                scrollbar.value = Mathf.Lerp(scrollbar.value, TargetPos, swipeTime);                                                  // 터치 스크롤 종료 후 SetPos에 의한 Page 이동
    }
    /// <summary>
    /// 각 페이지 및 페이지 내부 Scroll 동작을 위한 Gird Size 조정
    /// </summary>
    private void Update_Grid()
    {
        Grid_OnValue();
        Set_Value();
    }
    /// <summary>
    /// 버튼으로 Page 이동 함수
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
    ///  현재 선택된 Page 강조를 위한 Slider
    /// </summary>
    private void Tab_Slider_Pos()
    {
        tabSlider.value = scrollbar.value;

        tabSlider_Width = -(Content_Size / 2) * scrollbar.value;
        tabSlider_Image.GetComponent<RectTransform>().anchoredPosition = new Vector2(tabSlider_Width, 0);

        Emphasis_Button();
    }
    /// <summary>
    /// 현재 보고있는 Page 강조를 위한 버튼 및 Contents(이미지, Text) 사이즈 조절
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
    /// ScrollBar를 슬라이드 말고 버튼 터치(클릭)시 원하는 페이지로 이동하기 위한 Page 데이터 받기
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