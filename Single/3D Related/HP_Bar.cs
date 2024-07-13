using UnityEngine;

public class HP_Bar : MonoBehaviour
{
    RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
    /// <summary>
    /// ���� ���� ������Ʈ ��ġ�� ��ũ�� ��ǥ�� ��ȯ, Player�� ����ٴϴ� HPBar
    /// </summary>
    private void FixedUpdate()
    {
        rect.position = Camera.main.WorldToScreenPoint(GameManager.Instance.player.transform.position);
    }
}
