using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CreatureController;

/*
 * 멀티 컨텐츠 레이드에서 보스의 행동 제어를 담당하는 매니저 스크립트.
 */


public class RaidGameManager
{
    MonsterController _raidBoss; // 레이드 보스 컨트롤러
    UI_BossStatus _bossStatusUI;

    public void Init()
    {
        _raidBoss = 
            GameObject.Find("RaidBoss").GetComponentInChildren<MonsterController>();

        if (_raidBoss == null)
        {
            Debug.Log("오브젝트 에러 : 보스 오브젝트를 찾을 수 없습니다.");
            return;
        }

        // 보스 스테이터스 UI 갱신
        UI_GameScene SceneUI = Managers.UI.SceneUI as UI_GameScene;
        _bossStatusUI = SceneUI.BossStatusUI;

        _bossStatusUI.SetHpSlider(_raidBoss.STAT.MaxHp, _raidBoss.STAT.Hp);
    }

    // 서버로부터 받은 정보를 통해 보스 정보 갱신
    public void RefreshBoss(S_BossStatChange bossStatChange)
    {
        _raidBoss.HP = bossStatChange.CurHp;
        _bossStatusUI.SetHpSlider(_raidBoss.STAT.MaxHp, bossStatChange.CurHp);

        if (bossStatChange.State == CreateureState.None) // None 인경우 상태는 그대로 유지
            return;

        _raidBoss.STATE = (CharacterState)bossStatChange.State;

        Debug.Log(_raidBoss.STATE);
    }

}
