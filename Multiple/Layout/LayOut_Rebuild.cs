using UnityEngine;
using UnityEngine.UI;

public class LayOut_Rebuild : MonoBehaviour
{
    /// <summary>
    /// Horiznotal or Vertical LayoutGroup ������Ʈ ����� �籸��
    /// </summary>
    /// <returns></returns>
    public bool ReBuild()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());

        return true;
    }
}
