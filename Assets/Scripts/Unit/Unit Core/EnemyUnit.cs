using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Unit", menuName = "Enemy Unit")]
public class EnemyUnit : Unit
{
    //in Base
    //public int HP;
    //public int Creativity;
    //public int draw;
    public List<EnemyActionFormat> actionList;

    public int normalDamage;
    public int smallDamage;
    public int bigDamage;

    public int normalBlock;
    public int smallBlock;
    public int bigBlock;


}