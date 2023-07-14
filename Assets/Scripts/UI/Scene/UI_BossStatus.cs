using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 레이드 보스의 현재 Hp 상태를 나타내기 위한 UI 스크립트
 */

public class UI_BossStatus : UI_Base
{
    [SerializeField]
    [Tooltip("Hp 표시용 슬라이더 객체")]
    Slider HpSlider;


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

        Managers.RaidGame.Init();
    }


    // Hp 게이지 갱신
    public void SetHpSlider(int MaxHp, int Hp)
    {
        if (_hpText == null || MaxHp == 0)
            return;


        _hpText.text = Hp + " / " + MaxHp;
        float ratio = (float)Hp / MaxHp;
        if (HpSlider != null)
        {
            if (ratio >= 1)
                ratio = 1;

            HpSlider.value = ratio;
        }
    }
}
