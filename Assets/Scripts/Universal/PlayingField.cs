using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingField : MonoBehaviour
{
    public CombatManager combatManager;
    public DeckManager deckManager;

    public GameObject playerPrefab;
    public GameObject enemyHolder;
    public GameObject playerHand;
    public PlayerHand playerHandScript;
}
