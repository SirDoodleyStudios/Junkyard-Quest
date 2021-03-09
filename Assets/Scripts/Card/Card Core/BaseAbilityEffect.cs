using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAbilityEffect : BaseCardEffect
{
    public abstract AllAbilities enumKeyAbility { get; }

    //gets check first before the actual card effect to see if ability is available for use
    //PlayingField script is the accepted parameter since it has access to allobjects under Playing Field Object
    public abstract bool RequirementCheck(PlayingField playingField);
}
