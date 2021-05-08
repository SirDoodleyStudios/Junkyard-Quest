using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardsManager : MonoBehaviour
{
    //reward gameObject Prefabs
    public GameObject cardDraftPrefab;
    public GameObject scrapGainPrefab;
    public GameObject treasureGainPrefab;

    UniversalInformation universalInfo;

    //current list of rewards options
    public List<CombatRewards> rewardsList = new List<CombatRewards>();

    //dictionary of rewardsKey and rewardPrefab
    public Dictionary<CombatRewards, GameObject> rewardsRepository = new Dictionary<CombatRewards, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        universalInfo = UniversalSaveState.LoadUniversalInformation();

        rewardsRepository.Add(CombatRewards.CardDraft, cardDraftPrefab);
        rewardsRepository.Add(CombatRewards.Abilities, cardDraftPrefab);
        rewardsRepository.Add(CombatRewards.Scraps, scrapGainPrefab);
        //rewardsRepository.Add(CombatRewards.Treasures, treasureGainPrefab);

        LoadRewardChoicesList();
        LoadRewardObjects();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //for spawning the choices in the rewards panel
    void LoadRewardChoicesList()
    {
        if (universalInfo.nodeActivity == NodeActivityEnum.Combat)
        {
            rewardsList.Add(CombatRewards.CardDraft);
            rewardsList.Add(CombatRewards.Scraps);

            if (universalInfo.enemyCount == universalInfo.overkills)
            {
                rewardsList.Add(CombatRewards.CardDraft);
            }
        }

    }

    //for rigging the rewards objects in the panel
    void LoadRewardObjects()
    {
        for(int i = 0; rewardsList.Count > i; i++)
        {
            //gets the object in holder and assign an object prefab depending on the combatRewards key enumerated in rewardsList
            RewardObject rewardObject = transform.GetChild(i).gameObject.GetComponent<RewardObject>();
            rewardObject.AssignReward(rewardsList[i], rewardsRepository[rewardsList[i]]);

        }
    }






}
