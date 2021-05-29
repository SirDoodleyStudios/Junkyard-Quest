using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this factory is for getting the base SO script when loading from save using the enemyEnum
public static class EnemyUnitFactory
{
    private static Dictionary<EnemyEnumName, EnemyUnit> EnemySODictionary;
    private static bool isEnemiesInitialized => EnemySODictionary != null;

    public static void InitializeEnemyUnitFactory()
    {
        //if effect factory is not yet initialized, proceed
        if (isEnemiesInitialized)
        {
            return;
        }

        //Dictionary for getting the effects by enum
        EnemySODictionary = new Dictionary<EnemyEnumName, EnemyUnit>();

        //loads all enemyUnit SOs in resources and saves them here
        EnemyUnit[] enemyEnumNames = Resources.LoadAll<EnemyUnit>($"EnemyUnits");

        //assign enumName as key for getting corresponding base enemyUnit
        foreach(EnemyUnit enemyUnit in enemyEnumNames)
        {
            EnemySODictionary.Add(enemyUnit.enemyEnumName, enemyUnit);
        }
    }

    //when we call this, we can now get the base Scriptable object
    public static EnemyUnit GetEnemySO(EnemyEnumName enemyKey)
    {

        if (EnemySODictionary.ContainsKey(enemyKey))
        {
            EnemyUnit enemyCopy = EnemySODictionary[enemyKey];
            return enemyCopy;
        }
        return null;
    }
}
