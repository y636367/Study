using UnityEngine;

public class Camera_Fov : MonoBehaviour
{
    private void Awake()
    {
        AdjustFOVForDevice();
    }
    /// <summary>
    /// 게임 화면은 Lobby, Login 등의 씬처럼 2D 가 아닌 3D 이기에 현 실행 중인 플랫폼에 맞춰 카메라 시야각 조절
    /// </summary>
    private void AdjustFOVForDevice()
    {
        float targetAspect = 9f / 16f;                                                                                                      // 타겟 측면 비율( 현재 900:1600)
        float currentAspect = (float)Screen.height / Screen.width;                                                                          //현재 종횡비

        if (currentAspect > targetAspect)                                                                                                   // 종횡비가 설정한 종횡비보다 크다면
        {
            float targetFOV = CalculateFOV(currentAspect, targetAspect, Camera.main.fieldOfView);
            Camera.main.fieldOfView = targetFOV - 50f;
        }
    }
    /// <summary>
    /// 현재의 시야각을 기준으로 새로운 fov 값 계산
    /// </summary>
    /// <param name="currentAspect"></param>
    /// <param name="targetAspect"></param>
    /// <param name="baseFOV"></param>
    /// <returns></returns>
    private float CalculateFOV(float currentAspect, float targetAspect, float baseFOV)
    {
        float fov = Mathf.Rad2Deg * (2f * Mathf.Atan(Mathf.Tan(Mathf.Deg2Rad * baseFOV / 2f) * (currentAspect / targetAspect)));
        /// <summary>
        /// Mathf.Deg2Rad * baseFOV / 2f 기본 FOV 값을 라디안으로 변환하고, 2로 나눠서 기준 각도를 얻는다
        /// Mathf.Tan(...) * (currentAspect / targetAspect): 이 각도의 탄젠트를 계산하고, 현재 화면 비율과 목표 화면 비율을 고려하여 조정
        /// 2f * Mathf.Atan(...) : 다시 각도로 변환, 이를 2배로 만들어 반영 ( 수평이면 2 제외, 수직이기에 2 곱해야함)
        /// 
        /// google Fov 계산 기준 ChatGPT 참고
        /// </summary> 
        return fov;
    }
}
// Rad2Deg : 라디안을 각도로 변환해주는 상수
