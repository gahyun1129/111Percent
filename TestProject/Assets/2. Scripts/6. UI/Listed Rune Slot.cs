using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening; // 1. DOTween 네임스페이스 추가

public class ListedRuneSlot : MonoBehaviour,
    IPointerDownHandler,   // 누를 때
    IPointerUpHandler,     // 뗄 때
    IPointerClickHandler,  // 같은 곳에서 눌렀다 뗄 때 (클릭)
    IPointerEnterHandler,  // 포인터가 들어올 때
    IPointerExitHandler    // 포인터가 나갈 때
{
    [Header("아이템 정보")]
    public RuneData Data = null;

    [Header("크기 조절")]
    public RectTransform itemIconTransform; // 크기를 조절할 아이콘 이미지의 Transform
    public Vector3 pressedScale = new Vector3(0.9f, 0.9f, 1f); // 눌렀을 때 크기
    private Vector3 normalScale; // 평소 크기

    [Header("DOTween 설정")] // 2. 애니메이션 시간 변수 추가
    public float scaleDuration = 0.1f; // 스케일이 변하는 데 걸리는 시간
    public Ease scaleEase = Ease.OutQuad; // 적용할 Ease (인스펙터에서 변경 가능)

    // 현재 이 슬롯이 눌려있는지 상태 저장
    private bool isPointerDown = false;

    void Start()
    {
        // 평소 크기를 저장해둠
        if (itemIconTransform != null)
        {
            normalScale = itemIconTransform.localScale;
        }
    }

    public void SetRuneData(RuneData _data)
    {
        Data = _data;
    }

    // --- 요구사항 구현 ---

    // 1. (요구사항 #2, #5) 아이템을 누르기 시작할 때
    public void OnPointerDown(PointerEventData eventData)
    {
        if (Data == null) return;

        isPointerDown = true;

        TooltipManager.Instance.HideTooltip();

        // 요구사항 #2: 꾹 누르면 크기가 조절돼 작게
        if (itemIconTransform != null)
        {
            // 3. DOTween 코드로 변경
            itemIconTransform.DOKill(); // 기존 애니메이션 중지
            itemIconTransform.DOScale(pressedScale, scaleDuration).SetEase(scaleEase);
        }
    }

    // 2. (요구사항 #3, #4, #6) 아이템 위에서 손을 뗄 때 (Click 이벤트)
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Data == null) return;

        TooltipManager.Instance.ShowTooltip(this, Data);
    }

    // 3. (크기 복원) 손가락을 뗐을 때 (위치 상관없이)
    public void OnPointerUp(PointerEventData eventData)
    {
        // Data == null 체크는 isPointerDown이 false가 되므로 
        // 굳이 필요 없지만, 안전을 위해 isPointerDown 체크 전에 수행
        if (Data == null) return;

        isPointerDown = false;

        // 아이콘 크기를 원래대로 복원
        if (itemIconTransform != null)
        {
            // 3. DOTween 코드로 변경
            itemIconTransform.DOKill(); // 기존 애니메이션 중지
            itemIconTransform.DOScale(normalScale, scaleDuration).SetEase(scaleEase);
        }
    }

    // 4. (크기 복원) 누른 상태로 UI 바깥으로 나갔을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        if (Data == null) return;

        // 누르고 있는 상태에서 나갔다면 크기 복원
        if (isPointerDown && itemIconTransform != null)
        {
            // 3. DOTween 코드로 변경
            itemIconTransform.DOKill(); // 기존 애니메이션 중지
            itemIconTransform.DOScale(normalScale, scaleDuration).SetEase(scaleEase);
        }
    }

    // 5. (크기 재조절) 누른 상태로 나갔다가 다시 들어왔을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Data == null) return;

        // 누르고 있는 상태에서 다시 들어왔다면 크기 축소
        if (isPointerDown && itemIconTransform != null)
        {
            // 3. DOTween 코드로 변경
            itemIconTransform.DOKill(); // 기존 애니메이션 중지
            itemIconTransform.DOScale(pressedScale, scaleDuration).SetEase(scaleEase);
        }
    }
}