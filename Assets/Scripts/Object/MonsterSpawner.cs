using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ���� ������ ������ �����ϴ� ��ũ��Ʈ�̴�.
 * ���͸� ���� ��Ű�� ����� 2������ �ϳ��� Collider�� ���� �� ��ȯ �ϰų�
 * �ٸ� ��Ϳ� ���� ������ ��ȯ�ϴ� ������� �����Ѵ�.
 * 
 * ������ ���ʹ� ü���� �ٵǾ� �ױ� ������ �����Ǹ� Monster Count�� �����ִٸ� ���� �ڸ���
 * �ٽ� ���͸� ��ȯ�Ѵ�.
 */


public class MonsterSpawner : InteractableObject
{
    [SerializeField]
    [Tooltip("��ȯ�� ���� ����")]
    Define.MonsterType monsterType;


    [SerializeField]
    [Tooltip("��ȯ�� ������ ���� ��")]
    float MonsterCount = 1;

    [SerializeField]
    [Tooltip("���� �������� ����, ���� ������ ��� ����� �������� Ŭ���� ó��")]
    bool isBoss = false;

    [HideInInspector]
    public Gimmick RelatedGimmic;

    SphereCollider _spawnerCollider; // �÷��̾� ���� ���� Collider
    ParticleSystem _spawnerEffect; // ���� ���� �� ����Ʈ 
    Vector3 _spawnPosition; // ���� ���� (�����ʰ� ��ġ�� ������ ��ȯ)
    public bool Spawnable = true; // ������ ��ȯ�� ���Ͱ� �ִ� ��� ��ȯ �Ұ� -> �ش� �������� ���� ��� �� �ٽ� ��ȯ ����




    // ȣ�� �� ������ ���͸� ��ȯ�ϴ� �Լ�
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

            Debug.Log($"{monsterInfo.Name} ��ȯ");

            // ���� ����
            GameObject go = Managers.Object.Add(monsterInfo, _spawnPosition, new Quaternion(0, 0, 0, 0));
            if(RelatedGimmic != null)
            {
                MonsterController mc;
                go.TryGetComponent<MonsterController>(out mc);
                if (mc != null)
                    mc.RelatedGimmic = this.RelatedGimmic;
            }

            // ���� ��ȯ �� ����Ʈ�� ���
            _spawnerEffect.gameObject.SetActive(true);
            _spawnerEffect.Play();

            // ���� ��ȯ Ƚ�� ����
            MonsterCount--;
        }
    }

    void Awake()
    {
        if (transform.TryGetComponent<SphereCollider>(out _spawnerCollider)) // Collider ȹ��
        {
            transform.Find("SpawnEffect").TryGetComponent<ParticleSystem>(out _spawnerEffect);

            _spawnPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
        else
        {
            Debug.Log("Spawner Trigger ȹ�� ����");
        }

    }

    // �÷��̾� ���� �� ��ȯ
    private void OnTriggerEnter(Collider other) 
    { 
        if(other.gameObject.CompareTag("Player"))
        {
            InterAct();
        }
    }





    // =========================== ���߿� �ڵ� ==================================
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
