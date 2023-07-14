using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * ���������� Ŀ���ٰ� �۾����� ������ ���� �ִϸ��̼� ��ũ��Ʈ
 * ������ ����Ʈ ǥ�� �� �ַ� ���
 */
public class DamageEffectAnim : MonoBehaviour
{
    private Text text;

    [SerializeField]
    DamageTextEffect damageTextEffect;

    // --------------------- �ִϸ��̼� �ð� ���� ���� -------------------------
    public float scaleFactor = 2f; // Ŀ���� �۾����� ������ ��
    public float growUpTime = 0.2f; // Ŀ���� ȿ�� �ð�
    public float getSmallerTime = 1f; // �۾����� ȿ�� �ð�

    public float fadeOutTime = 1f;

    public float duration = 3f; // �ִϸ��̼��� �� ���� �ð�

    private float timer; // ���� �ִϸ��̼� ���� �ð�

    // -------------------------------------------------------------------------


    // ----------------------- ������Ʈ ��ȭ ���� ���� -------------------------
    private Vector3 initialScale = new Vector3(0.01f, 0.01f, 0.01f);
    private float curScale; // ������Ʈ ũ��

    private float curAlpha; // ������Ʈ ����
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
        // ���� ���� ������ ��ġ�� ���� ��Ų��
        GetComponent<RectTransform>().localPosition = 
            new Vector3(Random.value * 0.5f, Random.value + 1.0f, Random.value * 0.5f);

        text.color = initColor;
        StartCoroutine(CoStartEffect());
    }

    IEnumerator CoStartEffect()
    {

        while (timer < duration)
        {

            if (timer < growUpTime) // �ش� �ð����� Ŀ����
            {
                curScale = Mathf.Lerp(1f, scaleFactor, timer / growUpTime);
                transform.localScale = initialScale * curScale;

            }
            else if (timer < getSmallerTime) // �ش� �ð����� �۾�����
            {
                curScale = Mathf.Lerp(scaleFactor, 1f, timer / getSmallerTime);
                transform.localScale = initialScale * curScale;

            }
            else if (timer < duration) // ���� �ð��� ���� ������ ������ �������
            {
                if (text != null)
                {
                    curColor.a = 1f - Mathf.Clamp01((timer / fadeOutTime) - getSmallerTime);
                    text.color = curColor;
                }

            }

            // �ִϸ��̼� ���� �ð� ����
            timer += Time.deltaTime;

            yield return null;

        }

        timer = 0;

        // ������Ʈ ���� (Ǯ��)
        damageTextEffect.isDestroy = true;

    }

}
