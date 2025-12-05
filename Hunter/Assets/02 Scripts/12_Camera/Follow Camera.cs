using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

public class FollowCamera : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target; // 플레이어 Transform
    
    private Camera cam;
    private bool isFollowing = false;
    private Vector3 targetVelocity = Vector3.zero;
    private CancellationTokenSource bounceCts;
    private GameData gameData;

    void Start()
    {
        cam = GetComponent<Camera>();
        gameData = GameDataManager.Instance.GAMEDATA;
        
        if (cam == null)
        {
            Debug.LogError("Follow Camera: Camera component not found!");
        }
        
        if (target == null)
        {
            Debug.LogWarning("Follow Camera: Target not assigned!");
        }
    }

    void LateUpdate()
    {
        if (target == null || cam == null) return;

        // 플레이어의 뷰포트 위치 계산
        Vector3 viewportPos = cam.WorldToViewportPoint(target.position);

        // 플레이어가 threshold보다 낮아지면 카메라가 따라감 (왼쪽으로 이동 시)
        if (viewportPos.x <= gameData.viewportThresholdX)
        {
            isFollowing = true;
        }

        // 카메라 따라가기
        if (isFollowing)
        {
            FollowTarget();
        }
    }

    private void FollowTarget()
    {
        // 플레이어가 항상 viewportThresholdX 위치에 있도록 카메라 위치 계산
        Vector3 playerViewportPos = cam.WorldToViewportPoint(target.position);
        
        // 플레이어를 viewportThresholdX 위치에 고정시키기 위한 카메라 오프셋 계산
        float viewportOffset = playerViewportPos.x - gameData.viewportThresholdX;
        
        // 뷰포트 오프셋을 월드 좌표로 변환
        Vector3 leftPoint = cam.ViewportToWorldPoint(new Vector3(0, 0.5f, cam.nearClipPlane));
        Vector3 rightPoint = cam.ViewportToWorldPoint(new Vector3(1, 0.5f, cam.nearClipPlane));
        float viewportWidth = rightPoint.x - leftPoint.x;
        float worldOffset = viewportOffset * viewportWidth;
        
        // 목표 카메라 위치 (X축만 조정, Y와 Z는 유지)
        Vector3 targetPosition = new Vector3(
            transform.position.x + worldOffset,
            transform.position.y,
            transform.position.z
        );

        // 부드럽게 따라가기
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref targetVelocity,
            gameData.smoothStopDuration,
            gameData.followSpeed
        );
    }

    // 플레이어가 이동을 시작할 때 호출
    public void OnPlayerStartMoving()
    {
        // 진행 중인 바운스 효과 취소
        if (bounceCts != null)
        {
            bounceCts.Cancel();
            bounceCts.Dispose();
            bounceCts = null;
        }
        
        // DOTween 애니메이션 중지
        transform.DOKill();
    }

    // 플레이어가 이동을 멈출 때 호출
    public void OnPlayerStopMoving()
    {
        if (isFollowing)
        {
            PlayBounceEffect().Forget();
        }
    }

    // 튕기는 효과 (관성 느낌 - 왼쪽으로 살짝 더 갔다가 돌아옴)
    private async UniTaskVoid PlayBounceEffect()
    {
        // 이전 바운스 효과 취소
        if (bounceCts != null)
        {
            bounceCts.Cancel();
            bounceCts.Dispose();
        }
        
        bounceCts = new CancellationTokenSource();
        
        try
        {
            // 현재 카메라 위치
            Vector3 startPos = transform.position;
            
            // 왼쪽으로 살짝 더 가는 위치 (관성)
            Vector3 overshootPos = startPos + Vector3.left * gameData.bounceDistance;
            
            // 플레이어가 viewportThresholdX에 있을 때의 올바른 카메라 위치 계산
            Vector3 playerViewportPos = cam.WorldToViewportPoint(target.position);
            float viewportOffset = playerViewportPos.x - gameData.viewportThresholdX;
            Vector3 leftPoint = cam.ViewportToWorldPoint(new Vector3(0, 0.5f, cam.nearClipPlane));
            Vector3 rightPoint = cam.ViewportToWorldPoint(new Vector3(1, 0.5f, cam.nearClipPlane));
            float viewportWidth = rightPoint.x - leftPoint.x;
            float worldOffset = viewportOffset * viewportWidth;
            Vector3 finalPos = new Vector3(
                transform.position.x + worldOffset,
                transform.position.y,
                transform.position.z
            );
            
            // DOTween 시퀀스: 왼쪽으로 갔다가 -> 원래 위치로 돌아옴
            var sequence = DOTween.Sequence();
            _= sequence.Append(transform.DOMove(overshootPos, gameData.bounceDuration * 0.4f).SetEase(Ease.OutQuad));
            _= sequence.Append(transform.DOMove(finalPos, gameData.bounceDuration * 0.6f).SetEase(gameData.bounceEase));
            
            await sequence.ToUniTask(cancellationToken: bounceCts.Token);
        }
        catch (System.OperationCanceledException)
        {
            // 취소됨 - 정상적인 동작
        }
        finally
        {
            if (bounceCts != null)
            {
                bounceCts.Dispose();
                bounceCts = null;
            }
        }
    }

    // 카메라 따라가기 리셋 (필요시 사용)
    public void ResetFollow()
    {
        isFollowing = false;
        targetVelocity = Vector3.zero;
        
        if (bounceCts != null)
        {
            bounceCts.Cancel();
            bounceCts.Dispose();
            bounceCts = null;
        }
        
        transform.DOKill();
    }

    // Target 설정
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void OnDestroy()
    {
        // 정리
        if (bounceCts != null)
        {
            bounceCts.Cancel();
            bounceCts.Dispose();
        }
        
        transform.DOKill();
    }

    // 디버그용 - 뷰포트 threshold 시각화
    void OnDrawGizmos()
    {
        if (cam == null) cam = GetComponent<Camera>();
        if (cam == null) return;
        if (gameData == null) return;

        // Threshold 라인 그리기
        Vector3 bottomPoint = cam.ViewportToWorldPoint(new Vector3(gameData.viewportThresholdX, 0, cam.nearClipPlane));
        Vector3 topPoint = cam.ViewportToWorldPoint(new Vector3(gameData.viewportThresholdX, 1, cam.nearClipPlane));
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(bottomPoint, topPoint);
    }
}
