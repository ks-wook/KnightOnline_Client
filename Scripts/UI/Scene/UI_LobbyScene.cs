using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LobbyScene : UI_Scene
{
    // Scnene
    public UI_Adventure AdventureUI { get; private set; }
    public UI_Stat StatUI { get; private set; }
    public UI_Inventory InvenUI { get; private set; }
    public UI_Quest QuestUI { get; private set; }

    public override void Init()
    {
        base.Init();
        AdventureUI = GetComponentInChildren<UI_Adventure>();
        StatUI = GetComponentInChildren<UI_Stat>();
        InvenUI = GetComponentInChildren<UI_Inventory>();
        QuestUI = GetComponentInChildren<UI_Quest>();

        // 처음에는 비활성화
        AdventureUI.gameObject.SetActive(false);
        StatUI.gameObject.SetActive(false);
        InvenUI.gameObject.SetActive(false);
        QuestUI.gameObject.SetActive(true);
    }
}
