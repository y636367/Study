using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class EffectPause : MonoBehaviour
{
    private ParticleSystem ps;

    private bool isPause;

    [SerializeField]
    private bool CountNum;
    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }
    private void Start()
    {
        StartCoroutine(nameof(Pause));
    }
    /// <summary>
    /// ��ƼŬ �Ͻ����� �� �ٽ� ���
    /// </summary>
    /// <returns></returns>
    private IEnumerator Pause()
    {
        while(true)
        {
            if (GameManager.instance.isPause)
            {
                if (!ps.isPaused)
                    ps.Pause();
                else
                    yield return null;
            }
            else
            {
                if(ps.isPaused)
                    ps.Play();
                else
                    yield return null;
            }
        }
    }
    /// <summary>
    /// ���� Ȱ��ȭ �Ǿ��ϴ� ������ ���ѵ��ִ� ���, �̸� Ȯ���ϱ� ���� ������ �� ����
    /// </summary>
    private void OnDisable()
    {
        if (GameManager.instance.effectpool.GE != null && CountNum)
            GameManager.instance.effectpool.GE.Count--;
    }
}
