using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class CombatManager : MonoBehaviour
{

    CombatState state;
    //for thedropfield moving up approach targetting system
    //drop field for dropped cards
    //public GameObject dropField;
    //Vector3 originalDropFieldPosition;

    //related to player
    public GameObject Player;
    //list of enemies
    //public List<GameObject> enemyList = new List<GameObject>();
    public GameObject enemyHolder;
    //for creativeField
    public GameObject creativeUI;
    public GameObject creativeUnleash;
    List<GameObject> creativeList = new List<GameObject>();

    public DeckManager deckManager;
    public CreativeManager creativeManager;
    public PlayerHand playerHand;

    //related to energy
    int defaultEnergy = 3;
    int Energy;
    public Text energyText;

    //related to decks
    int defaultDraw = 5;
    int Draw;
    public Text deckText;
    public Text discardText;

    //card clicked during Player Turn phase
    GameObject activeCard;
    //cache that switches a card's state and interaction functions
    DragNDrop activeDragNDrop;


    //for mouse pointing and clicking
    Vector2 PointRay;
    Ray ray;
    RaycastHit2D pointedObject;
    RaycastHit2D pointedTarget;

    public void Start()
    {
        //for thedropfield moving up approach targetting system
        //originalDropFieldPosition = dropField.transform.localPosition;

        Energy = defaultEnergy;
        Draw = defaultDraw;
        DrawHand();
        EnergyUpdater();
        DeckUpdater();
        

    }

    public void Update()
    {

        PointRay = Input.mousePosition;
        ray = Camera.main.ScreenPointToRay(PointRay);
        pointedObject = Physics2D.GetRayIntersection(ray);
        //pointedTarget = Physics2D.GetRayIntersection(ray);
        //pointedTarget = Physics2D.GetRayIntersection(ray);

        if (Input.GetMouseButtonDown(2))
        {
            Debug.Log($"GameObject is {pointedObject.collider.gameObject.name}");
        }


        //For Choosing Cards to play
        if (state == CombatState.PlayerTurn)
        {

            //Test for drawing cards///////////////////////
            //if (Input.GetMouseButtonDown(2))
            //{
            //    state = CombatState.DrawPahase;
            //    deckManager.DrawCards(Draw);
            //    DeckUpdater();
            //    playerHand.StateChanger(state);
            //    state = CombatState.PlayerTurn;
            //    playerHand.StateChanger(state);
            //}
            ////////////////////////////////////


            //clicking cards during player phase
            //load delegate function effect of card
            if (Input.GetMouseButtonDown(0))
            {

                if (pointedObject.collider != null && pointedObject.collider.gameObject.tag == "Card" )
                {      

                    activeCard = pointedObject.collider.gameObject;
                    Card activeCardCard = activeCard.GetComponent<Display>().card;

                    //makes the dropzone move up so that becomes the target for drop cards
                    //for thedropfield moving up approach targetting system
                    //if (activeCardCard.cardMethod == CardMethod.Dropped)
                    //{                        
                    //    Vector3 dropFieldShift = dropField.transform.localPosition;
                    //    dropField.transform.localPosition = new Vector3(dropFieldShift.x, dropFieldShift.y, -1);
                    //}

                    //checks the scriptable attached in Display if cost can be accomodated by Energy
                    if (activeCardCard.energyCost <= Energy)
                    {
                        state = CombatState.ActiveCard;
                        //activeCard.GetComponent<DragNDrop>().StateChanger(state); /////////////////////
                        playerHand.StateChanger(state);
                        
                    }
                    else if(activeCard.gameObject.GetComponent<Display>().card.energyCost > Energy)
                    {
                        Debug.Log("Not enough Energy");
                    }                 
                    
                }

            }


            //Entering Creative Mode
            else if (Input.GetKeyDown(KeyCode.C))
            {
                state = CombatState.CreativeMode;
                
                creativeUI.SetActive(true);
            }

        }

        else if (state == CombatState.ActiveCard)
        {
            EffectLoader activeEffectLoader = activeCard.GetComponent<EffectLoader>();
            Card activeCardCard = activeCard.GetComponent<Display>().card;
            

            //Right-click is back function
            if (Input.GetMouseButtonDown(1))
            {
                state = CombatState.PlayerTurn;
                //activeCard.GetComponent<DragNDrop>().StateChanger(state); ///////////////////////////
                playerHand.StateChanger(state);
                playerHand.ResetOriginal();

                //for thedropfield moving up approach targetting system
                //if (activeCardCard.cardMethod == CardMethod.Dropped)
                //{
                //    //returns dropfield to back after activate of card
                //    dropField.transform.localPosition = originalDropFieldPosition;
                //}

            }

            //pass targetted object
            if (Input.GetMouseButtonDown(0))
            {

                GameObject targetObject = pointedObject.collider.gameObject;

                if (targetObject.tag == "Enemy" && activeCardCard.cardMethod == CardMethod.Targetted)
                {
                    activeEffectLoader.EffectLoaderActivate(targetObject);
                    Energy = Energy - activeCard.gameObject.GetComponent<Display>().card.energyCost;
                    EnergyUpdater();
                    //calls discard method and puts active card in discard pile
                    DiscardFromHand();
                    activeCard.transform.SetAsLastSibling();
                    //retuns to player turn phase
                    state = CombatState.PlayerTurn;
                    //activeCard.GetComponent<DragNDrop>().StateChanger(state);////////////////////
                    playerHand.StateChanger(state);
                    DeckUpdater();

                    //////just for testing unli attacks///
                    //state = CombatState.PlayerTurn;
                    //activeDragNDrop.StateChanger(state);
                    /////////////////////////////////////
                }
                //layer 13 is Playing Field
                else if (targetObject.layer == 13 && activeCardCard.cardMethod == CardMethod.Dropped)
                {
                    activeEffectLoader.EffectLoaderActivate(targetObject);
                    Energy = Energy - activeCard.gameObject.GetComponent<Display>().card.energyCost;
                    EnergyUpdater();
                    //calls discard method and puts active card in discard pile
                    DiscardFromHand();
                    activeCard.transform.SetAsLastSibling();
                    //retuns to player turn phase
                    state = CombatState.PlayerTurn;
                    //activeCard.GetComponent<DragNDrop>().StateChanger(state);////////////////////
                    playerHand.StateChanger(state);
                    DeckUpdater();

                    //for thedropfield moving up approach targetting system
                    ////returns dropfield to back after activate of card
                    //dropField.transform.localPosition = originalDropFieldPosition;
                }
                else
                {
                    Debug.Log("no target here");
                }
            }
        }


        else if (state == CombatState.CreativeMode)
        {           

            if (Input.GetMouseButtonDown(0))
            {
                if (pointedObject.collider != null && pointedObject.collider.gameObject.tag == "Card")
                {
                    activeCard = pointedObject.collider.gameObject;
                    Card activeCardCard = activeCard.GetComponent<Display>().card;

                    //checks the scriptable attached in Display if cost can be accomodated by Energy
                    //checks if current chosen card's input link matches the output of the previous card
                    if (activeCardCard.energyCost <= Energy && creativeManager.CheckLinkEligibility(activeCardCard) 
                                                            && activeCardCard.jigsawEffect != null
                                                            && creativeManager.creativityCost < Player.GetComponent<PlayerFunctions>().currCreativity)
                    {
                        //transfers the card in creative mode then disables the prefab                        
                        Energy -= activeCardCard.energyCost;
                        EnergyUpdater();

                        creativeManager.ChooseForCreative(activeCardCard);
                        creativeList.Add(activeCard);
                        //cardDictionary.Add(activeCard.transform.GetSiblingIndex(), activeCard);
                        activeCard.SetActive(false);
                    }
                    //else if
                    //{
                    //    creativeManager.MessagePrompt(Energy, Player.GetComponent<PlayerFunctions>().currCreativity, activeCardCard);
                    //}
                    else if (activeCardCard.energyCost > Energy)
                    {
                        creativeManager.MessagePrompt("Insufficient Energy");
                    }
                    else if (!creativeManager.CheckLinkEligibility(activeCardCard))
                    {
                        creativeManager.MessagePrompt("Jigsaw Links Doesn't Match");
                    }
                    else if (activeCardCard.jigsawEffect == null)
                    {
                        creativeManager.MessagePrompt("Card Has No Jigsaw");
                    }
                    else if (creativeManager.creativityCost >= Player.GetComponent<PlayerFunctions>().currCreativity)
                    {
                        creativeManager.MessagePrompt("Insufficient Creativity");
                    }

                }

            }
            //for executing the link
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                if (creativeList.Count == 0)
                {
                    creativeManager.MessagePrompt("No Card Selected");
                }
                else
                {
                    //activate like execute button
                    ExecuteUnleashButton();
                }


            }

            //Right-click is back function
            else if (Input.GetMouseButtonDown(1))
            {

                //returns dropfield to back after activate of card
                if (creativeList.Count > 0)
                {
                    int tempIndex = creativeManager.ReturnFromCreative();
                    //adds back energy when backing out
                    Energy += creativeList[tempIndex].GetComponent<Display>().card.energyCost;
                    EnergyUpdater();

                    creativeList[tempIndex].SetActive(true);
                    creativeList.Remove(creativeList[tempIndex]);
                    //removes scale increase on all cards
                    playerHand.ResetOriginal();
                }


            }

            //completely backs out from creative mode
            else if (Input.GetKeyDown(KeyCode.C))
            {
                foreach (GameObject card in creativeList)
                {
                    int tempIndex = creativeManager.ReturnFromCreative();
                    Energy += creativeList[tempIndex].GetComponent<Display>().card.energyCost;
                    EnergyUpdater();
                    creativeList[tempIndex].SetActive(true);
                }
                creativeList.Clear();
                //removes scale increase on all cards
                playerHand.ResetOriginal();
                state = CombatState.PlayerTurn;
                creativeUI.SetActive(false);
            }
        }

        //when picking target for unleasing creativity
        else if (state == CombatState.UnleashCreativity)
        {
            //lift click in targetting during unleash
            if (Input.GetMouseButtonDown(0))
            {
                GameObject targetObject = pointedObject.collider.gameObject;
                //this method takes the cardMethod of linked cards
                //if there is a targetted card, the linked are all targetted, it is dropped if there are no targetted cards
                CardMethod linkMethod = creativeManager.FinalizeLinks();
                //if targetted, click will work one specific enemy units only
                if (pointedObject.collider != null && linkMethod == CardMethod.Targetted && targetObject.tag == "Enemy" )
                {
                    //access player stats and reduces their creativity meter
                    Player.GetComponent<PlayerFunctions>().ReduceCreativity(creativeManager.creativityCost);
                    //initiates link effects in CreativeManager
                    //returns the cost for crativity
                    creativeManager.UnleashCreativity(targetObject);

                }
                //if dropped, click will wor on anything
                else if (pointedObject.collider != null && linkMethod == CardMethod.Dropped && targetObject.layer == 13 )
                {
                    //access player stats and reduces their creativity meter
                    Player.GetComponent<PlayerFunctions>().ReduceCreativity(creativeManager.creativityCost);
                    //initiates link effects in CreativeManager
                    //returns the cost for crativity
                    creativeManager.UnleashCreativity(targetObject);
                    
                }
                //removes scale increase on all cards
                playerHand.ResetOriginal();                
                //Discards cards used in creative mode
                foreach(GameObject linkedCard in creativeList)
                {
                    activeCard = linkedCard;
                    //calls discard method and puts active card in discard pile
                    DiscardFromHand();
                }
                //clearing CardObjects 
                creativeList.Clear();
                state = CombatState.PlayerTurn;
                creativeUnleash.SetActive(false);
                //playerHand.StateChanger(state); -- not yet sure
                DeckUpdater();

            }
            //back button, goes back to last state of creativity panel and re-enables it
            else if (Input.GetMouseButtonDown(1))
            {
                state = CombatState.CreativeMode;
                creativeUI.SetActive(true);
                creativeUnleash.SetActive(false);
            }
        }
        
        //just for testing enemy phases/////
        //else if(state == CombatState.EnemyTurn)
        //{
        //    if(Input.GetKeyDown(KeyCode.Backspace))
        //    {
        //        state = CombatState.PlayerTurn;
        //        Energy = defaultEnergy;
        //        EnergyUpdater();
        //    }
        //}
        ///////////////////////////////
    }

    //enters creative mode and shows creative mode panel
    public void CreativeModeButton()
    {
        //can only enter  CreativeMode during PlayerTurn
        if(state == CombatState.PlayerTurn)
        {
            state = CombatState.CreativeMode;
            creativeUI.SetActive(true);
        }

    }

    //
    public void ExecuteUnleashButton()
    {

        state = CombatState.UnleashCreativity;
        //removes ccreative panel UI
        creativeUI.SetActive(false);
        //activates targetting UI
        creativeUnleash.SetActive(true);




    }

    //calls deck manager to transfer card from hand to discard pile
    //aslo disables prefab of Card object
    public void DiscardFromHand()
    {
        Card activeCardCard = activeCard.GetComponent<Display>().card;
        deckManager.DiscardCards(activeCardCard);
        activeCard.SetActive(false);
        
    }

    //function for drawing during start of turn
    public void DrawHand()
    {
        state = CombatState.DrawPahase;
        deckManager.DrawCards(Draw);
        DeckUpdater();
        playerHand.StateChanger(state);
        state = CombatState.PlayerTurn;
        playerHand.StateChanger(state);
    }

    //updates energy number
    public void EnergyUpdater()
    {
        energyText.text = Energy.ToString();
    }

    //updates deck and discardpile numbers
    public void DeckUpdater()
    {
        int deckCardsCount = deckManager.deckCount;
        int discardCardsCount = deckManager.discardCount;
        deckText.text = deckCardsCount.ToString();
        discardText.text = discardCardsCount.ToString();

    }



    //Ending turn to enemy action to new player phase
    public void EndTurnButton()
    {
        foreach(Transform cardInHand in playerHand.transform)
        {
            if(cardInHand.gameObject.activeSelf == true)
            {
                activeCard = cardInHand.gameObject;
                DiscardFromHand();
            }
           
        }

        DeckUpdater();
        state = CombatState.EnemyTurn;
        //Activate Enemy Actions from enemylist
        StartCoroutine(ProceedEnemyActions());
    }

    public IEnumerator ProceedEnemyActions()
    {
        //foreach(GameObject enemy in enemyList)
        //{
        //    enemy.GetComponent<BasicEnemy>().EnemyAct();
        //    yield return new WaitForSeconds(.2f);
        //}

        foreach(Transform enemy in enemyHolder.transform)
        {
            enemy.gameObject.GetComponent<BasicEnemy>().EnemyAct();
            yield return new WaitForSeconds(.5f);
        }

        Player.GetComponent<BaseUnitFunctions>().RemoveBlock();
        Debug.Log("Losing block");
        yield return new WaitForSeconds(1f);
        state = CombatState.PlayerTurn;
        Energy = defaultEnergy;
        EnergyUpdater();
        DrawHand();
    }

}
//add some new effects