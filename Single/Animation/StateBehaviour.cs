using System.Collections;
using UnityEngine;

public class StateBehaviour : StateMachineBehaviour
{
    [SerializeField]
    private int _numberOfBoardAnimations;

    [Header("TimeUntil")]                                                                               // ���� �ִϸ��̼� ������ ���ѵ� Time ��
    [SerializeField]
    private float MaxTIme;                                                                              // �ִ� ��� �ð�
    [SerializeField]
    private float MinTime;                                                                              // �ּ� ��� �ð�
    [SerializeField]
    private float timeUntilBored;                                                                       // ���� ��� �ð�

    private int boredAnimation;
    private float _idleTime;
    private bool _isBored;
    private bool isChanged;

    /// <summary>
    /// ó�� �� �ִϸ��̼� ���¿� �����Ͽ�����
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ResetIdle();                                                                                                            // Ư�� �ִϸ��̼����� ��ȯ(ĳ���Ͱ� ȭ�� �ʸ� �÷��̾ �ٶ󺸵���)
        SettingTimeUntilBored();                                                                                                // ���� Idle �ִϸ��̼� ���� ��ٷ����� �ð� ����
    }
    /// <summary>
    /// Idle �ִϸ��̼� ���� �� ���
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!isChanged)                                                                                                         // ó�� ȸ�� �ִϸ��̼� ��� �Ϸ� �� �ϰ��
        {
            if (stateInfo.normalizedTime % 1 > 0.98f)                                                                           // ���� ����� �Ϸ�� ��
            {
                isChanged = true;
                boredAnimation = 1;                                                                                             // ��� ��� ������ ���� ���� �� ����
            }
        }
        else
        {
            if (!_isBored)                                                                                                      // ���� ��� ����̶��
            {
                _idleTime += Time.deltaTime;

                if (_idleTime > timeUntilBored && stateInfo.normalizedTime % 1 < 0.02f)                                         // ���ð��� �����Ͽ��� �ִϸ��̼� ��� �󵵰� 0.02f(�� ù�κ�)�Ͻ�
                {
                    _isBored = true;                                                                                            
                    boredAnimation = Random.Range(1, _numberOfBoardAnimations + 1);                                             // �����Ϳ��� ���Ե� �ִϸ��̼� ������ ���� ����
                    boredAnimation *= 2;                                                                                        // BlendTree Type 1D�� �߰��߰� ��� ����� �����Ͽ��⿡ ������ �� *2

                    animator.SetFloat("Idle", boredAnimation - 1);                                                              // �ִϸ��̼��� �ڿ������� ��ȯ�� ���� Idle�ִϸ��̼� �ٷ� �� ����ǿ��� ����ϱ� ���� Float�� ����
                }
            }
            else
            {
                if (stateInfo.normalizedTime % 1 > 0.98f)                                                                       // Idle �ִϸ��̼��� ���� ���� ���ٸ�
                {
                    ResetIdle();                                                                                                // �ʱ�ȭ
                }
            }

            animator.SetFloat("Idle", boredAnimation, 0.2f, Time.deltaTime);                                                    // �ִϸ��̼� ��ȯ
        }
    }
    /// <summary>
    /// Idle �ִϸ��̼� ���� �� ��� �ִϸ��̼����� ��ȯ �� ���� ���� �ʱ�ȭ
    /// </summary>
    /// <param name="aniamtionNumber"></param>
    private void ResetIdle()
    {
        if (_isBored)                                                                                                       // �ִϸ��̼� ��� �� ��� ����� ���� �� ����
        {
            boredAnimation--;
        }

        _isBored = false;
        _idleTime = 0;
        SettingTimeUntilBored();
    }
    /// <summary>
    /// �ּ� ��� �ð�, �ִ� ��� �ð��� �������� ������ ���� ���� ���ð����� ����
    /// </summary>
    private void SettingTimeUntilBored()
    {
        timeUntilBored = Random.Range(MinTime, MaxTIme);
    }
    /// <summary>
    /// ���� ��ȯ(Exit �Ͽ� �� �̻� Idle �� �ƴϰ� �ɽ� ���� ���� �� �� �ʱ�ȭ
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        isChanged = false;
        _isBored = false;
        _idleTime = 0;
        boredAnimation = 0;
    }
}
