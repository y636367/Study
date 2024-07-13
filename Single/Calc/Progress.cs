using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Progress : MonoBehaviour
{
    [SerializeField]
    private Slider Loadding_bar;                                            // ���� �ε� ���� ��Ȳ�� ǥ���� Slider
    [SerializeField]
    private Text Progress_text;                                             // ���� �ε� ���� ��Ȳ�� ǥ���� Text
    [SerializeField]
    private float Loadding_time;                                            // �ε� �׸� ��(�ε� �ð�)

    [Header("Complite_Sound")]
    [SerializeField]
    private string Complite_Sound;                                          // �ε� �ϷḦ �˸��� �Ҹ� �̸�    

    public void Play(UnityAction action=null)                               // ��� �Ϸ�� ���ϴ� �޼ҵ� �����ϱ� ���� Aciton ����)
    {
        StartCoroutine(OnProgress(action));                                 // �ڷ�ƾ ����
    }
    private IEnumerator OnProgress(UnityAction action)
    {
        float percent = 0;                                                  // Slider, Text �� ǥ���� ���� ���� ����
        float current = 0;

        while(percent < 1)
        {
            current += Time.deltaTime;                                      // �ε� �׸���� �Ϸ� �ɶ����� current �� ���� �ϵ��� ���� ��ȯ �ʿ�
            percent = current / Loadding_time;                              // �ε� �ð�(��ü �׸�)���� �ݺ��ǰ� ���� ���� percent�� ����

            Progress_text.text = $"{Loadding_bar.value * 100:F0}%";         // text�� ����� �ۼ�Ʈ ǥ��    
            Loadding_bar.value = Mathf.Lerp(0, 1, percent);                 // ������ �̵�

            yield return null;
        }
        Progress_text.text = "100%";                                        // �Ϸ�� Loading_bar.value�� �Ϻ��ϰ� 100���� ������ �ʱ⿡ �ð��� ǥ���� ���� �ؽ�Ʈ ����
        soundManager.Instance.PlaySoundEffect(Complite_Sound);              // �Ҹ� ���

        action?.Invoke();                                                   // action�� null�� �ƴϸ� action �޼ҵ� ����
    }

    // unityAction : ��ȯ ���� ���� �޼��� ��� ����
}
