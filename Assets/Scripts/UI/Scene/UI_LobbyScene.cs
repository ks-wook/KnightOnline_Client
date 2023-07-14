using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LobbyScene : UI_Scene
{
    // Scnene
    public UI_PlayerStatus PlayerStatusUI { get; private set; }
    public UI_Adventure AdventureUI { get; private set; }
    public UI_Stat StatUI { get; private set; }
    public UI_Inventory InvenUI { get; private set; }
    public UI_Quest QuestUI { get; private set; }

    public override void Init()
    {
        base.Init();
        PlayerStatusUI = GetComponentInChildren<UI_PlayerStatus>(); // 캐릭터 현재 상태 UI
        AdventureUI = GetComponentInChildren<UI_Adventure>(); // 던전 선택 ui
        StatUI = GetComponentInChildren<UI_Stat>(); // 스탯 창
        InvenUI = GetComponentInChildren<UI_Inventory>(); // 인벤 창
        QuestUI = GetComponentInChildren<UI_Quest>(); // 퀘스트 ui

        // 처음에는 비활성화
        PlayerStatusUI.gameObject.SetActive(true); // 상태창은 표시
        AdventureUI.gameObject.SetActive(false);
        StatUI.gameObject.SetActive(false);
        InvenUI.gameObject.SetActive(false);
        QuestUI.gameObject.SetActive(true); // 퀘스트는 표시
    }
}
