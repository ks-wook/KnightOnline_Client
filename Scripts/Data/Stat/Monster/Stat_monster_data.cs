using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Monster Data")]
public class Stat_monster_data : ScriptableObject
{
    [SerializeField]
    private int id;
    public int ID { get { return id; } }

    public string MonsterName;
    // public Sprite ItemIcon;

    public int Attack;
    public int Deffense;
    public int MaxHp;

    public float VisualRange;

    // public GameObject ItemPrefab;
}
