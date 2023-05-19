using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatData_player : MonoBehaviour
{
    public Stat_player_data stat_player_data;

    [SerializeField]
    public int Level = 1;

    private int Attack;
    public int ATTACK { get { return Attack; } }

    private int Deffense;
    public int DEFFENSE { get { return Deffense; } }

    private int MaxHp;
    public int MAXHP { get { return MaxHp; } }

    private int Hp;
    public int HP { get { return Hp; } }

    private int Id;
    public int ID { get { return Id; } }

    private void Start()
    {
        Attack = stat_player_data.Attack + Level;
        Deffense = stat_player_data.Deffense + Level;
        MaxHp = stat_player_data.MaxHp + Level;
        Hp = MaxHp;
        Id = stat_player_data.ID;
    }

    public void GetDemage(int demage)
    {
        Hp = -demage;
    }
}
