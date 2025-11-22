using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HorizontalSnapper : MonoBehaviour
{
    [SerializeField] private Scrollbar scrollBar;
    [SerializeField] private float swipeTime = 0.2f;
    [SerializeField] private float swipeDistance = 50f;
    [SerializeField] private float widthPerPage = 50f;

    private float[] scrollPageValues;
    private float valueDistance = 0;
    private int currentPage = 0;
    private int maxPage = 0;
    private float startTouchX;
    private float endTouchX;
    private bool isSwipeMod = false;


    private void Awake()
    {
        scrollPageValues = new float[transform.childCount];
        valueDistance = 1f / (scrollPageValues.Length - 1f);
        for ( int i = 0; i < scrollPageValues.Length; ++i)
        {
            scrollPageValues[i] = valueDistance * i;
        }

        maxPage = transform.childCount - 2;
    }

    private void Start()
    {
        SetScrollBarValue(0);
    }

    public void SetScrollBarValue(int index)
    {
        currentPage = index;
        scrollBar.value = scrollPageValues[index];
    }

    private void Update()
    {
        UpdateInput();
    }

    private void UpdateInput()
    {
        if (isSwipeMod) return;

#if UNITY_EDITOR
        if ( Input.GetMouseButtonDown(0) )
        {
            startTouchX = Input.mousePosition.x;
        }
        else if ( Input.GetMouseButtonUp(0))
        {
            endTouchX = Input.mousePosition.x;

            UpdateSwipe();
        }
#endif
    }

    private void UpdateSwipe()
    {
        bool isLeft = startTouchX < endTouchX ? true : false;

        if (isLeft)
        {
            if (currentPage == 0) return;

            int num = (int)Mathf.Ceil( Mathf.Abs(startTouchX - endTouchX) / widthPerPage);
            currentPage -= num;

            if ( currentPage < 0) currentPage = 0;
        }
        else
        {
            if (currentPage == maxPage - 1 ) return;
            int num = (int)Mathf.Ceil(Mathf.Abs(startTouchX - endTouchX) / widthPerPage);
            currentPage += num;

            if (currentPage >= maxPage) currentPage = maxPage - 1;
        }

        StartCoroutine(OnSwipeOneStep(currentPage));
    }

    private IEnumerator OnSwipeOneStep(int index)
    {
        float start = scrollBar.value;
        float current = 0;
        float percent = 0;

        isSwipeMod = true;

        while ( percent < 1 )
        {
            current += Time.deltaTime;
            percent = current / swipeTime;

            scrollBar.value = Mathf.Lerp(start, scrollPageValues[index], percent);

            yield return null;
        }

        isSwipeMod = false;
    }
}