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
    /// 파티클 일시정지 및 다시 재생
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
    /// 씬에 활성화 되야하는 개수가 제한되있는 경우, 이를 확인하기 위한 변수의 값 변경
    /// </summary>
    private void OnDisable()
    {
        if (GameManager.instance.effectpool.GE != null && CountNum)
            GameManager.instance.effectpool.GE.Count--;
    }
}
