using Assets.Scripts.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ��� ���۽��� �̺�Ʈ�� ������Ʈ�� ���߽�Ű�� Destroy�ϴ� ��ũ��Ʈ
 */

public class ExplosionGimmick : Gimmick
{
    [SerializeField]
    [Tooltip("���� ��� ���� �� ����ϴ� ����Ʈ")]
    ParticleSystem explosionEffect;


    protected override void Init()
    {
        base.Init();

        if (explosionEffect == null)
            Debug.Log("��� �ʱ�ȭ ���� : ����Ʈ �Ҵ� �ʿ�");

    }

    public override void CountUp()
    {
        if (++Condition >= GimmicConditionCount)
        {
            Explosion();
        }
    }

    // ������ ��� �̺�Ʈ
    protected override void Explosion()
    {
        // ���� ��� ����
        Debug.Log("Explosion!!");

        // ���� ȿ���� ���
        Managers.Sound.Play("Effect/Explosion");

        if (explosionEffect != null)
        {
            explosionEffect.gameObject.SetActive(true);
            explosionEffect.Stop();
            explosionEffect.Play();
        }

        // ī�޶� ����
        _cinemachineController.ZoomFocus(camTarget, camPosition, 2.5f);

        Destroy(gameObject, 1f);

    }

    private void Start()
    {
        Init();
    }
}
