using UnityEngine;

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
}
