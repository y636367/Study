using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class Layout_Off : MonoBehaviour
{
    [SerializeField]
    private LayOut_Rebuild[] layoutG;

    private void Start()
    {
        StartCoroutine(nameof(ReBuild_V));
    }
    /// <summary>
    /// 등록된 오브젝트 layout 재구성 후 컴포넌트 off
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReBuild_V()
    {
        yield return null;

        for(int index = 0; index < layoutG.Length; index++)
        {
            if (layoutG[index].ReBuild())
            {
                try
                {
                    layoutG[index].GetComponent<VerticalLayoutGroup>().enabled = false;
                }
                catch
                {
                    layoutG[index].GetComponent<HorizontalLayoutGroup>().enabled = false;
                }
            }
        }
    }
}
