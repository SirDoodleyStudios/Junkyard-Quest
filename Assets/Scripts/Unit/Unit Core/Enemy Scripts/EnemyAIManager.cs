using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAIManager : MonoBehaviour
{
    public Button EndTurnButton;

    //referenced from editor
    public CombatManager combatManager;

    //count on how many enemies there are in combat, to be used in RegisterEnemyKill
    public int enemyCount;

    private void Start()
    {
        //counts first how many enemies are enabled
        foreach (Transform enemy in transform)
        {
            if (enemy.gameObject.activeSelf)
            {
                enemyCount++;
            }
        }
    }

    public void EnemyStart()
    {
        
        foreach(Transform enemy in gameObject.transform)
        {
            EnemyFunctions tempenemy = enemy.gameObject.GetComponent<EnemyFunctions>();
            tempenemy.EnemyPrepare();
        }
    }

    public void RegisterEnemyKill()
    {
        //reduce enemycount then if count is 0, start Victory
        enemyCount--;
        if (enemyCount == 0)
        {
            combatManager.VictoryFunction();
        }
    }



}
