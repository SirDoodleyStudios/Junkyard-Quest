using UnityEngine;

public abstract class EnemyActionsLogic : MonoBehaviour
{
    //NOT NEEDED, USABLE FUNCTIONS ARE MIGRATED TO ENEMYFUNCTIONS SCRIPT
    //determines the cycle of actions for enemy turn
    protected int actionIndex;
    protected int cycleIndex;

    protected GameObject targetUnit;
    protected BaseUnitFunctions targetUnitFunctions;

    //All inherited classes will use this as the trigger for an enemy action
    public abstract void EnemyAct();

    public void TargetPlayer()
    {
        //goes to parent which is enemyHolder then goes to parent which is Playing Field then get Child of index 1 which is the player
        targetUnit = gameObject.transform.parent.gameObject.transform.parent.GetChild(1).gameObject;
    }
    public void TargetSelf()
    {
        targetUnit = gameObject;
    }
    public void TargetEnemy()
    {

    }
    public void TargetAllEnemies()
    {

    }



    public void DealDamage(int damageValue)
    {
        targetUnit.GetComponent<BaseUnitFunctions>().TakeDamage(damageValue);
    }

    public void GainBlock(int blockValue)
    {
        targetUnit.GetComponent<BaseUnitFunctions>().GainBlock(blockValue);
    }

    public void HealHealth()
    {

    }

    public void GetBuff()
    {

    }

    public void DealEffect()
    {

    }



}
