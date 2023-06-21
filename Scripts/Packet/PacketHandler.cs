using Assets.Scripts.Controller;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static QuestManager;

class PacketHandler
{
	public static string PlayerName { get; set; }
	public static int AccountUniqueId { get; set; } = 0;

	public static void S_EnterGameHandler(PacketSession session, IMessage packet)
	{
		Debug.Log("게임 입장 패킷 수신");

		S_EnterGame enterGamePacket = packet as S_EnterGame;
		Managers.Object.Add(enterGamePacket.Player, myPlayer: true);
	}

	public static void S_EnterLobbyHandler(PacketSession session, IMessage packet)
	{
		Debug.Log("로비 입장 패킷 수신");

		S_EnterLobby enterGamePacket = (S_EnterLobby)packet;
		Managers.Object.Add(enterGamePacket.Player, new Vector3(30, 0, 50), new Quaternion(0, 0, 0, 0), myPlayer: true, "Lobby");
	}

	public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
	{
		S_LeaveGame leaveGamePacket = packet as S_LeaveGame;
		Managers.Object.Clear();
	}

	public static void S_SpawnHandler(PacketSession session, IMessage packet)
	{
		S_Spawn spawnPacket = packet as S_Spawn;
		foreach (ObjectInfo obj in spawnPacket.Objects)
		{
			Managers.Object.Add(obj, myPlayer: false);
		}
	}

	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		S_Despawn despawnPacket = packet as S_Despawn;
		foreach (int id in despawnPacket.ObjectIds)
		{
			Managers.Object.Remove(id);
		}
	}

	public static void S_MoveHandler(PacketSession session, IMessage packet)
	{
		S_Move movePacket = packet as S_Move;
		ServerSession serverSession = session as ServerSession;

		GameObject go = Managers.Object.FindById(movePacket.ObjectId);
		if (go == null)
			return;

		PlayerController pc = go.GetComponent<PlayerController>();
		if (pc == null)
			return;

		pc.PosInfo = movePacket.PosInfo;
		// pc.DirInfo = movePacket.DirInfo;
	}

	public static void S_SkillHandler(PacketSession session, IMessage packet)
	{
		S_Skill skillPacket = packet as S_Skill;
		ServerSession serverSession = session as ServerSession;


		// 스킬 사용자 탐색
		GameObject go = Managers.Object.FindById(skillPacket.ObjectId);
		if (go == null)
			return;


		// 스킬을 자신이 쓴 경우
		MyPlayerController myPlayer = Managers.Object.MyPlayer;
		if (myPlayer != null && myPlayer.Id == skillPacket.ObjectId)
		{
			myPlayer.UseSkill(skillPacket.Info.SkillId);
		}

		// 다른 클라이언트에서 스킬을 쓴 경우
		PlayerController pc = go.GetComponent<PlayerController>();
		if (pc != null)
		{
			pc.UseSkill(skillPacket.Info.SkillId);
		}
	}

	public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
	{
		S_ChangeHp changePacket = packet as S_ChangeHp;
		ServerSession serverSession = session as ServerSession;


		// 스킬 사용자 탐색
		GameObject go = Managers.Object.FindById(changePacket.ObjectId);
		if (go == null)
			return;

		CreatureController cc = go.GetComponent<CreatureController>();

		if (cc != null)
		{
			cc.HP = changePacket.Hp;
			// TODO : UI
		}
	}

	public static void S_DieHandler(PacketSession session, IMessage packet)
	{
		S_Die diePacket = packet as S_Die;

		GameObject go = Managers.Object.FindById(diePacket.ObjectId);
		if (go == null)
			return;

		CreatureController cc = go.GetComponent<CreatureController>();
		if (cc != null)
		{
			cc.HP = 0;
			cc.OnDead();
			// TODO : UI
		}
	}

	public static void S_ConnectedHandler(PacketSession session, IMessage packet)
	{
		S_Connected connectPackat = packet as S_Connected;

		Debug.Log("S_ConnectedHandler");
		C_Login loginPacket = new C_Login();

		string path = Application.dataPath;

		loginPacket.UniqueId = AccountUniqueId.ToString();
		Managers.Network.Send(loginPacket);
		
	}

	public static void S_LoginHandler(PacketSession session, IMessage packet)
	{
		S_Login loginPacket = packet as S_Login;
		Debug.Log($"LoginOk({loginPacket.LoginOk})");

		// TODO : 받아온 캐릭터 정보들을 ui에 뿌림

		if (loginPacket.Players == null || loginPacket.Players.Count == 0)
		{
			// TEMP
			C_CreatePlayer createPacket = new C_CreatePlayer();

			// player db ID
			createPacket.Name = $"Player_{Random.Range(0, 10000).ToString("0000")}";
			Managers.Network.Send(createPacket);
			PlayerName = createPacket.Name;

		}
		else
		{
			// 임시 : 첫번째 캐릭터로 로그인
			LobbyPlayerInfo info = loginPacket.Players[0];
			C_EnterLobby enterLobbyPacket = new C_EnterLobby();
			enterLobbyPacket.Name = info.Name;
			Managers.Network.Send(enterLobbyPacket);
			PlayerName = info.Name;
		}
	}

	public static void S_CreatePlayerHandler(PacketSession session, IMessage packet)
	{
		S_CreatePlayer createOkPacket = packet as S_CreatePlayer;

		if (createOkPacket.Player == null)
		{
			C_CreatePlayer createPacket = new C_CreatePlayer();
			createPacket.Name = $"Player_{Random.Range(0, 10000).ToString("0000")}";

			Managers.Network.Send(createPacket);
		}
		else
		{
			C_EnterGame enterGamePacket = new C_EnterGame();
			enterGamePacket.Name = createOkPacket.Player.Name;
			Managers.Network.Send(enterGamePacket);
		}
	}

	public static void S_ItemListHandler(PacketSession session, IMessage packet)
	{
		S_ItemList itemList = packet as S_ItemList;


		Managers.Inven.Clear();

		// 로컬 메모리 아이템 저장
		foreach (ItemInfo itemInfo in itemList.Items)
		{
			Item item = Item.MakeItem(itemInfo);
			Managers.Inven.Add(item);
		}

		if (Managers.Object.MyPlayer == null)
			return;

		Managers.Object.MyPlayer.RefreshAdditionalStat();
	}

	public static void S_AddItemHandler(PacketSession session, IMessage packet)
	{
		S_AddItem itemList = packet as S_AddItem;


		List<Item> rewards = new List<Item>();

		// 로컬 메모리 아이템 저장
		foreach (ItemInfo itemInfo in itemList.Items)
		{
			Item item = Item.MakeItem(itemInfo);
			Managers.Inven.Add(item);

			rewards.Add(item);
		}

		if(Managers.UI.SceneType == Define.Scene.Game)
        {
			UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
			gameSceneUI.InvenUI.RefreshUI();
			gameSceneUI.StatUI.RefreshUI();
		}
		else if (Managers.UI.SceneType == Define.Scene.Lobby1)
		{
			UI_LobbyScene lobbyScene = Managers.UI.SceneUI as UI_LobbyScene;
			lobbyScene.InvenUI.RefreshUI();
			lobbyScene.StatUI.RefreshUI();
		}

		UI_RewardPopup RewardPopup = Managers.UI.ShowPopupUI<UI_RewardPopup>();
		RewardPopup.RewardUI.AddRewardItem(rewards);

		Debug.Log($"{itemList.Items.Count}개의 아이템을 획득하였습니다!");
	}

	public static void S_EquipItemHandler(PacketSession session, IMessage packet)
	{
		S_EquipItem equipItemOk = (S_EquipItem)packet;

		// 메모리에 아이템 정보 적용
		Item item = Managers.Inven.Get(equipItemOk.ItemDbId);
		if (item == null)
			return;

		item.Equipped = equipItemOk.Equipped;

		if (Managers.UI.SceneType == Define.Scene.Game)
        {
			UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
			gameSceneUI.InvenUI.RefreshUI();
			gameSceneUI.StatUI.RefreshUI();
		}
		else if (Managers.UI.SceneType == Define.Scene.Lobby1)
		{
			UI_LobbyScene lobbyScene = Managers.UI.SceneUI as UI_LobbyScene;
			lobbyScene.InvenUI.RefreshUI();
			lobbyScene.StatUI.RefreshUI();
		}


		if (Managers.Object.MyPlayer == null)
			return;

		Managers.Object.MyPlayer.RefreshAdditionalStat();

	}

	public static void S_ChangeStatHandler(PacketSession session, IMessage packet)
	{
		S_ChangeStat itemList = (S_ChangeStat)packet;

		// TODO
	}

	public static void S_RaidMatchHandler(PacketSession session, IMessage packet)
	{
		S_RaidMatch matchOkPacket = (S_RaidMatch)packet;
		ServerSession serverSession = session as ServerSession;

        Debug.Log("매칭 완료 패킷 수신");

		if (matchOkPacket.Matched)
        {
			C_EnterGame enterGamePacket = new C_EnterGame();
			enterGamePacket.Name = matchOkPacket.Player.Name;
			enterGamePacket.RoomNum = matchOkPacket.RoomNum;
			Managers.Network.Send(enterGamePacket);
        }

		Managers.Scene.LoadScene(Define.Scene.Game);

	}

	public static void S_QuestChangeHandler(PacketSession session, IMessage packet)
	{
		S_QuestChange questPacket = (S_QuestChange)packet;
		ServerSession serverSession = session as ServerSession;

		Debug.Log("퀘스트 완료 패킷 수신");

		if(questPacket.IsCleared)
        {
			Managers.Quest.HandleClearQuest(questPacket.QuestTemplatedId);
		}

	}

	public static void S_QuestListHandler(PacketSession session, IMessage packet)
	{
		S_QuestList questList = packet as S_QuestList;

		Debug.Log($"{questList.ToString()}");

		Managers.Quest.Clear();
		Managers.Quest.PlayerQuestInitSize = questList.Quests.Count;

		foreach (QuestInfo questInfo in questList.Quests)
		{
			// 나머지 퀘스트 세부정보는 퀘스트 최종 초기화에서 처리
			Quest quest = new Quest()
			{
				ID = questInfo.TemplateId,
				IsCleared = questInfo.IsCleared,
				IsRewarded = questInfo.IsRewarded
			};

			Managers.Quest.AddQuestRegister(quest);

		}

		Managers.Quest.IsRecievedPlayerQuestsByServer = true;
		Managers.Quest.FlushQuestDialogueTaskQueue();
	}

	public static void S_GetExpHandler(PacketSession session, IMessage packet)
	{
		S_GetExp expPacket = packet as S_GetExp;
		Managers.Object.MyPlayer.Stat.TotalExp = expPacket.TotalExp;
		Debug.Log("total exp: " + expPacket.TotalExp);
	}

	public static void S_LevelUpHandler(PacketSession session, IMessage packet)
	{
		S_LevelUp levelUpPacket = packet as S_LevelUp;

		Debug.Log("플레이어 레벨 업");

		// TODO : 플레이어 레벨 업에 대한 적용
		Managers.Object.MyPlayer.LevelUp(levelUpPacket);


		// TODO : 레벨 업 팝업 ui 띄우기
		UI_LevelUpPopup levelUpPopup = Managers.UI.ShowPopupUI<UI_LevelUpPopup>();
		levelUpPopup.SetLevelUpPop(levelUpPacket.NewLevel);





	}
}
