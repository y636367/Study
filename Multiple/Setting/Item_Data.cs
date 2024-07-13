using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable object/ItemData")]
public class Item_Data : ScriptableObject                                                                // ��ũ���ͺ� ������Ʈ�̱⿡ �̸� ������ Ŭ���� �ʿ�
{
    public enum ItemType { nomal_weapon, reinforced_weapon,                                              // ����, Ư�� ���� ���� ���� ������
        epic_weapon, Characteristic, epic_Characteristic, 
        one_time_performance, Coin }

    [Header("# Main Info")]
    public ItemType Type;                                                                                // Item Type
    public int ItemId;                                                                                   // ���� ���̵�
    public string ItemName;                                                                              // Desc � ���� ���� �̸�(User���� ��������)
    public Sprite ItemIcon;                                                                              // ���� �̹���
    public bool Weapon;                                                                                  // �������� Ư������ ���� ����
    [TextArea]
    public string ItemDesc;                                                                              // �� ���� string
    [Header("# Level Data")]
    public float baseDamage;                                                                             // �⺻ ������
    public float baseCount;                                                                              // �⺻ ����ü ��
    public float base_Cool_time;                                                                         // �⺻ ��Ÿ��(�Ǵ� ���ӽð�)
    public float[] damages;                                                                              // ������ �Է�(�ԷµǴ� ������ ���� Level ���Ѽ� ����)
    public float[] counts;                                                                               // ����ü �Է�
    public float[] Cool_time;                                                                            // ��Ÿ�� �Է�

    [Header("# Weapon")]
    public int Weapon_num;                                                                               // ���� ������ȣ

    [Header("Reinforced")]
    public bool Check;                                                                                   // ��ȭ�� ���� ���� Ȯ��
    public Item_Data ReinforcedMode;                                                                     // ��ȭ�� üũ
}
