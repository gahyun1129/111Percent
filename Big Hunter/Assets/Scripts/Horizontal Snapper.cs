using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ResponsiveMagnetScrollView : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [Header("Settings")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GridLayoutGroup gridLayout; // LayoutGroup 대신 GridLayoutGroup 권장
    [SerializeField] private RectTransform content;

    [Header("Snapping Option")]
    [SerializeField] private float snapDuration = 0.1f;
    [SerializeField] private Ease snapEase = Ease.InOutBack;
    [SerializeField] private float sensitivity = 0.2f; // 20%만 움직여도 넘어가게 (조금 더 예민하게 수정)

    // 화면에 몇 개를 보여줄 것인가?
    [SerializeField] private int itemsVisibleAtOnce = 3;

    private float itemWidth;
    private int totalPages;
    private int currentPage = 0;
    private float startDragX;

    private void Start()
    {
        if (scrollRect == null) scrollRect = GetComponent<ScrollRect>();
        if (content == null) content = scrollRect.content;
        if (gridLayout == null) gridLayout = content.GetComponent<GridLayoutGroup>();

        scrollRect.inertia = false;

        // [추가] 위아래로 흔들리지 않게 스크립트에서 아예 잠금
        scrollRect.vertical = false;
        scrollRect.horizontal = true;

        SetContentSize();
    }

    private void SetContentSize()
    {
        // 1. 뷰포트의 너비와 높이를 모두 가져옵니다.
        float viewportWidth = scrollRect.viewport.rect.width;
        float viewportHeight = scrollRect.viewport.rect.height; // [추가]

        // 2. 너비 계산 (기존과 동일)
        float spacingX = gridLayout.spacing.x;
        float totalSpacing = spacingX * (itemsVisibleAtOnce - 1);
        float singleItemWidth = (viewportWidth - totalSpacing) / itemsVisibleAtOnce;

        // 3. 높이 계산 (뷰포트 높이 - 위아래 패딩)
        // 패딩이 없다면 그냥 viewportHeight를 쓰면 됩니다.
        float paddingY = gridLayout.padding.top + gridLayout.padding.bottom;
        float singleItemHeight = viewportHeight - paddingY;

        // 4. 셀 크기 적용 (X, Y 모두 스크립트로 통제)
        gridLayout.cellSize = new Vector2(singleItemWidth, singleItemHeight);

        // 이동 계산용 변수 업데이트
        itemWidth = singleItemWidth + spacingX;
        totalPages = content.childCount;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        content.DOKill();
        startDragX = content.anchoredPosition.x;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float endDragX = content.anchoredPosition.x;
        float distance = startDragX - endDragX;

        int nearestPage = Mathf.RoundToInt(-content.anchoredPosition.x / itemWidth);

        if (distance > 0) // 왼쪽으로 드래그 (다음 페이지)
        {
            if (distance >= itemWidth * sensitivity)
            {
                if (nearestPage == currentPage) nearestPage = currentPage + 1;
            }
            else nearestPage = currentPage;
        }
        else if (distance < 0) // 오른쪽으로 드래그 (이전 페이지)
        {
            if (Mathf.Abs(distance) >= itemWidth * sensitivity)
            {
                if (nearestPage == currentPage) nearestPage = currentPage - 1;
            }
            else nearestPage = currentPage;
        }

        nearestPage = Mathf.Clamp(nearestPage, 0, totalPages - itemsVisibleAtOnce); // 마지막 페이지 범위 수정

        SnapToPage(nearestPage);
    }

    public void SnapToPage(int index)
    {
        currentPage = index;
        float targetX = -(index * itemWidth);

        content.DOAnchorPosX(targetX, snapDuration).SetEase(snapEase);
    }
}