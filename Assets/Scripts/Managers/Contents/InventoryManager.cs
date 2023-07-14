using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 플레이어의 인벤토리를 관리는 매니저 스크립트
 * 
 * 게임 접속 시 플레이어의 아이템 정보를 패킷으로 받고,
 * 해당 정보들을 인벤토리 매니저에 제공하여 플레이어에게
 * 시각적인 인벤토리 UI를 갱신하는데에 사용한다.
 */

public class InventoryManager
{
	public Dictionary<int, Item> Items { get; } = new Dictionary<int, Item>();

	public void Add(Item item)
	{
		Debug.Log($"도감 번호{item.TemplateId} 획득");
		Items.Add(item.ItemDbId, item);
	}

	public Item Get(int itemDbId)
	{
		Item item = null;
		Items.TryGetValue(itemDbId, out item);
		return item;
	}

	public Item Find(Func<Item, bool> condition)
	{
		foreach (Item item in Items.Values)
		{
			if (condition.Invoke(item))
				return item;
		}

		return null;
	}

	public void Clear()
	{
		Items.Clear();
	}
}