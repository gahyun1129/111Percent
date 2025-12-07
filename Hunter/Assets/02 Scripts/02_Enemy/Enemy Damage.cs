using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

public class EnemyDamage : MonoBehaviour
{
    public static EnemyDamage Instance { get; private set; }

    [Header("Components")]
    [SerializeField] private Animator animator;

    // 이제 단일 변수가 아니라 배열로 관리합니다.
    // 인스펙터에 일일이 넣을 필요 없이 코드가 알아서 찾습니다.
    private SpriteRenderer[] _allRenderers;
    private Material[] _allMaterials;

    [Header("Stats")]
    public float Health = 100f;

    [Header("Damage Effect (Visual)")]
    [Tooltip("띠용~ 하는 스케일 강도 (X, Y)")]
    [SerializeField] private Vector2 punchScaleStrength = new Vector2(0.3f, 0.3f);

    [Tooltip("띠용~ 하는 시간")]
    [SerializeField] private float punchDuration = 0.3f;

    [Tooltip("하얗게 변하는 시간")]
    [SerializeField] private float flashDuration = 0.15f;

    private Vector3 _originalScale;
    private CancellationTokenSource _cts;
    private Sequence _currentSeq;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        // 1. 내 자식들 중에 있는 "모든" 스프라이트 렌더러를 싹 다 긁어옵니다.
        _allRenderers = GetComponentsInChildren<SpriteRenderer>();

        // 2. 렌더러 개수만큼 마테리얼 배열 생성
        _allMaterials = new Material[_allRenderers.Length];

        for (int i = 0; i < _allRenderers.Length; i++)
        {
            // 각 렌더러의 재질(Material Instance)을 가져와서 저장
            _allMaterials[i] = _allRenderers[i].material;

            // 초기화: 플래시 꺼두기
            _allMaterials[i].SetFloat("_FlashAmount", 0f);
        }

        _originalScale = transform.localScale;
    }

    public void OnDamaged(float damage, Collider2D target)
    {
        if (target.CompareTag("Hand")) damage *= 3;

        Health -= damage;

        PlayDamageEffect().Forget();

        if (Health <= 0)
        {
            Health = 0;
            animator.SetTrigger("Dead");
        }
    }

    private async UniTaskVoid PlayDamageEffect()
    {
        // 1. 취소 및 초기화
        if (_cts != null) { _cts.Cancel(); _cts.Dispose(); }
        _cts = new CancellationTokenSource();

        if (_currentSeq != null && _currentSeq.IsActive()) _currentSeq.Kill();

        // 2. 강제 원상복구 (초기화)
        transform.localScale = _originalScale;

        // 모든 마테리얼의 하얀색을 0으로 끔
        foreach (var mat in _allMaterials)
        {
            if (mat != null) mat.SetFloat("_FlashAmount", 0f);
        }

        // 3. 연출 시작
        _currentSeq = DOTween.Sequence();

        // A. 몸체 전체 띠용 (부모만 띠용하면 자식들은 다 따라옴)
        _= _currentSeq.Join(transform.DOPunchScale(new Vector3(punchScaleStrength.x, punchScaleStrength.y, 0), punchDuration, 10, 1f));

        // B. [핵심] 모든 자식들 하얗게 만들기 (DOVirtual 사용)
        // 0부터 1까지 값을 변화시키면서, 그 값을 모든 마테리얼에 한꺼번에 대입합니다.
        // 스프라이트가 100개여도 트윈은 딱 1개만 돌아가서 성능이 좋습니다.
        _= _currentSeq.Join(DOVirtual.Float(0f, 1f, flashDuration, (value) =>
        {
            // 이 람다 함수는 매 프레임 호출됨
            foreach (var mat in _allMaterials)
            {
                if (mat != null) mat.SetFloat("_FlashAmount", value);
            }
        }).SetLoops(2, LoopType.Yoyo)); // 0->1->0 깜빡

        try
        {
            await _currentSeq.ToUniTask(cancellationToken: _cts.Token);
        }
        catch (System.OperationCanceledException)
        {
            // 무시
        }
        finally
        {
            if (!_cts.IsCancellationRequested)
            {
                transform.localScale = _originalScale;
                foreach (var mat in _allMaterials)
                {
                    if (mat != null) mat.SetFloat("_FlashAmount", 0f);
                }
            }
        }
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _currentSeq?.Kill();
    }
}