using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Scoll_rect : ScrollRect                                                                    // �����ü�� ������ ���⿡ ScrollRect ���
{
    bool forParent;
    Swipe_UI SW_Manager;
    ScrollRect Parent_scroll;                                                                           // �θ��� scrollRect�� ����ؾ� �ϱ⿡ ȣ��

    protected override void Start()                                                                     // ������ �ֱ⿡ protected�� ����
    {
        SW_Manager=GameObject.FindWithTag("Swip_P").GetComponent<Swipe_UI>();                           // Ŀ���͸���¡ �Ұ��� �ϱ⿡ FIndWithTag�� ã��
        Parent_scroll= GameObject.FindWithTag("Swip_P").GetComponent<ScrollRect>();
    }
    /// <summary>
    /// �巡�� �̵� ũ�⿡ ���� �θ�����, �ڽ��� �̺�Ʈ���� �����Ͽ� ����(scrollview ���� scrollview)
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnBeginDrag(PointerEventData eventData)                                        // ������ ���� override ���
    {
        forParent = Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y);                        // �巡�� ���� ���� �����̵�(����)�� ũ�� �θ�, �����̵�(����)�� ũ�� �ڽ��� �巡��, ũ��, �� ������ ���̸� ���ϸ� �Ǳ⿡ ���밪���� ��

        if (forParent)                                                                                  // �����̵� ���� ũ�ٸ� �θ��� �̺�Ʈ�� ����
        {
            SW_Manager.OnBeginDrag(eventData);
            Parent_scroll.OnBeginDrag(eventData);
        }
        else                                                                                            // �����̵� ���� ũ�ٸ� �θ��� �޼ҵ带 �����ؼ� �ڽ��� �̺�Ʈ ����
            base.OnBeginDrag(eventData);                                                                // �θ��� �޼��� ȣ��
    }
    /// <summary>
    /// �巡�� ���� �Ǵ� �Լ�
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnEndDrag(PointerEventData eventData)
    {
        if(forParent)                                                                                   // �θ��� �巡�װ� ���� �Ǿ��ٸ�
        {
            SW_Manager.OnEndDrag(eventData);
            Parent_scroll.OnEndDrag(eventData);
        }
        else                                                                                            // �ڽ��� �巡�װ� ���� �Ǿ��ٸ�
            base.OnEndDrag(eventData); 
    }
    /// <summary>
    /// �巡�� ������ �Ǵ��ϴ� �Լ�
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnDrag(PointerEventData eventData)
    { 
        if (forParent)                                                                                  // �θ� �巡�� ���̶��
        {
            SW_Manager.OnDrag(eventData);
            Parent_scroll.OnDrag(eventData);
        }
        else    base.OnDrag(eventData);                                                                 // �ڽ� �巡�� ���̶��
            base.OnDrag(eventData); 
    }
}
//base : �θ��� �޼ҵ带 ȣ�� �ϴ� ��
