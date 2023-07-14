using Assets.Scripts.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 기믹 동작시의 이벤트로 오브젝트를 폭발시키고 Destroy하는 스크립트
 */

public class ExplosionGimmick : Gimmick
{
    [SerializeField]
    [Tooltip("폭발 기믹 동작 시 사용하는 이펙트")]
    ParticleSystem explosionEffect;


    protected override void Init()
    {
        base.Init();

        if (explosionEffect == null)
            Debug.Log("기믹 초기화 실패 : 이펙트 할당 필요");

    }

    public override void CountUp()
    {
        if (++Condition >= GimmicConditionCount)
        {
            Explosion();
        }
    }

    // 폭발형 기믹 이벤트
    protected override void Explosion()
    {
        // 폭발 기믹 동작
        Debug.Log("Explosion!!");

        // 폭발 효과음 재생
        Managers.Sound.Play("Effect/Explosion");

        if (explosionEffect != null)
        {
            explosionEffect.gameObject.SetActive(true);
            explosionEffect.Stop();
            explosionEffect.Play();
        }

        // 카메라 줌인
        _cinemachineController.ZoomFocus(camTarget, camPosition, 2.5f);

        Destroy(gameObject, 1f);

    }

    private void Start()
    {
        Init();
    }
}
