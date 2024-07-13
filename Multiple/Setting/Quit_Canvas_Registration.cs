using UnityEngine;

public class Quit_Canvas_Registration : MonoBehaviour
{
    [SerializeField]
    private Canvas QuitCanvas;                              // QuitCanvas의 Main 카메라 등록을 위한 Canvas 변수
    
    /// <summary>
    /// 카메라 세팅
    /// </summary>
    public void Setting_Cammera()
    {
        if (QuitCanvas.worldCamera == null)                 // QuitCanvas가 Null(누락 시) 현재 씬의 Main 카메라를 worldCamera로 설정
            QuitCanvas.worldCamera = Camera.main;
    }
}
