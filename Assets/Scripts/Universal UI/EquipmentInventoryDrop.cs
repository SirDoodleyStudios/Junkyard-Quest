using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentInventoryDrop : MonoBehaviour /*IDropHandler*/
{
    //Assigned in editor
    //first position reference, this is taken from the equipped panel because equipped panel is fixed
    public GameObject firstEquippedElement;
    public GridLayoutGroup equippedGrid;
    public RectTransform equippedRect;
    //reference to the equipment viewer itself
    public EquipmentViewer equipmentViewer;
    //the invisible gameObject over the inventory screen that block raycast while tween animation is active
    public Image tweenCover;
    //the parent equipment viewer
    //public EquipmentViewer equipmentViewer;

    //the inventory grid, will be initialized first then turned off after so that the transforms of the elements are calculated
    public RectTransform inventoryRect;

    //holds all size references
    float inventoryScreenPadding;
    float inventoryScreenSpacing;
    float inventoryScreenElementWidth;
    float inventoryScreenElementHeight;
    float inventoryScreenHeight;
    float inventoryScreenWidth;

    //list of segments in the screen, used for dropping function
    //based on the screen size
    int segmentCount;
    List<float> inventoryScreenSegments = new List<float>();

    //identifier for first time run functions
    bool isInitialized;

    public void Awake()
    {
        DOTween.Init(true, true, LogBehaviour.Default);
        DOTween.SetTweensCapacity(20000, 100);

        //sets references to size from the equipmentGrid as reference
        inventoryScreenPadding = equippedGrid.padding.top;
        inventoryScreenSpacing = equippedGrid.spacing.y;
        inventoryScreenElementWidth = equippedGrid.cellSize.x;
        inventoryScreenElementHeight = equippedGrid.cellSize.y;



        //positions are mimicked from the GridLayout of the equipped list
        StartCoroutine(ResizeInventoryScreen());
    }

    //resize screen
    //will replace GridLayoutGroup and contentSizeFitter
    //will be called when inventory is going to be dropped to resize then repositon
    public IEnumerator ResizeInventoryScreen()
    {
        //make the cover block all raycast in inventory side
        tweenCover.raycastTarget = true;

        int inventoryCount = transform.childCount;

        //padding is for top and bottom
        inventoryScreenHeight = (inventoryScreenPadding * 2) + (inventoryScreenSpacing * (inventoryCount - 1)) + (inventoryScreenElementHeight * inventoryCount);

        //waits for a frame before proceeding to get the correct inventoryScreen width
        yield return new WaitForEndOfFrame();

        //set sizes that can only have value after frame end
        //also for sizes that will not change during the scene
        if (isInitialized == false)
        {
            SetRectSizes();
            isInitialized = true;
        }

        //sets the calculated sizes to the content view
        inventoryRect.sizeDelta = new Vector2(inventoryScreenWidth, inventoryScreenHeight);

        //actual repositioning of elements
        RepositionInventory();

        //after repositioning, turn tweencover off again
        tweenCover.raycastTarget = false;
    }
    //helper function
    //set RectSizes after the first frame end, must only be called at the beginning
    void SetRectSizes()
    {
        //detemines how many segments or how many full gear prefabs dcan fint in the screen
        segmentCount = Mathf.FloorToInt(Screen.height / inventoryScreenElementHeight);

        //used for determining the divisions
        //fist dividing point is the first, last is the height of the screen itself
        //this is in reverse because the coordinate of the segment on top is positive Y since origina of screen coordinate is at lower left
        //so first segment is actualy the highest screenPoint
        for (int i = segmentCount-1; 0 <= i; i--)
        {
            inventoryScreenSegments.Add(((float)i / segmentCount) * Screen.height);
        }
        //width of the inventory list area
        inventoryScreenWidth = equippedRect.rect.width;
    }


    //helper function to reposition the elements after resizing
    //also called independently by DragNDrop when returning during a rejected drop
    public void RepositionInventory()
    {
        //initial staring coordinates
        float objectX = (inventoryScreenWidth - inventoryScreenElementWidth) / 2;
        float objectY = -inventoryScreenPadding;

        //lagtime of tween
        float lagTime = .1f;
        for (int i =0; transform.childCount - 1 >= i; i++)
        {
            RectTransform objectRect = transform.GetChild(i).GetComponent<RectTransform>();
            objectRect.DOAnchorPos(new Vector2(objectX, objectY), lagTime, false);            

            //update objectY value for next object
            objectY = objectY - inventoryScreenElementHeight - inventoryScreenSpacing;
        }
    }

    //called when a gear is removed or added in list
    //function to calculate where to drop a gearPrefab back to the inventory screen
    //called by the EquipmentDragNDrop when the drag on the object ends
    public void PlaceGearInInventory(GameObject gearObj, float segmentYCoordinate)
    {
        //will contain the index segments that divides the screen
        List<int> indexList = new List<int>();
        //will be used for the for loop later in filling up the segmentList
        int startingIndex;

        //identifying what indices are assigned
        //scpecific scenario for position in the padding area - spacing so the the first item is included equally in the computation
        if(inventoryRect.anchoredPosition.y <= inventoryScreenPadding - inventoryScreenSpacing)
        {
            startingIndex = 0;
        }
        else
        {
            //total height minus the two paddings plus a spacing for the first object
            //dividing it by the spacing + element height will result to an int that will indicate on what item the first object in the screen is
            float calc = (inventoryRect.anchoredPosition.y - (inventoryScreenPadding - inventoryScreenSpacing)) / (inventoryScreenElementHeight + inventoryScreenSpacing);
            startingIndex = Mathf.RoundToInt(calc);
        }

        //populate the list with indices starting with index determined by formula
        //populate 3 as default but might change onece screen ratio fitting process occurs
        for (int i = 0; segmentCount > i; i++)
        {
            indexList.Add(i+ startingIndex);
        }

        //logic for adding the prefab in the list itself, taking up the position in the segment list depending on the received Y position from drop call
        //first element in segment list is the highest Y because it's on top
        int siblingIndex;
        Transform returningTrans = gearObj.transform;
        for (int i = 0; segmentCount > i; i++)
        {
            //if first segment is being checked, check with 0 as the min
            //break if a match is found
            if (i == 0)
            {
                if (Screen.height >= segmentYCoordinate && segmentYCoordinate >= inventoryScreenSegments[i])
                {
                    siblingIndex = indexList[i];
                    returningTrans.SetParent(transform);
                    returningTrans.SetSiblingIndex(siblingIndex);
                    break;
                }
            }
            else
            {
                if (inventoryScreenSegments[i-1] >= segmentYCoordinate && segmentYCoordinate >= inventoryScreenSegments[i])
                {
                    siblingIndex = indexList[i];
                    returningTrans.SetParent(transform);
                    returningTrans.SetSiblingIndex(siblingIndex);
                    break;
                }
            }


        }

        //call resize and reposition
        StartCoroutine(ResizeInventoryScreen());

        //the dragged EquipmentDragNDrop, calls the dragged gear to change it's origin enum
        EquipmentDragNDrop equippedDragged = gearObj.GetComponent<EquipmentDragNDrop>();
        //call the equipmentViewer to update the gearLists
        //this will only activate if the object originally came from gearslot
        if (equippedDragged.gearOrigin == EquipmentDragNDrop.GearOrigin.equipSlot)
        {
            equipmentViewer.MoveGearSOToInventory(equippedDragged.gearSO, equippedDragged.previousEquipmentSlot.GetSiblingIndex());
        }
        //redetermine origin after placing it back
        equippedDragged.DetermineGearOrigin();


    }


}
