using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private int stageNum = 0;
    private int enemyNum = 10;

    private float gameTime = 0f;
    private bool gameStart = false;

    private void Start()
    {
        EnemyManager.instance.StartSpawnEnemy(enemyNum);
        
    }

    private void Update()
    {
        if ( gameStart)
        {
            gameTime += Time.deltaTime;
        }
    }

    public float GameTime => gameTime;

    public void SetGameState(bool _game) => gameStart = _game;

    public bool GameStart => gameStart;

}
