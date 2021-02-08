using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

public class EnemyActionFactory
{
    private static Dictionary<AllEnemyActions, Type> JigsawDictionary;
    private static bool IsInitialized => JigsawDictionary != null;

    private static void InitializeFactory()
    {
        if (IsInitialized)
        {
            return;
        }

        //scan through projects for all classes that are of type Card but not abstract
        //only children of Card wil be taken
        var effectTypes = Assembly.GetAssembly(typeof(BaseEnemyActions)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(BaseEnemyActions)));

        //Dictionary for getting the effects by enum
        JigsawDictionary = new Dictionary<AllEnemyActions, Type>();

        //input Card types in dictionary with their AllCards enum as teir key
        foreach (var type in effectTypes)
        {
            var tempEffect = Activator.CreateInstance(type) as BaseEnemyActions;
            JigsawDictionary.Add(tempEffect.enumKeyEnemyAction, type);
        }

    }

    public static BaseEnemyActions GetEnemyActionEffect(AllEnemyActions enumKey)
    {
        InitializeFactory();

        if (JigsawDictionary.ContainsKey(enumKey))
        {
            Type type = JigsawDictionary[enumKey];
            var effect = Activator.CreateInstance(type) as BaseEnemyActions;
            return effect;
        }
        return null;
    }
}
