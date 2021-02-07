using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Ability")]
//ability romat will inherit from jigsaw so that they can use the same variable when sending the scriptable
//Abilities cannot be given jigsaw effects
public class AbilityFormat : ScriptableObject
{
    public Sprite abilityIconSprite;
    public AllAbilities enumAbilityName;

}
