using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GameScene : UI_Scene
{
    public UI_Stat StatUI { get; private set; }
    public UI_Inventory InvenUI { get; private set; }
    public UI_PlayerStatus PlayerStatusUI { get; private set; }
    public UI_BossStatus BossStatusUI { get; private set; }

    public override void Init()
    {
        base.Init();
        StatUI = GetComponentInChildren<UI_Stat>();
        InvenUI = GetComponentInChildren<UI_Inventory>();
        PlayerStatusUI = GetComponentInChildren<UI_PlayerStatus>();
        BossStatusUI = GetComponentInChildren<UI_BossStatus>();

        // ó������ ��Ȱ��ȭ
        StatUI.gameObject.SetActive(false);
        InvenUI.gameObject.SetActive(false);

        // �÷��̾� �������ͽ��� Ȱ��ȭ
        PlayerStatusUI.gameObject.SetActive(true);


        // ���� �������ͽ��� ���� ���̵� ��忡�� Ȱ��ȭ
        BossStatusUI.gameObject.SetActive(false);
    }
}
