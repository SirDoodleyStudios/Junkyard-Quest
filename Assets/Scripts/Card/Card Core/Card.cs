using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public string cardName;

    public string effectText;

    //commented out for testing, putting jigsaw text description in JigsawFormat////////////////////////
    //public string attachmentText;
    //public Sprite attachmentImage;

    public int energyCost;
    //public int creativityCost;

    public Sprite artwork;


    //Which Cardpool is it
    public CardClass cardClass;

    //offense, utility, ability
    public CardType cardType;

    //whether it's targetted or dropped
    public CardMethod cardMethod;


    //abstract enum for identifying keys in the dictionary for effect factory
    public AllCards enumCardName;

    public delegate void D_CardEffect();
    public D_CardEffect d_cardEffect;

    //none initially, must be added later
    //must only be added for offense and utility cards

    public JigsawFormat jigsawEffect;



    //must only be used for ability cardss
    public AbilityFormat abilityEffect;

    //Card Tags that act like statuses and must be considered even when not played
    public bool Keep;
    public bool Combo;

    //List of tags that the card have, only for displaying text during Hover
    public List<CardMechanics> cardTags = new List<CardMechanics>();








}
