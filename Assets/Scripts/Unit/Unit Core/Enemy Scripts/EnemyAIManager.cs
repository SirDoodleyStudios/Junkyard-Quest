using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAIManager : MonoBehaviour
{
    public Button EndTurnButton;

    public void EnemyStart()
    {
        
        foreach(Transform enemy in gameObject.transform)
        {
            EnemyFunctions tempenemy = enemy.gameObject.GetComponent<EnemyFunctions>();
            tempenemy.EnemyPrepare();
        }
    }




}
