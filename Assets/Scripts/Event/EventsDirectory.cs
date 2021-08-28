using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//choose 1 of 2 unknown materials
public class Event_TwoChestsOneKey : EventAbstractClass
{
    //constant amounts
    int bashDamage = 10;

    public override AllEvents eventEnum => AllEvents.TwoChestsOneKey;
    //called by the event Manager
    public override void ActivateEvent()
    {
        EventTextDeterminer();
        //base will reference files like universalInformation
        //will also handle the choices population
        base.ActivateEvent();

    }

    //called through the EventsManager 
    public override void EventChoiceMade(int buttonIndex)
    {
        //initial event
        if (eventSequence == 0)
        {
            //one chest is going to have another key inside it
            //0 for left, 1 for right
            int luckyChest = Random.Range(0, 2);
            //if the lucky chest is opened, player gets a material
            if (buttonIndex == luckyChest)
            {
                AlterMaterialInventory(true);
                eventSequence = 1;
            }
            //if not, player has the choice to break the chest that will injure you 
            else
            {
                eventSequence = 2;
            }
        }
        //second event choice whether if player was lucky or not
        else if (eventSequence == 1 || eventSequence == 2)
        {
            //if player chooses to bash the other chest for more loot
            if (buttonIndex == 0) 
            {
                //70% chance to find nothing
                int chance = Random.Range(0, 100);
                //if roll is lower than 70, lose HP and get nothing
                if (chance < 70)
                {
                    AlterHP(false, bashDamage);
                    eventSequence = 3;
                }
                //if roll reached 70
                else
                {
                    AlterMaterialInventory(true);
                    AlterHP(false, bashDamage);
                    eventSequence = 4;
                }
            }
            //player chooses to leave instead
            else
            {
                LeaveEvent();
            }
        }
        //if at the end of the event
        else
        {
            //only one buton for leave at the final event
            if(buttonIndex == 0)
            {
                LeaveEvent();
            }
        }
        //base must be at the end because the determiner for populating the texts should be done after the logics
        base.EventChoiceMade(buttonIndex);

    }
    //used for follow up choices or ending texts in events
    public override void EventTextDeterminer()
    {
        //base clears the text list first
        base.EventTextDeterminer();

        //initial event texts
        if (eventSequence == 0)
        {
            //assign texts unique per event
            eventDescription = "You see two identical locked chests and a single key placed on a small rock between them. " +
                "It looks like you can open either of them using the key. Which one will you open?";
            eventChoices.Add("Open the right chest with the key");
            eventChoices.Add("Open the left chest with the key");
            choiceCount = 2;
        }
        //followup if the lucky chest is chosen
        else if (eventSequence == 1)
        {
            //assign texts unique per event
            eventDescription = "You opened the chest. Inside, you see a useful looking material. Awesome!";
            eventChoices.Add($"Bash the remaining chest open for more loot (Take {bashDamage} damage)");
            eventChoices.Add("Move along with high spirits");
            choiceCount = 2;
        }
        //event when the chosen chest is empty
        else if(eventSequence == 2)
        {
            //assign texts unique per event
            eventDescription = "You opened the chest. There's nothing inside.";
            eventChoices.Add($"Bash the remaining chest open, you deserve loot! (Take {bashDamage} damage)");
            eventChoices.Add("Leave the area thinking that it's just not your day today.");
            choiceCount = 2;
        }
        //event when no loot was gained from the two chests
        else if (eventSequence == 3)
        {
            //assign texts unique per event
            eventDescription = "You bashed the chest open leaving your hand with bruises only to find nothing but the scattered fragments of the chest.";
            eventChoices.Add("Leave the area");
            choiceCount = 1;
        }
        //event when the remaining chest has loot
        else if (eventSequence == 4)
        {
            //assign texts unique per event
            eventDescription = "You bashed the chest open leaving your hand with bruises. It was worth it! you find a useful looking material Inside";
            eventChoices.Add("Leave the area with a smile");
            choiceCount = 1;
        }

        PopulateChoices();

    }

}
