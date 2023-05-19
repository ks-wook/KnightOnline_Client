using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat_player_data : ScriptableObject
{
    [SerializeField]
    private int id;
    public int ID { get { return id; } }

    public string PlayerName;
    // public Sprite ItemIcon;

    public int Attack;
    public int Deffense;
    public int MaxHp;

    public float VisualRange;

    // public GameObject ItemPrefab;
}
