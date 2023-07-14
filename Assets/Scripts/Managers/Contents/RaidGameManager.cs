using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CreatureController;

/*
 * ��Ƽ ������ ���̵忡�� ������ �ൿ ��� ����ϴ� �Ŵ��� ��ũ��Ʈ.
 */


public class RaidGameManager
{
    MonsterController _raidBoss; // ���̵� ���� ��Ʈ�ѷ�
    UI_BossStatus _bossStatusUI;

    public void Init()
    {
        _raidBoss = 
            GameObject.Find("RaidBoss").GetComponentInChildren<MonsterController>();

        if (_raidBoss == null)
        {
            Debug.Log("������Ʈ ���� : ���� ������Ʈ�� ã�� �� �����ϴ�.");
            return;
        }

        // ���� �������ͽ� UI ����
        UI_GameScene SceneUI = Managers.UI.SceneUI as UI_GameScene;
        _bossStatusUI = SceneUI.BossStatusUI;

        _bossStatusUI.SetHpSlider(_raidBoss.STAT.MaxHp, _raidBoss.STAT.Hp);
    }

    // �����κ��� ���� ������ ���� ���� ���� ����
    public void RefreshBoss(S_BossStatChange bossStatChange)
    {
        _raidBoss.HP = bossStatChange.CurHp;
        _bossStatusUI.SetHpSlider(_raidBoss.STAT.MaxHp, bossStatChange.CurHp);

        if (bossStatChange.State == CreateureState.None) // None �ΰ�� ���´� �״�� ����
            return;

        _raidBoss.STATE = (CharacterState)bossStatChange.State;

        Debug.Log(_raidBoss.STATE);
    }

}
