using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour
{
    public Transform abilityPanel;
    List<AbilityFormat> abilityList = new List<AbilityFormat>();

    private void Start()
    {
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
                abilityButton.onClick.AddListener(() => ActivateAbilitySlot(abilityList[abilitySlot.GetSiblingIndex()]));
                break;
            }

        }
        //create some form of function here
        //if we get here, the ability is not activated and wasted instead
        Debug.Log("Ability Slots Full");
    }

    public void ActivateAbilitySlot(AbilityFormat abilityEffect)
    {
        
        if(abilityEffect != null)
        {
            //default target is the playing field because it has references to everyting under playing Field Objects
            //this gives us more flexibility since ability effects needs to look at everything
            GameObject playingField = gameObject.transform.parent.gameObject;

            //if requirement check returns true
            if (AbilityFactory.GetAbilityEffect(abilityEffect.enumAbilityName).RequirementCheck(playingField))
            {
                AbilityFactory.GetAbilityEffect(abilityEffect.enumAbilityName).CardEffectActivate(playingField);
            }
            else
            {
                Debug.Log("not enough Energy");
            }

        }
    }
}
