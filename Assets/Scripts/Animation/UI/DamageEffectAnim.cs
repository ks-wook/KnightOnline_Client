using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 순간적으로 커졌다가 작아지는 연출을 위한 애니메이션 스크립트
 * 데미지 이펙트 표현 시 주로 사용
 */
public class DamageEffectAnim : MonoBehaviour
{
    private Text text;

    [SerializeField]
    DamageTextEffect damageTextEffect;

    // --------------------- 애니메이션 시간 관련 변수 -------------------------
    public float scaleFactor = 2f; // 커졌다 작아지는 스케일 값
    public float growUpTime = 0.2f; // 커지는 효과 시간
    public float getSmallerTime = 1f; // 작아지는 효과 시간

    public float fadeOutTime = 1f;

    public float duration = 3f; // 애니메이션의 총 지속 시간

    private float timer; // 현재 애니메이션 진행 시간

    // -------------------------------------------------------------------------


    // ----------------------- 오브젝트 변화 관련 변수 -------------------------
    private Vector3 initialScale = new Vector3(0.01f, 0.01f, 0.01f);
    private float curScale; // 오브젝트 크기

    private float curAlpha; // 오브젝트 투명도
    private Color curColor;
    private Color initColor;
    // -------------------------------------------------------------------------



    private void Awake()
    {
        text = GetComponent<Text>();
        curColor = new Color(text.color.r, text.color.g, text.color.b, 1f);
        initColor = text.color;
    }

    public void StartEffect()
    {
        // 몬스터 주위 랜덤한 위치에 스폰 시킨다
        GetComponent<RectTransform>().localPosition = 
            new Vector3(Random.value * 0.5f, Random.value + 1.0f, Random.value * 0.5f);

        text.color = initColor;
        StartCoroutine(CoStartEffect());
    }

    IEnumerator CoStartEffect()
    {

        while (timer < duration)
        {

            if (timer < growUpTime) // 해당 시간동안 커진다
            {
                curScale = Mathf.Lerp(1f, scaleFactor, timer / growUpTime);
                transform.localScale = initialScale * curScale;

            }
            else if (timer < getSmallerTime) // 해당 시간동안 작아진다
            {
                curScale = Mathf.Lerp(scaleFactor, 1f, timer / getSmallerTime);
                transform.localScale = initialScale * curScale;

            }
            else if (timer < duration) // 지속 시간이 끝날 때까지 서서히 사라진다
            {
                if (text != null)
                {
                    curColor.a = 1f - Mathf.Clamp01((timer / fadeOutTime) - getSmallerTime);
                    text.color = curColor;
                }

            }

            // 애니메이션 진행 시간 증가
            timer += Time.deltaTime;

            yield return null;

        }

        timer = 0;

        // 오브젝트 삭제 (풀링)
        damageTextEffect.isDestroy = true;

    }

}
