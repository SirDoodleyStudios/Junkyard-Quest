using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CameraUIScript : MonoBehaviour
{
    public delegate void D_ResizeCameraUI(float i);
    public event D_ResizeCameraUI d_ResizeCameraUI;

    //loaded Information
    UniversalInformation universalInfo;

    //set in editor prefab
    public TextMeshProUGUI currHPText;
    public TextMeshProUGUI maxHPText;
    public TextMeshProUGUI currCreativityText;
    public TextMeshProUGUI creativityText;
    public TextMeshProUGUI scrapsText;
    public TextMeshProUGUI ticketEffectText;
    public TextMeshProUGUI ticketCountText;

    RectTransform objectRect;

    //for Deck Viewing
    //the actual UI to back drop to be activated 
    public GameObject deckViewUI;

    //holder for loaded cards from file
    List<Card> cardList = new List<Card>();

    //card prefab for viewing in deck
    public GameObject deckViewPrefab;
    //Content gameobject, parent of all prefabs to be shown after button click
    public Transform deckScrollContent;

    //reference SO for instantiating JigsawFormats during loading
    //set in editor
    public JigsawFormat referenceJigsawFormat;

    //for inventory viewing
    public GameObject inventoryViewUI;
    public InventoryViewer inventoryViewer;

    //for equipment viewing
    public GameObject equipmentViewUI;
    public EquipmentViewer equipmentViewer;

    //holders for the inventory view
    //this is used when the UI is called instead of just using universalInfo itself
    //it is this way because scenes that will edit the inventory contents can avoid saving the universalInfo everytime a change is made
    //we ca update this for the inventory view only before saving entirely similar to the UpdateCardList function
    List<AllGearTypes> inventoryBlueprints = new List<AllGearTypes>();
    List<CraftingMaterialWrapper> inventoryMaterials = new List<CraftingMaterialWrapper>();
    List<GearWrapper> inventoryGears = new List<GearWrapper>();
    GearWrapper[] equippedGears = new GearWrapper[6];

    //ticket button
    //called here to disable an enable at times by the sceneManager
    public Button ticketButton;


    public void Awake()
    {
        objectRect = gameObject.GetComponent<RectTransform>();

        universalInfo = UniversalSaveState.LoadUniversalInformation();

        //get text object of the overworld UI, the getChild(1) is the the TMPro itself
        //HP slot is in the index 2 under the Overworld UI
        //Creativity slot is in index 3
        //scraps is in index 4

        //this is removed and all assignments are now done in editor
        //HPText = transform.GetChild(2).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        //CreativityText = transform.GetChild(3).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        //ScrapsText = transform.GetChild(4).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
    }

    //called by overworld manager when it instantiates
    public void SetUISize(float orthoSize)
    {
        //sets the size of the UI based on 
        //objectRect.sizeDelta = new Vector2(orthoSize/3.6f, 0);
        //d_ResizeCameraUI(orthoSize / 3.8f);

        //fix the UI size to a screen percent
        //the 85% multiplier in height is offset for giving way to the menu panel above
        //0 width because the prefab usedis set as stretch horizontally
        objectRect.sizeDelta = new Vector2(0, Screen.height * .075f);

        //this is outdated, from the logic when Ui was in the side panel
        //d_ResizeCameraUI(Screen.width * .08f);
        Debug.Log(objectRect.sizeDelta);

        //OLD Font sizing logic
        //float fontSize = (orthoSize / 3.6f) * .35f;
        //sets the size of texts in UI depending on the Size of slot
        //currHPText.fontSize = fontSize;
        //maxHPText.fontSize = fontSize;
        //creativityText.fontSize = fontSize;
        //scrapsText.fontSize = fontSize;

        //new one where HP is auto-sized then all texts will copy it
        creativityText.fontSize = currHPText.fontSize;
        scrapsText.fontSize = currHPText.fontSize;



    }
    //function to be called when an update was created on the universalUnformation was made and the universalInfo of this instance needs to be updated as well
    public void UpdateUniversalInfo()
    {
        universalInfo = UniversalSaveState.LoadUniversalInformation();
    }

    //called by the scene's manager at awake
    //only used for initializing values from universlInfo load file
    public void AssignUIObjects(UniversalInformation universalInfo)
    {
        PlayerUnit playerStats = universalInfo.playerStats;
        currHPText.text = $"{playerStats.currHP}";
        maxHPText.text = $"{playerStats.HP}";
        currCreativityText.text = $"{playerStats.currCreativity }";
        creativityText.text = $"{playerStats.creativity}";
        scrapsText.text = $"{universalInfo.scraps}";
        ticketCountText.text = $"{universalInfo.tickets }";

    }

    //for actively updating the text in UI without using a universalInformation
    //mainly used by Combat Scene for UI for active updates
    //parameters are new values to be assigned

    //for updating UI text only for HP stats
    public void UpdateUIObjectsHP(int currHPChange, int maxHPChange)
    {
        currHPText.text = $"{currHPChange}";
        maxHPText.text = $"{ maxHPChange}";
    }
    //for updating UI text only for Creativity stats
    public void UpdateUIObjectsCretivity(int currCreativityChange, int maxCreativityChange)
    {
        currCreativityText.text = $"{currCreativityChange}";
        creativityText.text = $"{maxCreativityChange}";
    }
    //separate function for scraps because we can't get info for scraps from playerfunctions
    public void UpdateUIObjectsActiveScraps(int scrapsChange)
    {
        scrapsText.text = $"{scrapsChange}";
    }
    //for updating the ticket count  text in UI only
    //bool parameter is for changin interactability
    public void UpdateUIObjectsTickets(int ticketsChange, bool buttonAvailability)
    {
        ticketCountText.text = $"{ticketsChange}";
        ticketButton.interactable = buttonAvailability;
    }


    //initially part of AssignUIObjects but now separated
    //called by sceneManager's Awake
    //Depending on the current scene, it will send what kind of effect a ticket will have
    public void InitiateUniversalUIInfoData(UniversalInformation universalInfo)
    {
        //initializes theCardSO factory
        CardSOFactory.InitializeCardSOFactory(universalInfo.chosenPlayer, universalInfo.chosenClass);
        //generates the CardSO itself
        //updated to check the key of allCards inside the cardandJigsawWrapper list
        foreach (CardAndJigsaWrapper CJW in universalInfo.currentDeckWithJigsaw)
        {
            //calls the factory to instantiate a copy of the base card SO
            Card tempCard = Instantiate(CardSOFactory.GetCardSO(CJW.cardEnum));
            tempCard.effectText = CardTagManager.GetCardEffectDescriptions(tempCard);

            if (CJW.jigsawEnum != AllCards.Jigsaw)
            {
                //calls a universal function that will extract a JigsawFormat using the wrapper and jigsaw Allcards enum

                tempCard.jigsawEffect = UniversalFunctions.LoadJigsawFormat(Instantiate(referenceJigsawFormat), CJW);

            }
            cardList.Add(tempCard);
        }

        //initiates the Universal UI's inventory and Equipment list
        inventoryMaterials.AddRange(universalInfo.craftingMaterialWrapperList);
        inventoryGears.AddRange(universalInfo.gearWrapperList);
        inventoryBlueprints.AddRange(universalInfo.bluePrints);
        //remained equal association for the time being
        equippedGears = universalInfo.equippedGears;

        //placeholder for the current scene
        Scene sceneHolder = SceneManager.GetActiveScene();
        //assign the effect text for tickets depending on scene
        if (sceneHolder.name == "CombatScene")
        {
            ticketEffectText.text = "Retain hand for 1 turn";
        }
        else if (sceneHolder.name == "ForgeScene")
        {
            ticketEffectText.text = "Reset forge cost";
        }
        else if (sceneHolder.name == "MerchantScene")
        {
            ticketEffectText.text = "Get 20% discount";
        }
        else if (sceneHolder.name == "OverworldScene")
        {
            ticketEffectText.text = "Heal 10% HP";
        }
        else if (sceneHolder.name == "RestScene")
        {
            ticketEffectText.text = "Gain 1 action";
        }
        else if (sceneHolder.name == "LinkActivitiesScene")
        {
            ticketEffectText.text = "Skip next activity";
        }
        else
        {
            ticketEffectText.text = "No use for ticket";
        }


    }

    //single method for viewing a card collection in view
    //usually used in deck viewing button but in forgeMaster, can aslo call it with the set Main or augment buttons
    public void ViewSavedDeck()
    {
        deckViewUI.SetActive(true);

        //actual logic to show each card in UI one by one

        foreach (Card deckCard in cardList)
        {
            bool hasNoDisabledPrefabs = true;
            deckViewPrefab.GetComponent<Display>().card = deckCard;

            //sets the size of each cell in the content holder depending on the size of screen
            //the numbers are calculated to get the exact amount needed
            //I just generated the cards on the screen wtih a sixe that I liked with a fixed 100,143.75 cell size then divided the screen sizes from them
            GridLayoutGroup gridLayout = deckScrollContent.GetComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(Screen.width*.13440860215f, Screen.height*.34389952153f);
            gridLayout.spacing = new Vector2(Screen.width * .0237247924f, Screen.height * .04219409282f);

            //checks the scroll contents if there are already instantiated card prefabs that can be recycled
            foreach (Transform content in deckScrollContent)
            {
                GameObject disabledPrefabs = content.gameObject;
                if (!disabledPrefabs.activeSelf)
                {
                    disabledPrefabs.GetComponent<Display>().card = deckCard;
                    disabledPrefabs.SetActive(true);
                    hasNoDisabledPrefabs = false;
                    break;
                }
                //if no card prefab can be recycled, instantiate a new one
            }

            if (hasNoDisabledPrefabs)
            {
                GameObject instantiatedPrefab = Instantiate(deckViewPrefab, deckScrollContent);
                RectTransform instantiatedRect = instantiatedPrefab.GetComponent<RectTransform>();
                Display instantiatedDisplay = instantiatedPrefab.GetComponent<Display>();
                CardDescriptionLayout instantiatedPopups = instantiatedPrefab.GetComponent<CardDescriptionLayout>();
                instantiatedRect.sizeDelta = new Vector2(Screen.width * .13440860215f, Screen.height * .34389952153f);
                instantiatedDisplay.card = deckCard;
                instantiatedDisplay.FontResize();
                instantiatedPopups.ResizePopups();
                instantiatedPrefab.SetActive(true);
            }
        }
    }
    //close deck view
    public void UnviewSavedDeck()
    {
        foreach (Transform content in deckScrollContent)
        {
            content.gameObject.SetActive(false);
        }
        deckViewUI.SetActive(false);
    }

    //function for updating the deck
    //currently only used by forgemanager since the deck is actively updated there
    //also used by merchantManager
    //can add or remove single cards, bool identifier to determine if add or remove, true for add, false for remove
    public void UpdateCurrentDeck(Card newCardList, bool isToAdd)
    {
        if (isToAdd)
        {
            cardList.Add(newCardList);
        }
        else
        {
            cardList.Remove(newCardList);
        }

    }

    //fetching the deck 
    public List<Card> FetchDeck()
    {
        return cardList;
    }

    //update inventory in Universal UI\
    //only the UI info gets updated, the changes aren't saved in universalInfo yet for optimization purposes
    //used for scenes that alter your inventory items    
    public void UpdateMaterialInventory(CraftingMaterialWrapper materialWrapper, bool isToAdd)
    {
        if (isToAdd)
        {
            inventoryMaterials.Add(materialWrapper);
        }
        else
        {
            inventoryMaterials.Remove(materialWrapper);
        }
    }
    public void UpdateGearInventory(GearWrapper gearWrapper, bool isToAdd)
    {
        if (isToAdd)
        {
            inventoryGears.Add(gearWrapper);
        }
        else
        {
            inventoryGears.Remove(gearWrapper);
        }
    }
    public void UpdateBlueprintInventory(AllGearTypes blueprint, bool isToAdd)
    {
        if (isToAdd)
        {
            inventoryBlueprints.Add(blueprint);
        }
        else
        {
            inventoryBlueprints.Remove(blueprint);
        }
    }
    //full updates are used when the lists are going to be updated by overriding the list itself
    //mostly used if there is removal
    public void UpdateFullGearInventory(List<GearWrapper> gearWrappers)
    {
        inventoryGears = gearWrappers;
    }
    public void UpdateFullMaterialInventory(List<CraftingMaterialWrapper> craftingMaterialWrappers)
    {
        inventoryMaterials = craftingMaterialWrappers;
    }


    //when a manager script wants to view a deck but not the whole deck
    //currently used by ForgeManager for viewing the deck but only with available cards to choose from
    public void ViewFilteredDeck(List<Card> filteredDeck)
    {
        List<Card> originalList = cardList;
        cardList = filteredDeck;
        ViewSavedDeck();
        cardList = originalList;
    }

    //for viewing the inventory
    public void ViewInventory()
    {
        inventoryViewUI.SetActive(true);
        //inventoryViewer.InitializeInventoryView(universalInfo.bluePrints, universalInfo.craftingMaterialWrapperList, universalInfo.gearWrapperList);
        inventoryViewer.InitializeInventoryView(inventoryBlueprints, inventoryMaterials, inventoryGears);

    }

    //for viewing gears
    public void ViewEquipment()
    {
        equipmentViewUI.SetActive(true);
        //equipmentViewer.InitiateEquipment(universalInfo.equippedGears, universalInfo.gearWrapperList/*, universalInfo*/);
        equipmentViewer.InitiateEquipment(equippedGears, inventoryGears);
    }
    //called by OverWorld only to make all gears in equipment viewer draggable and droppable
    public void EnableGearManagement()
    {
        equipmentViewer.MakeGearsDragNDroppable();
    }



    //Confirm Choice Function
    //called when making a choice to allow chonfirmation stage
    public void ChoiceConfirmation()
    {

    }

}
