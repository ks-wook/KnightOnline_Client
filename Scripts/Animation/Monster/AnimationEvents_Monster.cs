using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents_Monster : MonoBehaviour
{
    [SerializeField]
    Transform charBody;

    MonsterController _monsterController;
    StatData_monster _stat;
    Animator _animator;

    LayerMask _hittalbeMask;

    private void Start()
    {
        if (charBody == null)
            charBody = transform.parent;

        // ������ �� �ִ� ��� ����ũ
        _hittalbeMask = LayerMask.GetMask("Player");

        // �ִϸ��̼� ����� ���� ������Ʈ�� ������
        _animator = charBody.GetComponent<Animator>();

        // ������Ʈ ������ ���� ��Ʈ�ѷ��� �����´�
        _monsterController = charBody.GetComponent<MonsterController>();
    }

    void OnRunEvent()
    {
        // 1. Play foot sound
    }

    // ���� ���� ���
    public void OnHitted()
    {
        Debug.Log("Hitted!");

        // 2. �ǰ� �ִϸ��̼� ���
        _animator.Rebind(); // ���������� ���
        _animator.Play("Skeleton_Damage");


        // 3. ��Ʈ�ѷ��� �ǰ� ���� ����
        _monsterController.STATE = MonsterController.CharacterState.KnockBack;
    }

    void moveForward()
    {
        Rigidbody rigidBody = charBody.GetComponent<Rigidbody>();
        rigidBody.AddForce(transform.forward * 0.5f, ForceMode.Impulse);
    }

    void moveBackward()
    {
        Rigidbody rigidBody = charBody.GetComponent<Rigidbody>();
        rigidBody.AddForce(-transform.forward * 20.0f);
    }

    void OnHitEvent() 
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + (transform.forward * 2), 1.0f, _hittalbeMask);

        foreach (Collider collider in colliders)
        {
            if(collider.transform.GetChild(0).GetComponent<AnimationEvents_Player>() != null)
            {
                collider.transform.GetChild(0).GetComponent<AnimationEvents_Player>().OnHitted();
            }
        }
    }
}
