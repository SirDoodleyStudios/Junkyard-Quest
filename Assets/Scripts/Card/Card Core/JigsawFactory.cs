using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

public class JigsawFactory
{
    private static Dictionary<AllJigsaws, Type> JigsawDictionary;
    private static bool IsInitialized => JigsawDictionary != null;

    private static void InitializeFactory()
    {
        if (IsInitialized)
        {
            return;
        }

        //scan through projects for all classes that are of type Card but not abstract
        //only children of Card wil be taken
        var effectTypes = Assembly.GetAssembly(typeof(BaseJigsawEffect)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(BaseJigsawEffect)));

        //Dictionary for getting the effects by enum
        JigsawDictionary = new Dictionary<AllJigsaws, Type>();

        //input Card types in dictionary with their AllCards enum as teir key
        foreach (var type in effectTypes)
        {
            var tempEffect = Activator.CreateInstance(type) as BaseJigsawEffect;
            JigsawDictionary.Add(tempEffect.enumKeyJigsaw, type);
        }

    }

    public static BaseJigsawEffect GetJigsawEffect(AllJigsaws enumKey)
    {
        InitializeFactory();

        if (JigsawDictionary.ContainsKey(enumKey))
        {
            Type type = JigsawDictionary[enumKey];
            var effect = Activator.CreateInstance(type) as BaseJigsawEffect;
            return effect;
        }
        return null;
    }
}
