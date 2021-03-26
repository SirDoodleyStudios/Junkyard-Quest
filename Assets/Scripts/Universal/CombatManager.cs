﻿using System.Collections;
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
    TargetArrowHandler creativeUnleashArrow;
    List<GameObject> creativeList = new List<GameObject>();
    //playing field, automatically sent when a card is a dropped type and not an ability
    public GameObject playingField;

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
    public Text consumeText;

    //card clicked during Player Turn phase
    GameObject activeCard;
    //cache that switches a card's state and interaction functions
    DragNDrop activeDragNDrop;
    //references the EndTurn button to be disabled during enemyphase
    public Button EndTurnButt;
    //contains all card tag descriptions

    //for mouse pointing and clicking
    Vector2 PointRay;
    Ray ray;
    RaycastHit2D pointedObject;
    RaycastHit2D pointedTarget;
    //for arrow pointing dynamic position
    TargetArrowHandler targetArrowHandler;

    public void Awake()
    {
        //one time run to add dictionary entries of cardMechanic enums and text descriptions
        CardTagManager.InitializeTextDescriptionDictionaries();
        EffectFactory.InitializeCardFactory();

    }

    //Tried making this Awake(), destroyed some logic in EnemyFunctions
    public void Start()
    {
        Debug.Log("Starting");


        //initial caching of player stats
        playerFunctions = player.GetComponent<PlayerFunctions>();

        //initial caching of creativeUnleash object's arrow handler
        creativeUnleashArrow = creativeUnleash.GetComponent<TargetArrowHandler>();

        
        //for just copying the default energy and draws from playerFunctions
        //defaultEnergy = player.GetComponent<PlayerFunctions>().defaultEnergy;
        //Draw = player.GetComponent<PlayerFunctions>().defaultDraw;

        //DrawHand();
        ////sends default energy to updater
        //EnergyUpdater(defaultEnergy);
        //DeckUpdater();

        d_StartTurn += StartTurnInCombatManager;
        d_StartTurn += enemyHolder.GetComponent<EnemyAIManager>().EnemyStart;
        d_StartTurn += player.GetComponent<PlayerFunctions>().PlayerTurn;
        d_StartTurn += player.GetComponent<UnitStatusHolder>().StatusUpdateForNewTurn;
        //d_StartTurn += player.GetComponent<UnitStatusHolder>().TurnStatusUpdater;
        //d_StartTurn += player.GetComponent<UnitStatusHolder>().ConsumeTurnStackUpdate;
        foreach (Transform enemy in enemyHolder.transform)
        {
            d_StartTurn += enemy.GetComponent<UnitStatusHolder>().StatusUpdateForNewTurn;
            //d_StartTurn += enemy.gameObject.GetComponent<UnitStatusHolder>().TurnStatusUpdater;
            //d_StartTurn += enemy.GetComponent<UnitStatusHolder>().ConsumeTurnStackUpdate;
        }


        //d_StartTurn += Player.GetComponent<AbilityManager>().EnableAbilities;
        //d_StartTurn += Player.GetComponent<PlayerFunctions>().AlterPlayerCreativity;
        //d_StartTurn += playerFunctions.StartTurnUpdates;
        d_StartTurn();
        

    }

    //this is an event called everytime a player turn is started
    //called by d_StartTurn()
    public void StartTurnInCombatManager()
    {
        state = CombatState.PlayerTurn;

        //Energy = 0;
        //EnergyUpdater(defaultEnergy);

        playerFunctions.currEnergy = 0;
        EnergyUpdater(playerFunctions.defaultEnergy);
        DrawHand();
        //makes the endTurnButton interactable again during player turn
        EndTurnButt.interactable = true;


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
            //if (Input.GetMouseButtonUp(0))
            if (Input.GetMouseButtonDown(0))
            {

                if (pointedObject.collider != null && pointedObject.collider.gameObject.tag == "Card" )
                {
                    Debug.Log("getting here");

                    activeCard = pointedObject.collider.gameObject;
                    Card activeCardCard = activeCard.GetComponent<Display>().card;
                    //assigning targetArrowHandler in Object prefab
                    targetArrowHandler = activeCard.GetComponent<TargetArrowHandler>();

                    //checks the scriptable attached in Display if cost can be accomodated by Energy
                    //if (activeCardCard.energyCost <= Energy)
                    if (activeCardCard.energyCost <= playerFunctions.currEnergy)
                    {
                        state = CombatState.ActiveCard;
                        //activeCard.GetComponent<DragNDrop>().StateChanger(state); /////////////////////
                        playerHand.StateChanger(state);

                        //if card is targetted type, enable the dots and arrowhead holder in TargetArrowHandler script
                        if (activeCardCard.cardMethod == CardMethod.Targetted)
                        {
                            targetArrowHandler.EnableArrow();
                        }
                        
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
            DragNDrop activeCardDragNDrop = activeCard.GetComponent<DragNDrop>();
            Card activeCardCard = activeCard.GetComponent<Display>().card;

            ////////////        
            



            //if card is targettable, enable logic for dynamic arrow morphing
            //this is continuous
            if (activeCardCard.cardMethod == CardMethod.Targetted)
            {
                //sends mouse position to active card's target arrow handlker and calculates all of the dot's positions
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                targetArrowHandler.DynamicPositionArrow(mousePos);
            }
            else if (activeCardCard.cardMethod == CardMethod.Dropped)
            {
                activeCardDragNDrop.ActivateSingleClickDrag();
            }


            //Right-click is back function
            if (Input.GetMouseButtonDown(1))
            {
                state = CombatState.PlayerTurn;
                //activeCard.GetComponent<DragNDrop>().StateChanger(state); ///////////////////////////
                playerHand.StateChanger(state);
                playerHand.ResetToDeckPosition();

                if (activeCardCard.cardMethod == CardMethod.Targetted)
                {
                    targetArrowHandler.DisableArrow();
                }
                else if (activeCardCard.cardMethod == CardMethod.Dropped)
                {
                    activeCardDragNDrop.DeactivateDrag();
                }
                //resets sorting orders, box colliders, and positions
                activeCardDragNDrop.ResetSortingCanvasAndCollider();
            }

            //pass targetted object
            if (Input.GetMouseButtonUp(0) && pointedObject.collider!=null )
            {
                GameObject targetObject = pointedObject.collider.gameObject;

                if (targetObject.tag == "Enemy" && activeCardCard.cardMethod == CardMethod.Targetted)
                {

                    
                    //activeEffectLoader.EffectLoaderActivate(targetObject, player); ---- This is to be used if single jigsaws ccan be activated without creative mode
                    activeEffectLoader.ActivateCardEffect(targetObject, player);

                    //Energy = Energy - activeCard.gameObject.GetComponent<Display>().card.energyCost;
                    EnergyUpdater(-activeCard.gameObject.GetComponent<Display>().card.energyCost);
                    //calls discard method and puts active card in discard pile
                    //Set sibling as last must be called first before discarding because it messes with the card hand arangement logic
                    activeCard.transform.SetAsLastSibling();

                    //for disabling the targetting arrow
                    targetArrowHandler.DisableArrow();

                    //resets sorting orders, box colliders, and positions
                    activeCardDragNDrop.ResetSortingCanvasAndCollider();

                    //reset oiginal must come first before discarding, beccause overridesorting can only be set if object is active
                    playerHand.ResetToDeckPosition();
                    //consume a card if the Card has a Consume tag confirmed
                    if (activeCardCard.cardTags.Contains(CardMechanics.Consume) || activeCardCard.cardTags.Contains(CardMechanics.Ability))
                    {
                        //deckManager.ConsumeCards(activeCard);
                        StartCoroutine(deckManager.ConsumeCards(activeCard));
                    }
                    else
                    {
                        //deckManager.DiscardCards(activeCard);
                        StartCoroutine(deckManager.DiscardCards(activeCard));
                    }

                    //retuns to player turn phase
                    state = CombatState.PlayerTurn;
                    //activeCard.GetComponent<DragNDrop>().StateChanger(state);////////////////////
                    //playerHand.StateChanger(state);
                    DeckUpdater();




                    //////just for testing unli attacks///
                    //state = CombatState.PlayerTurn;
                    //activeDragNDrop.StateChanger(state);
                    /////////////////////////////////////
                }
                //layer 13 is Playing Field
                //if its utility or offense dropped
                //else if (targetObject.layer == 13 && activeCardCard.cardMethod == CardMethod.Dropped && activeCardCard.cardType != CardType.Ability)
                else if (activeCardCard.cardMethod == CardMethod.Dropped && activeCardCard.cardType != CardType.Ability)
                {
                   
                    //activeEffectLoader.EffectLoaderActivate(targetObject, player); ---- to be used if single jigsaw effect is to be able to activate without creative mode
                    //playing field target, player actor //////////this one is temporary
                    activeEffectLoader.ActivateCardEffect(targetObject, player);

                    //Energy = Energy - activeCard.gameObject.GetComponent<Display>().card.energyCost;
                    EnergyUpdater(-activeCard.gameObject.GetComponent<Display>().card.energyCost);
                    //calls discard method and puts active card in discard pile
                    //Set sibling as last must be called first before discarding because it messes with the card hand arangement logic
                    activeCard.transform.SetAsLastSibling();

                    //for stopping the drag
                    activeCardDragNDrop.DeactivateDrag();

                    //reset oiginal must come first before discarding, beccause overridesorting can only be set if object is active
                    //every time a card is played, reset it's scale down from
                    playerHand.ResetToDeckPosition();
                    //consume a card if the Card has a Consume tag confirmed
                    if (activeCardCard.cardTags.Contains(CardMechanics.Consume) || activeCardCard.cardTags.Contains(CardMechanics.Ability))
                    {
                        //deckManager.ConsumeCards(activeCard);
                        StartCoroutine(deckManager.ConsumeCards(activeCard));
                    }
                    else
                    {
                        //deckManager.DiscardCards(activeCard);
                        StartCoroutine(deckManager.DiscardCards(activeCard));
                    }

                    //retuns to player turn phase
                    state = CombatState.PlayerTurn;
                    //activeCard.GetComponent<DragNDrop>().StateChanger(state);////////////////////
                    //playerHand.StateChanger(state);
                    //resets sorting orders, box colliders, and positions
                    //activeCardDragNDrop.ResetSortingCanvasAndCollider();
                    DeckUpdater();


                    //for thedropfield moving up approach targetting system
                    ////returns dropfield to back after activate of card
                    //dropField.transform.localPosition = originalDropFieldPosition;
                }
                //if card is ablity, target object will always be player
                //else if (targetObject.layer == 13 && activeCardCard.cardMethod == CardMethod.Dropped && activeCardCard.cardType == CardType.Ability)
                else if (activeCardCard.cardMethod == CardMethod.Dropped && activeCardCard.cardType == CardType.Ability)
                {
                    //player as taget then pass player as actor
                    activeEffectLoader.EffectLoaderActivate(player, player);
                    //Energy = Energy - activeCard.gameObject.GetComponent<Display>().card.energyCost;
                    EnergyUpdater(-activeCard.gameObject.GetComponent<Display>().card.energyCost);
                    //calls discard method and puts active card in discard pile
                    //Set sibling as last must be called first before discarding because it messes with the card hand arangement logic
                    activeCard.transform.SetAsLastSibling();

                    //for stopping the drag
                    activeCardDragNDrop.DeactivateDrag();

                    //reset oiginal must come first before discarding, beccause overridesorting can only be set if object is active
                    //every time a card is played, reset it's scale down from
                    playerHand.ResetToDeckPosition();
                    //consume a card if the Card has a Consume tag confirmed
                    if (activeCardCard.cardTags.Contains(CardMechanics.Consume) || activeCardCard.cardTags.Contains(CardMechanics.Ability))
                    {
                        //deckManager.ConsumeCards(activeCard);
                        StartCoroutine(deckManager.ConsumeCards(activeCard));
                    }
                    else
                    {
                        //deckManager.DiscardCards(activeCard);
                        StartCoroutine(deckManager.DiscardCards(activeCard));
                    }

                    //retuns to player turn phase
                    state = CombatState.PlayerTurn;
                    //activeCard.GetComponent<DragNDrop>().StateChanger(state);////////////////////
                    //playerHand.StateChanger(state);
                    //resets sorting orders, box colliders, and positions
                    //activeCardDragNDrop.ResetSortingCanvasAndCollider();
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
                //check if object has a collider first to prevent an error on misclick
                if (pointedObject.collider != null)
                {
                    activeCard = pointedObject.collider.gameObject;
                    if (activeCard.GetComponent<DragNDrop>() != null)
                    {
                        Card activeCardCard = activeCard.GetComponent<Display>().card;
                        DragNDrop activeCardDragNDrop = activeCard.GetComponent<DragNDrop>();


                        if (pointedObject.collider != null && pointedObject.collider.gameObject.tag == "Card")
                        {
                            //checks the scriptable attached in Display if cost can be accomodated by Energy
                            //checks if current chosen card's input link matches the output of the previous card
                            if (activeCardCard.energyCost <= playerFunctions.currEnergy // activeCardCard.energyCost <= playerFunctions.currEnergy 
                                && creativeManager.CheckLinkEligibility(activeCardCard)
                                && activeCardCard.jigsawEffect != null  //jigsawEffect
                                && creativeManager.creativityCost < player.GetComponent<PlayerFunctions>().currCreativity
                                && activeCardCard.cardType != CardType.Ability)
                            {
                                //transfers the card in creative mode then disables the prefab                        
                                //Energy -= activeCardCard.energyCost;
                                EnergyUpdater(-activeCardCard.energyCost);

                                creativeManager.ChooseForCreative(activeCardCard);
                                creativeList.Add(activeCard);
                                //cardDictionary.Add(activeCard.transform.GetSiblingIndex(), activeCard);
                                activeCard.SetActive(false);
                            }
                            else if (activeCardCard.cardType == CardType.Ability)
                            {
                                creativeManager.MessagePrompt("Abilities cannot be linked");
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
                    //cache for the chosen prefab for creative
                    DragNDrop creativeListDragNDrop = creativeList[tempIndex].GetComponent<DragNDrop>();
                    creativeList.Remove(creativeList[tempIndex]);
                    //removes scale increase on all cards
                    playerHand.ResetToDeckPosition();

                    //resets sorting orders, box colliders, and positions
                    creativeListDragNDrop.ResetSortingCanvasAndCollider();

                    //for calling the plain rearrange function
                    StartCoroutine(deckManager.PlainRearrange(creativeListDragNDrop.gameObject));
                }


            }

            //completely backs out from creative mode
            else if (Input.GetKeyDown(KeyCode.C))
            {
                foreach (GameObject card in creativeList)
                {
                    //cache for returning scales and positions to default
                    DragNDrop activeDragDrop = card.GetComponent<DragNDrop>();

                    int tempIndex = creativeManager.ReturnFromCreative();
                    //Energy += creativeList[tempIndex].GetComponent<Display>().card.energyCost;
                    EnergyUpdater(creativeList[tempIndex].GetComponent<Display>().card.energyCost);
                    creativeList[tempIndex].SetActive(true);

                    //for calling the plain rearrange function
                    StartCoroutine(deckManager.PlainRearrange(creativeList[tempIndex]));
                    //for returning scales and positions to default
                    //activeDragDrop.ResetSortingCanvasAndCollider();

                }
                creativeList.Clear();
                //removes scale increase on all cards
                playerHand.ResetToDeckPosition();
                state = CombatState.PlayerTurn;
                creativeUI.SetActive(false);
            }
        }

        //when picking target for unleasing creativity
        else if (state == CombatState.UnleashCreativity)
        {
            
            //this method takes the cardMethod of linked cards and jigsaws
            //if there is a targetted card, the linked are all targetted, it is dropped if there are no targetted cards
            CardMethod linkMethod = creativeManager.FinalizeLinks();


            if (linkMethod == CardMethod.Targetted)
            {
                //sends mouse position to active card's target arrow handlker and calculates all of the dot's positions
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                creativeUnleashArrow.EnableArrow();
                creativeUnleashArrow.DynamicPositionArrow(mousePos);


                //lift click in targetting during unleash
                if (Input.GetMouseButtonDown(0))
                {
                    GameObject targetObject = pointedObject.collider.gameObject;

                    //if targetted, click will work one specific enemy units only
                    if (pointedObject.collider != null && targetObject.tag == "Enemy")
                    {
                        //access player stats and reduces their creativity meter
                        player.GetComponent<PlayerFunctions>().AlterPlayerCreativity(-creativeManager.creativityCost);
                        //initiates link effects in CreativeManager
                        //returns the cost for crativity
                        creativeManager.UnleashCreativity(targetObject, player);

                        //removes scale increase on all cards
                        playerHand.ResetToDeckPosition();
                        //Discards cards used in creative mode
                        foreach (GameObject linkedCard in creativeList)
                        {
                            //cache for card object and the card SO
                            activeCard = linkedCard;
                            activeDragNDrop = activeCard.GetComponent<DragNDrop>();
                            Card activeCardCard = activeCard.GetComponent<Display>().card;
                            //calls discard method and puts active card in discard pile
                            activeCard.transform.SetAsLastSibling();
                            //reset oiginal must come first before discarding, beccause overridesorting can only be set if object is active
                            //every time a card is played, reset it's scale down from
                            playerHand.ResetToDeckPosition();
                            //resets sorting orders, box colliders, and positions
                            activeDragNDrop.ResetSortingCanvasAndCollider();
                            //consume a card if the Card has a Consume tag confirmed
                            if (activeCardCard.cardTags.Contains(CardMechanics.Consume) || activeCardCard.cardTags.Contains(CardMechanics.Ability))
                            {
                                //deckManager.ConsumeCards(activeCard);
                                StartCoroutine(deckManager.ConsumeCards(activeCard));
                            }
                            else
                            {
                                //deckManager.DiscardCards(activeCard);
                                StartCoroutine(deckManager.DiscardCards(activeCard));
                            }
                            //resets sorting orders, box colliders, and positions
                            activeDragNDrop.ResetSortingCanvasAndCollider();
                        }
                        //clearing CardObjects 
                        creativeList.Clear();
                        state = CombatState.PlayerTurn;
                        creativeUnleash.SetActive(false);
                        //playerHand.StateChanger(state); -- not yet sure

                        DeckUpdater();

                    }
                    //disables arrow after use
                    creativeUnleashArrow.DisableArrow();


                }

            }
            else if (linkMethod == CardMethod.Dropped)
            {
                //access player stats and reduces their creativity meter
                player.GetComponent<PlayerFunctions>().AlterPlayerCreativity(-creativeManager.creativityCost);
                //initiates link effects in CreativeManager
                //returns the cost for crativity
                creativeManager.UnleashCreativity(enemyHolder, player);

                playerHand.ResetToDeckPosition();
                //Discards cards used in creative mode
                foreach (GameObject linkedCard in creativeList)
                {
                    //cache for card object and the card SO
                    activeCard = linkedCard;
                    activeDragNDrop = activeCard.GetComponent<DragNDrop>();
                    Card activeCardCard = activeCard.GetComponent<Display>().card;
                    //calls discard method and puts active card in discard pile
                    activeCard.transform.SetAsLastSibling();
                    //reset oiginal must come first before discarding, beccause overridesorting can only be set if object is active
                    //every time a card is played, reset it's scale down from
                    playerHand.ResetToDeckPosition();
                    //resets sorting orders, box colliders, and positions
                    activeDragNDrop.ResetSortingCanvasAndCollider();
                    //consume a card if the Card has a Consume tag confirmed
                    if (activeCardCard.cardTags.Contains(CardMechanics.Consume) || activeCardCard.cardTags.Contains(CardMechanics.Ability))
                    {
                        //deckManager.ConsumeCards(activeCard);
                        StartCoroutine(deckManager.ConsumeCards(activeCard));
                    }
                    else
                    {
                        //deckManager.DiscardCards(activeCard);
                        StartCoroutine(deckManager.DiscardCards(activeCard));
                    }
                    //resets sorting orders, box colliders, and positions
                    activeDragNDrop.ResetSortingCanvasAndCollider();
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
        //deckManager.DiscardCards(activeCard);
        StartCoroutine(deckManager.DiscardCards(activeCard));
        activeCard.SetActive(false);
        
    }

    //function for drawing during start of turn
    public void DrawHand()
    {
        //setting the state of all cards to drawsphase will supposedly prevent accidental onPointerExit logic while drawing cards
        //playerHand.StateChanger(CombatState.DrawPhase);

        deckManager.StartCoroutine(deckManager.DrawCards(playerFunctions.defaultDraw));
        //deckManager.DrawCards(playerFunctions.defaultDraw);

        DeckUpdater();
        //turn back to playerTurn phase after drawing
        //playerHand.StateChanger(CombatState.PlayerTurn);
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
        int consumeCardsCount = deckManager.consumeCount;

        deckText.text = deckCardsCount.ToString();
        discardText.text = discardCardsCount.ToString();
        consumeText.text = consumeCardsCount.ToString();

    }


    //Ending turn to enemy action to new player phase
    public void EndTurnButton()
    {
        //makes the end turn button uninteractable
        EndTurnButt.interactable = false;
        //iterates through each card in hand and calls on disCardFromHand dor each
        foreach(Transform cardInHand in playerHand.transform)
        {
            if(cardInHand.gameObject.activeSelf == true)
            {
                activeCard = cardInHand.gameObject;
                //deckManager.DiscardCards(activeCard);
                StartCoroutine(deckManager.DiscardCards(activeCard));
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