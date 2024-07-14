using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public int bpm = 0;
    double currentTime = 0d;
    //float 보다 오차가 적은 double로

    [SerializeField]
    Transform tfNoteCreativeZone = null;
    //Note가 생성될 위치 변수


    TimingManager timingManager;
    EffectManager EffectManager;
    ComboManager ComboManager;

    void Start()
    {
        timingManager = GetComponent<TimingManager>();
        EffectManager= FindObjectOfType<EffectManager>();
        ComboManager= FindObjectOfType<ComboManager>();
    }
    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isStartGame)//클리어 전 상태에는 계속 생성
        {
            currentTime += Time.deltaTime;

            if (currentTime >= 60d / bpm)//초당 bpm 등장 개수
            {
                GameObject t_note = ObjectPool.instance.noteQueue.Dequeue();
                //생성된 인스턴스로 noteQueue에 생성된 객체를 하나 빼와서
                t_note.transform.position = tfNoteCreativeZone.position;
                //위치 정보값을 주고
                t_note.SetActive(true);
                //활성화

                timingManager.boxNoteList.Add(t_note);
                //노트 생성과 동시에 NoteList에 추가
                currentTime -= 60d / bpm;
                //Time.deltaTime 0.51005551...만큼의 오차 발생, 0으로 초기화 X
                //오차 조정을 위해 60d/bpm 사용
            }

        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Note"))
        {
            if (collision.GetComponent<Note>().GetNoteFlag())
            //이미지가 활성화 상태일때 Miss 이미지 호출
            {
                EffectManager.judgementEffect(4);
                ComboManager.ResetCombo();
                timingManager.MissRecord();
            }
            //노트가 적합 존에서 처리되어 이미지가 비활성회 되고 이 객체가 큐에 들어갔다가 다시 재사용될 때
            //이미지가 비활성화 된 상태로 호출이 되니 Note 스크립트 수정 필요
            timingManager.boxNoteList.Remove(collision.gameObject);
            //노트 List에서 삭제
            ObjectPool.instance.noteQueue.Enqueue(collision.gameObject);
            //사용한 노트(지나간 노트) 다시 noteQueue에 반납
            collision.gameObject.SetActive(false);
            //다시 비활성화
        }
    }

    public void RemoveNote()
        //스테이지 클리어시 노트 삭제 함수
    {
        GameManager.Instance.isStartGame= false;

        for(int i = 0; i < timingManager.boxNoteList.Count; i++)
        {
            timingManager.boxNoteList[i].SetActive(false);
            //전부 비활성화
            ObjectPool.instance.noteQueue.Enqueue(timingManager.boxNoteList[i]);
            //전부 반납
        }

        timingManager.boxNoteList.Clear();
        //리스트 초기화
    }
}
