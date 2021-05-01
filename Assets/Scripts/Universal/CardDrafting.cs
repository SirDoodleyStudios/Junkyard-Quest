using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDrafting : MonoBehaviour
{
    //the holder of choice cards
    public GameObject choiceHolder;
    GridLayoutGroup gridLayout;
    RectTransform parentCanvas;
    

    //reference to the Deck Directory Holder
    public DeckPools deckPoolDirectories;
    List<Card> poolCards = new List<Card>();
    private void Awake()
    {
        //first child is the holder of cards
        parentCanvas = transform.parent.GetComponent<RectTransform>();
        gridLayout = choiceHolder.GetComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(parentCanvas.rect.width * .13416815742f, parentCanvas.rect.height * .34280604133f);
        
    }

    public void InitializeDraftPool(ChosenPlayer playerKey, ChosenClass classKey)
    {
        poolCards.AddRange(deckPoolDirectories.GetClassPool(classKey));
        poolCards.AddRange(deckPoolDirectories.GetPlayerPool(playerKey));

    }

    //enables the Drafting Window
    public void StartCardDraft()
    {


        List<int> indices = new List<int>();
        for (int i = 0; indices.Count < 3; i++)
        {
            int tempInt = Random.Range(0, poolCards.Count - 1);
            if (!indices.Contains(tempInt))
            {
                indices.Add(tempInt);

                Display cardDisplay = choiceHolder.transform.GetChild(i).gameObject.GetComponent<Display>();
                cardDisplay.card = poolCards[tempInt];
            }
            else
            {
                i--;
            }
        }

        gameObject.SetActive(true);



    }
}
