using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StageManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] stageArray = null;
    //스테이지 객체

    GameObject currentStage;
    //현재 스테이지

    Transform[] stagePlates;
    //플레이트 담을 변수

    [SerializeField]
    float offsetY = 3f;
    [SerializeField]
    float plateSpeed = 10f;
    //플레이트 애니메이션 연출을 위한 변수
    
    int stepCount = 0;
    //이동한 걸음 수 확인 변수
    int totalPlateCount = 0;
    //총 플레이트 수
    
    public void RemoveStage()
    {
        if(currentStage!= null)
        {
            Destroy(currentStage);
        }
    }
    public void SettingStage(int p_songNum)
    {
        stepCount = 0;
        //새로 생긴 스테이지를 위한 stepcount 초기화

        currentStage = Instantiate(stageArray[p_songNum],Vector3.zero,Quaternion.identity);
        //스테이지 생성

        stagePlates = currentStage.GetComponent<Stage>().plates;
        //스테이지 객체 안에 들어있는 플레이트들을 담음
        totalPlateCount= stagePlates.Length;

        for(int i = 0; i < totalPlateCount; i++)
        {
            stagePlates[i].position = new Vector3(stagePlates[i].position.x,
                                                  stagePlates[i].position.y+offsetY,
                                                  stagePlates[i].position.z);
        }
        //시작과 동시에 전부 위치를 내리고
    }
    public void ShowNextPlate()
    {
        if (stepCount < totalPlateCount)
        {
            StartCoroutine(MovePlateCo(stepCount++));
        }
    }
    IEnumerator MovePlateCo(int p_num)
    {
        stagePlates[p_num].gameObject.SetActive(true);
        Vector3 t_destination=new Vector3(stagePlates[p_num].position.x,
                                          stagePlates[p_num].position.y - offsetY,
                                          stagePlates[p_num].position.z);

        while (Vector3.SqrMagnitude(stagePlates[p_num].position - t_destination) >= 0.001f)
            //현재 플레이트 위치가 목적지(원위치 시킬 플레이트 위치)가까워 질때까지
        {
            stagePlates[p_num].position = Vector3.Lerp(stagePlates[p_num].position, t_destination, plateSpeed * Time.deltaTime);
            yield return null;
        }

        stagePlates[p_num].position = t_destination;
    }
}
