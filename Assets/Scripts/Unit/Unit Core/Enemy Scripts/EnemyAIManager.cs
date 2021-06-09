using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class EnemyAIManager : MonoBehaviour
{
    public Button EndTurnButton;

    //referenced from editor
    public CombatManager combatManager;

    //These ints need to be saved in combatSaveState
    //count on how many enemies there are in combat, to be used in RegisterEnemyKill
    public int enemyCount;
    //count on how many overkills are there already
    public int overKillCount;

    //bool indicator that tells us if the first turn loaded is from file
    //if so, don't use the generic method but load details from file
    //should always be initially false
    bool isFirstTurnLoadOfFile;

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
        //if first turn is loded fom file
        if (File.Exists(Application.persistentDataPath + "/Combat.json") && isFirstTurnLoadOfFile == false)
        {
            //load unitwrappers from the combat save file then send them to the enemyfunctions for loading
            CombatSaveState combatSaveState = UniversalSaveState.LoadCombatState();
            List<EnemyUnitStatsWrapper> enemyUnitWrappers = combatSaveState.enemyUnitWrappers;
            for (int i = 0; enemyUnitWrappers.Count - 1 >= i; i++)
            {
                EnemyFunctions tempenemy = transform.GetChild(i).gameObject.GetComponent<EnemyFunctions>();
                tempenemy.EnemyPrepareFromFileLoad(enemyUnitWrappers[i]); ;
            }
            isFirstTurnLoadOfFile = true;
        }
        //if combat file does not exist yet at first load or during normal consecutive turns
        else
        {
            foreach (Transform enemy in gameObject.transform)
            {
                EnemyFunctions tempenemy = enemy.gameObject.GetComponent<EnemyFunctions>();
                tempenemy.EnemyPrepare();
            }
            isFirstTurnLoadOfFile = true;
        }

        //foreach (Transform enemy in gameObject.transform)
        //{
        //    EnemyFunctions tempenemy = enemy.gameObject.GetComponent<EnemyFunctions>();
        //    tempenemy.EnemyPrepare();
        //}

    }

    //parameter enemyFunction is the actual enemy that was destroyed
    public void RegisterEnemyKill()
    {
        //generate creativity when killing 
        combatManager.playerFunctions.AlterPlayerCreativity(10);

        //reduce enemycount then if count is 0, start Victory
        enemyCount--;
        if (enemyCount == 0)
        {
            //int parameter will let the save know how many overkills were done
            combatManager.VictoryFunction(overKillCount);
        }
    }



}
