using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreativeManager : MonoBehaviour
{
    CombatState state;
    CardMethod linkMethod;

    public CombatManager combatManager;

    int curentEnergy;
    int currentCreativity;
    int totalEnergyCost;
    public int creativityCost;
    //will contain enabled card prefabs under creative field
    List<GameObject> cardPrefabList = new List<GameObject>();
    //will contain Cards for each enabled prefab under creative field
    List<Card> creativeList = new List<Card>();

    //updates texts
    public GameObject creativePanelUI;
    public Text creativePrompt;
    public Text creativityCostText;
    
    Card card;

    private void OnEnable()
    {
        //creativePanelUI.SetActive(true);
        //totalEnergyCost = 0;
        //totalCreativityCost = 0;

    }
    private void OnDisable()
    {
        //creativePanelUI.SetActive(false);
        //totalEnergyCost = 0;
        //totalCreativityCost = 0;
    }

    //used to check if previous jigsaw matches the current
    public bool CheckLinkEligibility(Card chosenCard)
    {
        //logic will always run if creatielist is empty
        //original condition is ==0, changed it to >1 to preven single card links
        if (chosenCard.jigsawEffect != null)
        {
            if (cardPrefabList.Count == 0)
            {
                totalEnergyCost += chosenCard.energyCost;
                return true;
            }
            else if (chosenCard.jigsawEffect.inputLink == cardPrefabList[cardPrefabList.Count - 1].GetComponent<Display>().card.jigsawEffect.outputLink) // jigsawEffect
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //if reached here, there is no jigsaw
        return false;
        
    }

    public void ChooseForCreative(Card chosenCard)
    {
        //if not empty, will check if previous cards output link is equal to current's input
        if (CheckLinkEligibility(chosenCard))
        {
            foreach (Transform cardPrefab in gameObject.transform)
            {
                if (cardPrefab.transform.gameObject.activeSelf == false)
                {
                    //assigns the Card object in Display
                    cardPrefab.gameObject.GetComponent<Display>().card = chosenCard;
                    //assigns the Card object in EffectLoader
                    cardPrefab.gameObject.GetComponent<EffectLoader>().card = chosenCard;
                    cardPrefab.gameObject.SetActive(true);
                    cardPrefabList.Add(cardPrefab.gameObject);
                    //adds one creativity cost per link
                    if (cardPrefabList.Count > 1)
                    {
                        creativityCost++;
                    }
                    
                    creativityCostText.text = creativityCost.ToString();
                    break;
                }

            }
        }

    }

    //removes card from the link list and puts it back in player hand
    public int ReturnFromCreative()
    {
        //-1 because we need the index
        int tempCount = cardPrefabList.Count - 1;
        cardPrefabList[tempCount].SetActive(false);
        cardPrefabList.Remove(cardPrefabList[tempCount]);
        //decreases creativity cost after removing a link
        if (cardPrefabList.Count >= 1)
        {
            creativityCost--;
        }
        creativityCostText.text = creativityCost.ToString();
        return tempCount;
    }

    public void MessagePrompt(string message)
    {
        //int tempEnergy = playerEnergy - card.energyCost;
        //if (tempEnergy < 0)
        //{
        //    creativePrompt.text = "Insufficient Energy";
        //}
        //else if (!CheckLinkEligibility(card))
        //{
        //    Card prevLink = cardPrefabList[cardPrefabList.Count - 1].GetComponent<Display>().card;
        //    creativePrompt.text = $"Can't link {prevLink.jigsawEffect.outputLink} ouput with {card.jigsawEffect.inputLink} input";
        //}
        //else if (card.jigsawEffect == null)
        //{
        //    creativePrompt.text = "Card has no Jigsaw";
        //}
        //else if (playerCreativity <= creativityCost)
        //{
        //    creativePrompt.text = "Insufficient Creativity";
        //}
        creativePrompt.text = message;

    }

    //dictates how the unleash should be its targetted mode
    public CardMethod FinalizeLinks()
    {
        foreach (GameObject cardPrefab in cardPrefabList)
        {
            //if a card or jigsaw needs a target, the finalized link is a targetted
            Card cardScriptable = cardPrefab.GetComponent<Display>().card;
            if (cardScriptable.cardMethod == CardMethod.Targetted || cardScriptable.jigsawEffect.jigsawMethod == CardMethod.Targetted)
            {

                //returns targetted
                linkMethod = CardMethod.Targetted;
                return linkMethod;
            }
            else
            {
                linkMethod = CardMethod.Dropped;
                
            }            
        }
        //returns dropped
        return linkMethod;
    }
    //iterates through list of cardprefabs and activate their effects
    //returned int is creativity points to be subtracted to from player gauge
    public void UnleashCreativity(GameObject target, GameObject actor)
    {
        foreach(GameObject cardPrefab in cardPrefabList)
        {
            ////jigsaws will activate first before the cardeffect
            ////in the case of jigsaws later in the list index, it will activate the preceeding jigsaws first
            ////for example, 2nd jigsaw will activate the 1st's then it's own.... 3rd jigsaw will activate the 1st's then the second's the its own            
            //for (int i = 0; cardPrefabList.IndexOf(cardPrefab) >= i; i++)
            //{
            //    cardPrefabList[i].GetComponent<EffectLoader>().ActivateJigsawEffect(target, actor);
            //}
            ////the effect of the actual card will be activated last
            //cardPrefab.GetComponent<EffectLoader>().ActivateCardEffect(target, actor);

            //Transition of jigsaws as actual cards embedded in another card instead of a separate list of effects
            cardPrefab.GetComponent<EffectLoader>().ActivateJigsawEffect(target, actor);
            cardPrefab.GetComponent<EffectLoader>().ActivateCardEffect(target, actor);
        }

        foreach(GameObject cardPrefab in cardPrefabList)
        {
            //disables cardobjects in creativity panel
            cardPrefab.SetActive(false);
        }
        //stores cost for returning to combatmanager
        //int tempCreativityCost = creativityCost;
        //resets text cost
        creativityCost = 0;
        creativityCostText.text = creativityCost.ToString();
        //resets GameObject list
        cardPrefabList.Clear();
        //return tempCreativityCost;
    }


}
