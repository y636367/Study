using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextGradation : MonoBehaviour
{
    Text text;

    bool R;
    bool G;
    bool B;

    [SerializeField]
    private bool first_Change;
    [SerializeField]
    private float speed;
    private void Awake()
    {
        text = GetComponent<Text>();
    }
    public void StartGradient()
    {
        StartCoroutine(nameof(ChangedColor_R));
    }
    /// <summary>
    /// 색상 변경 R
    /// </summary>
    /// <returns></returns>
    IEnumerator ChangedColor_R()
    {
        if (!R)
        {
            while (text.color.r > 0.0f)
            {
                text.color = new Color(text.color.r - (Time.deltaTime / speed), text.color.g, text.color.b);
                yield return null;
            }
            text.color = new Color(0.0f, text.color.g, text.color.b);
            R = true;
        }
        else
        {
            while (text.color.r < 1.0f)
            {
                text.color = new Color(text.color.r + (Time.deltaTime / speed), text.color.g, text.color.b);
                yield return null;
            }
            text.color = new Color(1.0f, text.color.g, text.color.b);
            R = false;
        }

        if (first_Change)
            StartCoroutine(nameof(ChangedColor_G));
        else
        {
            first_Change = true;
            StartCoroutine(nameof(ChangedColor_B));
        }
    }
    /// <summary>
    /// 색상변경 G
    /// </summary>
    /// <returns></returns>
    IEnumerator ChangedColor_G()
    {
        if (!G)
        {
            while (text.color.g > 0.0f)
            {
                text.color = new Color(text.color.r, text.color.g - (Time.deltaTime / speed), text.color.b);
                yield return null;
            }
            text.color = new Color(text.color.r, 0.0f, text.color.b);
            G = true;
        }
        else
        {
            while (text.color.g < 1.0f)
            {
                text.color = new Color(text.color.r, text.color.g + (Time.deltaTime / speed), text.color.b);
                yield return null;
            }
            text.color = new Color(text.color.r, 1.0f, text.color.b);
            G = false;
        }
        StartCoroutine(nameof(ChangedColor_B));
    }
    /// <summary>
    /// 색상변경 B
    /// </summary>
    /// <returns></returns>
    IEnumerator ChangedColor_B()
    {
        if (!B)
        {
            while (text.color.b > 0.0f)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b - (Time.deltaTime / speed));
                yield return null;
            }
            text.color = new Color(text.color.r, text.color.g, 0.0f);
            B = true;
        }
        else
        {
            while (text.color.b < 1.0f)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b + (Time.deltaTime / speed));
                yield return null;
            }
            text.color = new Color(text.color.r, text.color.g, 1.0f);
            B = false;
        }
        StartCoroutine(nameof(ChangedColor_R));
    }
    public void StopAll_C()
    {
        StopAllCoroutines();
    }
}
