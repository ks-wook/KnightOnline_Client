using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MonsterController;


/*
 * 기본 몬스터의 애니메이션 이벤트 처리를 위한 스크립트
 */

public class AnimationEvents_Monster : MonoBehaviour
{
    Collider[] hitColliders; // 공격 시도 후 명중한 오브젝트
    MonsterController _monsterController;
    Animator _animator;
    MonsterAI _monsterAITrigger; // 몬스터의 시야에 따른 행동 트리거
    LayerMask _hittalbeMask;


    MonsterSkillSet[] _skillSets; // 몬스터 스킬셋 정보


    void Init()
    {
        // 공격할 수 있는 레이어
        _hittalbeMask = LayerMask.GetMask("Player");

        // 애니메이션 재생을 위해 컴포넌트를 가져옴
        _animator = transform.GetComponent<Animator>();

        // 스테이트 조절을 위해 컨트롤러를 가져온다
        _monsterController = transform.GetComponent<MonsterController>();

        if (_monsterController != null)
        {
            _skillSets = _monsterController.MonsterSkillSets;
        }

        // 행동 횟수에 따른 궁극기 사용을 위해 트리거를 가져온다
        _monsterAITrigger = transform.GetComponentInChildren<MonsterAI>();
    }



    // ------------------------------- 애니메이션 EventHandler ------------------------------------


    // 스킬 사용 시 히트 판정을 하는 시점
    public void OnSkillHit(int skillId)
    {
        // 공격 판정 시도
        hitColliders = Physics.OverlapSphere(
                transform.position + (transform.forward * 1),
                _skillSets[skillId - 1].SkillRange, _hittalbeMask); // Skill Id 1 은 기본공격, 2부터 스킬

        if (hitColliders.Length != 0) // 공격 명중 시 데미지 처리
        {
            // 플레이어 데미지 처리
            _monsterController.HandleDamage(skillId, hitColliders);
        }

        // 애니메이션의 재생 횟수를 기록하여 일정 횟수의 공격 후 궁극기 사용하도록 설정
        _monsterController._ultimateStack++; 
    }

    // 사운드 재생
    public void PlayAudio(string path)
    {
        Managers.Sound.Play("Creature/Monster/" + path);
    }

    // Inputable 변수 설정
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
