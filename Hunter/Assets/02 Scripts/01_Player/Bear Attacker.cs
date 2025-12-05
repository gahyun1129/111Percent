using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // UI 감지용
using DG.Tweening; // 연출용

public class BearAttacker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject dotPrefab;   // 하얀색 점 프리팹
    [SerializeField] private Transform throwPoint;   // 창 생성 위치 (손)
    [SerializeField] private SpearProjectile projectilePrefab; // 창 프리팹

    [Header("Control Feel (조작감)")]
    [Range(0.1f, 3.0f)]
    [SerializeField] private float sensitivity = 1.5f; // 감도 (높을수록 살짝 당겨도 셈)
    [Range(1f, 50f)]
    [SerializeField] private float aimSmoothing = 15f; // 스무딩 (손떨림 방지, 15 추천)
    [SerializeField] private float minDragDistance = 40f; // 탭 오작동 방지용 최소 드래그 거리

    [Header("Physics Settings")]
    [SerializeField] private float minPower = 5f;         // 최소 파워 (이것보다 약하면 발사 X)
    [SerializeField] private float powerMultiplier = 10f; // 힘 배율
    [SerializeField] private float maxPower = 25f;        // 최대 힘 제한
    [SerializeField] private float gravity = -25f;        // 중력 (빅헌터 느낌은 -20 ~ -30 추천)

    [SerializeField] private float fallGravityMultiplier = 2.5f;
    [SerializeField] private float initialSpeedBoost = 1.2f;

    [Header("Trajectory Visuals")]
    [SerializeField] private int dotCount = 30;           // 점 개수
    [SerializeField] private float dotSpacing = 0.05f;    // 점 간격 (시간 단위)
    [SerializeField] private float minAlpha = 0.2f;       // 꼬리 부분 투명도
    [SerializeField] private float flowSpeed = 2f;        // 점 흐르는 속도

    [Header("Aim Constraints")]
    [SerializeField] private float minAngle = -10f;       // 최소 각도 (땅 쪽)
    [SerializeField] private float maxAngle = 85f;        // 최대 각도 (하늘 쪽)

    // 내부 변수
    private List<GameObject> dots;
    private List<SpriteRenderer> dotRenderers;
    private bool isAiming = false;

    private Vector3 startInputPos;
    private Vector3 currentInputPos;
    private Vector3 currentSmoothedVelocity; // 스무딩된 최종 벡터

    private void Start()
    {
        CreateDots();
        HideDots();
    }

    // 외부 입력이나 Update에서 호출될 함수들
    public void DoAiming()
    {
        // UI(버튼 등)를 누른 상태라면 조준 시작 안 함
        if (IsPointerOverUI()) return;

        isAiming = true;
        startInputPos = Input.mousePosition;
        currentSmoothedVelocity = Vector3.zero;

        HideDots(); // 드래그 하기 전까진 숨김
    }

    public void UpdateAiming()
    {
        if (!isAiming) return;

        currentInputPos = Input.mousePosition;

        // 1. 드래그 거리 체크
        float dragDistance = Vector3.Distance(startInputPos, currentInputPos);
        if (dragDistance < minDragDistance)
        {
            HideDots();
            return;
        }

        // 2. 물리 벡터 계산 (각도 고정 로직 포함)
        Vector3 targetVelocity = CalculateClampedVelocity(startInputPos, currentInputPos);

        // 3. 계산 결과가 0이면(숨김 조건) 숨김
        if (targetVelocity == Vector3.zero)
        {
            HideDots();
        }
        else
        {
            // 4. 스무딩 처리 (Lerp) - 손떨림 보정
            currentSmoothedVelocity = Vector3.Lerp(currentSmoothedVelocity, targetVelocity, Time.unscaledDeltaTime * aimSmoothing);

            // 5. 최소 파워 이상일 때만 조준선 표시
            if (currentSmoothedVelocity.magnitude >= minPower)
            {
                SimulateTrajectory(currentSmoothedVelocity);
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

        // 최종 계산된 스무딩 벡터로 발사
        if (currentSmoothedVelocity.magnitude >= minPower)
        {
            Vector3 finalVelocity = currentSmoothedVelocity * initialSpeedBoost;

            SpearProjectile spear = Instantiate(projectilePrefab, throwPoint.position, Quaternion.identity);
            spear.Launch(throwPoint.position, finalVelocity, gravity);
        }
    }

    // --- 핵심 로직: 속도 및 각도 계산 ---
    private Vector3 CalculateClampedVelocity(Vector3 start, Vector3 current)
    {
        Vector3 direction = start - current; // 슬링샷: 드래그 반대 방향

        // 1. 기본 파워 계산
        float rawMagnitude = direction.magnitude * 0.01f * sensitivity;
        float finalPower = Mathf.Clamp(rawMagnitude * powerMultiplier, 0, maxPower);

        // 2. 각도 계산 (Rad -> Deg)
        float angleRad = Mathf.Atan2(direction.y, direction.x);
        float angleDeg = angleRad * Mathf.Rad2Deg;

        // 3. 4사분면 로직 처리 (요청 사항 반영)
        // direction.x < 0: 뒤쪽(왼쪽)으로 날아가는 상황 (즉, 사용자가 오른쪽으로 당김)
        if (direction.x < 0)
        {
            // 뒤쪽 아래(3사분면 방향) -> "안 나오게"
            if (direction.y < 0)
            {
                return Vector3.zero;
            }
            // 뒤쪽 위(2사분면 방향) -> "min각도에 고정"
            else
            {
                // 플레이어가 뒤로 당겼지만, 시스템은 억지로 앞쪽 최소 각도로 고정
                angleDeg = maxAngle;
            }
        }

        // direction.x >= 0: 앞쪽(오른쪽)으로 날아가는 상황 (정상)
        // 여기서도 minAngle ~ maxAngle 사이로 각도를 고정하여 땅이나 머리 뒤로 넘어가는 것 방지
        angleDeg = Mathf.Clamp(angleDeg, minAngle, maxAngle);

        // 4. 고정된 각도와 파워로 벡터 재구성
        float clampedRad = angleDeg * Mathf.Deg2Rad;
        Vector3 finalDir = new Vector3(Mathf.Cos(clampedRad), Mathf.Sin(clampedRad), 0);

        return finalDir * finalPower;
    }

    // --- [수정됨] 흐르는 효과가 적용된 궤적 그리기 ---
    private void SimulateTrajectory(Vector3 startVelocity)
    {
        // 1. 물리 궤적 계산 (점 개수보다 1개 더 많이 계산해야 마지막 점이 갈 곳이 생김)
        // 30개의 점을 보여주려면 31개의 좌표가 있어야 30->31로 흐를 수 있음
        int physicalPointCount = dotCount + 1;
        Vector3[] pathPoints = new Vector3[physicalPointCount];

        Vector3 tempPos = throwPoint.position;
        Vector3 tempVel = startVelocity;

        pathPoints[0] = tempPos; // 시작점

        for (int i = 1; i < physicalPointCount; i++)
        {
            float timeStep = dotSpacing;

            // ★ 떨어질 때 중력 더 세게 적용 (기존 로직 유지)
            float currentGravity = (tempVel.y < 0) ? gravity * fallGravityMultiplier : gravity;

            tempVel.y += currentGravity * timeStep;
            tempPos += tempVel * timeStep;

            pathPoints[i] = tempPos;
        }

        // 2. Flow(흐름) 계산
        // 0.0 ~ 1.0 사이를 반복하는 진행률 값 (Time.time으로 계속 증가)
        // flowSpeed가 빠를수록 0에서 1로 변하는 속도가 빨라짐
        float progress = (Time.time * flowSpeed) % 1f;

        // 3. 점 배치 및 보간
        for (int i = 0; i < dotCount; i++)
        {
            // i번째 점은 pathPoints[i] 에서 pathPoints[i+1] 로 이동 중이어야 함
            // Lerp(A, B, t): A와 B 사이를 t(0~1)만큼 이동한 위치
            Vector3 flowPos = Vector3.Lerp(pathPoints[i], pathPoints[i + 1], progress);

            dots[i].transform.position = flowPos;

            // --- 시각적 처리 (투명도/크기) ---
            // 끝으로 갈수록 흐려지게
            float alphaRatio = 1f - ((float)i / dotCount);

            Color c = dotRenderers[i].color;
            // 깜빡이는 효과를 살짝 주려면 아래 주석 해제 (선택사항)
            // float blink = Mathf.PingPong(Time.time * 2f, 0.2f); 
            // c.a = Mathf.Lerp(minAlpha, 1f, alphaRatio) - blink;
            c.a = Mathf.Lerp(minAlpha, 1f, alphaRatio);
            dotRenderers[i].color = c;

            // 끝으로 갈수록 작아지게
            float scale = Mathf.Lerp(0.2f, 0.1f, 1f - alphaRatio); // 시작 크기 0.3 -> 끝 크기 0.1
            dots[i].transform.localScale = Vector3.one * scale;
        }
    }

    private void Fire(Vector3 velocity)
    {
        Vector3 finalVelocity = velocity * initialSpeedBoost;
        SpearProjectile spear = Instantiate(projectilePrefab, throwPoint.position, Quaternion.identity);
        spear.Launch(throwPoint.position, velocity, gravity);
    }

    // UI 터치 방지 (PC/모바일 통합)
    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;
        // PC 마우스
        if (EventSystem.current.IsPointerOverGameObject()) return true;
        // 모바일 터치
        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return true;

        return false;
    }

    // --- 초기화 헬퍼 ---
    private void CreateDots()
    {
        dots = new List<GameObject>();
        dotRenderers = new List<SpriteRenderer>();
        for (int i = 0; i < dotCount; i++)
        {
            GameObject dot = Instantiate(dotPrefab, transform);
            dots.Add(dot);
            dotRenderers.Add(dot.GetComponent<SpriteRenderer>());
        }
    }
    private void ShowDots() { foreach (var dot in dots) dot.SetActive(true); }
    private void HideDots() { foreach (var dot in dots) dot.SetActive(false); }
}
