using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneManager : MonoBehaviour
{
    public static RuneManager Instance { get; private set; }
    
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

    [Header("룬 데이터들")]
    [SerializeField] private List<RuneData> runeDatas = new List<RuneData>();

    [Header("룬 프리팹")]
    [SerializeField] private GameObject runePrefab;

    [Header("스폰된 룬")]
    [SerializeField] private Queue<RuneData> listedRunes = new Queue<RuneData>();

    [Header("스폰될 캔버스")]
    [SerializeField] private RectTransform runeCanvas;

    [Header("UI 매니저")]
    [SerializeField] private InGameUI gameUI;

    public void SpawnRune()
    {
        Vector2 spawnPos = GetRandomSpawnPosition();
        GameObject rune = Instantiate(runePrefab, runeCanvas);
        rune.GetComponent<Rune>().SetRunePrefab(GetRandomRuneData(), gameUI.GetListedRuneSlotRectTransfrom());

        rune.GetComponent<RectTransform>().anchoredPosition = spawnPos;
    }

    public RuneData GetRandomRuneData()
    {
        int num = Random.Range(0, runeDatas.Count);
        return runeDatas[num];
    }

    [Header("UI 스폰 영역")]
    [Tooltip("이 RectTransform의 앵커를 기준으로 하는 스폰 영역입니다.\n(x, y) = 좌하단 시작점, (width, height) = 너비와 높이")]
    public Rect spawnArea = new Rect(-300, -200, 600, 400); // 기본값: (-300, -200)에서 (300, 200)까지의 영역

    public Vector2 GetRandomSpawnPosition()
    {
        float randomX = Random.Range(spawnArea.xMin, spawnArea.xMax);
        float randomY = Random.Range(spawnArea.yMin, spawnArea.yMax);

        return new Vector2(randomX, randomY);
    }

    public void SaveToListedRune(GameObject rune)
    {
        RuneData _data = rune.GetComponent<Rune>().GetRuneData();

        listedRunes.Enqueue(_data);

        gameUI.UpdateRuneNumText(listedRunes.Count);
        gameUI.UpdateCurrentRuneIcon(listedRunes.Peek());
    }

    public void SkipToNextRune()
    {
        if (listedRunes.Count <= 0) return;

        RuneData _data = null;
        listedRunes.Dequeue();
        
        if (listedRunes.Count > 0)
        {
            _data = listedRunes.Peek();

        }

        gameUI.UpdateRuneNumText(listedRunes.Count);
        gameUI.UpdateCurrentRuneIcon(_data);
    }
}
