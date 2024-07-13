using UnityEngine;

public class Camera_Fov : MonoBehaviour
{
    private void Awake()
    {
        AdjustFOVForDevice();
    }
    /// <summary>
    /// ���� ȭ���� Lobby, Login ���� ��ó�� 2D �� �ƴ� 3D �̱⿡ �� ���� ���� �÷����� ���� ī�޶� �þ߰� ����
    /// </summary>
    private void AdjustFOVForDevice()
    {
        float targetAspect = 9f / 16f;                                                                                                      // Ÿ�� ���� ����( ���� 900:1600)
        float currentAspect = (float)Screen.height / Screen.width;                                                                          //���� ��Ⱦ��

        if (currentAspect > targetAspect)                                                                                                   // ��Ⱦ�� ������ ��Ⱦ�񺸴� ũ�ٸ�
        {
            float targetFOV = CalculateFOV(currentAspect, targetAspect, Camera.main.fieldOfView);
            Camera.main.fieldOfView = targetFOV - 50f;
        }
    }
    /// <summary>
    /// ������ �þ߰��� �������� ���ο� fov �� ���
    /// </summary>
    /// <param name="currentAspect"></param>
    /// <param name="targetAspect"></param>
    /// <param name="baseFOV"></param>
    /// <returns></returns>
    private float CalculateFOV(float currentAspect, float targetAspect, float baseFOV)
    {
        float fov = Mathf.Rad2Deg * (2f * Mathf.Atan(Mathf.Tan(Mathf.Deg2Rad * baseFOV / 2f) * (currentAspect / targetAspect)));
        /// <summary>
        /// Mathf.Deg2Rad * baseFOV / 2f �⺻ FOV ���� �������� ��ȯ�ϰ�, 2�� ������ ���� ������ ��´�
        /// Mathf.Tan(...) * (currentAspect / targetAspect): �� ������ ź��Ʈ�� ����ϰ�, ���� ȭ�� ������ ��ǥ ȭ�� ������ ����Ͽ� ����
        /// 2f * Mathf.Atan(...) : �ٽ� ������ ��ȯ, �̸� 2��� ����� �ݿ� ( �����̸� 2 ����, �����̱⿡ 2 ���ؾ���)
        /// 
        /// google Fov ��� ���� ChatGPT ����
        /// </summary> 
        return fov;
    }
}
// Rad2Deg : ������ ������ ��ȯ���ִ� ���
