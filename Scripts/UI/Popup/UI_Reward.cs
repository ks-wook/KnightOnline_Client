using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Reward : UI_Base
{
    enum Images
    {
        Background,
    }

    GameObject _rewardGrid;

    public override void Init()
    {
        Bind<Image>(typeof(Images));

        GetImage((int)Images.Background).gameObject.BindEvent(OnClickBackground);

        _rewardGrid = transform.Find("RewardScroll/Viewport/RewardGrid").gameObject;
    }

    public void OnClickBackground(PointerEventData evt)
    {
        Managers.UI.CloseAllPopupUI();
    }

    public void AddRewardItem(List<Item> rewards)
    {
        StartCoroutine(AddReward(rewards));
    }

    IEnumerator AddReward(List<Item> rewards)
    {
        foreach (Item item in rewards)
        {
            GameObject go = Managers.Resource.Instantiate("UI/Popup/UI_Reward_Item", _rewardGrid.transform);

            Data.ItemData itemData = null;
            Managers.Data.ItemDict.TryGetValue(item.TemplateId, out itemData);

            Sprite icon = Managers.Resource.Load<Sprite>(itemData.iconPath);
            Image rewardImage = go.transform.GetChild(1).GetComponent<Image>();
            rewardImage.sprite = icon;

            yield return new WaitForSeconds(0.2f);
        }
    }
}
