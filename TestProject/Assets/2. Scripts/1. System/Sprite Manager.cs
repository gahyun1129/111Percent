using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스프라이트를 이름으로 관리하고 로드하는 매니저
/// Resources 폴더의 스프라이트를 캐싱하여 빠르게 접근 가능
/// </summary>
public class SpriteManager : MonoBehaviour
{
    public static SpriteManager Instance { get; private set; }

    [Header("스프라이트 경로 설정")]
    [SerializeField]
    private string[] spriteFolderPaths = new string[]
    {
    };

    [Header("디버그")]
    [SerializeField] private bool logLoadedSprites = false;

    // 스프라이트 캐시: 이름 -> Sprite
    private Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAllSprites();
    }

    /// <summary>
    /// 모든 스프라이트를 Resources 폴더에서 로드하여 캐시에 저장
    /// </summary>
    private void LoadAllSprites()
    {
        spriteCache.Clear();
        int totalLoaded = 0;

        foreach (string folderPath in spriteFolderPaths)
        {
            Debug.Log($"[SpriteManager] Attempting to load from: {folderPath}");

            // Object 타입으로 먼저 로드해서 무엇이 있는지 확인
            Object[] allObjects = Resources.LoadAll(folderPath);
            Debug.Log($"[SpriteManager] Found {allObjects.Length} objects in {folderPath}");

            Sprite[] sprites = Resources.LoadAll<Sprite>(folderPath);
            Debug.Log($"[SpriteManager] Found {sprites.Length} sprites in {folderPath}");

            foreach (Sprite sprite in sprites)
            {
                if (sprite != null)
                {
                    // 중복 이름 체크
                    if (spriteCache.ContainsKey(sprite.name))
                    {
                        Debug.LogWarning($"[SpriteManager] Duplicate sprite name found: '{sprite.name}' in {folderPath}. Using the first occurrence.");
                    }
                    else
                    {
                        spriteCache[sprite.name] = sprite;
                        totalLoaded++;

                        if (logLoadedSprites)
                        {
                            Debug.Log($"[SpriteManager] Loaded: {sprite.name} from {folderPath}");
                        }
                    }
                }
            }
        }

        Debug.Log($"[SpriteManager] Total sprites loaded: {totalLoaded}");
    }

    /// <summary>
    /// 이름으로 스프라이트를 가져옵니다
    /// </summary>
    /// <param name="spriteName">스프라이트 이름</param>
    /// <returns>찾은 스프라이트, 없으면 null</returns>
    public Sprite GetSprite(string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName))
        {
            Debug.LogWarning($"[SpriteManager] {spriteName} Sprite name is null or empty.");
            return null;
        }

        if (spriteCache.TryGetValue(spriteName, out Sprite sprite))
        {
            return sprite;
        }

        Debug.LogWarning($"[SpriteManager] Sprite not found: '{spriteName}'");
        return null;
    }

    /// <summary>
    /// 이름으로 스프라이트를 가져오며, 없으면 기본 스프라이트를 반환합니다
    /// </summary>
    /// <param name="spriteName">스프라이트 이름</param>
    /// <param name="defaultSprite">찾지 못했을 때 반환할 기본 스프라이트</param>
    /// <returns>찾은 스프라이트 또는 기본 스프라이트</returns>
    public Sprite GetSpriteOrDefault(string spriteName, Sprite defaultSprite = null)
    {
        Sprite sprite = GetSprite(spriteName);
        return sprite != null ? sprite : defaultSprite;
    }

    /// <summary>
    /// 특정 경로에서 스프라이트를 직접 로드합니다 (캐시에 없을 경우)
    /// </summary>
    /// <param name="spriteName">스프라이트 이름</param>
    /// <param name="resourcePath">Resources 폴더 기준 경로</param>
    /// <returns>로드된 스프라이트</returns>
    public Sprite LoadSpriteFromPath(string spriteName, string resourcePath)
    {
        // 먼저 캐시 확인
        if (spriteCache.TryGetValue(spriteName, out Sprite cachedSprite))
        {
            return cachedSprite;
        }

        // 캐시에 없으면 직접 로드
        Sprite sprite = Resources.Load<Sprite>($"{resourcePath}/{spriteName}");

        if (sprite != null)
        {
            spriteCache[spriteName] = sprite;
            Debug.Log($"[SpriteManager] Loaded sprite '{spriteName}' from path: {resourcePath}");
        }
        else
        {
            Debug.LogWarning($"[SpriteManager] Failed to load sprite '{spriteName}' from path: {resourcePath}");
        }

        return sprite;
    }

    /// <summary>
    /// 스프라이트 캐시를 다시 로드합니다
    /// </summary>
    public void ReloadSprites()
    {
        Debug.Log("[SpriteManager] Reloading all sprites...");
        LoadAllSprites();
    }

    /// <summary>
    /// 특정 스프라이트가 캐시에 있는지 확인합니다
    /// </summary>
    /// <param name="spriteName">스프라이트 이름</param>
    /// <returns>존재 여부</returns>
    public bool HasSprite(string spriteName)
    {
        return !string.IsNullOrEmpty(spriteName) && spriteCache.ContainsKey(spriteName);
    }

    /// <summary>
    /// 캐시된 모든 스프라이트 이름을 반환합니다
    /// </summary>
    /// <returns>스프라이트 이름 목록</returns>
    public List<string> GetAllSpriteNames()
    {
        return new List<string>(spriteCache.Keys);
    }

    /// <summary>
    /// 캐시된 스프라이트 개수를 반환합니다
    /// </summary>
    /// <returns>스프라이트 개수</returns>
    public int GetSpriteCount()
    {
        return spriteCache.Count;
    }

#if UNITY_EDITOR
    /// <summary>
    /// 에디터 전용: 모든 캐시된 스프라이트 정보를 출력합니다
    /// </summary>
    [ContextMenu("Print All Cached Sprites")]
    private void PrintAllCachedSprites()
    {
        Debug.Log($"[SpriteManager] Total cached sprites: {spriteCache.Count}");
        foreach (var kvp in spriteCache)
        {
            Debug.Log($"  - {kvp.Key}");
        }
    }
#endif
}
