using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * �������� ���� �����ʸ� �ڽ����� �Ͽ�
 * �� �ڽ� �����ʵ��� �����ϴ� ��ũ��Ʈ�̴�.
 * 
 * �������� �����ʸ� ����Ͽ� �����ϰ� ���� ��� ����Ѵ�.
 */


public class MonsterSpawnerController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("�÷��̾� ���� �� ���͸� ��ȯ�ϴ� �Ÿ� (Ʈ���ŷ� �����ϴ� ��츸 ��ȿ�� ��)")]
    float TriggerRadius;

    [SerializeField]
    [Tooltip("������ ��� (���� ��� null)")]
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

        // Collider ���� ���� �ʱ�ȭ
        SphereCollider sphereCollider;
        TryGetComponent<SphereCollider>(out sphereCollider);
        sphereCollider.radius = TriggerRadius;
    }

    // �ڽ����� �ִ� ��� �����ʵ� ���� ���
    void AllSpawn()
    {
        foreach(MonsterSpawner spawner in _spawners)
        {
            spawner.Spawn();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")) // �÷��̾��� ��� ���� ��ȯ
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
