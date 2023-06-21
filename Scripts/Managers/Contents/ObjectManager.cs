using Assets.Scripts.Controller;
using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
	public MyPlayerController MyPlayer { get; set; }
	Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();


	// 기본 스폰
	public void Add(ObjectInfo info, bool myPlayer = false)
	{
		GameObjectType objectType = GetObjectTypeById(info.ObjectId);
		Debug.Log($"{objectType}, {info.ObjectId}");
		if(objectType == GameObjectType.Player) // 플레이어인 경우
        {
			if (myPlayer)
			{
				Debug.Log("PlayerLoad");

				GameObject go = Managers.Resource.Instantiate("Creature/MyPlayer");
				go.name = info.Name;
				_objects.Add(info.ObjectId, go);

				MyPlayer = go.GetComponent<MyPlayerController>();
				MyPlayer.Id = info.ObjectId;
				MyPlayer.Stat = info.StatInfo;
				MyPlayer.PosInfo.MergeFrom(info.PosInfo);
				MyPlayer.SyncPos();
			}
			else
			{
				GameObject go = Managers.Resource.Instantiate("Creature/Player");

				go.name = info.Name;
				_objects.Add(info.ObjectId, go);

				PlayerController pc = go.GetComponent<PlayerController>();
				pc.Id = info.ObjectId;
				pc.PosInfo = info.PosInfo;
				pc.Stat.MergeFrom(info.StatInfo);
				pc.SyncPos();

				Debug.Log($"player id : {pc.Id} joined the game");
			}
		}
		else if(objectType == GameObjectType.Monster) // 몬스터인 경우
        {
			GameObject go = Managers.Resource.Instantiate("Creature/Monster_Skeleton");
			go.name = info.Name;
			_objects.Add(info.ObjectId, go);

			MonsterController mc = go.GetComponent<MonsterController>();
			mc.Id = info.ObjectId;
			mc.PosInfo = info.PosInfo;
			mc.Stat = info.StatInfo;
			mc.SyncPos();

			Debug.Log("Monster sqawn");
		}
	}

	// 위치 지정 플레이어 스폰
	public void Add(ObjectInfo info, Vector3 position, Quaternion rotation, bool myPlayer = false, string playerSceneType = null) // 본인 플레이어외의 다른 플레이어도 해당 함수 이용
	{
		GameObjectType objectType = GetObjectTypeById(info.ObjectId);
		Debug.Log($"{objectType}, {info.ObjectId}");
		if (objectType == GameObjectType.Player) // 플레이어인 경우
		{
			if (myPlayer)
			{
				Debug.Log("PlayerLoad");

				GameObject go = null;
				if (playerSceneType == "Lobby")
					go = Managers.Resource.Instantiate("Creature/MyPlayer_Lobby", position, rotation);
				else if(playerSceneType == "Game")
					go = Managers.Resource.Instantiate("Creature/MyPlayer_Game", position, rotation);

				go.name = info.Name;
				_objects.Add(info.ObjectId, go);

				MyPlayer = go.GetComponent<MyPlayerController>();
				MyPlayer.Id = info.ObjectId;
				MyPlayer.Stat = info.StatInfo;
				MyPlayer.PosInfo.MergeFrom(info.PosInfo);
				MyPlayer.DestPosition = position;
				MyPlayer.SyncPos();
			}
			else
			{
				GameObject go = Managers.Resource.Instantiate("Creature/Player");

				go.name = info.Name;
				_objects.Add(info.ObjectId, go);

				PlayerController pc = go.GetComponent<PlayerController>();
				pc.Id = info.ObjectId;
				pc.PosInfo = info.PosInfo;
				pc.Stat.MergeFrom(info.StatInfo);
				pc.SyncPos();

				Debug.Log($"player id : {pc.Id} joined the game");
			}
		}
		else if (objectType == GameObjectType.Monster) // 몬스터인 경우
		{
			GameObject go = Managers.Resource.Instantiate("Creature/Monster_Skeleton");
			go.name = info.Name;
			_objects.Add(info.ObjectId, go);

			MonsterController mc = go.GetComponent<MonsterController>();
			mc.Id = info.ObjectId;
			mc.PosInfo = info.PosInfo;
			mc.Stat = info.StatInfo;
			mc.SyncPos();

			Debug.Log("Monster sqawn");
		}
	}

	public static GameObjectType GetObjectTypeById(int id)
	{
		int type = (id >> 24) & 0x7F;// 0111 1111
		return (GameObjectType)type;
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

			//if (cc.CellPos == cellPos)
			//	return obj;
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

	public void Clear()
	{
		foreach (GameObject obj in _objects.Values)
			Managers.Resource.Destroy(obj);
		_objects.Clear();
	}
}
