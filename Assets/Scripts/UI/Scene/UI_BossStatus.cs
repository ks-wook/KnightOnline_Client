using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * ���̵� ������ ���� Hp ���¸� ��Ÿ���� ���� UI ��ũ��Ʈ
 */

public class UI_BossStatus : UI_Base
{
    [SerializeField]
    [Tooltip("Hp ǥ�ÿ� �����̴� ��ü")]
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


    // Hp ������ ����
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
