using Assets.Scripts.Controller;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static QuestManager;

/*
 * 서버로부터 받은 패킷에 대한 핸들러를 모아놓은 스크립트
 */

class PacketHandler
{
	public static string PlayerName { get; set; }
	public static int AccountUniqueId { get; set; } = 0;	

	// 메인스테이지 입장 시 패킷 처리
	public static void S_EnterGameHandler(PacketSession session, IMessage packet)
	{
		Debug.Log("게임 입장 패킷 수신");

		S_EnterGame enterGamePacket = packet as S_EnterGame;
		Managers.Object.Add(enterGamePacket.Player, myPlayer: true, "Game");
	}

	// 로비 입장 시 패킷 처리
	public static void S_EnterLobbyHandler(PacketSession session, IMessage packet)
	{
		Debug.Log("로비 입장 패킷 수신");

		S_EnterLobby entetLobbyPacket = (S_EnterLobby)packet;
		Managers.Object.Add(entetLobbyPacket.Player, new Vector3(30, 0, 50), new Quaternion(0, 0, 0, 0), myPlayer: true, "Lobby", null);
	}

	// 게임에서 떠날 때 패킷 처리
	public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
	{
		S_LeaveGame leaveGamePacket = packet as S_LeaveGame;
		Managers.Object.Clear();
	}

	// 룸 내에서 몬스터나 플레이어 스폰 시 패킷 처리
	public static void S_SpawnHandler(PacketSession session, IMessage packet)
	{
		S_Spawn spawnPacket = packet as S_Spawn;
		foreach (ObjectInfo obj in spawnPacket.Objects)
		{
			Managers.Object.Add(obj, myPlayer: false);
		}
	}

	// 룸 내에서 소환되어 있는 플레이어나 몬스터를 없앨 때의 패킷 처리
	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		S_Despawn despawnPacket = packet as S_Despawn;
		foreach (int id in despawnPacket.ObjectIds)
		{
			Managers.Object.Remove(id);
		}
	}

	// 이동 동기화용 패킷 처리
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
	}

	// 스킬 사용 시 패킷 처리
	public static void S_SkillHandler(PacketSession session, IMessage packet)
	{
		S_Skill skillPacket = packet as S_Skill;
		ServerSession serverSession = session as ServerSession;

		Debug.Log("스킬 사용자 ID : " + skillPacket.ObjectId );

		// 스킬 사용자 탐색
		GameObject go = Managers.Object.FindById(skillPacket.ObjectId);
		if (go == null)
        {
			Debug.Log("스킬 사용 오류 : 오브젝트가 존재하지 않습니다.");
			return;
		}

        MyPlayerController myPlayer = Managers.Object.MyPlayer;

        if (myPlayer != null && myPlayer.Id == skillPacket.ObjectId) // 자신이 쓴 스킬이 맞을 경우
		{
			// 실제로 스킬을 시전
			myPlayer.UseSkill(skillPacket.Info.SkillId);
		}
	}

	// 플레이어의 hp에 변동이 생겼을 때의 패킷 처리
	public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
	{
		S_ChangeHp changePacket = packet as S_ChangeHp;
		ServerSession serverSession = session as ServerSession;


		// 데미지를 받은 플레이어 탐색
		GameObject go = Managers.Object.FindById(changePacket.ObjectId);
		if (go == null)
			return;

		if(Managers.Object.MyPlayer.Id == changePacket.ObjectId) // 나 자신이 데미지를 입은 경우
        {
			Managers.Object.MyPlayer.HP = changePacket.Hp;
        }
		else
        {
			PlayerController pc = go.GetComponent<PlayerController>();

			if (pc != null)
			{
				pc.HP = changePacket.Hp;
			}
		}

	}

	// 플레이어 사망시의 패킷 처리
	public static void S_DieHandler(PacketSession session, IMessage packet)
	{
		S_Die diePacket = packet as S_Die;

		GameObject go = Managers.Object.FindById(diePacket.ObjectId);
		if (go == null)
			return;

		CreatureController cc = go.GetComponent<CreatureController>();
		if (cc != null)
		{
			cc.HP = 0; // 사망 처리
		}
	}

	// 플레이어 게임 접속 시 패킷 처리
	public static void S_ConnectedHandler(PacketSession session, IMessage packet)
	{
		S_Connected connectPackat = packet as S_Connected;

		Debug.Log("S_ConnectedHandler");
		C_Login loginPacket = new C_Login();

		string path = Application.dataPath;

		loginPacket.UniqueId = AccountUniqueId.ToString();
		Managers.Network.Send(loginPacket);
		
	}

	// 플레이어 게임 로그인 시 패킷 처리
	public static void S_LoginHandler(PacketSession session, IMessage packet)
	{
		S_Login loginPacket = packet as S_Login;
		Debug.Log($"LoginOk({loginPacket.LoginOk})");

		if (loginPacket.Players == null || loginPacket.Players.Count == 0)
		{
			Debug.Log("새로운 캐릭터 생성");
			C_CreatePlayer createPacket = new C_CreatePlayer();

			// player db ID
			createPacket.Name = $"Player_{Random.Range(0, 10000).ToString("0000")}";
			Managers.Network.Send(createPacket);
			PlayerName = createPacket.Name;

		}
		else
		{
			// 캐릭터로 로그인
			LobbyPlayerInfo info = loginPacket.Players[0];
			C_EnterLobby enterLobbyPacket = new C_EnterLobby();
			enterLobbyPacket.Name = info.Name;
			enterLobbyPacket.IsGameToLobby = false;

			// 씬 전환
			Managers.Scene.LoadScene(Define.Scene.Lobby1);
			Managers.Network.Send(enterLobbyPacket);
			PlayerName = info.Name;
		}
	}

	// 플레이어 캐릭터 생성시의 패킷 처리
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
			// 캐릭터로 로그인
			LobbyPlayerInfo info = createOkPacket.Player;
			C_EnterLobby enterLobbyPacket = new C_EnterLobby();
			enterLobbyPacket.Name = info.Name;
			enterLobbyPacket.IsGameToLobby = false;

			Managers.Scene.LoadScene(Define.Scene.Lobby1);
			Managers.Network.Send(enterLobbyPacket);
			PlayerName = info.Name;
		}
	}

	// 플레이어가 게임 접속 시 플레이어 소유의 아이템 로딩 시 패킷 처리
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

	// 플레이어가 보상등의 이유로 아이템 획득 시 패킷 처리
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

	// 플레이어 게임 접속 시 자신이 클리어한 던전 정보 획득 처리
	public static void S_StageListHandler(PacketSession session, IMessage packet)
	{
		S_StageList stageList = (S_StageList)packet;
		
		// 스테이지 클리어 정보를 매니저에 추가
		foreach(string stageName in stageList.StageNames)
        {
			Debug.Log("메인스테이지 클리어 데이터 : " + stageName);

			if (!Managers.Object.ClearedStages.Contains(stageName))
				Managers.Object.ClearedStages.Add(stageName);
		}

	}

	// 플레이어 장비 착용 시 패킷 처리
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

	// 플레이어 스탯이 변동 되었을 때의 패킷 처리
	public static void S_ChangeStatHandler(PacketSession session, IMessage packet)
	{
		S_ChangeStat statPacket = (S_ChangeStat)packet;
	}

	// 플레이어가 레이드(멀티 컨텐츠) 매칭을 요청 했을 때 답장 패킷 처리
	public static void S_RaidMatchHandler(PacketSession session, IMessage packet)
	{
		S_RaidMatch matchOkPacket = (S_RaidMatch)packet;
		ServerSession serverSession = session as ServerSession;
        Debug.Log("매칭 완료 패킷 수신");

		if (matchOkPacket.Matched)
        {
			// 매칭 완료 패킷에 담긴 id를 이용하여 지정된 게임 room으로 입장 요청
			C_EnterGame enterGamePacket = new C_EnterGame();
			enterGamePacket.Name = matchOkPacket.Player.Name;
			enterGamePacket.RoomNum = matchOkPacket.RoomNum;
			enterGamePacket.PlayerOrder = matchOkPacket.Order;
			enterGamePacket.EquippedItemTemplatedId =
				Managers.Object.MyPlayer.LastWeaponTemplatedId;

			Managers.Network.Send(enterGamePacket);
        }

		// 씬을 먼저 전환 후, 서버로 부터 게임 입장 패킷을 받았을 때 플레이어를 로딩 시킨다
		Managers.Scene.LoadScene(Define.Scene.RaidBoss);

	}

	// 플레이어의 퀘스트 정보에 변화(획득, 클리어)가 있을 때 답장 패킷 처리
	public static void S_QuestChangeHandler(PacketSession session, IMessage packet)
	{
		S_QuestChange questPacket = (S_QuestChange)packet;
		ServerSession serverSession = session as ServerSession;
		
		Debug.Log("퀘스트 정보 패킷 수신");

		if(questPacket.IsCleared) // 클리어된 퀘스트의 경우
		{
			Managers.Quest.HandleClearedQuest(questPacket);
		}



	}

	// 플레이어 게임 접속 시 자신의 퀘스트 진행 정보를 받아올 때 패킷 처리
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
		Managers.Quest.FlushQuestDialogueQueue();
	}

	// 플레이어 경험치 획득 시 패킷 처리
	public static void S_GetExpHandler(PacketSession session, IMessage packet)
	{
		S_GetExp expPacket = packet as S_GetExp;
		Managers.Object.MyPlayer.STAT.TotalExp = expPacket.TotalExp;
		Debug.Log("total exp: " + expPacket.TotalExp);
	}

	// 플레이어 레벨 업 시 패킷 처리
	public static void S_LevelUpHandler(PacketSession session, IMessage packet)
	{
		S_LevelUp levelUpPacket = packet as S_LevelUp;

		Debug.Log("플레이어 레벨 업");

		// 플레이어 레벨 업에 대한 적용
		Managers.Object.MyPlayer.LevelUp(levelUpPacket);


		// 레벨 업 팝업 UI 출력
		UI_LevelUpPopup levelUpPopup = Managers.UI.ShowPopupUI<UI_LevelUpPopup>();
		levelUpPopup.SetLevelUpPop(levelUpPacket.NewLevel);

	}

	// 레이드 보스의 스탯 변화시 패킷 처리
	public static void S_BossStatChangeHandler(PacketSession session, IMessage packet)
	{
		S_BossStatChange bossStatChange = packet as S_BossStatChange;

		// 현재 게임씬인 경우에만
		if(Managers.Scene.CurrentScene.SceneType == Define.Scene.Game)
        {
			Managers.RaidGame.RefreshBoss(bossStatChange); // 변화된 스탯 적용
		}
	}
}
