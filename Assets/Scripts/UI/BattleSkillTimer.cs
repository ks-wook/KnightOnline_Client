using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*
 * ��ų�� ��Ÿ���� UI�� ǥ���ϱ� ���� ����� ��ũ��Ʈ
 */


public class BattleSkillTimer : MonoBehaviour
{
    Text coolTimeText; // ��Ÿ���� ǥ���� �ؽ�Ʈ
    Image cooldownImage; // ��Ÿ���� ǥ���� �̹���

    private float currentTime = 0f;

    void Init()
    {
        coolTimeText = transform.GetComponentInChildren<Text>();
        cooldownImage = transform.GetComponent<Image>();
    }

    public void StartBattleSkillCooldown(float coolTime)
    {
        if (Managers.Object.MyPlayer.EnableBattleSkill)
        {
            currentTime = coolTime;
            cooldownImage.fillAmount = 0f; // �ʱ� �̹����� ä���� ���� ����

            // �̹����� ������ �ٿ����� �ڷ�ƾ ����
            StartCoroutine("DecreaseImageCoroutine", coolTime);
        }
    }

    IEnumerator DecreaseImageCoroutine(float coolTime)
    {
        Managers.Object.MyPlayer.EnableBattleSkill = false;

        while (currentTime > 0f)
        {
            currentTime -= Time.deltaTime;
            coolTimeText.text = Mathf.Round(currentTime * 10f) / 10f + "";

            // �̹����� ä���� ������ ����
            float fillAmount = currentTime / coolTime;
            cooldownImage.fillAmount = fillAmount;

            yield return null;
        }

        // ��Ÿ�� ���� �� ���� ��ų ���� ����
        Debug.Log("��Ÿ�� ����");
        Managers.Object.MyPlayer.EnableBattleSkill = true;
        coolTimeText.text = "";
    }



    // ------------------------ Start -------------------------------
    private void Start()
    {
        Init();
    }

    // --------------------------------------------------------------
}
