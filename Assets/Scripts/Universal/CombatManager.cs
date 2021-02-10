using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class CombatManager : MonoBehaviour
{
    public delegate void D_StartTurn();
    public event D_StartTurn d_StartTurn;

    CombatState state;
    //for thedropfield moving up approach targetting system
    //drop field for dropped cards
    //public GameObject dropField;
    //Vector3 originalDropFieldPosition;

    //related to player
    public GameObject player;
    public PlayerFunctions playerFunctions;
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
    int defaultEnergy;
    //Energy gets accessed by playingField
    public int Energy;
    public Text energyText;

    //related to decks
    //int defaultDraw = 5;
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
        Debug.Log("Starting");

        //initial caching of player stats
        playerFunctions = player.GetComponent<PlayerFunctions>();
        
        //for just copying the default energy and draws from playerFunctions
        //defaultEnergy = player.GetComponent<PlayerFunctions>().defaultEnergy;
        //Draw = player.GetComponent<PlayerFunctions>().defaultDraw;

        //DrawHand();
        ////sends default energy to updater
        //EnergyUpdater(defaultEnergy);
        //DeckUpdater();

        d_StartTurn += StartTurn;
        d_StartTurn += enemyHolder.GetComponent<EnemyAIManager>().EnemyStart;
        //d_StartTurn += Player.GetComponent<AbilityManager>().EnableAbilities;
        //d_StartTurn += Player.GetComponent<PlayerFunctions>().AlterPlayerCreativity;
        //d_StartTurn += playerFunctions.StartTurnUpdates;
        d_StartTurn();
        

    }

    public void StartTurn()
    {
        state = CombatState.PlayerTurn;

        //Energy = 0;
        //EnergyUpdater(defaultEnergy);

        playerFunctions.currEnergy = 0;
        EnergyUpdater(playerFunctions.defaultEnergy);
        DrawHand();
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
                    //if (activeCardCard.energyCost <= Energy)
                    if (activeCardCard.energyCost <= playerFunctions.currEnergy)
                    {
                        state = CombatState.ActiveCard;
                        //activeCard.GetComponent<DragNDrop>().StateChanger(state); /////////////////////
                        playerHand.StateChanger(state);
                        
                    }
                    //else if(activeCard.gameObject.GetComponent<Display>().card.energyCost > Energy)
                    else if (activeCard.gameObject.GetComponent<Display>().card.energyCost > playerFunctions.currEnergy)
                    {
                        Debug.Log("Not enough Energy");
                    }
                    
                    else if(pointedObject.collider != null && pointedObject.collider.gameObject.tag == "Card")
                    {
                        Debug.Log($"GameObject is {pointedObject.collider.gameObject.name}");
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
                    
                    //activeEffectLoader.EffectLoaderActivate(targetObject);
                    activeEffectLoader.ActivateCardEffect(targetObject);
                    //Energy = Energy - activeCard.gameObject.GetComponent<Display>().card.energyCost;
                    EnergyUpdater(-activeCard.gameObject.GetComponent<Display>().card.energyCost);
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
                //if its utility or offense dropped
                else if (targetObject.layer == 13 && activeCardCard.cardMethod == CardMethod.Dropped && activeCardCard.cardType != CardType.Ability)
                {
                   
                    //activeEffectLoader.EffectLoaderActivate(targetObject);
                    activeEffectLoader.ActivateCardEffect(targetObject);
                    //Energy = Energy - activeCard.gameObject.GetComponent<Display>().card.energyCost;
                    EnergyUpdater(-activeCard.gameObject.GetComponent<Display>().card.energyCost);
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
                //if card is ablity, target object will always be player
                else if (targetObject.layer == 13 && activeCardCard.cardMethod == CardMethod.Dropped && activeCardCard.cardType == CardType.Ability)
                {
                    activeEffectLoader.EffectLoaderActivate(player);
                    //Energy = Energy - activeCard.gameObject.GetComponent<Display>().card.energyCost;
                    EnergyUpdater(-activeCard.gameObject.GetComponent<Display>().card.energyCost);
                    //calls discard method and puts active card in discard pile
                    DiscardFromHand();
                    activeCard.transform.SetAsLastSibling();
                    //retuns to player turn phase
                    state = CombatState.PlayerTurn;
                    //activeCard.GetComponent<DragNDrop>().StateChanger(state);////////////////////
                    playerHand.StateChanger(state);
                    DeckUpdater();
                }
                else
                {
                    Debug.Log("no target here");
                }
            }
        }


        else if (state == CombatState.CreativeMode)
        {           
            //for choosing cards to go in creative mode
            if (Input.GetMouseButtonDown(0))
            {
                if (pointedObject.collider != null && pointedObject.collider.gameObject.tag == "Card")
                {
                    activeCard = pointedObject.collider.gameObject;
                    Card activeCardCard = activeCard.GetComponent<Display>().card;

                    //checks the scriptable attached in Display if cost can be accomodated by Energy
                    //checks if current chosen card's input link matches the output of the previous card
                    if (activeCardCard.energyCost <= playerFunctions.currEnergy // activeCardCard.energyCost <= playerFunctions.currEnergy 
                        && creativeManager.CheckLinkEligibility(activeCardCard) 
                        && activeCardCard.jigsawEffect != null  //jigsawEffect
                        && creativeManager.creativityCost < player.GetComponent<PlayerFunctions>().currCreativity)
                    {
                        //transfers the card in creative mode then disables the prefab                        
                        //Energy -= activeCardCard.energyCost;
                        EnergyUpdater(-activeCardCard.energyCost);

                        creativeManager.ChooseForCreative(activeCardCard);
                        creativeList.Add(activeCard);
                        //cardDictionary.Add(activeCard.transform.GetSiblingIndex(), activeCard);
                        activeCard.SetActive(false);
                    }
                    //else if (activeCardCard.energyCost > playerFunctions.currEnergy)
                    else if (activeCardCard.energyCost > playerFunctions.currEnergy)
                    {
                        creativeManager.MessagePrompt("Insufficient Energy");
                    }
                    else if (!creativeManager.CheckLinkEligibility(activeCardCard))
                    {
                        creativeManager.MessagePrompt("Jigsaw Links Doesn't Match");
                    }
                    else if (activeCardCard.jigsawEffect == null) //jigsawEffect
                    {
                        creativeManager.MessagePrompt("Card Has No Jigsaw");
                    }
                    else if (creativeManager.creativityCost >= player.GetComponent<PlayerFunctions>().currCreativity)
                    {
                        creativeManager.MessagePrompt("Insufficient Creativity");
                    }

                }

            }
            //for executing the link
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                //prevents single cards for using creative mode
                if (creativeList.Count <= 1)
                {
                    Debug.Log("got here");
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
                    //Energy += creativeList[tempIndex].GetComponent<Display>().card.energyCost;
                    EnergyUpdater(creativeList[tempIndex].GetComponent<Display>().card.energyCost);

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
                    //Energy += creativeList[tempIndex].GetComponent<Display>().card.energyCost;
                    EnergyUpdater(creativeList[tempIndex].GetComponent<Display>().card.energyCost);
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
                    player.GetComponent<PlayerFunctions>().AlterPlayerCreativity(-creativeManager.creativityCost);
                    //initiates link effects in CreativeManager
                    //returns the cost for crativity
                    creativeManager.UnleashCreativity(targetObject);

                }
                //if dropped, click will wor on anything
                else if (pointedObject.collider != null && linkMethod == CardMethod.Dropped && targetObject.layer == 13 )
                {
                    //access player stats and reduces their creativity meter
                    player.GetComponent<PlayerFunctions>().AlterPlayerCreativity(-creativeManager.creativityCost);
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
        //state = CombatState.DrawPahase;
        //deckManager.DrawCards(Draw);
        //DeckUpdater();
        //playerHand.StateChanger(state);
        //state = CombatState.PlayerTurn;
        //playerHand.StateChanger(state);

        state = CombatState.DrawPahase;
        deckManager.DrawCards(playerFunctions.defaultDraw);
        DeckUpdater();
        playerHand.StateChanger(state);
        state = CombatState.PlayerTurn;
        playerHand.StateChanger(state);
    }

    //updates energy number
    //expect to receive negative ints for costs and positive ints for gains
    public void EnergyUpdater(int value)
    {
        //Energy += value;
        //energyText.text = Energy.ToString();

        //playerFunctions.currEnergy += value;
        playerFunctions.AlterEnergy(value);
        energyText.text = playerFunctions.currEnergy.ToString();
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
        //iterates through each card in hand and calls on disCardFromHand dor each
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
            //enemy.gameObject.GetComponent<EnemyActionsLogic>().EnemyAct();
            //yield return new WaitForSeconds(.5f);
            EnemyFunctions enemyFunctions = enemy.gameObject.GetComponent<EnemyFunctions>();
            enemyFunctions.EnemyAct();
            yield return new WaitForSeconds(.5f);
        }

        //player.GetComponent<BaseUnitFunctions>().RemoveBlock();
        Debug.Log("Losing block");
        yield return new WaitForSeconds(1f);
        //delegate for startTurn Event
        d_StartTurn();
    }

}
//add some new effects