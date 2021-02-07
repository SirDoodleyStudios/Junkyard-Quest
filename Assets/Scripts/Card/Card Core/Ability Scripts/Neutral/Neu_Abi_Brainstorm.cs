using UnityEngine;
//Ability Effects have a requirement check function unique to it because effects and requirements can vary greatly per ability
//Recover creativity at the expense of energy
public class Neu_Abi_Brainstorm: BaseAbilityEffect
{
    public override AllCards enumKeyCard => AllCards.Ability;
    public override AllAbilities enumKeyAbility => AllAbilities.Neu_Abi_Brainstorm;

    public override void CardEffectActivate(GameObject target)
    {
        energy = -2;
        creativity = 2;
        AffectPlayingField(target);
        AlterEnergy();
        AlterCreativity();
    }

    public override bool RequirementCheck(GameObject targetPlayingField)
    {
        energy = -2;
        PlayingField playField = targetPlayingField.GetComponent<PlayingField>();
        //-energy because its cost
        if(playField.combatManager.Energy >= -energy)
        {
            return true;
        }
        else
        {
            return false;
        }

        
    }
}
