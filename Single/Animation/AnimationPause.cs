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
    /// �ִϸ����� �ӵ� ������ �Ͻ����� �� ��� ����
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
