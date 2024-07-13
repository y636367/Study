using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine.AI;
using UnityEngine;

public class Second_TileManager : MonoBehaviour
{
    NavMeshSurface nav_s;

    [SerializeField]
    private List<Ground_Tile> Tiles = new List<Ground_Tile>();

    [SerializeField]
    public GameObject Center;
    public GameObject Preview_Center;

    public int direction_number = 0;
    public int Change_count = 0;

    public float Max_Distance = 0f;
    public float Middle_Distance;

    public bool Change = false;

    private void Awake()
    {
        Tiles = new List<Ground_Tile>(GetComponentsInChildren<Ground_Tile>(true));
        nav_s = GetComponent<NavMeshSurface>();

        Middle_Distance = Vector3.Distance(Tiles[0].transform.position, Tiles[1].transform.position);

        for (int i = 0; i < Tiles.Count; i++)
        {
            if (Max_Distance < Vector3.Distance(Tiles[i].transform.position, Center.transform.position))
                Max_Distance = Vector3.Distance(Tiles[i].transform.position, Center.transform.position);
        }

        Preview_Center = Center;
    }
    private void LateUpdate()
    {
        if (Change_count >= 1)
        {
            Change_count = 0;
            ReSet_Nav();
            Change = false;
        }
    }
    private void ReSet_Nav()
    {
        nav_s.RemoveData();
        nav_s.BuildNavMesh();

        foreach (Ground_Tile tile in Tiles)
        {
            if (tile.inMonster)
                tile.Turn_On_agent();
        }
    }
    public void ReSetting_Tile()
    {
        if (Preview_Center.transform.position.x > Center.transform.position.x) // 플레이어가 왼쪽으로 이동
        {
            direction_number = 1;
        }
        else if (Preview_Center.transform.position.x < Center.transform.position.x) // 플레이어가 오른쪽으로 이동
        {
            direction_number = 2;
        }
        else if (Preview_Center.transform.position.z > Center.transform.position.z) // 플레이어가 아래쪽을 이동
        {
            direction_number = 3;
        }
        else if (Preview_Center.transform.position.z < Center.transform.position.z) // 플레이어가 위쪽으로 이동
        {
            direction_number = 4;
        }

        Change = true;
    }
}
