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

        // 공격할 수 있는 대상 마스크
        _hittalbeMask = LayerMask.GetMask("Player");

        // 애니메이션 재생을 위해 컴포넌트를 가져옴
        _animator = charBody.GetComponent<Animator>();

        // 스테이트 조절을 위해 컨트롤러를 가져온다
        _monsterController = charBody.GetComponent<MonsterController>();
    }

    void OnRunEvent()
    {
        // 1. Play foot sound
    }

    // 공격 받은 경우
    public void OnHitted()
    {
        Debug.Log("Hitted!");

        // 2. 피격 애니메이션 재생
        _animator.Rebind(); // 맞을때마다 재생
        _animator.Play("Skeleton_Damage");


        // 3. 컨트롤러에 피격 상태 전달
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
