using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    #region Singleton
    public static EffectManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Initialize();
    }
    #endregion

    [System.Serializable]
    public class EffectPair
    {
        public EffectType type;
        [Tooltip("이 타입이 실행될 때 동시에 재생될 모든 프리팹들을 넣으세요.")]
        public GameObject[] effectPrefabs;
    }

    [Header("Effect Settings")]
    [SerializeField] private List<EffectPair> effectDataList = new List<EffectPair>();

    // 풀 관리용 딕셔너리
    private Dictionary<EffectType, Queue<EffectBase>> _effectPool = new Dictionary<EffectType, Queue<EffectBase>>();
    private Transform _poolRoot;

    private void Initialize()
    {
        _effectPool.Clear();
        GameObject rootObj = new GameObject("@Effect_Root");
        rootObj.transform.SetParent(this.transform);
        _poolRoot = rootObj.transform;
    }

    public void PlayEffect(EffectType type, Vector3 position, Vector3 scale)
    {
        EffectBase effect = GetEffect(type);
        if (effect != null)
        {
            effect.Play(position, scale);
        }
    }

    private EffectBase GetEffect(EffectType type)
    {
        if (!_effectPool.ContainsKey(type))
        {
            _effectPool.Add(type, new Queue<EffectBase>());
        }

        if (_effectPool[type].Count > 0)
        {
            EffectBase pooledEffect = _effectPool[type].Dequeue();
            // 혹시 파괴되었을 경우 방어 코드
            if (pooledEffect == null) return GetEffect(type);
            return pooledEffect;
        }
        else
        {
            return CreateNewEffect(type);
        }
    }

    // ★ 여기가 가장 중요한 변경점입니다 ★
    private EffectBase CreateNewEffect(EffectType type)
    {
        var data = effectDataList.Find(x => x.type == type);

        if (data == null || data.effectPrefabs == null || data.effectPrefabs.Length == 0)
        {
            Debug.LogWarning($"[EffectManager] {type} 데이터가 없거나 프리팹이 비어있습니다.");
            return null;
        }

        // 1. 빈 컨테이너(부모) 생성
        GameObject container = new GameObject($"{type}_EffectGroup");
        container.transform.SetParent(_poolRoot);

        // 2. 배열에 있는 모든 프리팹을 컨테이너의 자식으로 생성
        foreach (GameObject prefab in data.effectPrefabs)
        {
            if (prefab != null)
            {
                GameObject child = Instantiate(prefab, container.transform);
                // 자식의 로컬 위치/회전은 프리팹 설정을 따라갑니다.
                child.transform.localPosition = prefab.transform.position;
                child.transform.localRotation = prefab.transform.rotation;
            }
        }

        // 3. 컨테이너에 EffectBase 컴포넌트 부착 (이게 전체를 총괄)
        EffectBase effectComponent = container.AddComponent<EffectBase>();
        effectComponent.Initialize(this, type);

        return effectComponent;
    }

    public void ReturnEffectToPool(EffectType type, EffectBase effect)
    {
        effect.gameObject.SetActive(false);
        if (!_effectPool.ContainsKey(type))
        {
            _effectPool.Add(type, new Queue<EffectBase>());
        }
        _effectPool[type].Enqueue(effect);
    }
}