using UnityEngine;

public class BindPairing : MonoBehaviour
{
    [SerializeField]
    public Action action;                                              // 현재 키(Action)
    [SerializeField]                                                    
    public KeyCode keycode;                                            // 키에 매치된 값(KeyCode)
}
