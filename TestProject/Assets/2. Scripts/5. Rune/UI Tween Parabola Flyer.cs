using UnityEngine;
using DG.Tweening; // DOTween 네임스페이스가 필요합니다.

/// <summary>
/// UI 요소를 RectTransform의 anchoredPosition 기준으로
/// 포물선을 그리며 목표 지점까지 날려 보냅니다.
/// 월드 좌표를 입력받아 내부에서 로컬 좌표로 변환합니다.
/// </summary>
[RequireComponent(typeof(RectTransform))] // RectTransform이 필수입니다.
public class UITweenParabolaFlyer : MonoBehaviour
{
    [Header("애니메이션 설정")]
    [Tooltip("애니메이션 총 지속 시간")]
    public float duration = 0.8f;

    [Tooltip("포물선의 수직 높이 (위쪽으로 볼록한 정도)")]
    public float verticalArc = 200f;

    [Tooltip("포물선의 수평 휘어짐 정도 (좌/우로 볼록한 정도)")]
    public float horizontalArc = 150f;

    [Tooltip("애니메이션에 적용할 Ease 타입")]
    public Ease easeType = Ease.InOutQuad;

    [Header("상태")]
    [Tooltip("애니메이션이 완료되었는지 여부")]
    public bool isDone = false;

    private RectTransform rectTransform;

    // 부모 캔버스와 카메라를 저장할 변수
    private RectTransform parentCanvasRect;
    private Camera parentCanvasCamera;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // 이 스크립트가 붙은 UI가 속한 최상위 Canvas를 찾습니다.
        Canvas parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null)
        {
            // 캔버스 자체의 RectTransform을 저장
            parentCanvasRect = parentCanvas.transform as RectTransform;

            // 캔버스 렌더 모드에 따라 카메라를 설정
            if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                parentCanvasCamera = null; // 오버레이 모드는 카메라가 null
            }
            else
            {
                parentCanvasCamera = parentCanvas.worldCamera; // 그 외에는 worldCamera
            }
        }
        else
        {
            Debug.LogWarning("UITweenParabolaFlyer: 부모 캔버스를 찾을 수 없습니다! 좌표 변환에 문제가 생길 수 있습니다.");
        }
    }

    /// <summary>
    /// 지정된 '월드 좌표'로 포물선 비행 애니메이션을 재생합니다.
    /// </summary>
    /// <param name="targetWorldPosition">목표가 되는 지점의 '월드' 좌표</param>
    public void PlayFlyAnimation(Vector3 targetWorldPosition) // 파라미터가 Vector3로 변경됨
    {
        isDone = false;
        rectTransform.DOKill(true);

        // 1. 시작점 (Rune의 현재 로컬 좌표. 이건 이미 'parentCanvasRect' 기준임)
        Vector2 startPosition = rectTransform.anchoredPosition;

        // --- 좌표 변환 로직 ---
        // 2. 타겟의 '월드 좌표(targetWorldPosition)'를 '스크린 좌표'로 변환
        Vector2 targetScreenPosition = RectTransformUtility.WorldToScreenPoint(parentCanvasCamera, targetWorldPosition);

        // 3. '스크린 좌표'를 'parentCanvasRect' 기준의 '로컬 좌표'로 변환
        Vector2 targetLocalPosition;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvasRect,   // 기준이 될 캔버스
                targetScreenPosition, // 변환할 스크린 좌표
                parentCanvasCamera,   // 캔버스의 카메라
                out targetLocalPosition // 변환된 로컬 좌표(결과)
            ))
        {
            // 만약 변환에 실패하면 (예: 캔버스 뒤) 일단 경고를 띄우고 계속 진행
            Debug.LogWarning("목표 좌표 변환에 실패했습니다.");
        }
        // --- 좌표 변환 끝 ---

        // 4. 기존 계산식에서 'targetAnchoredPosition' 대신 'targetLocalPosition'을 사용
        Vector2 midPoint = (startPosition + targetLocalPosition) / 2;

        float directionX = Mathf.Sign(targetLocalPosition.x - startPosition.x);
        if (directionX == 0) directionX = 1; // 수직 이동 시 기본값

        Vector2 controlPoint = new Vector2(
            midPoint.x + (directionX * horizontalArc),
            midPoint.y + verticalArc
        );

        // 5. DOTween.To() 타이머 (무료 버전용 베지어 곡선 계산)
        float timer = 0f;
        DOTween.To(() => timer, setter => timer = setter, 1f, duration)
            .SetEase(easeType)
            .OnUpdate(() =>
            {
                // OnUpdate마다 0~1 사이의 timer값을 이용해 '이차 베지어 곡선' 위치를 수동으로 계산
                // B(t) = (1-t)^2 * P0 + 2(1-t)t * P1 + t^2 * P2
                // (P0 = startPosition, P1 = controlPoint, P2 = targetLocalPosition)
                float oneMinusT = 1f - timer;
                Vector2 newPos = (oneMinusT * oneMinusT * startPosition) +
                                 (2f * oneMinusT * timer * controlPoint) +
                                 (timer * timer * targetLocalPosition); // targetLocalPosition 사용

                rectTransform.anchoredPosition = newPos;
            })
            .OnComplete(() =>
            {
                Debug.Log(gameObject.name + " 포물선 비행 완료!");
                isDone = true;
            });
    }
}