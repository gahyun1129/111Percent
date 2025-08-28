using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject Player;
    public GameObject Enemy;

    public static GameManager Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void GameEnd(string loser)
    {
        if (loser == "Enemy")
        {
            Player.GetComponent<PlayerController>().WinPerformance();
        }
        else if (loser == "player")
        {
            Enemy.GetComponent<EnemyAI>().WinPerformance();
        }
    }

}
