using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*
 * 스킬의 쿨타임을 UI에 표시하기 위해 사용한 스크립트
 */


public class BattleSkillTimer : MonoBehaviour
{
    Text coolTimeText; // 쿨타임을 표시할 텍스트
    Image cooldownImage; // 쿨타임을 표시할 이미지

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
            cooldownImage.fillAmount = 0f; // 초기 이미지의 채워진 정도 설정

            // 이미지를 서서히 줄여가는 코루틴 실행
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

            // 이미지의 채워진 정도를 갱신
            float fillAmount = currentTime / coolTime;
            cooldownImage.fillAmount = fillAmount;

            yield return null;
        }

        // 쿨타임 종료 후 전투 스킬 재사용 가능
        Debug.Log("쿨타임 종료");
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
