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
    /// ��ϵ� ������Ʈ layout �籸�� �� ������Ʈ off
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
