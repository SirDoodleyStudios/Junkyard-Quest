using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//THIS SCRIPT IS TO BE GENERATED AND ASSIGNED TO THE INVENTORY ITEMS DURING TRADING
public class TradeDragNDrop : MonoBehaviour, IPointerClickHandler
{
    //reference to tradeActivities
    //this is assiged immediately after generatting the script
    public TradeActivities tradeActivities;
    

    //called immediately when the gameObject is created
    public void AssignTradeReference(TradeActivities referenceTrade)
    {
        tradeActivities = referenceTrade;
        tradeActivities.d_DisableInventoryObjects += DisableObject;
    }

    //WILL BE USED IF WE DECIDE TO SKIP CONFIRMATION STAGE IN CHOOSING
    public void OnPointerClick(PointerEventData eventData)
    {
        tradeActivities.ConfirmTrade(gameObject);
    }
    public void DisableObject()
    {
        tradeActivities.d_DisableInventoryObjects -= DisableObject;
        gameObject.SetActive(false);
    }

    //WILL BE USED IF WE DECIDE TO HAVE CONFIRMATION STAGE IN CHOOSING
    //TO BE USED WITH EVENT SYSTEMS

    //called by the tradeActivities when an inventory object is chosen for the trade
    public void ChooseHighlight()
    {

    }
    //
    public void UnchooseDehighlight()
    {

    }

}
