using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable object/ItemData")]
public class Item_Data : ScriptableObject                                                                // 스크립터블 오브젝트이기에 이를 제어할 클래스 필요
{
    public enum ItemType { nomal_weapon, reinforced_weapon,                                              // 무기, 특성 등의 종합 정의 열거형
        epic_weapon, Characteristic, epic_Characteristic, 
        one_time_performance, Coin }

    [Header("# Main Info")]
    public ItemType Type;                                                                                // Item Type
    public int ItemId;                                                                                   // 고유 아이디
    public string ItemName;                                                                              // Desc 등에 사용될 고유 이름(User에게 보여지는)
    public Sprite ItemIcon;                                                                              // 고유 이미지
    public bool Weapon;                                                                                  // 무기인지 특성인지 구분 변수
    [TextArea]
    public string ItemDesc;                                                                              // 상세 설명 string
    [Header("# Level Data")]
    public float baseDamage;                                                                             // 기본 데미지
    public float baseCount;                                                                              // 기본 생성체 수
    public float base_Cool_time;                                                                         // 기본 쿨타임(또는 지속시간)
    public float[] damages;                                                                              // 데미지 입력(입력되는 개수에 따라 Level 상한선 조절)
    public float[] counts;                                                                               // 생성체 입력
    public float[] Cool_time;                                                                            // 쿨타임 입력

    [Header("# Weapon")]
    public int Weapon_num;                                                                               // 무기 고유번호

    [Header("Reinforced")]
    public bool Check;                                                                                   // 강화형 존재 여부 확인
    public Item_Data ReinforcedMode;                                                                     // 강화형 체크
}
