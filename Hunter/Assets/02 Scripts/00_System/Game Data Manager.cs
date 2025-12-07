using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    [SerializeField] private GameData gameData;
    [SerializeField] private ThrowData throwData;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public GameData GAMEDATA => gameData;
    public ThrowData THROWDATA => throwData;
}
