using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MonsterController;


/*
 * �⺻ ������ �ִϸ��̼� �̺�Ʈ ó���� ���� ��ũ��Ʈ
 */

public class AnimationEvents_Monster : MonoBehaviour
{
    Collider[] hitColliders; // ���� �õ� �� ������ ������Ʈ
    MonsterController _monsterController;
    Animator _animator;
    MonsterAI _monsterAITrigger; // ������ �þ߿� ���� �ൿ Ʈ����
    LayerMask _hittalbeMask;


    MonsterSkillSet[] _skillSets; // ���� ��ų�� ����


    void Init()
    {
        // ������ �� �ִ� ���̾�
        _hittalbeMask = LayerMask.GetMask("Player");

        // �ִϸ��̼� ����� ���� ������Ʈ�� ������
        _animator = transform.GetComponent<Animator>();

        // ������Ʈ ������ ���� ��Ʈ�ѷ��� �����´�
        _monsterController = transform.GetComponent<MonsterController>();

        if (_monsterController != null)
        {
            _skillSets = _monsterController.MonsterSkillSets;
        }

        // �ൿ Ƚ���� ���� �ñر� ����� ���� Ʈ���Ÿ� �����´�
        _monsterAITrigger = transform.GetComponentInChildren<MonsterAI>();
    }



    // ------------------------------- �ִϸ��̼� EventHandler ------------------------------------


    // ��ų ��� �� ��Ʈ ������ �ϴ� ����
    public void OnSkillHit(int skillId)
    {
        // ���� ���� �õ�
        hitColliders = Physics.OverlapSphere(
                transform.position + (transform.forward * 1),
                _skillSets[skillId - 1].SkillRange, _hittalbeMask); // Skill Id 1 �� �⺻����, 2���� ��ų

        if (hitColliders.Length != 0) // ���� ���� �� ������ ó��
        {
            // �÷��̾� ������ ó��
            _monsterController.HandleDamage(skillId, hitColliders);
        }

        // �ִϸ��̼��� ��� Ƚ���� ����Ͽ� ���� Ƚ���� ���� �� �ñر� ����ϵ��� ����
        _monsterController._ultimateStack++; 
    }

    // ���� ���
    public void PlayAudio(string path)
    {
        Managers.Sound.Play("Creature/Monster/" + path);
    }

    // Inputable ���� ����
    public void SetInputable(int inputable)
    {
        Debug.Log("SetInputable: " + inputable);

        if (inputable == 1)
            _monsterController.Inputable = true;
        else if (inputable == 0)
            _monsterController.Inputable = false;
    }

    // -----------------------------------------------------------------------------------------------









    // ----------------------------------------- Start -----------------------------------------------

    void Start()
    {
        Init();
    }



    // ----------------------------------------------------------------------------------------------






}
