using System.Collections.Generic;
using UnityEngine;

public class Ground_Tile : MonoBehaviour
{
    Tile_Manager tile_manager;

    List<Slime> slimes =new List<Slime>();
    List<Monster> monsters =new List<Monster>();

    public bool inMonster;

    int Value;
    private void Awake()
    {
        tile_manager=GetComponentInParent<Tile_Manager>();
    }
    /// <summary>
    /// 실시간 Player가 밟고 있는 Tile 감지를 위해 Update문 갱신
    /// </summary>
    private void LateUpdate()
    {
        Positioning_tile();
    }
    /// <summary>
    /// Player가 있닌 Tile을 중심으로 Player가 다른 타일로 이동하게되면 타일의 기준 변경, 변경된 타일의 기준을 기반으로 나머지 타일 및 타일 위의 몬스터 재배치
    /// </summary>
    private void Positioning_tile()
    {
        if (tile_manager.Change)                                                                                                                    // 변환 확인
            if (tile_manager.Max_Distance < Vector3.Distance(transform.position, tile_manager.Center.transform.position))                           // 중심된 타일을 기준으로 최대 거리보다 멀어졌다면
            {
                if(GetComponentInChildren<Slime>() != null)                                                                                         // 이동 시 타일 위 모든 몬스터 또한 이동 필요
                {
                    slimes = new List<Slime>(GetComponentsInChildren<Slime>());                                                                     // 몬스터들을 담기 위한 List 할당

                    foreach(Slime slime in slimes)
                    {
                        slime.run = false;                                                                                                          // Slime의 Nav false
                        slime.agent.enabled = false;
                    }
                    inMonster = true;                                                                                                               // Tile위에 몬스터 존재
                }

                if(GetComponentInChildren<Monster>() != null)                                                                                       // 이동 시 타일 위 모든 Boss 급 또한 이동 필요
                {
                    monsters = new List<Monster>(GetComponentsInChildren<Monster>());                                                               // Boss들을 담기 위한 List 할당

                    foreach(Monster monster in monsters)
                    {
                        monster.run = false;
                        monster.agent.enabled = false;                                                                                              // Slime의 Nav false
                    }
                    inMonster = true;                                                                                                               // Tile위에 몬스터 존재
                }

                switch (tile_manager.direction_number)                                                                                              // 중심된 타일을 기반으로 하여 동서남북 방향
                {                                                                                                                                   // 현재 타일 이동
                    case 1:
                        transform.position = new Vector3(tile_manager.Center.transform.position.x - tile_manager.Middle_Distance * Value, transform.position.y, transform.position.z);
                        break;
                    case 2:
                        transform.position = new Vector3(tile_manager.Center.transform.position.x + tile_manager.Middle_Distance * Value, transform.position.y, transform.position.z);
                        break;
                    case 3:
                        transform.position = new Vector3(transform.position.x, transform.position.y, tile_manager.Center.transform.position.z - tile_manager.Middle_Distance * Value);
                        break;
                    case 4:
                        transform.position = new Vector3(transform.position.x, transform.position.y, tile_manager.Center.transform.position.z + tile_manager.Middle_Distance * Value);
                        break;
                }
                tile_manager.Change_count += 1;                                                                                                     // 이동하게 되면 타일 카운트 ++
                tile_manager.Tile_Surface_Resetting();                                                                                              // TIle Resetting
            }
    }
    /// <summary>
    /// 타일 이동 완료 시 타일 위의 모든 몬스터(보스 포함) 다시 Navagent
    /// </summary>
    public void Turn_On_agent()
    {
        inMonster = false;

        if (GetComponentInChildren<Slime>() != null)
        {
            foreach (Slime slime in slimes)
            {
                slime.agent.enabled = true;
                slime.run = true;
            }
        }

        if (GetComponentInChildren<Monster>() != null)
        {
            foreach (Monster monster in monsters)
            {
                monster.agent.enabled = true;
                monster.run = true;
            }
        }
    }
    /// <summary>
    /// Tile 위의 몬스터, Player 감지
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionStay(Collision collision)
    {
        if (PD_Control.Instance.StageManager_.Stage_num == 3)
            Value = 1;
        else if (PD_Control.Instance.StageManager_.Stage_num == 4)
        {
            Value = 2;

            Get_Wall wall = GetComponentInChildren<Get_Wall>();                                                                     // Stage5의 각 패널들에 붙어있는 Turtle Point 받기

            GameManager.Instance.walls = wall;                                                                                      // Wall 값 설정
        }

        if (collision.gameObject.CompareTag("Enemy"))                                                                               // 현재 TIle위에 있는 몬스터 인식을 위해 올라와있는 모든 몬스터(보스 포함) 자식으로 설정
        {
            collision.gameObject.transform.parent = this.transform;                                                                 // TIle 이동과 함께 몬스터 함께 이동 편의
        }

        if (tile_manager.Change)                                                                                                    // 만약 이동중이라면
            return;

        if (collision.gameObject.CompareTag("Player"))                                                                              // Player 가 현재 밟고 있다면 밟고 있는 Tile을 Center로 설정
        {
            if (tile_manager.Center != this.gameObject)
            {
                tile_manager.Preview_Center = tile_manager.Center;
                tile_manager.Center = this.gameObject;
                tile_manager.ReSetting_Tile();
            }
        }
    }
}
