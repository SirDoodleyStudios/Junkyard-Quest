using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

public static class EffectFactory 
{
    private static Dictionary<AllCards, Type> EffectDictionary;
    private static bool IsCardsInitialized => EffectDictionary != null;

    private static void InitializeCardFactory()
    {
        //if effect factory is not yet initialized, proceed
        if (IsCardsInitialized)
        {
            return;
        }

        //scan through projects for all classes that are of type Card but not abstract
        //only children of Card wil be taken
        var effectTypes = Assembly.GetAssembly(typeof(BaseCardEffect)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(BaseCardEffect)));

        //Dictionary for getting the effects by enum
        EffectDictionary = new Dictionary<AllCards, Type>();

        //input Card types in dictionary with their AllCards enum as teir key
        foreach (var type in effectTypes)
        {
            var tempEffect = Activator.CreateInstance(type) as BaseCardEffect;
            if (tempEffect.enumKeyCard != AllCards.Jigsaw && tempEffect.enumKeyCard != AllCards.Ability)
            {
                EffectDictionary.Add(tempEffect.enumKeyCard, type);
            }

        }

    }


    public static BaseCardEffect GetCardEffect (AllCards cardKey)
    {
        InitializeCardFactory();

        //does not activate effects that has Jigsaw key
        //JigsawFactory has the effect for that
        if (EffectDictionary.ContainsKey(cardKey) && cardKey!= AllCards.Jigsaw)
        {
            Type type = EffectDictionary[cardKey];
            var effect = Activator.CreateInstance(type) as BaseCardEffect;
            return effect;
        }
        return null;
    }


}


