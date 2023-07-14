using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 플레이어의 현재 Hp및 스킬 쿨타임 상태를 나타내기 위한 UI 스크립트
 */

public class UI_PlayerStatus : UI_Base
{
    [SerializeField]
    [Tooltip("Hp 표시용 슬라이더 객체")]
    Slider HpSlider;

    [SerializeField]
    [Tooltip("궁극기 게이지 표시용 슬라이더 객체")]
    Slider UltimateSlider;

    [SerializeField]
    [Tooltip("전투 스킬 쿨타임 타이머")]
    BattleSkillTimer BattleSkillTimer;


    Text _hpText;

    enum Images
    {
        
    }

    enum Texts
    {
        HpText,
    }

    public override void Init()
    {
        Bind<Text>(typeof(Texts));

        _hpText = GetText((int)Texts.HpText);
    }


    // Hp 게이지 갱신
    public void SetHpSlider(int MaxHp, int Hp)
    {
        _hpText.text = Hp + " / " + MaxHp;

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


    // 궁극기 게이지 갱신
    public void SetUltimateSlider(int ultimateCount, int ultimateStack) // 최대치 스택, 현재 스택
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
