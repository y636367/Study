using UnityEngine;

public class HP_Bar : MonoBehaviour
{
    RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
    /// <summary>
    /// 월드 상의 오브젝트 위치를 스크린 좌표로 전환, Player를 따라다니는 HPBar
    /// </summary>
    private void FixedUpdate()
    {
        rect.position = Camera.main.WorldToScreenPoint(GameManager.Instance.player.transform.position);
    }
}
