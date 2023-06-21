using Assets.Scripts.Controller;
using Data;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class UI_Stat : UI_Base
{
    Transform WeaponIcon;
    Transform ShieldIcon;
    Transform AccessoryIcon;
    Transform BootsIcon;
    Transform ArmorIcon;

    [SerializeField]
    Sprite weaponIcon;

    [SerializeField]
    Sprite shieldIcon;

    [SerializeField]
    Sprite accessoryIcon;

    [SerializeField]
    Sprite bootsIcon;

    [SerializeField]
    Sprite armorIcon;



    enum Images
    {
        Slot_Weapon,
        Slot_Shield,
        Slot_Accessories,
        Slot_Boots,
        Slot_Armor,
    }

    enum Texts
    {
        NameText,
        LevelText,
        ExpText,
        AttackValueText,
        DefenceValueText
    }

    bool _init = false;
    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));

        WeaponIcon = (Get<Image>((int)Images.Slot_Weapon)).transform.GetChild(1);
        ShieldIcon = (Get<Image>((int)Images.Slot_Shield)).transform.GetChild(1);
        AccessoryIcon = (Get<Image>((int)Images.Slot_Accessories)).transform.GetChild(1);
        BootsIcon = (Get<Image>((int)Images.Slot_Boots)).transform.GetChild(1);
        ArmorIcon = (Get<Image>((int)Images.Slot_Armor)).transform.GetChild(1);

        _init = true;
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

        // 아무것도 착용하지 않는 슬롯은 기본 이미지
        WeaponIcon.GetComponent<Image>().enabled = false;
        ShieldIcon.GetComponent<Image>().enabled = false;
        AccessoryIcon.GetComponent<Image>().enabled = false;
        BootsIcon.GetComponent<Image>().enabled = false;
        ArmorIcon.GetComponent<Image>().enabled = false;

        // 착용한 부위가 있다면 채워준다.
        foreach (Item item in Managers.Inven.Items.Values)
        {
            if (item.Equipped == false)
                continue;

            ItemData itemData = null;
            Managers.Data.ItemDict.TryGetValue(item.TemplateId, out itemData);
            Sprite icon = Managers.Resource.Load<Sprite>(itemData.iconPath);

            if(item.ItemType == ItemType.Weapon)
            {
                WeaponIcon.GetComponent<Image>().enabled = true;
                WeaponIcon.GetComponent<Image>().sprite = icon;
            }
            else if(item.ItemType == ItemType.Armor)
            {
                Armor armor = (Armor)item;
                switch(armor.ArmorType)
                {
                    case ArmorType.Armor:
                        ArmorIcon.GetComponent<Image>().enabled = true;
                        ArmorIcon.GetComponent<Image>().sprite = icon;
                        break;
                    case ArmorType.Boots:
                        BootsIcon.GetComponent<Image>().enabled = true;
                        BootsIcon.GetComponent<Image>().sprite = icon;
                        break;
                }
            }

        }


        // Text
        MyPlayerController player = Managers.Object.MyPlayer;
        player.RefreshAdditionalStat(); // 능력치 재계산

        Get<Text>((int)Texts.NameText).text = player.name;
        

        int totalDamage = player.Stat.Attack + player.WeaponDamage;
        Get<Text>((int)Texts.AttackValueText).text = $"{totalDamage} + {player.WeaponDamage}";
        Get<Text>((int)Texts.DefenceValueText).text = $"{player.ArmorDefence}";
        Get<Text>((int)Texts.LevelText).text = $" Lv.{player.Stat.Level}";

        
        StatInfo curStatInfo = null;
        Managers.Data.StatDict.TryGetValue(player.Stat.Level, out curStatInfo);
        StatInfo nextStatInfo = null;
        Managers.Data.StatDict.TryGetValue(player.Stat.Level + 1, out nextStatInfo);


        int curExp = player.Stat.TotalExp - curStatInfo.TotalExp;
        Get<Text>((int)Texts.ExpText).text = $"{curExp}/{(nextStatInfo.TotalExp - curStatInfo.TotalExp)}";

        float expRatio = (float) curExp / (nextStatInfo.TotalExp - curStatInfo.TotalExp);
        SetExpBar(expRatio);
    }

    void SetExpBar(float expRatio) 
    {
        transform.Find("ExpBar").GetComponent<Slider>().value = expRatio; 
    }
}
