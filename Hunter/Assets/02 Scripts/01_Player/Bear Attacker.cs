using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class BearAttacker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private SpearProjectile projectilePrefab;

    private List<GameObject> dots;
    private List<SpriteRenderer> dotRenderers;
    private bool isAiming = false;

    private Vector3 startInputPos;
    private Vector3 currentInputPos;
    private Vector3 currentSmoothedVelocity;

    private ThrowData throwData;

    private void Start()
    {
        throwData = GameDataManager.Instance.THROWDATA;

        if ( throwData == null)
        {
            Debug.Log("null");
        }

        CreateDots();
        HideDots();

    }

    public void DoAiming()
    {
        if (IsPointerOverUI()) return;

        isAiming = true;
        startInputPos = Input.mousePosition;
        currentSmoothedVelocity = Vector3.zero;

        HideDots();
    }

    public void UpdateAiming()
    {
        if (!isAiming) return;

        currentInputPos = Input.mousePosition;

        float dragDistance = Vector3.Distance(startInputPos, currentInputPos);
        if (dragDistance < throwData.minDragDistance)
        {
            HideDots();
            return;
        }

        Vector3 targetVelocity = CalculateClampedVelocity(startInputPos, currentInputPos);

        if (targetVelocity == Vector3.zero)
        {
            HideDots();
        }
        else
        {
            currentSmoothedVelocity = Vector3.Lerp(currentSmoothedVelocity, targetVelocity, Time.unscaledDeltaTime * throwData.aimSmoothing);

            if (currentSmoothedVelocity.magnitude >= throwData.minPower)
            {
                // ★ [중요 수정] 시뮬레이션 할 때도 Boost를 적용해야 실제랑 똑같이 보임!
                SimulateTrajectory(currentSmoothedVelocity * throwData.initialSpeedBoost);
                if (!dots[0].activeSelf) ShowDots();
            }
            else
            {
                HideDots();
            }
        }
    }

    public void DoFire()
    {
        if (!isAiming) return;

        isAiming = false;
        HideDots();

        if (currentSmoothedVelocity.magnitude >= throwData.minPower)
        {
            // 실제 발사 벡터
            Vector3 finalVelocity = currentSmoothedVelocity * throwData.initialSpeedBoost;

            SpearProjectile spear = Instantiate(projectilePrefab, throwPoint.position, Quaternion.identity);

            // ★ 수정됨: 생성된 투사체에 우리가 계산한 '정확한 값'들을 전달
            spear.Launch(throwPoint.position, finalVelocity, throwData.gravity, throwData.fallGravityMultiplier);
        }
    }

    // --- 물리 계산 로직 (변경 없음) ---
    private Vector3 CalculateClampedVelocity(Vector3 start, Vector3 current)
    {
        Vector3 direction = start - current;
        float rawMagnitude = direction.magnitude * 0.01f * throwData.sensitivity;
        float finalPower = Mathf.Clamp(rawMagnitude * throwData.powerMultiplier, 0, throwData.maxPower);

        float angleRad = Mathf.Atan2(direction.y, direction.x);
        float angleDeg = angleRad * Mathf.Rad2Deg;

        if (direction.x < 0)
        {
            if (direction.y < 0) return Vector3.zero;
            else angleDeg = throwData.maxAngle;
        }

        angleDeg = Mathf.Clamp(angleDeg, throwData.minAngle, throwData.maxAngle);
        float clampedRad = angleDeg * Mathf.Deg2Rad;
        Vector3 finalDir = new Vector3(Mathf.Cos(clampedRad), Mathf.Sin(clampedRad), 0);

        return finalDir * finalPower;
    }

    private void SimulateTrajectory(Vector3 startVelocity)
    {
        int physicalPointCount = throwData.dotCount + 1;
        Vector3[] pathPoints = new Vector3[physicalPointCount];

        Vector3 tempPos = throwPoint.position;
        Vector3 tempVel = startVelocity;

        pathPoints[0] = tempPos;

        for (int i = 1; i < physicalPointCount; i++)
        {
            float timeStep = throwData.dotSpacing;

            // 시뮬레이션 중력 (BearAttacker의 gravity 값 사용)
            float currentGravity = (tempVel.y < 0) ? throwData.gravity * throwData.fallGravityMultiplier : throwData.gravity;

            tempVel.y += currentGravity * timeStep;
            tempPos += tempVel * timeStep;

            pathPoints[i] = tempPos;
        }

        float progress = (Time.time * throwData.flowSpeed) % 1f;

        for (int i = 0; i < throwData.dotCount; i++)
        {
            Vector3 flowPos = Vector3.Lerp(pathPoints[i], pathPoints[i + 1], progress);
            dots[i].transform.position = flowPos;

            float alphaRatio = 1f - ((float)i / throwData.dotCount);
            Color c = dotRenderers[i].color;
            c.a = Mathf.Lerp(throwData.minAlpha, 1f, alphaRatio);
            dotRenderers[i].color = c;

            float scale = Mathf.Lerp(0.2f, 0.05f, 1f - alphaRatio);
            dots[i].transform.localScale = Vector3.one * scale;
        }
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;
        if (EventSystem.current.IsPointerOverGameObject()) return true;
        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return true;
        return false;
    }

    private void CreateDots()
    {
        dots = new List<GameObject>();
        dotRenderers = new List<SpriteRenderer>();
        for (int i = 0; i < throwData.dotCount; i++)
        {
            GameObject dot = Instantiate(dotPrefab, transform);
            dots.Add(dot);
            dotRenderers.Add(dot.GetComponent<SpriteRenderer>());
        }
    }
    private void ShowDots() { foreach (var dot in dots) dot.SetActive(true); }
    private void HideDots() { foreach (var dot in dots) dot.SetActive(false); }
}