using Assets.Scripts.Controller;
using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 씬 내의 오브젝트를 instanciate 하거나 관리하는 매니저 스크립트.
 */


public class ObjectManager
{
	public MyPlayerController MyPlayer { get; set; }

	public List<string> ClearedStages = new List<string>();

	public Dictionary<int, NPCObject> NPCs = new Dictionary<int, NPCObject>();


	Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();

	private int _spawnCounter = 0;

	// 기본 스폰
	public void Add(ObjectInfo info, bool myPlayer = false, string playerSceneType = null)
	{
		GameObjectType objectType = GetObjectTypeById(info.ObjectId);
		Debug.Log($"{objectType}, {info.ObjectId}");
		if(objectType == GameObjectType.Player) // 플레이어인 경우
        {
			if (myPlayer) // 자신의 캐릭터 Instanciate
			{
				Debug.Log("PlayerLoad");

				GameObject go = null;
				if (playerSceneType == "Lobby")
					go = Managers.Resource.Instantiate("Creature/Player/MyPlayer_Lobby");
				else if (playerSceneType == "Game")
					go = Managers.Resource.Instantiate("Creature/Player/MyPlayer_Game");
				

				go.name = info.Name;
				GameObject player = null;
				if(_objects.TryGetValue(info.ObjectId, out player) == false) // 오브젝트 매니저에 플레이어가 없는 경우 추가
                {
					_objects.Add(info.ObjectId, go);
				}

				MyPlayer = go.GetComponent<MyPlayerController>();
				MyPlayer.Id = info.ObjectId;
				MyPlayer.STAT = info.StatInfo;
				MyPlayer.PosInfo.MergeFrom(info.PosInfo);
				MyPlayer.SyncPos();
			}
			else // 다른 유저의 캐릭터 Instanciate
			{
				GameObject go = Managers.Resource.Instantiate("Creature/Player/Player");

				go.name = info.Name;
				_objects.Add(info.ObjectId, go);

				PlayerController pc = go.GetComponent<PlayerController>();
				pc.Id = info.ObjectId;
				pc.PosInfo = info.PosInfo;
				pc.STAT.MergeFrom(info.StatInfo);
				pc.SetWeaponPrefab(info);
				pc.SyncPos();

				Debug.Log($"player id : {pc.Id} joined the game");
			}
		}
	}

	// 위치 지정 스폰
	public GameObject Add(ObjectInfo info, Vector3 position, Quaternion rotation, bool myPlayer = false, string playerSceneType = null, Transform parent = null) // 본인 플레이어외의 다른 플레이어도 해당 함수 이용
	{
		GameObjectType objectType = GetObjectTypeById(info.ObjectId);
		Debug.Log($"{objectType}, {info.ObjectId}");
		if (objectType == GameObjectType.Player) // 플레이어인 경우
		{
			if (myPlayer) // 자신의 캐릭터 Instanciate
			{
				Debug.Log("PlayerLoad");

				GameObject go = null;
				if (playerSceneType == "Lobby")
					go = Managers.Resource.Instantiate("Creature/Player/MyPlayer_Lobby", position, rotation);
				else if(playerSceneType == "Game")
					go = Managers.Resource.Instantiate("Creature/Player/MyPlayer_Game", position, rotation);

				go.name = info.Name;
				GameObject player = null;
				if (_objects.TryGetValue(info.ObjectId, out player) == false) // 오브젝트 매니저에 플레이어가 없는 경우 추가
				{
					_objects.Add(info.ObjectId, go);
				}

				MyPlayer = go.GetComponent<MyPlayerController>();
				MyPlayer.Id = info.ObjectId;
				MyPlayer.STAT = info.StatInfo;
				MyPlayer.PosInfo.MergeFrom(info.PosInfo);
				MyPlayer.VectorPosInfo = position;
				MyPlayer.SyncPos();

				return go;
			}
			else // 다른 유저의 캐릭터 Instanciate
			{
				GameObject go = Managers.Resource.Instantiate("Creature/Player/Player");

				go.name = info.Name;
				GameObject player = null;
				if (_objects.TryGetValue(info.ObjectId, out player) == false) // 오브젝트 매니저에 플레이어가 없는 경우 추가
				{
					_objects.Add(info.ObjectId, go);
				}

				PlayerController pc = go.GetComponent<PlayerController>();
				pc.Id = info.ObjectId;
				pc.PosInfo = info.PosInfo;
				pc.STAT.MergeFrom(info.StatInfo);
				pc.SyncPos();

				Debug.Log($"player id : {pc.Id} joined the game");

				return go;
			}
		}
		else if (objectType == GameObjectType.Monster) // 몬스터인 경우
		{
			GameObject go = Managers.Resource.Instantiate($"Creature/Monster/{info.Name}", position, rotation);
			go.name = info.Name;
			_objects.Add(info.ObjectId, go);

			MonsterController mc = go.GetComponent<MonsterController>();
			mc.Id = info.ObjectId;
			mc.STAT = info.StatInfo;
			mc.VectorPosInfo = position;
			mc.BasePosition = position;
			// mc.SyncPos();

			Debug.Log("Monster sqawn");

			return go;
		}

		return null;
	}

	public static GameObjectType GetObjectTypeById(int id)
	{
		int type = (id >> 24) & 0x7F;// 0111 1111
		return (GameObjectType)type;
	}

	public int GenerateId(GameObjectType type)
	{
		return ((int)type << 24) | (_spawnCounter++);
	}

	public void Remove(int id)
	{
		GameObject go = FindById(id);
		if (go == null)
			return;

		_objects.Remove(id);
		Managers.Resource.Destroy(go);
	}

	public void RemoveMyPlayer()
	{
		if (MyPlayer == null)
			return;

		Remove(MyPlayer.Id);
		MyPlayer = null;
	}

	public GameObject FindById(int Id)
    {
		GameObject go = null;
		_objects.TryGetValue(Id, out go);
		return go;

	}

	public GameObject Find(Vector3Int cellPos)
	{
		foreach (GameObject obj in _objects.Values)
		{
			CreatureController cc = obj.GetComponent<CreatureController>();
			if (cc == null)
				continue;

		}

		return null;
	}

	public GameObject Find(Func<GameObject, bool> condition)
	{
		foreach (GameObject obj in _objects.Values)
		{
			if (condition.Invoke(obj))
				return obj;
		}

		return null;
	}

	// 로비씬에서 NPC 객체 획득
	public void GetNPC()
    {
		// 현재 씬이 로비인 경우에만 동작 (NPC들은 로비씬에만 존재하므로)
		if(Managers.Scene.CurrentScene.SceneType == Define.Scene.Lobby1)
        {
			NPCObject[] npcs = GameObject.FindObjectsOfType<NPCObject>();

			foreach(NPCObject npc in npcs)
            {
				NPCs.Add(npc.npcID, npc);
			}
		}
	}


	public void Clear()
	{
		foreach (GameObject obj in _objects.Values)
			Managers.Resource.Destroy(obj);
		_objects.Clear();
		NPCs.Clear();
	}
}
