using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {PreGame, Playing, GameOver};

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject Player;
    public GameObject Enemy;


    public GameState state = GameState.PreGame;
    private float gameTime = 0f;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(UIManager.Instance.ShowCountdown(3));
    }

    private void Update()
    {
        if (state == GameState.Playing)
        {
            gameTime += Time.deltaTime;
        }
    }

    public void StartGameRoutine()
    { 
        gameTime = 0f;
        state = GameState.Playing;

        UIManager.Instance.StartGameTimer();
    }


    public void GameOver(string loser)
    {
        state = GameState.GameOver;
        UIManager.Instance.countDownText.gameObject.SetActive(true);
        if (loser == "Enemy")
        {
            Player.GetComponent<PlayerController>().WinPerformance();
            UIManager.Instance.countDownText.text = "VICTORY";
        }
        else if (loser == "player")
        {
            Enemy.GetComponent<EnemyAI>().WinPerformance();
            UIManager.Instance.countDownText.text = "LOSE...";
        }
    }

    public bool IsPlaying => state == GameState.Playing;
    public float GameTime => gameTime;
    public GameObject GetEnemy() => Enemy;
    public GameObject GetPlayer() => Player;    

}
