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
    /// �ǽð� Player�� ��� �ִ� Tile ������ ���� Update�� ����
    /// </summary>
    private void LateUpdate()
    {
        Positioning_tile();
    }
    /// <summary>
    /// Player�� �ִ� Tile�� �߽����� Player�� �ٸ� Ÿ�Ϸ� �̵��ϰԵǸ� Ÿ���� ���� ����, ����� Ÿ���� ������ ������� ������ Ÿ�� �� Ÿ�� ���� ���� ���ġ
    /// </summary>
    private void Positioning_tile()
    {
        if (tile_manager.Change)                                                                                                                    // ��ȯ Ȯ��
            if (tile_manager.Max_Distance < Vector3.Distance(transform.position, tile_manager.Center.transform.position))                           // �߽ɵ� Ÿ���� �������� �ִ� �Ÿ����� �־����ٸ�
            {
                if(GetComponentInChildren<Slime>() != null)                                                                                         // �̵� �� Ÿ�� �� ��� ���� ���� �̵� �ʿ�
                {
                    slimes = new List<Slime>(GetComponentsInChildren<Slime>());                                                                     // ���͵��� ��� ���� List �Ҵ�

                    foreach(Slime slime in slimes)
                    {
                        slime.run = false;                                                                                                          // Slime�� Nav false
                        slime.agent.enabled = false;
                    }
                    inMonster = true;                                                                                                               // Tile���� ���� ����
                }

                if(GetComponentInChildren<Monster>() != null)                                                                                       // �̵� �� Ÿ�� �� ��� Boss �� ���� �̵� �ʿ�
                {
                    monsters = new List<Monster>(GetComponentsInChildren<Monster>());                                                               // Boss���� ��� ���� List �Ҵ�

                    foreach(Monster monster in monsters)
                    {
                        monster.run = false;
                        monster.agent.enabled = false;                                                                                              // Slime�� Nav false
                    }
                    inMonster = true;                                                                                                               // Tile���� ���� ����
                }

                switch (tile_manager.direction_number)                                                                                              // �߽ɵ� Ÿ���� ������� �Ͽ� �������� ����
                {                                                                                                                                   // ���� Ÿ�� �̵�
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
                tile_manager.Change_count += 1;                                                                                                     // �̵��ϰ� �Ǹ� Ÿ�� ī��Ʈ ++
                tile_manager.Tile_Surface_Resetting();                                                                                              // TIle Resetting
            }
    }
    /// <summary>
    /// Ÿ�� �̵� �Ϸ� �� Ÿ�� ���� ��� ����(���� ����) �ٽ� Navagent
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
    /// Tile ���� ����, Player ����
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionStay(Collision collision)
    {
        if (PD_Control.Instance.StageManager_.Stage_num == 3)
            Value = 1;
        else if (PD_Control.Instance.StageManager_.Stage_num == 4)
        {
            Value = 2;

            Get_Wall wall = GetComponentInChildren<Get_Wall>();                                                                     // Stage5�� �� �гε鿡 �پ��ִ� Turtle Point �ޱ�

            GameManager.Instance.walls = wall;                                                                                      // Wall �� ����
        }

        if (collision.gameObject.CompareTag("Enemy"))                                                                               // ���� TIle���� �ִ� ���� �ν��� ���� �ö���ִ� ��� ����(���� ����) �ڽ����� ����
        {
            collision.gameObject.transform.parent = this.transform;                                                                 // TIle �̵��� �Բ� ���� �Բ� �̵� ����
        }

        if (tile_manager.Change)                                                                                                    // ���� �̵����̶��
            return;

        if (collision.gameObject.CompareTag("Player"))                                                                              // Player �� ���� ��� �ִٸ� ��� �ִ� Tile�� Center�� ����
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
