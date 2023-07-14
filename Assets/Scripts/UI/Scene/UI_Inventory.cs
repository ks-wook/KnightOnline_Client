using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : UI_Base
{
	enum Texts
    {
		ItemNameText,
		ItemTipText
    }

	public List<UI_Inventory_Item> Slot { get; } = new List<UI_Inventory_Item>();

	public override void Init()
	{
		Slot.Clear();

		GameObject grid = transform.Find("ItemGrid").gameObject;
		foreach (Transform child in grid.transform)
			Destroy(child.gameObject);


		// ������ 12�� ���� ��ġ
		for (int i = 0; i < 12; i++)
		{
			GameObject go = Managers.Resource.Instantiate("UI/Scene/UI_Inventory_Item", grid.transform);
			UI_Inventory_Item item = go.GetOrAddComponent<UI_Inventory_Item>();
			Slot.Add(item);
		}


		Bind<Text>(typeof(Texts));


		RefreshUI();
	}

	// UI �缳��
	public void RefreshUI()
	{
		if (Slot.Count == 0) // ���Կ� �������� ���ٸ�
			return;

		// ������ �������� �κ��丮�� ������ �۾� �ʿ�
		List<Item> items = Managers.Inven.Items.Values.ToList();

		for(int i = 0; i < items.Count; i++)
        {
			Slot[i].SetItem(items[i]);
        }
	}

	// ������ ������ ����
	public void SetItemData(string itemName, string itemTipText)
    {
		GetText((int)Texts.ItemNameText).text = itemName;
		GetText((int)Texts.ItemTipText).text = itemTipText;
	}

}
