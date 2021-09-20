using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseCardEffect
{

    //Scanned by EffectFactory for identifying which Effect class to activate
    public abstract AllCards enumKeyCard { get; }
    //Scanned by Effect factory when  enumKeyName is Jigsaw
    //public abstract AllJigsaws enumKeyJigsaw { get; }

    //generic cache for target Game Object
    protected GameObject targetObject;
    //cache for playing field, will get initialized if AffectPlayingField is called
    PlayingField targetPlayingField;
    //cache for BaseUnitFunctions applicable to all units, will get initialized
    BaseUnitFunctions targetUnit;
    BaseUnitFunctions actorUnit;

    //cache for UnitStatusHolder, this will be the reference after assigning it
    protected UnitStatusHolder actorUnitStatus;
    protected UnitStatusHolder targetUnitStatus;

    //cache for Cards that need the information in card itself
    protected Card card;

    //ticked if an offense card is dropped I think
    protected bool AOE;
    //will be used as indicator whether gameBOject is to be discarderd at end turn
    protected bool keep;
    //will be used to determine if the method being called is for card or enemy
    //MAYBE STATUSES ARE ONLY AFFECTED BY CARDS OR ENEMYACTIONS
    protected bool isCardOrEnemyAction;
    //indicator if an enemy is slain during card activate, cards will have an if(isSlain) segment then add the effects in the card script itself
    protected bool isSlain;

    //variables for values
    protected int damage;
    protected int block;
    protected int hits;
    protected int heal;
    protected int multiplier;
    protected int draw;
    protected int discard;
    protected int creativity;
    protected int energy;

    //base variables are for checking. baseStatus is what status is going to be checked for card effect
    //baseStack is for chedcking how much status stack is needed for effect, mostly used by MOMENTUM
    protected CardMechanics baseStatus;
    protected int baseStack;

    //status and stack that will actually be applied to unit
    protected CardMechanics status;
    protected int stack;





    //Activator Function for cards with actual card overload
    public virtual void CardEffectActivate(GameObject target, GameObject actor, Card card)
    {

    }

    //for Jigsaw and enemy effects
    public virtual void CardEffectActivate(GameObject target, GameObject actor)
    {

    }

    public virtual void ActingCardLoad(Card actingCard)
    {
        card = actingCard;
    }

    //NEEDS TO BE CALLED BY EFFECT LOADER
    //the game object sent is determined in combat manager, if the effectloader is called during player phase, Acting unit will be player
    //if Effect loader is called during enemyAI, then acting unit sent is the enemy gameObject
    public virtual void ActingUnitStatusLoad(GameObject actingUnit)
    {
        actorUnitStatus = actingUnit.GetComponent<UnitStatusHolder>();
        actorUnit = actingUnit.GetComponent<BaseUnitFunctions>();

        //the identifier for card play is here because this funcion is always being called for all cards
        if (actorUnitStatus.isPlayCounting)
        {
            actorUnitStatus.AlterStatusStackByPlayCounter(card);
        }
    }

    //set target to actual choice target
    public  void AffectSingleEnemy(GameObject target)
    {
        targetObject = target;
        targetUnit = targetObject.GetComponent<BaseUnitFunctions>();
        targetUnitStatus = targetObject.GetComponent<UnitStatusHolder>();
    }

    //set target to player when card is dropped
    //Child index 1 is player
    public void AffectPlayer(GameObject target)
    {
        if (target.tag == "Player")
        {
            targetObject = target;
            targetUnit = targetObject.GetComponent<PlayerFunctions>();
            targetUnitStatus = targetObject.GetComponent<UnitStatusHolder>();
        }
        else if (target.tag == "Enemy")
        {
            //gets parent enemy holder then gets parent playing field
            GameObject enemyHolder = target.transform.parent.gameObject;
            targetObject = enemyHolder.transform.parent.gameObject.transform.GetChild(1).gameObject;
            targetUnit = targetObject.GetComponent<PlayerFunctions>();
            targetUnitStatus = targetObject.GetComponent<UnitStatusHolder>();
        }
        //if drop field
        else
        {
            targetObject = target.transform.parent.GetChild(1).gameObject;
            targetUnit = targetObject.GetComponent<PlayerFunctions>();
            targetUnitStatus = targetObject.GetComponent<UnitStatusHolder>();
        }

    }

    //set all enemies as tagets when card is dropped
    //TargetUnit will be the EnemyHolder
    //CHANGE LOGIC, THIS IS WRONG
    public void AffectAllEnemies(GameObject target)
    {
        //sets AOE effect
        AOE = true;
        if (target.tag == "Enemy")
        {
            //gets parent enemy holder then gets parent playing field
            targetObject = target.transform.parent.gameObject;
            targetUnit = targetObject.GetComponent<BaseUnitFunctions>();
            //targetUnitStatus = targetObject.GetComponent<UnitStatusHolder>();
        }
        else
        {
            //child index 2 is the enemy holder
            targetObject = target.transform.parent.GetChild(2).gameObject;
            targetUnit = targetObject.GetComponent<BaseUnitFunctions>();
            //targetUnitStatus = targetObject.GetComponent<UnitStatusHolder>();
        }
        //for dropfield approach
        //child index 2 is the enemy holder
        //targetUnit = target.transform.parent.GetChild(2).gameObject;

    }

    //for effects that affects combat like draw and energy gain
    //layer 13 are all objects under PlayingField
    public void AffectPlayingField(GameObject target)
    {

        //checks if the target itself is the PlayingField
        if (target.GetComponent<PlayingField>() != null)
        {
            targetObject = target;
            targetUnit = targetObject.GetComponent<BaseUnitFunctions>();
            targetPlayingField = target.GetComponent<PlayingField>();

        }
        //checks if the target's parent is the PlayingField
        //target must be either player or EnemyHolder
        else if (target.transform.parent.gameObject.GetComponent<PlayingField>() != null)
        {
            targetObject = target.transform.parent.gameObject;
            targetUnit = targetObject.GetComponent<BaseUnitFunctions>();
            targetPlayingField = targetObject.GetComponent<PlayingField>();
        }
        //checks if the target is an enemy
        //gets parent of parent for PlayingField
        else if (target.tag == "Enemy")
        {
            //gets parent enemy holder then gets parent playing field
            GameObject enemyHolder = target.transform.parent.gameObject;
            targetObject = enemyHolder.transform.parent.gameObject;
            targetUnit = targetObject.GetComponent<BaseUnitFunctions>();
            targetPlayingField = targetObject.GetComponent<PlayingField>();
        }
    }

    //passing the animation sprite to be tweened
    //passes to EffectAnimationManager
    public void PlayEffectAnimation()
    {

    }

    //damage needds to be populated
    //hits needs to be populated
    public void DealDamage()
    {
        //calls the unitStatusHolder to calculate damage based on existing statuses on unit actor

        int modifiedDamage = actorUnitStatus.DamageDealingModifierCalculator(damage);

        if(AOE == false)
        {
            for (int i = 1; hits >= i; i++)
            {
                //targetUnit.TakeDamage(damage);
                targetUnit.TakeDamage(modifiedDamage);
                //if the hit counter in actor's UnitStatusHolder is active, start counting hits
                if (actorUnitStatus.isHitCounting)
                {
                    actorUnitStatus.AlterStatusStackByHitCounter();
                }

                //checks if the the target has any return Damage statuses like counter and spikes
                //if so, deal damage to actor depending on amount calculated with ReturnDamangeCalculator
                int returnDamage = targetUnitStatus.ReturnDamageCalculator(modifiedDamage);
                if (returnDamage > 0)
                {
                    actorUnit.TakeDamage(returnDamage);
                }
            }

        }
        //AOE true affects all enemies
        else
        {
            foreach (Transform enemy in targetObject.transform)
            {
                //checks first if the enemy object is disabled so that it's scripts doesnt get called during AOE effects
                if (enemy.gameObject.activeSelf)
                {
                    //this cache is for accessing unitStatusholder of all enemies in enemy holder
                    targetUnitStatus = enemy.gameObject.GetComponent<UnitStatusHolder>();
                    //cache override from AffectAll enemies
                    targetUnit = enemy.gameObject.GetComponent<BaseUnitFunctions>();
                    for (int i = 1; hits >= i; i++)
                    {
                        //targetUnit.TakeDamage(damage);
                        targetUnit.TakeDamage(modifiedDamage);
                        //if the hit counter in actor's UnitStatusHolder is active, start counting hits
                        if (actorUnitStatus.isHitCounting)
                        {
                            actorUnitStatus.AlterStatusStackByHitCounter();
                        }

                        //checks if the the target has any return Damage statuses like counter and spikes
                        //if so, deal damage to actor depending on amount calculated with ReturnDamangeCalculator
                        int returnDamage = targetUnitStatus.ReturnDamageCalculator(modifiedDamage);
                        if (returnDamage > 0)
                        {
                            actorUnit.TakeDamage(returnDamage);
                        }
                    }

                }
            }
            AOE = false;
        }
    }
    

    public void GainBlock()
    {
        int modifiedBlock = actorUnitStatus.BlockModifierCalculator(block);
        targetUnit.GainBlock(modifiedBlock);
    }

    public void HealHealth()
    {
        targetUnit.HealHealth(heal);
    }
    //this a hack
    public void DrawCard()
    {
        targetPlayingField.deckManager.DrawCards(draw);
        //targetPlayingField.deckManager.StartCoroutine(targetPlayingField.deckManager.DrawCards(draw));
        targetPlayingField.playerHandScript.StateChanger(CombatState.PlayerTurn);
    }

    public void DiscardCard()
    {

    }

    public void AlterCreativity()
    {
        //make this applicable to enemies as well
        targetPlayingField.playerPrefab.GetComponent<PlayerFunctions>().AlterCreativity(creativity);

    }

    public void AlterEnergy()
    {
        targetPlayingField.combatManager.EnergyUpdater(energy);
    }

    public void GainAbility()
    {

    }

    public void Attune()
    {

    }

    public void Discover()
    {

    }

    public void Deplete()
    {

    }

    public void ApplyStatus()
    {
        if (targetObject.GetComponent<UnitStatusHolder>()!=null)
        {
            //targetObject.GetComponent<UnitStatusHolder>().AlterStatusStack(enumKey, stack);
            targetObject.GetComponent<UnitStatusHolder>().AlterStatusStack(status, stack);
        }
        
    }

    //AlterStatusNextTurn updates the UnitStatusHolder's TurnStatusUpdate which is run as part of the event turn start
    public void ApplyStatusByCounter()
    {
        if (targetObject.GetComponent<UnitStatusHolder>() != null)
        {
            targetObject.GetComponent<UnitStatusHolder>().AddStatusStackToCountingDict(status, stack);
        }
    }

    //check if the target is still enabled or not
    //not enabled means it met its death
    public void SlayCheck()
    {
        if (!targetObject.activeSelf)
        {
            isSlain = true;
        }
    }












}
