using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

public class AbilityFactory
{
    private static Dictionary<AllAbilities, Type> AbilityDictionary;
    private static bool IsInitialized => AbilityDictionary != null;

    private static void InitializeFactory()
    {
        if (IsInitialized)
        {
            return;
        }

        //scan through projects for all classes that are of type Card but not abstract
        //only children of Card wil be taken
        var effectTypes = Assembly.GetAssembly(typeof(BaseAbilityEffect)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(BaseAbilityEffect)));

        //Dictionary for getting the effects by enum
        AbilityDictionary = new Dictionary<AllAbilities, Type>();

        //input Card types in dictionary with their AllCards enum as teir key
        foreach (var type in effectTypes)
        {
            var tempEffect = Activator.CreateInstance(type) as BaseAbilityEffect;
            AbilityDictionary.Add(tempEffect.enumKeyAbility, type);
        }

    }

    public static BaseAbilityEffect GetAbilityEffect(AllAbilities enumKey)
    {
        InitializeFactory();

        if (AbilityDictionary.ContainsKey(enumKey))
        {
            Type type = AbilityDictionary[enumKey];
            var effect = Activator.CreateInstance(type) as BaseAbilityEffect;
            return effect;
        }
        return null;
    }
}
