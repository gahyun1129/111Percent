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
    [SerializeField] private List<GameObject> runes;

    [Header("스폰될 캔버스")]
    [SerializeField] private RectTransform runeCanvas;

    public void SpawnRune()
    {
        Vector2 spawnPos = GetRandomSpawnPosition();
        GameObject rune = Instantiate(runePrefab, runeCanvas);
        rune.GetComponent<Rune>().SetRuneData(GetRandomRuneData());

        rune.GetComponent<RectTransform>().anchoredPosition = spawnPos;
        runes.Add(rune);
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

    //private void OnDrawGizmosSelected()
    //{
    //    RectTransform rectTransform = runeCanvas.GetComponent<RectTransform>();

    //    // 기즈모 색상 설정
    //    Gizmos.color = new Color(0, 1, 0, 0.5f); // 반투명 녹색

    //    // spawnArea Rect를 RectTransform의 로컬 좌표에서 월드 좌표로 변환
    //    // 앵커가 중앙일 때를 기준으로 계산
    //    Vector3 center = rectTransform.TransformPoint(spawnArea.center);
    //    Vector3 size = rectTransform.TransformVector(new Vector2(spawnArea.width, spawnArea.height));

    //    // Gizmos는 3D 큐브만 그릴 수 있으므로 얇은 큐브로 표시
    //    size.z = 0.1f;

    //    Gizmos.DrawWireCube(center, size);
    //}
}
