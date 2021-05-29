using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPools : MonoBehaviour
{
    //Holders of enemy Lists
    public List<List<EnemyUnit>> T1_Roster = new List<List<EnemyUnit>>();
    public List<List<EnemyUnit>> T2_Roster = new List<List<EnemyUnit>>();

    //actual roster of enemies that will hold the enemyUnits
    public List<EnemyUnit> T1_SpearieDuo = new List<EnemyUnit>();
    public List<EnemyUnit> T2_SpearieGang = new List<EnemyUnit>();

    private void Start()
    {
        T1_Roster.Add(T1_SpearieDuo);

        T2_Roster.Add(T2_SpearieGang);
    }

    //method to fetch availale enemy spawns, will depend on how many nodes and links have been traversed
    public List<EnemyUnit> GetEnemySpawn(int nodeCount)
    {
        //THIS IS AN EXPERIMENTAL FORMULA, WILL UPDATE ONCE THE ENEMY DESIGN IS EXPANDED UPON
        if (nodeCount >= 3)
        {
            int tempIndex = Random.Range(0, T2_Roster.Count - 1);
            return T2_Roster[tempIndex];
        }
        //lowest condition is T1
        else
        {
            int tempIndex = Random.Range(0, T1_Roster.Count - 1);
            return T1_Roster[tempIndex];
        }
    }

}
