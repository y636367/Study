using UnityEngine;
using UnityEngine.UI;

public class LayOut_Rebuild : MonoBehaviour
{
    /// <summary>
    /// Horiznotal or Vertical LayoutGroup 컴포넌트 존재시 재구성
    /// </summary>
    /// <returns></returns>
    public bool ReBuild()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());

        return true;
    }
}
