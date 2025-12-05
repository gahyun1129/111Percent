using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyController : MonoBehaviour
{   
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;

    private GameData gameData;
    private Camera mainCamera;
    private BearController bearController;
    private bool isMoving = false;
    private bool isPreparing = false;
    private bool isResting = false;
    
    // 가속 관련 변수
    private float moveStartTime = 0f;
    private float totalChaseTime = 0f; // 실제 추적한 누적 시간

    private CancellationTokenSource moveCts;

    void Start()
    {
        moveCts = new CancellationTokenSource();

        // GameData 가져오기
        gameData = GameDataManager.Instance.GAMEDATA;

        // 메인 카메라 참조
        mainCamera = Camera.main;

        Vector3 enemyViewportPos = new Vector3(gameData.enemyViewportX, gameData.enemyViewportY, 0);
        Vector3 enemyWorldPos = mainCamera.ViewportToWorldPoint(enemyViewportPos);
        enemyWorldPos.z = transform.position.z; // z값 유지
        transform.position = enemyWorldPos;

        // 플레이어 자동 찾기
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                // Tag가 없으면 BearController로 찾기
                bearController = FindObjectOfType<BearController>();
                if (bearController != null)
                {
                    player = bearController.transform;
                }
            }
        }
        
        // BearController 참조 가져오기
        if (bearController == null && player != null)
        {
            bearController = player.GetComponent<BearController>();
        }
    }

    void Update()
    {
        if (gameData == null || mainCamera == null || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 휴식 중이 아닐 때만 거리 체크
        if (!isResting)
        {
            if (distanceToPlayer > gameData.stopDistance)
            {
                if (!isMoving && !isPreparing)
                {
                    PrepareToMoveAsync().Forget();
                }
            }
            else
            {
                if (isMoving || isPreparing)
                {
                    StopEnemy();
                }
            }
        }

        if (isMoving)
        {
            MoveTowardsPlayer();
        }

        DrawStopDistanceLine();
    }

    private async UniTaskVoid PrepareToMoveAsync()
    {
        isPreparing = true;
        
        // 이전의 취소 토큰이 있다면 정리하고 새로 발급 (혹시 모를 중복 방지)
        moveCts?.Cancel();
        moveCts = new CancellationTokenSource();

        // 캔슬 토큰을 두 개 합침 (게임 오브젝트 파괴 시 + 로직상 취소 시)
        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            moveCts.Token, 
            this.GetCancellationTokenOnDestroy()
        );

        try
        {
            // 설정한 딜레이만큼 대기 (이 시간 동안은 멈춰 있음)
            // 빅헌터 느낌을 위해 이때 "!" 말풍선을 띄우거나 울음소리 애니메이션을 넣으면 좋습니다.
            await UniTask.Delay((int)(gameData.reactionDelay * 1000), cancellationToken: linkedCts.Token);

            float currentDistance = Vector3.Distance(transform.position, player.position);
            if (currentDistance > gameData.stopDistance)
            {
                isMoving = true;
                isPreparing = false;
                moveStartTime = Time.time; // 이동 시작 시간 기록
                
                // 이동 애니메이션 트리거
                if (animator != null)
                {
                    animator.SetTrigger("Moving");
                }
            }
            else
            {
                isPreparing = false;
            }
        }
        catch (System.OperationCanceledException)
        {
            isPreparing = false;
        }
    }

    private void MoveTowardsPlayer()
    {
        // 플레이어가 이동 중일 때만 추적 시간 누적
        if (bearController != null && bearController.IsMoving())
        {
            totalChaseTime += Time.deltaTime;
        }
        
        // 누적 추적 시간이 chaseDuration 이상이면 휴식 모드로 전환
        if (totalChaseTime >= gameData.chaseDuration)
        {
            // 휴식 모드로 전환 (플레이어를 너무 오래 쫓았음)
            StartRestAsync().Forget();
            return;
        }
        
        // 이동 시작 후 경과 시간 (가속 커브용)
        float elapsedTime = Time.time - moveStartTime;
        
        // 커브 평가를 위한 정규화된 시간 (0~1)
        float normalizedTime = Mathf.Clamp01(elapsedTime / gameData.maxAccelerationTime);
        
        // 커브에서 속도 배율 가져오기
        float speedMultiplier = gameData.accelerationCurve.Evaluate(normalizedTime);
        
        // 가속이 적용된 속도로 이동
        transform.position += Vector3.left * gameData.enemySpeed * speedMultiplier * Time.deltaTime;
    }

    private async UniTaskVoid StartRestAsync()
    {
        // 이동 중지
        isMoving = false;
        isResting = true;
        
        // 추적 시간 리셋
        totalChaseTime = 0f;
        
        // 휴식 애니메이션 트리거
        if (animator != null)
        {
            animator.SetTrigger("Idle");
        }
        
        // 취소 토큰 준비
        moveCts?.Cancel();
        moveCts = new CancellationTokenSource();
        
        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            moveCts.Token, 
            this.GetCancellationTokenOnDestroy()
        );
        
        try
        {
            // 휴식 시간만큼 대기
            await UniTask.Delay((int)(gameData.restDuration * 1000), cancellationToken: linkedCts.Token);
            
            // 휴식 종료
            isResting = false;
            
            // 휴식 후 다시 거리 체크하여 필요하면 추적 재개
            // (Update에서 자동으로 처리됨)
        }
        catch (System.OperationCanceledException)
        {
            isResting = false;
        }
    }

    private void DrawStopDistanceLine()
    {
        // 현재 위치에서 x축 -방향으로 stopDistance만큼 선 그리기
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + Vector3.left * gameData.stopDistance;

        // 빨간색 선으로 그리기
        Debug.DrawLine(startPos, endPos, Color.red);
    }
    
    // 적 리셋 (필요시 외부에서 호출)
    public void ResetEnemy()
    {
        isMoving = false;
    }

    // 강제로 이동 시작
    public void ForceStartMoving()
    {
        isMoving = true;
        moveStartTime = Time.time; // 이동 시작 시간 기록
        
        // 이동 애니메이션 트리거
        if (animator != null)
        {
            animator.SetTrigger("Moving");
        }
    }

    // 강제로 멈춤
    public void ForceStop()
    {
        isMoving = false;
    }

    private void StopEnemy()
    {
        // 누적 추적 시간 체크 (플레이어가 이동한 시간만 카운트)
        if (totalChaseTime >= gameData.chaseDuration)
        {
            // 플레이어를 오래 쫓았으므로 휴식 모드로 전환
            StartRestAsync().Forget();
        }
        else
        {
            // 짧게 쫓았으므로 그냥 멈춤 (휴식 없이)
            isMoving = false;
            isPreparing = false;
            
            // 추적 시간은 유지 (다음에 다시 쫓을 때 누적됨)
            
            // 정지 애니메이션 트리거
            if (animator != null)
            {
                animator.SetTrigger("Idle");
            }
            
            // 대기 중이던 UniTask가 있다면 취소시킴
            moveCts?.Cancel();
        }
    }

    private void OnDestroy()
    {
        moveCts?.Cancel();
        moveCts?.Dispose();
    }
}
