using UnityEngine;

public class Camera_moving : MonoBehaviour
{
    [Header("�÷��̾�")]
    [SerializeField]
    private Transform Player;

    [Header("������")]
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private float smoothing;

    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    /// <summary>
    /// ī�޶� ���������� Player ����
    /// </summary>
    private void FixedUpdate()
    {
        if (!GameManager.Instance.Start_On)                                                                                                             // ���� �÷��� �߿���
        {
            Vector3 playerPos = new Vector3(Player.position.x + offset.x, Player.position.y + offset.y, Player.position.z + offset.z);                  // �÷��̾� ���� �Ÿ� offset �� ���ؼ� ���� ��ġ ����

            transform.position = Vector3.Lerp(transform.position, playerPos, smoothing);                                                                // �ε巴�� �����̰� �ϱ� ���� Lerp ����
        }
    }
    /// <summary>
    /// �� ���� ù ���� �� ī�޶� ���� �ִϸ��̼�
    /// </summary>
    private void UI_On()
    {
        GameManager.Instance.uimanager.UI_On();
    }
    /// <summary>
    /// �� ���� ������ ���� Start ���� ���� �� ���� �� ù �ִϸ��̼� off
    /// </summary>
    private void Game_Start()
    {
        GameManager.Instance.Start_On = false;
        animator.enabled = false;
    }
    private void Setting()
    {
        GameManager.Instance.Setting_();
        UIManager.instance.Setting();
    }
}
