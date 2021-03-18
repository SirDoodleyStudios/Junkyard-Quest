using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectLoader : MonoBehaviour
{
    //deckManager will assign this when card is enabled in hand
    public Card card;
    //Cache for the effect animation manager
    EffectAnimationManager eAnimationManager;
    private void Awake()
    {
        eAnimationManager = gameObject.GetComponent<EffectAnimationManager>();
    }

    private void OnEnable()
    {

    }

    public void SetActingUnit()
    {

    }
    //THE ACTOR PARAMETERS ARE FOR PASSING THE PLAYER GAMEOBJECT SO THAT CARD EFFECTS CAN CHECK STATUSES FIRST BEFORE ACTIVATING
    //Used for a mjore generic approach
    public void EffectLoaderActivate(GameObject target, GameObject actor)
    {
        //working old
        //card.Activate(target);  

        
        //ony for testing, JIGSAW EFFECTS ARE NOT TO BE EXECUTED DURING SINGLE CARD PLAY
        if (card.jigsawEffect!=null)    //jigsawEffect
        {
            //ActivateJigsawEffect(target);
            //ActivateCardEffect(target);
            ActivateJigsawEffect(target, actor);
            ActivateCardEffect(target, actor);
        }
        else if (card.abilityEffect != null)
        {
            AssignAbility(target);
        }
        else
        {
            ActivateCardEffect(target, actor);
        }
    }

    //target does not necessarily mean that the correct target is chosen, its just the game object clicked during play
    public void ActivateCardEffect(GameObject target, GameObject actor)
    {
        EffectFactory.GetCardEffect(card.enumCardName).CardEffectActivate(target, actor);
    }

    public void ActivateJigsawEffect(GameObject target, GameObject actor)
    {
        JigsawFactory.GetJigsawEffect(card.jigsawEffect.enumJigsawName).CardEffectActivate(target, actor);  //jigsawEffect
    }

    public void AssignAbility(GameObject targetPlayer)
    {
        targetPlayer.GetComponent<AbilityManager>().InstallAbility(card.abilityEffect);
    }


}
