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
    //count on how many overkills are there already
    public int overKillCount;
    //this is checked by ccombatManager in victoryFunction
    //original number of enemies at the very beginning of combat
    //also used to count down for determining if victory was achieved
    //saved in CombatSaveState as the counter and not the actual original enemycount
    public int enemyCounter;

    //bool indicator that tells us if the first turn loaded is from file
    //if so, don't use the generic method but load details from file
    //should always be initially false
    bool isFirstTurnLoadOfFile;

    private void Start()
    {
        ////counts first how many enemies are enabled
        //foreach (Transform enemy in transform)
        //{
        //    if (enemy.gameObject.activeSelf)
        //    {
        //        enemyDeathCounter++;
        //    }
        //}
        ////assigns the original count of enemies at the very beginning
        //enemyCount = enemyDeathCounter;
    }

    //pseudo start called by combat manager
    //if a file is loaded, the enemyCOunt is the saved enemyCounter from last session
    //if fresh, the enemyCount is from the unversalInfo
    public void PseudoStartCombat(int enemyCount)
    {
        enemyCounter = enemyCount;
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
        enemyCounter--;
        if (enemyCounter == 0)
        {
            //int parameter will let the save know how many overkills were done
            combatManager.VictoryFunction();
        }


        //int enemyCount = 0;
        //foreach (Transform enemy in transform)
        //{
        //    if (enemy.gameObject.activeSelf)
        //    {
        //        enemyCount++;
        //    }
        //}
        //if(enemyCount == 0)
        //{
        //    combatManager.VictoryFunction();
        //}


    }



}
