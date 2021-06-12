using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;


public class CombatManager : MonoBehaviour
{
    //event to call when a player turn is started
    public delegate void D_StartTurn();
    public event D_StartTurn d_StartTurn;

    //event to call when combat starts
    public delegate void D_StartCombat();
    public event D_StartCombat d_StartCombat;

    //After initializing stuffs, assign the player and enemy ststs
    public delegate void D_GenerateUnits();
    public event D_GenerateUnits d_GenerateUnits;

    //delegate to generate creativity for all units at the start of player turn
    public delegate void D_NaturalGenerateCreativity(int i);
    public event D_NaturalGenerateCreativity d_NaturalGenerateCreativity;


    CombatState state;
    //for thedropfield moving up approach targetting system
    //drop field for dropped cards
    //public GameObject dropField;
    //Vector3 originalDropFieldPosition;

    //related to player
    public GameObject player;
    public PlayerFunctions playerFunctions;

    //list of enemies
    public List<GameObject> enemyObjects = new List<GameObject>();
    public GameObject enemyHolder;
    //for creativeField
    public GameObject creativeUI;
    public GameObject creativeUnleash;
    TargetArrowHandler creativeUnleashArrow;
    List<GameObject> creativeList = new List<GameObject>();
    //playing field, automatically sent when a card is a dropped type and not an ability
    public GameObject playingField;

    //Assigned in editor
    public DeckManager deckManager;
    public CreativeManager creativeManager;
    public PlayerHand playerHand;
    public CardDrafting cardDrafting;
    public EnemyPools enemyPools;
    public AbilityManager abilityManager;
    public EnemyAIManager enemyAIManager;

    //related to energy
    //Energy gets accessed by playingField
    public int Energy;
    public Text energyText;

    //related to decks
    //int defaultDraw = 5;
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
    //RaycastHit2D pointedTarget;
    //for arrow pointing dynamic position
    TargetArrowHandler targetArrowHandler;

    //the loaded stats from universalInformation
    UniversalInformation universalInformation = new UniversalInformation();

    //identifier that the turn cis the first turn loaded from file
    bool isFirstTurnLoaded;
    //identifier if player is currently being overkilled
    bool isPlayerBeingOverkilled;

    public void Awake()
    {
        //one time run to add dictionary entries of cardMechanic enums and text descriptions
        CardTagManager.InitializeTextDescriptionDictionaries();
        EffectFactory.InitializeCardFactory();
        EnemyUnitFactory.InitializeEnemyUnitFactory();

    }

    //Tried making this Awake(), destroyed some logic in EnemyFunctions
    public void Start()
    {


        //initial caching of player stats
        playerFunctions = player.GetComponent<PlayerFunctions>();

        //initial caching of creativeUnleash object's arrow handler
        creativeUnleashArrow = creativeUnleash.GetComponent<TargetArrowHandler>();

        //for exe3ctuing stuff during startcombat
        //calls the stats initialize for all units
        d_StartCombat += player.GetComponent<BaseUnitFunctions>().BaseUnitStatsInitialize;

        //for just copying the default energy and draws from playerFunctions
        //defaultEnergy = player.GetComponent<PlayerFunctions>().defaultEnergy;
        //Draw = player.GetComponent<PlayerFunctions>().defaultDraw;

        //DrawHand();
        ////sends default energy to updater
        //EnergyUpdater(defaultEnergy);
        //DeckUpdater();

        //assigns the initializers in the event d_StartTurn
        d_StartTurn += StartTurnInCombatManager; //this must stay in front of StatusUpdateTurn
        d_StartTurn += enemyHolder.GetComponent<EnemyAIManager>().EnemyStart;
        //d_StartTurn += player.GetComponent<PlayerFunctions>().PlayerTurn;
        d_StartTurn += playerFunctions.PlayerTurn;
        d_StartTurn += player.GetComponent<UnitStatusHolder>().StatusUpdateForNewTurn;
        d_NaturalGenerateCreativity += playerFunctions.AlterCreativity;

        //d_StartTurn += player.GetComponent<UnitStatusHolder>().TurnStatusUpdater;
        //d_StartTurn += player.GetComponent<UnitStatusHolder>().ConsumeTurnStackUpdate;
        foreach (Transform enemy in enemyHolder.transform)
        {
            d_StartTurn += enemy.GetComponent<UnitStatusHolder>().StatusUpdateForNewTurn;
            d_StartCombat += enemy.GetComponent<BaseUnitFunctions>().BaseUnitStatsInitialize;
            d_NaturalGenerateCreativity += enemy.GetComponent<EnemyFunctions>().AlterCreativity;
            //d_StartTurn += enemy.gameObject.GetComponent<UnitStatusHolder>().TurnStatusUpdater;
            //d_StartTurn += enemy.GetComponent<UnitStatusHolder>().ConsumeTurnStackUpdate;
        }

        //load the universalInformation saved from the selection and overworld screen
        // sends playerfunctions to playerPrefab and card list to deckmanager
        universalInformation = UniversalSaveState.LoadUniversalInformation();
        CardSOFactory.InitializeCardSOFactory(universalInformation.chosenPlayer, universalInformation.chosenClass);
        playerFunctions.LoadPlayerUnitFromFile(universalInformation.playerStats);

        //moved to be at the end of InitiateCombatState
        //this is so that the test jigsaw generator can only activate if there is no combatFile loaded
        deckManager.InitializeBattleDeck(universalInformation.currentDeck);


        //Card Drafting migrated to rewardsscene
        //cardDrafting.InitializeDraftPool(universalInformation.chosenPlayer, universalInformation.chosenClass);

        //Save of combatState should be last
        //move this to be executed in the end of StartTurnInCombatManager()
        //d_StartTurn += SaveCombatState;

        //will be called only during the beginiing
        d_StartCombat();

        //determines whether player, cards and enemystats are generated for fresh combat or loaded from file
        InitiateCombatState();

        //d_StartTurn += Player.GetComponent<AbilityManager>().EnableAbilities;
        //d_StartTurn += Player.GetComponent<PlayerFunctions>().AlterPlayerCreativity;
        //d_StartTurn += playerFunctions.StartTurnUpdates;
        d_StartTurn();

        //save after all start turn prep is done
        SaveCombatState();


    }

    //Loading combatstate from file
    ////if a combat file exists, load the player unit and enemy unit from combatSaveState
    void InitiateCombatState()
    {
        if (File.Exists(Application.persistentDataPath + "/Combat.json"))
        {
            CombatSaveState combatSaveState = UniversalSaveState.LoadCombatState();

            //instantiate copies of the base SO per spawn in list and assign them to respective enemyHolder position
            for (int i = 0; combatSaveState.enemyUnitWrappers.Count - 1 >= i; i++)
            {
                GameObject enemy = enemyHolder.transform.GetChild(i).gameObject;
                EnemyFunctions enemyFunction = enemy.GetComponent<EnemyFunctions>();

                //enemyUnit generated from factory then edited to match the enemy stats
                EnemyUnitStatsWrapper enemyWrapper = combatSaveState.enemyUnitWrappers[i];
                Debug.Log($"debug on {enemyWrapper.enemyEnumName}");
                EnemyUnit enemyUnit = EnemyUnitFactory.GetEnemySO(enemyWrapper.enemyEnumName);

                //identifiers of mortality
                enemyFunction.isAlive = enemyWrapper.isAlive;
                enemyFunction.isBeingOverkilled = enemyWrapper.isBeingOverkilled;
                enemyFunction.isOverKilled = enemyWrapper.isOverKilled;

                //only load the enemy if it is is still alive
                if (enemyFunction.isAlive == true)
                {
                    enemy.SetActive(true);
                    //assigning values
                    enemyFunction.currHP = enemyWrapper.currHP;
                    enemyFunction.SliderValueUpdates();
                    enemyFunction.GainBlock(enemyWrapper.block);
                    enemy.GetComponent<EnemyFunctions>().enemyUnit = Instantiate(enemyUnit);
                    //assigning statuses
                    for (int j = 0; enemyWrapper.cardMechanics.Count - 1 >= j; j++)
                    {
                        UnitStatusHolder enemyStatus = enemy.GetComponent<UnitStatusHolder>();
                        enemyStatus.AlterStatusStack(enemyWrapper.cardMechanics[j], enemyWrapper.statusStacks[j]);
                    }
                }

            }
            //calling enemyAI functions
            //if from file, get the counter that was saved, since there might already be dead enemies there and the original enemy count is not the one needed
            enemyAIManager.PseudoStartCombat(combatSaveState.enemyCounter);
            

            //assign the player unit saved in file
            playerFunctions.LoadPlayerUnitFromFile(combatSaveState.playerUnit);
            //max is subtracted from current because alterCreativvity funcion needs negative values for reduction
            playerFunctions.AlterCreativity(combatSaveState.currCreativity/* - combatSaveState.playerUnit.Creativity*/);
            playerFunctions.currHP = combatSaveState.playerUnit.currHP;
            playerFunctions.SliderValueUpdates();
            playerFunctions.AlterEnergy(playerFunctions.defaultEnergy - combatSaveState.currEnergy);
            //loading abilities
            foreach (AllAbilities abilityEnum in combatSaveState.abilityList)
            {
                AbilityFormat abilityFormat = Resources.Load<AbilityFormat>($"AbilitySO/{abilityEnum}");
                abilityManager.InstallAbility(abilityFormat);
            }

            //Loading Player Status stacks from file
            UnitStatusHolder playerStatus = player.GetComponent<UnitStatusHolder>();
            for (int i = 0; combatSaveState.cardMechanics.Count-1 >= i; i++)
            {
                playerStatus.AlterStatusStack(combatSaveState.cardMechanics[i], combatSaveState.statusStacks[i]);
            }

            //identifiers
            isPlayerBeingOverkilled = combatSaveState.isPlayerBeingOverkilled;



        }
        //if combat file does not exist, get ebemy base unit from enemyPools
        else
        {
            UniversalInformation universalInfo = UniversalSaveState.LoadUniversalInformation();
            List<EnemyUnit> enemySpawn = enemyPools.GetEnemySpawn(universalInformation.nodeCount);

            //gives the player Worn-out Status if universalInfo has a worn-out count
            if(universalInfo.wornOutCount > 0)
            {
                UnitStatusHolder playerStatus = player.GetComponent<UnitStatusHolder>();
                playerStatus.AlterStatusStack(CardMechanics.WornOut, universalInfo.wornOutCount);
            }
            //instantiate copies of the base SO per spawn in list and assign them to respective enemyHolder position
            for (int i = 0; enemySpawn.Count - 1 >= i; i++)
            {
                GameObject enemy = enemyHolder.transform.GetChild(i).gameObject;
                EnemyFunctions enemyFunction = enemy.GetComponent<EnemyFunctions>();
                enemyFunction.enemyUnit = Instantiate(enemySpawn[i]);
                enemyFunction.isAlive = true;
                enemy.SetActive(true);
            }
            //use the predetermined enemy count if fresh combat scene is being generated
            //currently the enemyCount in universalInfo is 2 just for testing
            universalInfo.enemyCount = 2;
            //clear the overkills and worn-outs from last combat
            universalInfo.overkills = 0;
            universalInfo.wornOutCount = 0;
            UniversalSaveState.SaveUniversalInformation(universalInfo);

            enemyAIManager.PseudoStartCombat(universalInfo.enemyCount);
        }


    }


    //this is an event called everytime a player turn is started
    //called by d_StartTurn()
    public void StartTurnInCombatManager()
    {
        //if player HP is below 0 and being overkilled during the start of turn, call Defeat Function
        if (playerFunctions.currHP <= 0 && isPlayerBeingOverkilled)
        {
            //if current turn is the first time player is overkilled, proceed with turn and set identifier to true
            DefeatFunction();
            
        }
        //if not yet being overkilled, proceed to normal turn
        else
        {
            state = CombatState.PlayerTurn;

            //for getting status effects from unitStatusHolder of player
            UnitStatusHolder unitStatusHolder = player.GetComponent<UnitStatusHolder>();


            //update creativity every turn to regenerate naturally by 1
            //will only activate at the next turn onwards of start combat
            //if identifier is false which is default, set it to true, this way, all preceeding turns except the firs will avtivte
            if (!isFirstTurnLoaded)
            {
                isFirstTurnLoaded = true;
            }
            else
            {
                //all units now generate  creativity naturally
                //playerFunctions.AlterCreativity(1);
                d_NaturalGenerateCreativity(1 + unitStatusHolder.CreativityAlteringModifierCalculator());
            }

            //if has negative HP, set isBeingOverkilled to True
            //+1 is offset because status turn update runs after the initialized alterStatus
            if (playerFunctions.currHP <= 0)
            {
                unitStatusHolder.AlterStatusStack(CardMechanics.LastStand, 1+1);
                isPlayerBeingOverkilled = true;
            }

            //the energy 
            playerFunctions.currEnergy = 0;
            EnergyUpdater(playerFunctions.defaultEnergy + unitStatusHolder.EnergyAlteringModifierCalculator());
            DrawHand(playerFunctions.defaultDraw + unitStatusHolder.DrawAlteringModifierCalculator());

            //makes the endTurnButton interactable again during player turn
            EndTurnButt.interactable = true;


        }

    }
    //for saving all the Combat parameters
    public void SaveCombatState()
    {
        //for counting the list of gameObjects existing in the enemyHolder
        enemyObjects.Clear();
        foreach (Transform enemyTrans in enemyHolder.transform)
        {
            GameObject enemyObject = enemyTrans.gameObject;
            enemyObjects.Add(enemyObject);
        }

        //SAVE FUNCTION COMMENCES AT START OF TURN
        PlayerUnit playerUnit = Instantiate(player.GetComponent<PlayerFunctions>().playerUnit);
        UnitStatusHolder playerUnitStatuses = player.GetComponent<UnitStatusHolder>();
        CombatSaveState combatSaveState = new CombatSaveState(deckManager, playerUnit, playerUnitStatuses, enemyObjects);
        combatSaveState.currCreativity = playerFunctions.currCreativity;
        combatSaveState.currEnergy = playerFunctions.currEnergy;
        combatSaveState.enemyCounter = enemyAIManager.enemyCounter;
        combatSaveState.isPlayerBeingOverkilled = isPlayerBeingOverkilled;
        foreach (AbilityFormat abilityFormat in abilityManager.abilityList)
        {
            combatSaveState.abilityList.Add(abilityFormat.enumAbilityName);
        }
        //if 


        //saves combatState and generate save file
        UniversalSaveState.SaveCombatState(combatSaveState);

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

        if (Input.GetKey(KeyCode.Backspace))
        {
            SceneManager.LoadScene("OverWorldScene");
        }

        //for test saving
        if (Input.GetKey(KeyCode.F5))
        {

        }

        //test for showing card Draft
        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    VictoryFunction();
        //    Debug.Log("starting draft");
        //}

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
                    //DeckUpdater();




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
                    //DeckUpdater();


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
                    //DeckUpdater();

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
                        player.GetComponent<PlayerFunctions>().AlterCreativity(-creativeManager.creativityCost);
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

                        //DeckUpdater();

                    }
                    //disables arrow after use
                    creativeUnleashArrow.DisableArrow();


                }

            }
            else if (linkMethod == CardMethod.Dropped)
            {
                //access player stats and reduces their creativity meter
                player.GetComponent<PlayerFunctions>().AlterCreativity(-creativeManager.creativityCost);
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
                //DeckUpdater();
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
    public void DrawHand(int turnDraw)
    {
        //setting the state of all cards to drawsphase will supposedly prevent accidental onPointerExit logic while drawing cards
        //playerHand.StateChanger(CombatState.DrawPhase);

        //deckManager.StartCoroutine(deckManager.DrawCards(playerFunctions.defaultDraw));
        //deckManager.DrawCards(playerFunctions.defaultDraw); //replaced when draw affecting statuses were created, now number of card draw is dictated by combatManager from calculation
        deckManager.DrawCards(turnDraw);

        //moved to the end of draw, discard, and consume scripts in DeckManager to ensure sync
        //DeckUpdater();

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
    //Method moved to DeckManager so that the discard, draw and consume Coroutines match the timings with DeckUpdater
    public void DeckUpdater()
    {
        //int deckCardsCount = deckManager.deckCount;
        //int discardCardsCount = deckManager.discardCount;
        //int consumeCardsCount = deckManager.consumeCount;

        //deckText.text = deckCardsCount.ToString();
        //discardText.text = discardCardsCount.ToString();
        //consumeText.text = consumeCardsCount.ToString();
        //Debug.Log("deckupdater");
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

        //DeckUpdater();
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
            enemyFunctions.EnemyStartTurn();
            yield return new WaitForSeconds(.5f);
        }

        //player.GetComponent<BaseUnitFunctions>().RemoveBlock();
        yield return new WaitForSeconds(1f);

        //delegate for startTurn Event
        d_StartTurn();
        //save after all start turn prep is done
        SaveCombatState();
    }

    public void DefeatFunction()
    {
        //sends player to defeat screen then delete all save files
        Debug.Log("defeated");
    }

    //called by enemyAIManager when all enemies are destroyed
    //save overkill count in universalInformation
    public void VictoryFunction()
    {
        //cardDrafting.StartCardDraft();

        UniversalInformation universalInfo = UniversalSaveState.LoadUniversalInformation();

        //checks each enemyFunction if it was overkilled
        int tempOverkillCount = 0;
        foreach (Transform enemyObject in enemyHolder.transform)
        {
            EnemyFunctions enemyFunction = enemyObject.GetComponent<EnemyFunctions>();
            if (enemyFunction.isOverKilled)
            {
                tempOverkillCount++;
            }
        }
        universalInfo.overkills = tempOverkillCount;

        //if player HP is overkilled equal to full HP, get 3 worn-out turns next combat
        //the +1 in worn-out count is an offset because the turn at combat Initiate will decrease it by one
        if (playerFunctions.currHP <= -(playerFunctions.currHP))
        {
            universalInfo.wornOutCount = 3 + 1;
            playerFunctions.playerUnit.currHP = 1;
        }
        //if only overkilled through half health, get 2 worn-out necxt combat
        else if (playerFunctions.currHP <= -(playerFunctions.currHP/2))
        {
            universalInfo.wornOutCount = 2 + 1;
            playerFunctions.playerUnit.currHP = 1;
        }
        //if it just reached below 0, get 1 worn-out next combat
        else if (playerFunctions.currHP <= 0)
        {
            universalInfo.wornOutCount = 1 + 1;
            playerFunctions.playerUnit.currHP = 1;
        }

        universalInfo.playerStats = playerFunctions.playerUnit;
        UniversalSaveState.SaveUniversalInformation(universalInfo);

        //delete the combat file
        File.Delete(Application.persistentDataPath + "/Combat.json");


        //go to rewards scene if victorious
        SceneManager.LoadScene("RewardsScene");

    }

}


//add some new effects