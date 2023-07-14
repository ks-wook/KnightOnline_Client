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
        // ui 방향 조절
        transform.rotation = Camera.main.transform.rotation;

        // 오브젝트 풀에 들어가므로 완전히 삭제 되는 것이 아니다
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
