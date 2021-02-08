using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAction", menuName = "EnemyAction")]
public class EnemyActionFormat : ScriptableObject
{
    ////everytime a jigsaw is instantiated, inputs and outputs are going to be randomized
    //public override JigsawLink inputLink => inputLink;
    //public override JigsawLink outputLink => outputLink;

    ////THIS SHOULD BE ADDED AFTER RANDOMIZING
    //public override AllJigsaws enumJigsawName => enumJigsawName;

    //for enhance and debilitate, maybe a list?
    //for summon, a scriptable?
    //for special something special



    //everytime a jigsaw is instantiated, inputs and outputs are going to be randomized
    public JigsawLink inputLink;
    public JigsawLink outputLink;
    public EnemyActionType actionType;

    //THIS SHOULD BE ADDED AFTER RANDOMIZING
    public AllEnemyActions enumEnemyAction;

}

