using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using System;
using UnityEngine;
//Factory that will assign AllCardTypes enum with their scriptableObject, this will allow us to take the scriptableObject from the AllCardType enum then instantiate that
public class CardSOFactory
{
    private static Dictionary<AllCards, Card> CardSODictionary;
    private static bool IsCardsInitialized => CardSODictionary != null;

    public static void InitializeCardSOFactory(ChosenPlayer playerPool, ChosenClass classPool)
    {
        //if effect factory is not yet initialized, proceed
        if (IsCardsInitialized)
        {
            return;
        }
        Debug.Log("Card SOs initializing");

        //scan through projects for all classes that are of type Card but not abstract
        //only children of Card wil be taken
        //var effectTypes = Assembly.GetAssembly(typeof(Card)).GetTypes()
        //    .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Card)));

        //Dictionary for getting the effects by enum
        CardSODictionary = new Dictionary<AllCards, Card>();

        //input Card types in dictionary with their AllCards enum as teir key
        //foreach (var type in effectTypes)
        //{
        //    //will ignore jigsaws and enemyActions sice they are derived from BaseCardEffect
        //    var tempCard = Activator.CreateInstance(type) as Card;
        //    CardSODictionary.Add(tempCard.enumCardName, type);
        //}

        Card[] playerCards = Resources.LoadAll<Card>($"CardSO/{playerPool}");
        Card[] classCards = Resources.LoadAll<Card>($"CardSO/{classPool}");
        Card[] neutralCards = Resources.LoadAll<Card>($"CardSO/Neutral");

        foreach (Card playerCard in playerCards)
        {
            CardSODictionary.Add(playerCard.enumCardName, playerCard);
        }

        foreach (Card playerCard in classCards)
        {
            CardSODictionary.Add(playerCard.enumCardName, playerCard);
        }

        foreach (Card playerCard in neutralCards)
        {
            CardSODictionary.Add(playerCard.enumCardName, playerCard);
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
            Card cardCopy = CardSODictionary[cardKey];
            return cardCopy;
        }
        return null;
    }
}
