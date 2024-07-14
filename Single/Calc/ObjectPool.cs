using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ObjectInfo
{
    public GameObject goPrefab;
    //필요한 만큼 생성
    public int Count;
    //개수
    public Transform tfPoolParent;
    //어디에 생성할것인지
}
public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [SerializeField]
    ObjectInfo[] objectInfo = null;

    public Queue<GameObject> noteQueue=new Queue<GameObject>();
    //노트 적립으로 큐가 가장 적합한 자료형
    
    
    void Start()
    {
        instance = this;
        noteQueue = InsertQueue(objectInfo[0]);
        //생성과 파괴가 자주 이뤄지는 노드가 또 있다면 다른 배열을 생성해 다른 큐에 넣어주면 됨
        //noteQueue = insertQueue(objectInfo[1]);, noteQueue = insertQueue(objectInfo[2]);
    }
    Queue<GameObject> InsertQueue(ObjectInfo p_objectInfo)
        //GameObject형 타입을 가지고 있는 큐 를 리턴
    {
        Queue<GameObject> t_queue = new Queue<GameObject>();
        for(int i=0;i<p_objectInfo.Count;i++)
        {
            GameObject t_clone = Instantiate(p_objectInfo.goPrefab, transform.position,Quaternion.identity);
            //노트 생성
            t_clone.SetActive(false);
            if (p_objectInfo.tfPoolParent != null)
                //기존에 만든 Note객체(NoteManager 스트립트가 있는)가 존재할시 그 객체를 부모로
            {
                t_clone.transform.SetParent(p_objectInfo.tfPoolParent);
            }
            else
                t_clone.transform.SetParent(this.transform);
            //그렇지 않다면 이 스크립트가 붙어있는 객체를 부모로 설정

            t_queue.Enqueue(t_clone);
            //생성된 객체 큐에 삽입(Count 개수만큼 들어있음)
        }
        return t_queue;
    }
}
