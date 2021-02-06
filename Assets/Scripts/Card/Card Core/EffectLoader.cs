using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectLoader : MonoBehaviour
{
    public Card card;
    

    private void OnEnable()
    {

    }

    //Activate Effect should fetch the delegate itself
    public void EffectLoaderActivate(GameObject target)
    {
        //working old
        //card.Activate(target);   

        
        //ony for testing, JIGSAW EFFECTS ARE NOT TO BE EXECUTED DURING SINGLE CARD PLAY
        if (card.jigsawEffect!=null)    //jigsawEffect
        {
            ActivateJigsawEffect(target);
        }
        ActivateCardEffect(target);

    }

    public void ActivateCardEffect(GameObject target)
    {
        EffectFactory.GetCardEffect(card.enumCardName).CardEffectActivate(target);
    }

    public void ActivateJigsawEffect(GameObject target)
    {
        JigsawFactory.GetJigsawEffect(card.jigsawEffect.enumJigsawName).CardEffectActivate(target);  //jigsawEffect
    }


}
