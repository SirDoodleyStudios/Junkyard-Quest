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
        if (cardPrefabList.Count == 0)
        {
            totalEnergyCost += chosenCard.energyCost;
            return true;
        }

        else if(chosenCard.jigsawEffect.inputLink == cardPrefabList[cardPrefabList.Count - 1].GetComponent<Display>().card.jigsawEffect.outputLink)
        {
            return true;
        }
        else
        {
            return false;
        }
        
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
                    //adds one creativity cost per card in link
                    creativityCost++;
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
        //increases creativity cost after removing card
        creativityCost--;
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

    public CardMethod FinalizeLinks()
    {
        foreach (GameObject cardPrefab in cardPrefabList)
        {
            if (cardPrefab.GetComponent<Display>().card.cardMethod == CardMethod.Targetted)
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
    public void UnleashCreativity(GameObject target)
    {
        foreach(GameObject cardPrefab in cardPrefabList)
        {
            //jigsaws will activate first before the cardeffect
            //in the case of jigsaws later in the list index, it will activate the preceeding jigsaws first
            //for example, 2nd jigsaw will activate the 1st's then it's own.... 3rd jigsaw will activate the 1st's then the second's the its own
            
            for (int i = 0; cardPrefabList.IndexOf(cardPrefab) >= i; i++)
            {
                cardPrefabList[i].GetComponent<EffectLoader>().ActivateJigsawEffect(target);
            }
            //the effect of the actual card will be activated last
            cardPrefab.GetComponent<EffectLoader>().ActivateCardEffect(target);
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
