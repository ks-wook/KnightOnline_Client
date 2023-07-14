using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * 여러개의 몬스터 스포너를 자식으로 하여
 * 그 자식 스포너들을 관리하는 스크립트이다.
 * 
 * 여러개의 스포너를 사용하여 관리하고 싶은 경우 사용한다.
 */


public class MonsterSpawnerController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("플레이어 접근 시 몬스터를 소환하는 거리 (트리거로 동작하는 경우만 유효한 값)")]
    float TriggerRadius;

    [SerializeField]
    [Tooltip("연관된 기믹 (없는 경우 null)")]
    Gimmick RelatedGimmic;


    List<MonsterSpawner> _spawners = new List<MonsterSpawner>();

    void Init()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            _spawners.Add(transform.GetChild(i).GetComponent<MonsterSpawner>());
            if(RelatedGimmic != null)
            {
                _spawners[i].RelatedGimmic = this.RelatedGimmic;
            }
        }

        // Collider 반응 범위 초기화
        SphereCollider sphereCollider;
        TryGetComponent<SphereCollider>(out sphereCollider);
        sphereCollider.radius = TriggerRadius;
    }

    // 자식으로 있는 모든 스포너들 스폰 명령
    void AllSpawn()
    {
        foreach(MonsterSpawner spawner in _spawners)
        {
            spawner.Spawn();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")) // 플레이어인 경우 몬스터 소환
        {
            AllSpawn();
        }
    }




    // ---------------------- Start ----------------------------
    private void Start()
    {
        Init();
    }

    // ---------------------------------------------------------
}
