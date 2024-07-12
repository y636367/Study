using System.Collections;
using UnityEngine;

public class StateBehaviour : StateMachineBehaviour
{
    [SerializeField]
    private int _numberOfBoardAnimations;

    [Header("TimeUntil")]                                                                               // 다음 애니메이션 까지의 제한된 Time 값
    [SerializeField]
    private float MaxTIme;                                                                              // 최대 대기 시간
    [SerializeField]
    private float MinTime;                                                                              // 최소 대기 시간
    [SerializeField]
    private float timeUntilBored;                                                                       // 최종 대기 시간

    private int boredAnimation;
    private float _idleTime;
    private bool _isBored;
    private bool isChanged;

    /// <summary>
    /// 처음 이 애니메이션 상태에 돌입하였을때
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ResetIdle();                                                                                                            // 특정 애니메이션으로 전환(캐릭터가 화면 너머 플레이어를 바라보도록)
        SettingTimeUntilBored();                                                                                                // 다음 Idle 애니메이션 까지 기다려야할 시간 선정
    }
    /// <summary>
    /// Idle 애니메이션 선정 및 재생
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!isChanged)                                                                                                         // 처음 회전 애니메이션 재생 완료 전 일경우
        {
            if (stateInfo.normalizedTime % 1 > 0.98f)                                                                           // 거의 재생이 완료된 후
            {
                isChanged = true;
                boredAnimation = 1;                                                                                             // 대기 모션 진행을 위한 변수 값 설정
            }
        }
        else
        {
            if (!_isBored)                                                                                                      // 현재 대기 모션이라면
            {
                _idleTime += Time.deltaTime;

                if (_idleTime > timeUntilBored && stateInfo.normalizedTime % 1 < 0.02f)                                         // 대기시간에 도달하였고 애니메이션 재생 빈도가 0.02f(약 첫부분)일시
                {
                    _isBored = true;                                                                                            
                    boredAnimation = Random.Range(1, _numberOfBoardAnimations + 1);                                             // 에디터에서 삽입된 애니메이션 가지수 기준 설정
                    boredAnimation *= 2;                                                                                        // BlendTree Type 1D로 중간중간 대기 모션을 삽입하였기에 선정된 값 *2

                    animator.SetFloat("Idle", boredAnimation - 1);                                                              // 애니메이션의 자연스러운 전환을 위해 Idle애니메이션 바로 전 대기모션에서 출발하기 위해 Float값 변경
                }
            }
            else
            {
                if (stateInfo.normalizedTime % 1 > 0.98f)                                                                       // Idle 애니메이션이 거의 끝이 났다면
                {
                    ResetIdle();                                                                                                // 초기화
                }
            }

            animator.SetFloat("Idle", boredAnimation, 0.2f, Time.deltaTime);                                                    // 애니메이션 전환
        }
    }
    /// <summary>
    /// Idle 애니메이션 종료 후 대기 애니메이션으로 전환 및 각종 변수 초기화
    /// </summary>
    /// <param name="aniamtionNumber"></param>
    private void ResetIdle()
    {
        if (_isBored)                                                                                                       // 애니메이션 재생 후 대기 모션을 위한 값 변경
        {
            boredAnimation--;
        }

        _isBored = false;
        _idleTime = 0;
        SettingTimeUntilBored();
    }
    /// <summary>
    /// 최소 대기 시간, 최대 대기 시간을 기준으로 랜덤한 값을 최종 대기시간으로 설정
    /// </summary>
    private void SettingTimeUntilBored()
    {
        timeUntilBored = Random.Range(MinTime, MaxTIme);
    }
    /// <summary>
    /// 상태 전환(Exit 하여 더 이상 Idle 이 아니게 될시 관련 변수 및 값 초기화
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
