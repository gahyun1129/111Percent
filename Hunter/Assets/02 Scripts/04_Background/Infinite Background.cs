using UnityEngine;



public class InfiniteBackground : MonoBehaviour
{
    [Header("Background Layers")]
    [SerializeField] private BackgroundLayer[] layers;
    
    [Header("Player Reference")]
    [SerializeField] private BearController player;
    
    private Camera mainCamera;
    private GameData gameData;
    
    void Start()
    {
        mainCamera = Camera.main;
        
        // GameData 가져오기
        gameData = GameDataManager.Instance.GAMEDATA;
        
        // 플레이어 자동 찾기
        if (player == null)
        {
            player = FindObjectOfType<BearController>();
        }
        
        // 각 레이어의 너비 계산
        InitializeLayers();
    }
    
    void Update()
    {       
        // 배경 스크롤
        if (player.IsMoving())
        {
            ScrollLayers();
        }
    }
    
    // 레이어 초기화 (너비 계산 및 위치 설정)
    private void InitializeLayers()
    {
        foreach (var layer in layers)
        {
            if (layer.object1 == null || layer.object2 == null || layer.object3 == null) continue;
            
            // 레이어 너비 설정 (수동 설정이 없으면 자동 계산)
            if (layer.layerWidth <= 0f)
            {
                // 자동 계산: 모든 자식 SpriteRenderer의 범위를 계산
                Renderer[] renderers = layer.object1.GetComponentsInChildren<Renderer>();
                if (renderers.Length > 0)
                {
                    // 모든 렌더러의 바운드를 합쳐서 전체 너비 계산
                    Bounds combinedBounds = renderers[0].bounds;
                    for (int i = 1; i < renderers.Length; i++)
                    {
                        combinedBounds.Encapsulate(renderers[i].bounds);
                    }
                    layer.layerWidth = combinedBounds.size.x;
                    Debug.Log($"{layer.object1.name} 자동 계산된 너비: {layer.layerWidth}");
                }
                else
                {
                    Debug.LogWarning($"Renderer not found on {layer.object1.name}, 기본값 10 사용");
                    layer.layerWidth = 10f; // 기본값
                }
            }
            else
            {
                Debug.Log($"{layer.object1.name} 수동 설정된 너비 사용: {layer.layerWidth}");
            }
            
            layer.isWidthCalculated = true;
            
            // 초기 배치: object1은 가운데, object2는 왼쪽, object3는 오른쪽
            // object2를 object1 왼쪽에 배치
            layer.object2.position = new Vector3(
                layer.object1.position.x - layer.layerWidth,
                layer.object1.position.y,
                layer.object1.position.z
            );
            
            // object3를 object1 오른쪽에 배치
            layer.object3.position = new Vector3(
                layer.object1.position.x + layer.layerWidth,
                layer.object1.position.y,
                layer.object1.position.z
            );
        }
    }
    
    // 레이어들을 스크롤
    private void ScrollLayers()
    {
        if (gameData == null) return;
        
        foreach (var layer in layers)
        {
            if (layer.object1 == null || layer.object2 == null || layer.object3 == null) continue;
            
            // 플레이어 속도에 레이어별 배율을 곱해서 이동 속도 계산
            float moveAmount = gameData.playerSpeed * layer.speedMultiplier * Time.deltaTime;
            
            layer.object1.position += Vector3.right * moveAmount;
            layer.object2.position += Vector3.right * moveAmount;
            layer.object3.position += Vector3.right * moveAmount;
            
            // 무한 루프 처리
            CheckAndRepositionLayer(layer);
        }
    }
    
    // 레이어가 화면 밖으로 나가면 재배치
    private void CheckAndRepositionLayer(BackgroundLayer layer)
    {
        // 카메라의 오른쪽 끝 위치 계산
        float cameraRightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1, 0.5f, 0)).x;
        
        // 3개의 오브젝트 중 가장 왼쪽에 있는 것을 찾기
        Transform leftmost = layer.object1;
        if (layer.object2.position.x < leftmost.position.x) leftmost = layer.object2;
        if (layer.object3.position.x < leftmost.position.x) leftmost = layer.object3;
        
        // object1이 화면 오른쪽 밖으로 완전히 나갔는지 확인
        if (layer.object1.position.x - layer.layerWidth / 2 > cameraRightEdge)
        {
            // object1을 가장 왼쪽 오브젝트의 왼쪽으로 이동
            layer.object1.position = new Vector3(
                leftmost.position.x - layer.layerWidth,
                layer.object1.position.y,
                layer.object1.position.z
            );
        }
        
        // object2가 화면 오른쪽 밖으로 완전히 나갔는지 확인
        if (layer.object2.position.x - layer.layerWidth / 2 > cameraRightEdge)
        {
            // 가장 왼쪽 오브젝트 다시 찾기
            leftmost = layer.object1;
            if (layer.object2.position.x < leftmost.position.x) leftmost = layer.object2;
            if (layer.object3.position.x < leftmost.position.x) leftmost = layer.object3;
            
            // object2를 가장 왼쪽 오브젝트의 왼쪽으로 이동
            layer.object2.position = new Vector3(
                leftmost.position.x - layer.layerWidth,
                layer.object2.position.y,
                layer.object2.position.z
            );
        }
        
        // object3가 화면 오른쪽 밖으로 완전히 나갔는지 확인
        if (layer.object3.position.x - layer.layerWidth / 2 > cameraRightEdge)
        {
            // 가장 왼쪽 오브젝트 다시 찾기
            leftmost = layer.object1;
            if (layer.object2.position.x < leftmost.position.x) leftmost = layer.object2;
            if (layer.object3.position.x < leftmost.position.x) leftmost = layer.object3;
            
            // object3를 가장 왼쪽 오브젝트의 왼쪽으로 이동
            layer.object3.position = new Vector3(
                leftmost.position.x - layer.layerWidth,
                layer.object3.position.y,
                layer.object3.position.z
            );
        }
    }
    
    // 특정 레이어의 속도 배율 변경
    public void SetLayerSpeedMultiplier(int layerIndex, float multiplier)
    {
        if (layerIndex >= 0 && layerIndex < layers.Length)
        {
            layers[layerIndex].speedMultiplier = Mathf.Clamp(multiplier, 0f, 2f);
        }
    }
    
    // 특정 레이어의 속도 배율 가져오기
    public float GetLayerSpeedMultiplier(int layerIndex)
    {
        if (layerIndex >= 0 && layerIndex < layers.Length)
        {
            return layers[layerIndex].speedMultiplier;
        }
        return 0f;
    }
    
    // 레이어 개수 가져오기
    public int GetLayerCount()
    {
        return layers != null ? layers.Length : 0;
    }
    
    // 모든 레이어의 속도 배율 조정
    public void SetGlobalSpeedMultiplier(float multiplier)
    {
        foreach (var layer in layers)
        {
            layer.speedMultiplier = Mathf.Clamp(multiplier, 0f, 2f);
        }
    }
}
[System.Serializable]
public class BackgroundLayer
{
    [Header("Layer Objects")]
    public Transform object1; // 첫 번째 배경 오브젝트
    public Transform object2; // 두 번째 배경 오브젝트 (같은 모습)
    public Transform object3;
    
    [Header("Layer Settings")]
    [Tooltip("플레이어 속도 대비 배율 (1.0 = 플레이어와 같은 속도, 0.5 = 절반 속도)")]
    [Range(0f, 2f)]
    public float speedMultiplier = 1f; // 플레이어 속도 대비 배율
    
    [Tooltip("레이어의 너비 (0이면 자동 계산)")]
    public float layerWidth = 0f; // 레이어의 너비 (수동 설정 가능)
    
    [HideInInspector]
    public bool isWidthCalculated = false; // 너비가 계산되었는지 여부
}
