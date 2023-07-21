using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * �÷��̾��� ���� Hp�� ��ų ��Ÿ�� ���¸� ��Ÿ���� ���� UI ��ũ��Ʈ
 */

public class UI_PlayerStatus : UI_Base
{
    [SerializeField]
    [Tooltip("Hp ǥ�ÿ� �����̴� ��ü")]
    Slider HpSlider;

    [SerializeField]
    [Tooltip("�ñر� ������ ǥ�ÿ� �����̴� ��ü")]
    Slider UltimateSlider;

    [SerializeField]
    [Tooltip("���� ��ų ��Ÿ�� Ÿ�̸�")]
    BattleSkillTimer BattleSkillTimer;



  

    public override void Init()
    {

    }


    // Hp ������ ����
    public void SetHpSlider(int MaxHp, int Hp)
    {

        float ratio = (float) Hp / MaxHp;
        if (HpSlider != null)
        {
            if (ratio >= 1)
                ratio = 1;

            HpSlider.value = ratio;
        }
    }

    public void StartBattleSkillCooldown(float coolTime)
    {
        if(BattleSkillTimer != null)
        {
            BattleSkillTimer.StartBattleSkillCooldown(coolTime);
        }
    }


    // �ñر� ������ ����
    public void SetUltimateSlider(int ultimateCount, int ultimateStack) // �ִ�ġ ����, ���� ����
    {
        float ratio = (float) ultimateStack / ultimateCount;

        if(UltimateSlider != null)
        {
            if (ratio >= 1)
            {
                ratio = 1;

                Managers.Object.MyPlayer.EnableUltimate = true;
            }
            
            UltimateSlider.value = 1 - ratio;
        }
    }



}
