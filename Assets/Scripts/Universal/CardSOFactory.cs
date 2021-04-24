using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
//Factory that will assign AllCardTypes enum with their scriptableObject, this will allow us to take the scriptableObject from the AllCardType enum then instantiate that
public class CardSOFactory
{
    private static Dictionary<AllCards, Type> CardSODictionary;
    private static bool IsCardsInitialized => CardSODictionary != null;

    public static void InitializeCardFactory()
    {
        //if effect factory is not yet initialized, proceed
        if (IsCardsInitialized)
        {
            return;
        }

        //scan through projects for all classes that are of type Card but not abstract
        //only children of Card wil be taken
        var effectTypes = Assembly.GetAssembly(typeof(Card)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Card)));

        //Dictionary for getting the effects by enum
        CardSODictionary = new Dictionary<AllCards, Type>();

        //input Card types in dictionary with their AllCards enum as teir key
        foreach (var type in effectTypes)
        {
            //will ignore jigsaws and enemyActions sice they are derived from BaseCardEffect
            var tempCard = Activator.CreateInstance(type) as Card;
            CardSODictionary.Add(tempCard.enumCardName, type);
        }

    }

    //when we call this, we can now get the base Scriptable object
    public static Card GetCardSO(AllCards cardKey)
    {
        //tried putting Initialize trigger in CombatManager
        //InitializeCardFactory();

        //does not activate effects that has Jigsaw key
        //JigsawFactory has the effect for that
        if (CardSODictionary.ContainsKey(cardKey) && cardKey != AllCards.Jigsaw)
        {
            Type type = CardSODictionary[cardKey];
            var effect = Activator.CreateInstance(type) as Card;
            return effect;
        }
        return null;
    }
}
