using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageTextEffect : MonoBehaviour
{
    [SerializeField]
    DamageEffectAnim damageEffectAnim;

    public bool isDestroy = false;

    void Update()
    {
        // ui ���� ����
        transform.rotation = Camera.main.transform.rotation;

        // ������Ʈ Ǯ�� ���Ƿ� ������ ���� �Ǵ� ���� �ƴϴ�
        if (isDestroy)
        {
            isDestroy = false;
            Managers.Resource.Destroy(gameObject);
        }

    }


    public void SetDamageText(int damage)
    {
        GetComponent<Text>().text = damage.ToString();
        damageEffectAnim.StartEffect();
    }

}
