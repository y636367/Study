using System.Collections;
using UnityEngine;

public class AnimationPause : MonoBehaviour
{
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        StartCoroutine(nameof(Manager));
    }
    /// <summary>
    /// 애니메이터 속도 조절로 일시정지 및 재생 구현
    /// </summary>
    /// <returns></returns>
    private IEnumerator Manager()
    {
        while (true)
        {
            if(GameManager.instance.isPause)
                animator.speed = 0;
            else
                animator.speed = 1;

            yield return null;
        }
    }
}
