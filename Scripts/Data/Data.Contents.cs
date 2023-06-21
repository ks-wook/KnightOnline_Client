using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
	#region Stat
	[Serializable]
	public class StatData : ILoader<int, StatInfo>
	{
		public List<StatInfo> stats = new List<StatInfo>();

		public Dictionary<int, StatInfo> MakeDict()
		{
			Dictionary<int, StatInfo> dict = new Dictionary<int, StatInfo>();
			foreach (StatInfo stat in stats)
			{
				stat.Hp = stat.MaxHp;
				dict.Add(stat.Level, stat);
			}
			return dict;
		}
	}
	#endregion


	#region Skill
	[Serializable]
	public class Skill
	{
		public int id;
		public string name;
		public float cooldown;
		public int damage;
		public SkillType skillType;
	}

	[Serializable]
	public class SkillData : ILoader<int, Skill>
	{
		public List<Skill> skills = new List<Skill>();

		public Dictionary<int, Skill> MakeDict()
		{
			Dictionary<int, Skill> dict = new Dictionary<int, Skill>();
			foreach (Skill skill in skills)
			{
				dict.Add(skill.id, skill);
			}
			return dict;
		}
	}
	#endregion


	#region Item
	[Serializable]
	public class ItemData
	{
		public int id;
		public string name;
		public ItemType itemType;
		public string tip;
		public string iconPath;
	}

	[Serializable]
	public class WeaponData : ItemData
	{
		public WeaponType weaponType;
		public int damage;
	}

	[Serializable]
	public class ArmorData : ItemData
	{
		public ArmorType armorType;
		public int defence;
	}

	[Serializable]
	public class ConsumableData : ItemData
	{
		public ConsumableType consumableType;
		public int maxCount;
	}

	[Serializable]
	public class ItemLoader : ILoader<int, ItemData>
	{
		public List<WeaponData> weapons = new List<WeaponData>();
		public List<ArmorData> armors = new List<ArmorData>();
		public List<ConsumableData> consumables = new List<ConsumableData>();

		public Dictionary<int, ItemData> MakeDict()
		{
			Dictionary<int, ItemData> dict = new Dictionary<int, ItemData>();
			foreach (ItemData item in weapons)
			{
				item.itemType = ItemType.Weapon;
				dict.Add(item.id, item);
			}
			foreach (ItemData item in armors)
			{
				item.itemType = ItemType.Armor;
				dict.Add(item.id, item);
			}
			foreach (ItemData item in consumables)
			{
				item.itemType = ItemType.Consumable;
				dict.Add(item.id, item);
			}

			return dict;
		}
	}
	#endregion


	#region BossMonster
	[Serializable]
	public class BossMonsterData
	{
		public int id;
		public string name;
		public StatInfo stat;
		public string prefabPath;
	}

	[Serializable]
	public class BossMonsterLoader : ILoader<int, BossMonsterData>
	{
		public List<BossMonsterData> bossMonsters = new List<BossMonsterData>();

		public Dictionary<int, BossMonsterData> MakeDict()
		{
			Dictionary<int, BossMonsterData> dict = new Dictionary<int, BossMonsterData>();
			foreach (BossMonsterData bossMonster in bossMonsters)
			{
				dict.Add(bossMonster.id, bossMonster);
			}
			return dict;
		}
	}
	#endregion


	#region Stage
	// TODO : json 파일 생성 및 로드 -> 구글 스프레드 시트
	[Serializable]
	public class StageData
	{
		// 스테이지 number(id)
		public int id;
		// public List<RewardData> rewards;
	}

	[Serializable]
	public class StageLoader : ILoader<int, StageData>
	{
		public List<StageData> stages = new List<StageData>();

		public Dictionary<int, StageData> MakeDict()
		{
			Dictionary<int, StageData> dict = new Dictionary<int, StageData>();
			foreach (StageData stage in stages)
			{
				// TODO : 보상 목록 dict 에 삽입
				dict.Add(stage.id, stage);
			}
			return dict;
		}
	}

	#endregion
}