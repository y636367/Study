using UnityEngine;

public class Camera_moving : MonoBehaviour
{
    [Header("플레이어")]
    [SerializeField]
    private Transform Player;

    [Header("조절값")]
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
    /// 카메라가 지속적으로 Player 따라감
    /// </summary>
    private void FixedUpdate()
    {
        if (!GameManager.Instance.Start_On)                                                                                                             // 게임 플레이 중에만
        {
            Vector3 playerPos = new Vector3(Player.position.x + offset.x, Player.position.y + offset.y, Player.position.z + offset.z);                  // 플레이어 기준 거리 offset 값 구해서 일정 위치 지정

            transform.position = Vector3.Lerp(transform.position, playerPos, smoothing);                                                                // 부드럽게 움직이게 하기 위해 Lerp 적용
        }
    }
    /// <summary>
    /// 인 게임 첫 시작 시 카메라 무빙 애니메이션
    /// </summary>
    private void UI_On()
    {
        GameManager.Instance.uimanager.UI_On();
    }
    /// <summary>
    /// 본 게임 시작을 위한 Start 조절 변수 값 변경 및 첫 애니메이션 off
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
