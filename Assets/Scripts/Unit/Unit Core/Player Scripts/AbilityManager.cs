using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour
{
    public Transform abilityPanel;
    //will also be used for saving and loading
    public List<AbilityFormat> abilityList = new List<AbilityFormat>();
    public Button EndTurnButton;

    //cache for PlayingFieldScript
    GameObject playingFieldObject;
    PlayingField playingField;
    

    private void Start()
    {
        //when end turn button is clicked, make it so that abilities are non-interactable
        EndTurnButton.onClick.AddListener(() => DisableAbilities());
        //parent of player gameObject is the playing Field
        playingFieldObject = gameObject.transform.parent.gameObject;
        playingField = playingFieldObject.GetComponent<PlayingField>();
        
    }
    

    public void InstallAbility(AbilityFormat abilityData)
    {
        foreach(Transform abilitySlot in abilityPanel)
        {
            //cache for Button children of ability panel
            Button abilityButton = abilitySlot.gameObject.GetComponent<Button>();
            //cache for icon image child of each abilityButton
            GameObject abilityIcon = abilityButton.transform.GetChild(0).gameObject;

            //if button is not interactable
            if (!abilityButton.IsInteractable())
            {
                abilityButton.interactable = true;
                //makes the icon copied from abilityformat and makes it visible
                abilityIcon.GetComponent<Image>().sprite = abilityData.abilityIconSprite;
                abilityIcon.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                //adds the received abilityData to the list of abilities
                //once assigned, it will also attach that abilityData to the curent button being checked in foreachloop
                //the abilityData assigned in the index will be sent to the activate function once the assigned button is clicked
                abilityList.Add(abilityData);
                abilityButton.onClick.AddListener(() => ActivateAbility(abilityList[abilitySlot.GetSiblingIndex()], abilityButton));
                break;
            }

        }
        //create some form of function here
        //if we get here, the ability is not activated and wasted instead
        Debug.Log("Ability Slots Full");
    }

    public void ActivateAbility(AbilityFormat abilityEffect, Button abilityButton)
    {
        
        if(abilityEffect != null)
        {
            //default target is the playing field because it has references to everyting under playing Field Objects
            //this gives us more flexibility since ability effects needs to look at everything

            //if requirement check returns true
            if (AbilityFactory.GetAbilityEffect(abilityEffect.enumAbilityName).RequirementCheck(playingField))
            {
                //activates effect then disables button because abilities are once per turn only
                //second parameter gameObject is for sending the gameObject as the actor
                AbilityFactory.GetAbilityEffect(abilityEffect.enumAbilityName).CardEffectActivate(playingFieldObject, gameObject);
                abilityButton.interactable = false;
            }
            else
            {
                Debug.Log("Requirement not Met");
            }

        }
    }

    //listens to EndTurnButton and resets cooldowns
    public void DisableAbilities()
    {
        foreach(Transform abilitySlot in abilityPanel)
        {
            Button abilityButton = abilitySlot.gameObject.GetComponent<Button>();
            if (abilityButton.transform.GetChild(0).GetComponent<Image>().sprite != null)
            {
                abilityButton.interactable = false;
            }
           
        }
    }

    public void EnableAbilities()
    {
        foreach (Transform abilitySlot in abilityPanel)
        {
            Button abilityButton = abilitySlot.gameObject.GetComponent<Button>();
            if (abilityButton.transform.GetChild(0).GetComponent<Image>().sprite != null)
            {
                abilityButton.interactable = true;
            }

        }
    }
}
