using System;
using UnityEngine;
using UnityEngine.UI;

public class Item_Control : MonoBehaviour
{
    #region Variable
    [SerializeField]
    public Item_Data data;
    public int level;
    public Weapon weapon;

    [SerializeField]
    private Item_Control Reinforced_control;

    Image icon;
    Text text_level;
    Text text_Name;
    Text text_Desc;

    public bool Max_Level;

    GameObject weapon_;
    GameObject characteristic_;

    private int list_num;

    public bool Activate_Check;
    public bool Instance = false;
    #endregion
    private void Awake()
    {
        Activate_Check = true;
        Max_Level = false;
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.ItemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        text_level = texts[0];
        text_Name = texts[1];
        text_Desc = texts[2];

        text_Name.text = data.ItemName;
    }
    private void OnEnable()
    {
        if (this.data.Type != Item_Data.ItemType.Coin)
            text_level.text = "Lv." + (level + 1);
        else
            text_level.text = " ";

        switch (this.data.ItemId)
        {
            case 0: // µµ³¢
                text_Desc.text = string.Format(this.data.ItemDesc, this.data.damages[level], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                    this.data.counts[level], this.data.Cool_time[level]);
                break;
            case 1: // ±ÇÃÑ, ±ÇÃÑ°­È­, ·ÎÄÏ·±Ã³, À¯Åº¹ß»ç±â
            case 2: // Àú°Ý, Àú°ÝÃÑ°­È­
            case 3: // »êÅº, »êÅºÃÑ°­È­, ±â°ü´ÜÃÑ
                text_Desc.text = string.Format(this.data.ItemDesc, this.data.damages[level], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                    this.data.counts[level], this.data.Cool_time[level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                break;
            case 4: // ¿¬¸·Åº
                text_Desc.text = string.Format(this.data.ItemDesc, this.data.damages[level], Backend_GameData.Instance.Userstatusdatas.Attack_Power * 0.05f,
                    this.data.counts[level], this.data.Cool_time[level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                break;
            case 5: // ¼¶±¤Åº
                text_Desc.text = string.Format(this.data.ItemDesc, this.data.counts[level], this.data.Cool_time[level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                break;
            case 6: // Áö·Ú
                text_Desc.text = string.Format(this.data.ItemDesc, this.data.damages[level], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                    this.data.counts[level], this.data.Cool_time[level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                break;
            case 7: // °¡½º½¯µå, È­¿°½¯µå
                switch (data.Type)
                {
                    case Item_Data.ItemType.nomal_weapon:
                        text_Desc.text = string.Format(this.data.ItemDesc, this.data.damages[level], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                            this.data.Cool_time[level]);
                        break;
                    case Item_Data.ItemType.reinforced_weapon:
                        text_Desc.text = string.Format(this.data.ItemDesc, this.data.damages[level], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                            this.data.counts[level], this.data.Cool_time[level]);
                        break;
                }
                break;
            case 8: // È­¿°¹æ»ç±â, È­¿°¹æ»ç±â°­È­
                text_Desc.text = string.Format(this.data.ItemDesc, this.data.counts[level], Backend_GameData.Instance.Userstatusdatas.Attack_Power * 0.05f,
                    this.data.Cool_time[level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                break;
            case 9: // ½ÅÈ£Åº
            case 10: // ´ë°Å
                text_Desc.text = string.Format(this.data.ItemDesc, this.data.damages[level], Backend_GameData.Instance.Userstatusdatas.Attack_Power,
                    this.data.counts[level], this.data.Cool_time[level], Backend_GameData.Instance.Userstatusdatas.AttackSpeed);
                break;
            case 11:
            case 12:
            case 13:
                text_Desc.text = string.Format(this.data.ItemDesc, this.data.damages[level]);
                break;
            case 14:
                text_Desc.text = string.Format(this.data.ItemDesc, this.data.damages[level], this.data.counts[level], this.data.Cool_time[level]);
                break;
            case 15:
                text_Desc.text = string.Format(this.data.ItemDesc, this.data.damages[level]);
                break;
            case 16:
                text_Desc.text = string.Format(this.data.ItemDesc);
                break;
            case 17:
                text_Desc.text = string.Format(this.data.ItemDesc, this.data.damages[level]);
                break;
            case 18:
                try
                {
                    text_Desc.text = string.Format(this.data.ItemDesc, Mathf.FloorToInt((GameManager.Instance.current_Kill) * 0.1f));                       // ÄÚÀÎÀº ÇöÀç Å³¼ö * 0.1 ¹Ý¿Ã¸² ÇÏÁö ¾Ê´Â Á¤¼ö °ªÀ¸·Î ¹Ý¿µ
                }
                catch (NullReferenceException) { }                                                                                                                        
                break;
        }
    }
    /// <summary>
    /// LevelUP À¸·Î ÀÎÇÑ Choice ½Ã
    /// </summary>
    public void OnClick()
    {
        switch (this.data.Type)
        {
            case Item_Data.ItemType.nomal_weapon:
                if (this.level == 0)
                {
                    GameManager.Instance.weaponManager.weapons_data.Add(this.data);                                                                // Weapon µî·Ï
                    weapon_ = GameManager.Instance.weaponManager.Weapon_Registration(this.data.Weapon_num);
                    this.list_num = GameManager.Instance.weaponManager.Set_Desc(this.level, this.data);
                    GameManager.Instance.weaponManager.weapons.Add(weapon_.GetComponent<Weapon>());
                    GameManager.Instance.w_chart.Get_Image(this.icon.sprite, this);
                    GameManager.Instance.charManager.Update_Weapon_Status();
                    if (this.data.ReinforcedMode)
                    {
                        GameManager.Instance.uilevelup.Erasing(this.Reinforced_control);
                        GameManager.Instance.tresureui.Erasing(this.Reinforced_control);
                    }
                }
                else
                {
                    weapon_.GetComponent<Weapon>().LevelUp();
                    GameManager.Instance.weaponManager.Update_Desc(this.list_num, this.level, this.data);
                }
                break;
            case Item_Data.ItemType.reinforced_weapon:
                if (this.level == 0)
                {
                    GameManager.Instance.weaponManager.weapons_data.Add(this.data);
                    weapon_ = GameManager.Instance.weaponManager.Weapon_Registration(this.data.Weapon_num);
                    this.list_num = GameManager.Instance.weaponManager.Set_Desc(this.level, this.data);
                    GameManager.Instance.weaponManager.weapons.Add(weapon_.GetComponent<Weapon>());
                    GameManager.Instance.w_chart.Get_Image(this.icon.sprite, this);
                    GameManager.Instance.charManager.Update_Weapon_Status();
                    if (this.data.ReinforcedMode)
                    {
                        GameManager.Instance.uilevelup.Erasing(this.Reinforced_control);
                        GameManager.Instance.tresureui.Erasing(this.Reinforced_control);
                    }
                }
                else
                {
                    weapon_.GetComponent<Weapon>().LevelUp();
                    GameManager.Instance.weaponManager.Update_Desc(this.list_num, this.level, this.data);
                }
                break;
            case Item_Data.ItemType.epic_weapon:
                if (this.level == 0)
                {
                    GameManager.Instance.weaponManager.weapons_data.Add(this.data);
                    weapon_ = GameManager.Instance.weaponManager.Weapon_Registration(this.data.Weapon_num);
                    this.list_num = GameManager.Instance.weaponManager.Set_Desc(this.level, this.data);
                    GameManager.Instance.weaponManager.weapons.Add(weapon_.GetComponent<Weapon>());
                    GameManager.Instance.w_chart.Get_Image(this.icon.sprite, this);
                    GameManager.Instance.charManager.Update_Weapon_Status();
                }
                else
                {
                    weapon_.GetComponent<Weapon>().LevelUp();
                    GameManager.Instance.weaponManager.Update_Desc(this.list_num, this.level, this.data);
                }
                break;
            case Item_Data.ItemType.Characteristic:
            case Item_Data.ItemType.epic_Characteristic:
                if (this.level == 0)
                {
                    GameManager.Instance.charManager.characteristics_data.Add(this.data);
                    characteristic_ = GameManager.Instance.charManager.Characteristic_Registration(this.data.Weapon_num);
                    this.list_num = GameManager.Instance.charManager.Set_Desc(this.level, this.data);
                    GameManager.Instance.charManager.characteristics.Add(characteristic_.GetComponent<Characteristic>());
                    GameManager.Instance.c_chart.Get_Image(this.icon.sprite, this);
                }
                else
                {
                    characteristic_.GetComponent<Characteristic>().LevelUp();
                    GameManager.Instance.charManager.Update_Desc(this.list_num, this.level, this.data);
                }
                break;
            case Item_Data.ItemType.one_time_performance:
                GameManager.Instance.charManager.characteristics_data.Add(this.data);
                characteristic_ = GameManager.Instance.charManager.Characteristic_Registration(this.data.Weapon_num);
                this.level = data.damages.Length;
                this.list_num = GameManager.Instance.charManager.Set_Desc(this.level, this.data);
                GameManager.Instance.charManager.characteristics.Add(characteristic_.GetComponent<Characteristic>());
                GameManager.Instance.c_chart.Get_Image(this.icon.sprite, this);
                break;
            case Item_Data.ItemType.Coin:
                GameManager.Instance.current_Coin += Mathf.FloorToInt((GameManager.Instance.current_Kill) * 0.1f);
                level--;
                break;
        }
        if (level != data.damages.Length)
            level++;

        if (level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
            Max_Level = true;
            GameManager.Instance.uilevelup.Erasing(this);
            GameManager.Instance.tresureui.Erasing(this);
        }
    }
    /// <summary>
    /// º¸¹° »óÀÚ È¹µæÀ¸·Î ÀÎÇÑ Item LevelUP ¹× È¹µæ
    /// </summary>
    public void Tresure_Item()
    {
        switch (this.data.Type)
        {
            case Item_Data.ItemType.nomal_weapon:
                if (this.level == 0)
                {
                    GameManager.Instance.weaponManager.weapons_data.Add(this.data);
                    weapon_ = GameManager.Instance.weaponManager.Weapon_Registration(this.data.Weapon_num);
                    this.list_num = GameManager.Instance.weaponManager.Set_Desc(this.level, this.data);
                    GameManager.Instance.weaponManager.weapons.Add(weapon_.GetComponent<Weapon>());
                    GameManager.Instance.w_chart.Get_Image(this.icon.sprite, this);
                    GameManager.Instance.charManager.Update_Weapon_Status();
                    if (this.data.ReinforcedMode)
                    {
                        GameManager.Instance.uilevelup.Erasing(this.Reinforced_control);
                        GameManager.Instance.tresureui.Erasing(this.Reinforced_control);
                    }
                }
                else
                {
                    weapon_.GetComponent<Weapon>().LevelUp();
                    GameManager.Instance.weaponManager.Update_Desc(this.list_num, this.level, this.data);
                }
                break;
            case Item_Data.ItemType.reinforced_weapon:
                if (this.level == 0)
                {
                    GameManager.Instance.weaponManager.weapons_data.Add(this.data);
                    weapon_ = GameManager.Instance.weaponManager.Weapon_Registration(this.data.Weapon_num);
                    this.list_num = GameManager.Instance.weaponManager.Set_Desc(this.level, this.data);
                    GameManager.Instance.weaponManager.weapons.Add(weapon_.GetComponent<Weapon>());
                    GameManager.Instance.w_chart.Get_Image(this.icon.sprite, this);
                    GameManager.Instance.charManager.Update_Weapon_Status();
                    if (this.data.ReinforcedMode)
                    {
                        GameManager.Instance.uilevelup.Erasing(this.Reinforced_control);
                        GameManager.Instance.tresureui.Erasing(this.Reinforced_control);
                    }
                }
                else
                {
                    weapon_.GetComponent<Weapon>().LevelUp();
                    GameManager.Instance.weaponManager.Update_Desc(this.list_num, this.level, this.data);
                }
                break;
            case Item_Data.ItemType.epic_weapon:
                if (this.level == 0)
                {
                    GameManager.Instance.weaponManager.weapons_data.Add(this.data);
                    weapon_ = GameManager.Instance.weaponManager.Weapon_Registration(this.data.Weapon_num);
                    this.list_num = GameManager.Instance.weaponManager.Set_Desc(this.level, this.data);
                    GameManager.Instance.weaponManager.weapons.Add(weapon_.GetComponent<Weapon>());
                    GameManager.Instance.w_chart.Get_Image(this.icon.sprite, this);
                    GameManager.Instance.charManager.Update_Weapon_Status();
                }
                else
                {
                    weapon_.GetComponent<Weapon>().LevelUp();
                    GameManager.Instance.weaponManager.Update_Desc(this.list_num, this.level, this.data);
                }
                break;
            case Item_Data.ItemType.Characteristic:
            case Item_Data.ItemType.epic_Characteristic:
                if (this.level == 0)
                {
                    GameManager.Instance.charManager.characteristics_data.Add(this.data);
                    characteristic_ = GameManager.Instance.charManager.Characteristic_Registration(this.data.Weapon_num);
                    this.list_num = GameManager.Instance.charManager.Set_Desc(this.level, this.data);
                    GameManager.Instance.charManager.characteristics.Add(characteristic_.GetComponent<Characteristic>());
                    GameManager.Instance.c_chart.Get_Image(this.icon.sprite, this);
                }
                else
                {
                    characteristic_.GetComponent<Characteristic>().LevelUp();
                    GameManager.Instance.charManager.Update_Desc(this.list_num, this.level, this.data);
                }
                break;
            case Item_Data.ItemType.one_time_performance:
                GameManager.Instance.charManager.characteristics_data.Add(this.data);
                characteristic_ = GameManager.Instance.charManager.Characteristic_Registration(this.data.Weapon_num);
                this.level = data.damages.Length;
                this.list_num = GameManager.Instance.charManager.Set_Desc(this.level, this.data);
                GameManager.Instance.charManager.characteristics.Add(characteristic_.GetComponent<Characteristic>());
                GameManager.Instance.c_chart.Get_Image(this.icon.sprite, this);
                break;
            case Item_Data.ItemType.Coin:
                GameManager.Instance.current_Coin += Mathf.FloorToInt((GameManager.Instance.current_Kill) * 0.1f);
                level--;
                break;
        }
        if(level!=data.damages.Length)
            level++;

        if (level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
            Max_Level = true;
            GameManager.Instance.uilevelup.Erasing(this);
            GameManager.Instance.tresureui.Erasing(this);
        }
    }
}
