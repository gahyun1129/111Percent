using UnityEngine;
using DG.Tweening; // DOTween 네임스페이스가 필요합니다.

/// <summary>
/// UI 요소를 '제자리'로 튕기며 떨어뜨리는 애니메이션을 재생합니다.
/// 프리팹으로 생성될 때 Start()에서 자동으로 실행됩니다.
/// </summary>
[RequireComponent(typeof(RectTransform))] // RectTransform이 필수입니다.
public class UITweenBouncer : MonoBehaviour
{
    [Header("애니메이션 설정")]
    [Tooltip("애니메이션 지속 시간")]
    public float duration = 1.0f;

    [Tooltip("딜레이 시간 (초)")]
    public float delay = 0f;

    [Tooltip("애니메이션이 시작될 오프셋 위치 (최종 위치 기준)\n(0, 300)이면 최종 위치보다 300px 위에서 시작합니다.")]
    public Vector2 startOffset = new Vector2(0, 300f);

    private RectTransform rectTransform;
    private Vector2 finalAnchoredPosition; // UI가 최종적으로 머무를 '제자리'

    void Awake()
    {

    }

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        // Start()보다 먼저 호출되는 Awake()에서 
        // 프리팹이 놓인 '최종 위치(제자리)'를 저장합니다.
        finalAnchoredPosition = rectTransform.anchoredPosition;

        // 1. 애니메이션을 시작할 위치(제자리 + 오프셋)로 UI를 즉시 이동시킵니다.
        //    (예: 300px 위로 순간이동)
        rectTransform.anchoredPosition = finalAnchoredPosition + startOffset;

        // 2. 저장해둔 '제자리'로 튕기면서 돌아오는 애니메이션을 재생합니다.
        PlayBounceToFinalPosition();
    }

    /// <summary>
    /// 저장된 'finalAnchoredPosition'(제자리)으로 튕기며 이동하는 애니메이션을 재생합니다.
    /// </summary>
    public void PlayBounceToFinalPosition()
    {
        rectTransform.DOKill(true); // 혹시 실행 중인 애니메이션이 있다면 중지

        // finalAnchoredPosition (저장해둔 '제자리')로 이동
        rectTransform.DOAnchorPos(finalAnchoredPosition, duration, false)
            .SetEase(Ease.OutBounce) // "통, 통" 멈추는 효과
            .SetDelay(delay)
            .OnComplete(() => Debug.Log(gameObject.name + " 제자리 튕김 애니메이션 완료!"));
    }

    /// <summary>
    /// (선택 사항) 애니메이션을 다시 재생하고 싶을 때 이 함수를 호출하세요.
    /// (예: 버튼 클릭 시)
    /// </summary>
    public void ResetAndPlay()
    {
        rectTransform.DOKill(true);
        // 다시 오프셋 위치로 이동
        rectTransform.anchoredPosition = finalAnchoredPosition + startOffset;
        // 다시 재생
        PlayBounceToFinalPosition();
    }
}