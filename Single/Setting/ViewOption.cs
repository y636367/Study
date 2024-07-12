using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ViewOption : MonoBehaviour
{
    #region Variable
    [SerializeField]
    private Dropdown resolution;
    List<Resolution> resolutions = new List<Resolution>();                                                          // 해상도 목록을 저장할 리스트

    FullScreenMode fullScreenMode;

    [SerializeField]
    public Toggle screenbutton;
    int resolutionNum;

    [SerializeField]
    private Camera Main;

    float setWidth = 0;
    float setHeight = 0;
    #endregion
    /// <summary>
    /// 해상도 옵션을 위한 각 이벤트 할당을 위한 등록
    /// </summary>
    /// <param name="t_resolution"></param>
    /// <param name="t_screenbuttn"></param>
    /// <param name="t_main"></param>
    public void Set_Variable(Dropdown t_resolution, Camera t_main)
    {
        resolution = t_resolution;
        Main = t_main;
    }
    /// <summary>
    /// 해상도 변경을 위한 옵션 함수
    /// </summary>
    public void SetResolution()
    {
        resolution.ClearOptions();                                                                                  // Dropdown 메뉴 옵션 초기화

        foreach(Resolution resolution in Screen.resolutions)
        {
            if (resolution.refreshRate >= 60)                                                                       // 60Hz 이상의 해상도 일시
            {
                if(!resolutions.Any(r=>r.width==resolution.width && r.height == resolution.height))                 // 중복된 해상도가 없다면
                {
                    resolutions.Add(resolution);                                                                    // 추가
                }
                else                                                                                                // 중복된 해상도가 있다면
                {
                    Resolution existinRes = resolutions.Find(r => r.width == resolution.width && r.height == resolution.height);

                    if (resolution.refreshRate >= existinRes.refreshRate)                                           // 주사율을 비교하여
                    {
                        resolutions.Remove(existinRes);                                                             // 기존의 낮은 주사율의 해상도 삭제
                        resolutions.Add(resolution);                                                                // 더 높은 주사율을 가진 새로운 해상도 추가
                    }

                }
            }
        }

        List<string> options = new List<string>();                                                                  // Dropdonw 메뉴에 옵션 추가
        int defaultindex = 0;
        int index = 0;

        foreach (Resolution item in resolutions)
        {
            string optionText = $"{item.width}x{item.height} {item.refreshRate}Hz";                                 // resolutions에 저장된 항목들의 Text 화 하여 options 리스트에 저장
            options.Add(optionText);

            if(item.width == Screen.width && item.height == Screen.height)                                          // 현재 스크린 해상도와 일치하는 옵션을 기본 선택항목으로 설정
            {
                defaultindex = index;
            }

            index++;
        }
        resolution.AddOptions(options);                                                                             // Dropdown에 생성된 Text들 추가

        resolution.value = defaultindex;                                                                            // 기본 선택항목 현재 스크린 해상도로 설정
    }
    /// <summary>
    /// 실행 중인 기기의 종횡비에 따른 카메라 rect 조절
    /// </summary>
    public void Scene_Set_Resolution()
    {
        Screen.SetResolution(resolutions[resolutionNum].width, resolutions[resolutionNum].height, fullScreenMode);                                      // Screen 전체 옵션 적용    

        if ((float)setWidth / setHeight < (float)resolutions[resolutionNum].width / resolutions[resolutionNum].height)                                  // 기기의 해상도 비가 더 큰 경우
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)resolutions[resolutionNum].width / resolutions[resolutionNum].height);             // 새로운 너비
            Main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f);                                                                               // 새로운 Rect 적용
        }
        else                                                                                                                                            // 게임의 해상도 비가 더 큰 경우
        {
            float newHeight = ((float)resolutions[resolutionNum].width / resolutions[resolutionNum].height) / ((float)setWidth / setHeight);            // 새로운 높이
            Main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);                                                                             // 새로운 Rect 적용
        }
    }
    /// <summary>
    /// 드롭메뉴에서 선택된 해상도 인덱스 구분 함수
    /// </summary>
    /// <param name="x"></param>
    public void OptionChange(int x)
    {
        resolutionNum= x;
    }
    /// <summary>
    /// 참이면 전체화면, 거짓이면 창모드
    /// </summary>
    /// <param name="isFull"></param>
    public void FullScreenCheck(bool isFull)
    {
        fullScreenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }
}
