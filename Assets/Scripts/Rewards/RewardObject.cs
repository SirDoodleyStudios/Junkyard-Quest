using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RewardObject : MonoBehaviour
{
    //contains the reward prefab to be executed
    public GameObject rewardPrefab;
    Image rewardImage;
    CombatRewards rewardEnum;

    public void Awake()
    {
        rewardImage = gameObject.GetComponent<Image>();
    }

    public void AssignReward(CombatRewards rewardKey, GameObject reward)
    {
        rewardEnum = rewardKey;
        rewardPrefab = reward;
        //assign image from Resources, the text shoud be ezactly the same with enum
        rewardImage.sprite = Resources.Load<Sprite>($"Rewards/{rewardKey}");
    }

    public void InstantiateChoiceWindow()
    {
        //instantiate the scrap drafting object under the canvas object
        GameObject rewardWindow = Instantiate(rewardPrefab, transform.parent.parent);
        if (rewardEnum == CombatRewards.Scraps)
        {
            //test value only
            rewardWindow.transform.GetComponent<ScrapsDrafting>().InitializeScrapValue(60);   
        }
    }






}
