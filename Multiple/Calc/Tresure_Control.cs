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
    /// ���������� Tresure���� ���� Ư�� Player�� ����� �� �ֵ��� ���
    /// </summary>
    public void TresureOn()
    {
        this.control.Tresure_Item();
    }
}
