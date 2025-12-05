using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class SpearProjectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private bool rotateTowardsDirection = true;

    // [추가] 낙하 시 중력 배율 (2.0 ~ 3.0 추천)
    [SerializeField] private float fallGravityMultiplier = 2.5f;

    [Header("속도 & 타격감 조절")]
    [Range(1f, 5f)]
    public float speedMultiplier = 2.0f; // 전체 속도 배수 (빠르기)
    
    private Vector3 velocity;
    private float baseGravity; // 원래 설정된 중력값
    private bool isFlying = false;

    private float fastGravity;
    private Vector3 currentVelocity;

    public void Launch(Vector3 startPos, Vector3 initialVelocity, float gravityValue)
    {
        transform.position = startPos;

        currentVelocity = initialVelocity * speedMultiplier;
        fastGravity = gravityValue * (speedMultiplier * speedMultiplier);
        
        this.velocity = currentVelocity;
        this.baseGravity = fastGravity;
        
        FlyRoutine(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTaskVoid FlyRoutine(CancellationToken ct)
    {
        isFlying = true;
        float timer = 0f;

        while (isFlying && timer < lifeTime)
        {
            float dt = Time.deltaTime;

            // 1. 현재 적용할 중력 계산 (핵심!)
            // y 속도가 0보다 작으면(떨어지는 중) 중력을 배로 적용
            float currentGravity = (velocity.y < 0) ? baseGravity * fallGravityMultiplier : baseGravity;

            // 2. 물리 위치 갱신
            transform.position += velocity * dt;

            // 3. 속도 갱신 (가변 중력 적용)
            velocity.y += currentGravity * dt;

            // 4. 회전 처리
            if (rotateTowardsDirection && velocity != Vector3.zero)
            {
                float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            
            // 충돌 처리 (바닥 예시)
            if (transform.position.y <= -4f)
            {
                // 꽂히는 연출
                isFlying = false; 
                break;
            }

            timer += dt;
            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }

        if (timer >= lifeTime && this != null) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Enemy 레이어와 충돌 시 멈춤
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            isFlying = false;
            velocity = Vector3.zero;
            
            // 맞은 대상의 자식으로 설정 (적과 함께 움직임)
            transform.SetParent(other.transform);
        }
    }
}
