using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public string cardName;

    public string effect;
    //public string leftEffect;
    //public string rightEffect;
    public string jigsawText;

    public int energyCost;
    //public int creativityCost;

    public Sprite artwork;
    public Sprite jigsawImage;

    //Which Cardpool is it
    public CardClass cardClass;

    //offense, utility, ability
    public CardType cardType;

    //
    public CardMethod cardMethod;


    //abstract enum for identifying keys in the dictionary for effect factory
    public AllCards enumCardName;

    public delegate void D_CardEffect();
    public D_CardEffect d_cardEffect;

    //none initially, must be added later
    public JigsawFormat jigsawEffect;

    //public BaseScriptJigAbi JigsawOrAbility;








}
