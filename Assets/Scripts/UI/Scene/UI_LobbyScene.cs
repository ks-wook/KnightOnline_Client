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
        PlayerStatusUI = GetComponentInChildren<UI_PlayerStatus>(); // ĳ���� ���� ���� UI
        AdventureUI = GetComponentInChildren<UI_Adventure>(); // ���� ���� ui
        StatUI = GetComponentInChildren<UI_Stat>(); // ���� â
        InvenUI = GetComponentInChildren<UI_Inventory>(); // �κ� â
        QuestUI = GetComponentInChildren<UI_Quest>(); // ����Ʈ ui

        // ó������ ��Ȱ��ȭ
        PlayerStatusUI.gameObject.SetActive(true); // ����â�� ǥ��
        AdventureUI.gameObject.SetActive(false);
        StatUI.gameObject.SetActive(false);
        InvenUI.gameObject.SetActive(false);
        QuestUI.gameObject.SetActive(true); // ����Ʈ�� ǥ��
    }
}
