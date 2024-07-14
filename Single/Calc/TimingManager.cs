using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class TimingManager : MonoBehaviour
{
    public List<GameObject> boxNoteList= new List<GameObject>();
    //생성된 노트를 담는 List => 판정범위에 있는지 모든 노트 비교

    int[] judgementRecord=new int[5];
    //판정 카운트 배열

    [SerializeField]
    Transform Center = null;
    //판정 범위 중심 변수
    [SerializeField]
    RectTransform[] timingRect = null;
    //판정 범위(위치에 따른 점수 분배)

    EffectManager Effect;
    ScoreManager ScoreManager;
    ComboManager ComboManager;
    StageManager StageManager;
    PlayerController PlayerController;
    StatusManager StatusManager;
    AudioManager audioManager;

    Vector2[] timingBoxs = null;
    //실제 판정범위에 배열(RectTransform 값을 넣어줌) 최소 값, 최대 값
    void Start()
    {
        audioManager = AudioManager.instance;

        Effect = FindObjectOfType<EffectManager>();
        ScoreManager= FindObjectOfType<ScoreManager>();
        ComboManager= FindObjectOfType<ComboManager>();
        StageManager= FindObjectOfType<StageManager>();
        PlayerController = FindObjectOfType<PlayerController>();
        StatusManager = FindObjectOfType<StatusManager>();

        timingBoxs= new Vector2[timingRect.Length];
        // 0 = Perfect, 1 = Cool, 2 = Good, 3 = Bad
        for(int i = 0; i < timingRect.Length; i++)
            //각 판정 범위 넣어주기
        {
            timingBoxs[i].Set(Center.localPosition.x - timingRect[i].rect.width / 2,
                Center.localPosition.x + timingRect[i].rect.width / 2);
                //최소값 = 중심 - (이미지의 너비/2)최대값 = 중심 + (이미지의 너비/2)
        }
    }

    public bool CheckTiming()
    {
        for(int i=0;i<boxNoteList.Count;i++)
            //생성된 노트 개수 만큼 반복
        {
            float t_notePosX = boxNoteList[i].transform.localPosition.x;
            //노트의 x값을 통해서 판정범위 여부 파악(범위 최소<=노트X<=범위 최대)
            for(int x=0;x<timingRect.Length;x++)
                //판정 범위도 배열이기에 범위만큼 반복해서 어느 구역에 해당되는지 확인
            {
                if (timingBoxs[x].x <= t_notePosX && t_notePosX <= timingBoxs[x].y)
                    //Perfect~Bad = 0~3 순서로 하여 해당될시 그 구역 범위 호출
                {
                    boxNoteList[i].GetComponent<Note>().HideNote();
                    //노트의 컴포넌트 값을 가져와 이미지 비활성화 함수로 이미지 가림
                    boxNoteList.RemoveAt(i);
                    //판정범위에 맞춘 노트 리스트에서 제거

                    if (x < timingBoxs.Length - 1)
                        //x가 Bad범위를 제외한 곳에서 Hit 발생시 이펙트 발생
                    {
                        Effect.NoteHitEffect();
                        //노트 이펙트
                    }
                    
                    if (CheckCanNextPlate())
                    {
                        ScoreManager.InscreaseScore(x);
                        //점수 증가
                        StageManager.ShowNextPlate();
                        //다음 발판 호출(옳게 된 판정이었을시만 뛰우기)
                        Effect.judgementEffect(x);
                        //판정 범위 연출
                        judgementRecord[x]++;
                        //판정 기록 
                        StatusManager.CheckShield();
                        //쉴드 체크
                    }
                    else
                    {
                        Effect.judgementEffect(5);
                        //이미 밟은 플레이트의 이펙트
                    }
                    audioManager.PlaySfx("Clap");
                    return true;
                }
            }
        }
        ComboManager.ResetCombo();
        Effect.judgementEffect(timingBoxs.Length);
        MissRecord();
        return false;
        //판정 범위 바깥일 경우(Miss)
    }
    bool CheckCanNextPlate()
        //여기선 플레이어의 다음 목적지에서 광선을 밑으로 쏴서 맞은 대상이 있다면 그 정보를 가져옴
        //즉, 올바른 방향으로 이동했을 때 바닥이 올라오게끔 판정하기 위한 함수
    {
        if (Physics.Raycast(PlayerController.destination, Vector3.down, out RaycastHit t_hitInfo, 1.1f))
            //Physics.Raycast 가상의 광선을 솨서 맞은 대상의 정보 가져옴(광선위치, 방향, 충돌정보, 길이)필요
        {
            if (t_hitInfo.transform.CompareTag("BasicPlate"))
            {
                BasicPlate t_plate = t_hitInfo.transform.GetComponent<BasicPlate>();
                if (t_plate.flag)
                {
                    t_plate.flag = false;
                    //처음 밟은 발판에서만 방향이 옳다면 새로운 발판 등장위함
                    return true;
                }
            }
        }
        return false;
    }

    public int[] GetJudgementRecord()
    {
        return judgementRecord;
    }

    public void MissRecord()
    {
        judgementRecord[4]++;
        StatusManager.ResetShieldCombo();
    }

    public void Initialized()
    {
        for(int i=0;i<judgementRecord.Length;i++)
        {
            judgementRecord[i] = 0;
        }
    }
}
