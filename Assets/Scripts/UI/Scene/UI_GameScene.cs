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

        // 처음에는 비활성화
        StatUI.gameObject.SetActive(false);
        InvenUI.gameObject.SetActive(false);

        // 플레이어 스테이터스는 활성화
        PlayerStatusUI.gameObject.SetActive(true);


        // 보스 스테이터스는 보스 레이드 모드에만 활성화
        BossStatusUI.gameObject.SetActive(false);
    }
}
