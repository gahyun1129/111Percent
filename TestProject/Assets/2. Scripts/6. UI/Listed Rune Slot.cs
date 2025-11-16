using UnityEngine;
using UnityEngine.EventSystems;

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
    public Transform itemIconTransform; // 크기를 조절할 아이콘 이미지의 Transform
    public Vector3 pressedScale = new Vector3(0.9f, 0.9f, 1f); // 눌렀을 때 크기
    private Vector3 normalScale; // 평소 크기

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

        // 요구사항 #5: 다른 아이템을 누르면 그 설명창이 닫힌다.
        // -> 어떤 아이템이든 누르면, 현재 떠 있는 툴팁을 닫도록 요청
        TooltipManager.Instance.HideTooltip();

        // 요구사항 #2: 꾹 누르면 크기가 조절돼 작게
        if (itemIconTransform != null)
        {
            itemIconTransform.localScale = pressedScale;
        }
    }

    // 2. (요구사항 #3, #4, #6) 아이템 위에서 손을 뗄 때 (Click 이벤트)
    //    '클릭'은 Down과 Up이 같은 대상에서 일어나야만 호출됩니다.
    public void OnPointerClick(PointerEventData eventData)
    {


        if (Data == null) return;

        // 요구사항 #3: 해당 아이템 위에서 손을 떼면 설명창이 뜬다.
        // 요구사항 #6: 다른 아이템 위에서 손을 떼면 그때 설명창이 뜬다.
        TooltipManager.Instance.ShowTooltip(this, Data);

        // (요구사항 #4: 다른 데서 손을 떼면 안 뜬다 -> OnPointerClick 자체가 호출되지 않으므로 자동 만족)
    }

    // 3. (크기 복원) 손가락을 뗐을 때 (위치 상관없이)
    public void OnPointerUp(PointerEventData eventData)
    {

        if (Data == null) return;
        isPointerDown = false;

        // 아이콘 크기를 원래대로 복원
        if (itemIconTransform != null)
        {
            itemIconTransform.localScale = normalScale;
        }
    }

    // 4. (크기 복원) 누른 상태로 UI 바깥으로 나갔을 때
    public void OnPointerExit(PointerEventData eventData)
    {

        if (Data == null) return;
        // 누르고 있는 상태에서 나갔다면 크기 복원
        if (isPointerDown && itemIconTransform != null)
        {
            itemIconTransform.localScale = normalScale;
        }
    }

    // 5. (크기 재조절) 누른 상태로 나갔다가 다시 들어왔을 때
    public void OnPointerEnter(PointerEventData eventData)
    {

        if (Data == null) return;
        // 누르고 있는 상태에서 다시 들어왔다면 크기 축소
        if (isPointerDown && itemIconTransform != null)
        {
            itemIconTransform.localScale = pressedScale;
        }
    }
}