using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 몬스터 스포너 동작을 제어하는 스크립트이다.
 * 몬스터를 스폰 시키는 방식은 2가지로 하나는 Collider에 접촉 시 소환 하거나
 * 다른 기믹에 의해 강제로 소환하는 방식으로 동작한다.
 * 
 * 스폰된 몬스터는 체력이 다되어 죽기 전까지 유지되며 Monster Count가 남아있다면 같은 자리에
 * 다시 몬스터를 소환한다.
 */


public class MonsterSpawner : InteractableObject
{
    [SerializeField]
    [Tooltip("소환할 몬스터 종류")]
    Define.MonsterType monsterType;


    [SerializeField]
    [Tooltip("소환할 몬스터의 마리 수")]
    float MonsterCount = 1;

    [SerializeField]
    [Tooltip("보스 몬스터인지 여부, 보스 몬스터의 경우 사망시 스테이지 클리어 처리")]
    bool isBoss = false;

    [HideInInspector]
    public Gimmick RelatedGimmic;

    SphereCollider _spawnerCollider; // 플레이어 접근 감지 Collider
    ParticleSystem _spawnerEffect; // 몬스터 스폰 시 이펙트 
    Vector3 _spawnPosition; // 스폰 지점 (스포너가 위치한 지점에 소환)
    public bool Spawnable = true; // 이전에 소환한 몬스터가 있는 경우 소환 불가 -> 해당 스포너의 몬스터 사망 후 다시 소환 가능




    // 호출 시 지정된 몬스터를 소환하는 함수
    public void Spawn()
    {
        if (MonsterCount > 0)
        {
            Spawnable = false;
            ObjectInfo monsterInfo = new ObjectInfo()
            {
                ObjectId = Managers.Object.GenerateId(GameObjectType.Monster),
                Name = "Monster_" + Enum.GetName(typeof(Define.MonsterType), monsterType)
            };

            Debug.Log($"{monsterInfo.Name} 소환");

            // 몬스터 스폰
            GameObject go = Managers.Object.Add(monsterInfo, _spawnPosition, new Quaternion(0, 0, 0, 0));
            if(RelatedGimmic != null)
            {
                MonsterController mc;
                go.TryGetComponent<MonsterController>(out mc);
                if (mc != null)
                    mc.RelatedGimmic = this.RelatedGimmic;
            }

            // 몬스터 소환 시 이펙트를 재생
            _spawnerEffect.gameObject.SetActive(true);
            _spawnerEffect.Play();

            // 몬스터 소환 횟수 차감
            MonsterCount--;
        }
    }

    void Awake()
    {
        if (transform.TryGetComponent<SphereCollider>(out _spawnerCollider)) // Collider 획득
        {
            transform.Find("SpawnEffect").TryGetComponent<ParticleSystem>(out _spawnerEffect);

            _spawnPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
        else
        {
            Debug.Log("Spawner Trigger 획득 실패");
        }

    }

    // 플레이어 접근 시 소환
    private void OnTriggerEnter(Collider other) 
    { 
        if(other.gameObject.CompareTag("Player"))
        {
            InterAct();
        }
    }





    // =========================== 개발용 코드 ==================================
    private void OnDrawGizmos()
    {
        Color c = new Color(2f, 1f, 1f, 0.5f);
        Gizmos.color = c;
        Gizmos.DrawSphere(transform.position, 3.0f);
    }

    public override void InterAct()
    {
        Spawn();
    }
}
