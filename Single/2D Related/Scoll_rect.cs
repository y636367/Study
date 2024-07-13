using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Scoll_rect : ScrollRect                                                                    // 기능자체는 온전히 같기에 ScrollRect 상속
{
    bool forParent;
    Swipe_UI SW_Manager;
    ScrollRect Parent_scroll;                                                                           // 부모의 scrollRect도 사용해야 하기에 호출

    protected override void Start()                                                                     // 숨겨져 있기에 protected로 선언
    {
        SW_Manager=GameObject.FindWithTag("Swip_P").GetComponent<Swipe_UI>();                           // 커스터마이징 불가능 하기에 FIndWithTag로 찾기
        Parent_scroll= GameObject.FindWithTag("Swip_P").GetComponent<ScrollRect>();
    }
    /// <summary>
    /// 드래그 이동 크기에 따른 부모인지, 자식의 이벤트인지 구분하여 실행(scrollview 안의 scrollview)
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnBeginDrag(PointerEventData eventData)                                        // 재정의 위함 override 사용
    {
        forParent = Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y);                        // 드래그 시작 순간 수평이동(가로)이 크면 부모가, 수직이동(세로)이 크면 자식이 드래그, 크기, 즉 벡터의 길이만 비교하면 되기에 절대값으로 비교

        if (forParent)                                                                                  // 수평이동 값이 크다면 부모의 이벤트로 실행
        {
            SW_Manager.OnBeginDrag(eventData);
            Parent_scroll.OnBeginDrag(eventData);
        }
        else                                                                                            // 수직이동 값이 크다면 부모의 메소드를 참조해서 자식의 이벤트 실행
            base.OnBeginDrag(eventData);                                                                // 부모의 메서드 호출
    }
    /// <summary>
    /// 드래그 종료 판단 함수
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnEndDrag(PointerEventData eventData)
    {
        if(forParent)                                                                                   // 부모의 드래그가 종료 되었다면
        {
            SW_Manager.OnEndDrag(eventData);
            Parent_scroll.OnEndDrag(eventData);
        }
        else                                                                                            // 자식의 드래그가 종료 되었다면
            base.OnEndDrag(eventData); 
    }
    /// <summary>
    /// 드래그 중임을 판단하는 함수
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnDrag(PointerEventData eventData)
    { 
        if (forParent)                                                                                  // 부모 드래그 중이라면
        {
            SW_Manager.OnDrag(eventData);
            Parent_scroll.OnDrag(eventData);
        }
        else    base.OnDrag(eventData);                                                                 // 자식 드래그 중이라면
            base.OnDrag(eventData); 
    }
}
//base : 부모의 메소드를 호출 하는 것
