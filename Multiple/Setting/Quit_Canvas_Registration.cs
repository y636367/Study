using UnityEngine;

public class Quit_Canvas_Registration : MonoBehaviour
{
    [SerializeField]
    private Canvas QuitCanvas;                              // QuitCanvas�� Main ī�޶� ����� ���� Canvas ����
    
    /// <summary>
    /// ī�޶� ����
    /// </summary>
    public void Setting_Cammera()
    {
        if (QuitCanvas.worldCamera == null)                 // QuitCanvas�� Null(���� ��) ���� ���� Main ī�޶� worldCamera�� ����
            QuitCanvas.worldCamera = Camera.main;
    }
}
