using UnityEngine;

public class Panel_Registration : MonoBehaviour
{
    /// <summary>
    /// 해당 스크립트 활성화 될때마다 Quit의 패널 List에 (해당 스크립트 컴포넌트가 추가되어있는) 현재 오브젝트 추가
    /// </summary>
    private void OnEnable()
    {
        Sounds.instance.WindowOpen_Sound();
        Quit_Game.Instance.Panel_In(this.gameObject);
    }
}
