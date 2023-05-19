using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory_Item : UI_Base
{
	[SerializeField]
	Image _icon = null;

	[SerializeField]
	Image _equipFrame = null;

	public int ItemDbId { get; private set; }
	public int TemplateId { get; private set; }
	public int Count { get; private set; }
	public bool Equipped { get; private set; }


	public override void Init()
	{
		_icon.gameObject.BindEvent((e) =>
		{
			Debug.Log($"Click Item {ItemDbId}");

			Data.ItemData itemData = null;
			Managers.Data.ItemDict.TryGetValue(TemplateId, out itemData);
			
			if (itemData == null)
				return;


			// ��� �������� ���
			if (itemData.itemType == ItemType.Consumable)
            {
				// TODO : ��� �������� ���, C_USE_ITEM ������ ��� ��Ŷ



				return;
			}


			// ������ ���� â �ʱ�ȭ
			{
				if (Managers.UI.SceneType == Define.Scene.Lobby1)
				{
					UI_LobbyScene lobbyScene = Managers.UI.SceneUI as UI_LobbyScene;
					Debug.Log(itemData.tip + itemData.name);
					lobbyScene.InvenUI.SetItemData(itemData.name, itemData.tip);
				}
				else if (Managers.UI.SceneType == Define.Scene.Game)
				{
					UI_GameScene gameScene = Managers.UI.SceneUI as UI_GameScene;
					gameScene.InvenUI.SetItemData(itemData.name, itemData.tip);
				}
			}


			// ���� �������� ���
			{
				// ������ ���� ��Ŷ ����
				C_EquipItem equipPacket = new C_EquipItem();
				equipPacket.ItemDbId = ItemDbId;
				equipPacket.Equipped = !Equipped;

				Managers.Network.Send(equipPacket);


				// ��� ���� ���� ����Ʈ Ȯ��
				Managers.Quest.CheckEquipTypeQuest(ItemDbId);
			}

		});
	}

	public void SetItem(Item item)
	{
		if(item == null)
        {
			ItemDbId = 0;
			TemplateId = 0;
			Count = 0;
			Equipped = false;

			_icon.gameObject.SetActive(false);
			_equipFrame.gameObject.SetActive(false);
        }

		ItemDbId = item.ItemDbId;
		TemplateId = item.TemplateId;
		Count = item.Count;
		Equipped = item.Equipped;

		Data.ItemData itemData = null;
		Managers.Data.ItemDict.TryGetValue(TemplateId, out itemData);

		Sprite icon = Managers.Resource.Load<Sprite>(itemData.iconPath);
		_icon.sprite = icon;

		_icon.gameObject.SetActive(true);
		_equipFrame.gameObject.SetActive(Equipped);
	}
}
