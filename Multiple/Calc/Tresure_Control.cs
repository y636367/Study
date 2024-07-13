using UnityEngine;
using UnityEngine.UI;
public class Tresure_Control : MonoBehaviour
{
    [SerializeField]
    public Item_Control control;

    Image icon;

    public bool Not_Clear;
    public bool Multiple_Check;
    public bool instance = false;
    private void Awake()
    {
        Not_Clear = false;
        Multiple_Check = false;
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = control.data.ItemIcon;
    }
    /// <summary>
    /// 최종적으로 Tresure에서 나온 특성 Player가 사용할 수 있도록 등록
    /// </summary>
    public void TresureOn()
    {
        this.control.Tresure_Item();
    }
}
