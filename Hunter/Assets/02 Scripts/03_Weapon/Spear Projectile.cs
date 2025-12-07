using UnityEngine;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Rigidbody2D))]
public class SpearProjectile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [Tooltip("실제로 꽂힐 기준점 (새의 부리 끝 위치)")]
    [SerializeField] private Transform tipPoint;

    [Header("Layers")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask blockLayer;
    [SerializeField] private LayerMask groundLayer;

    // 내부 변수
    private Rigidbody2D rb;
    private Collider2D col;

    private float _baseGravityScale;
    private float _fallMultiplier;
    private bool isStuck = false;
    private bool hasRicocheted = false;
    private bool isLanded = false;

    private ThrowData throwData;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponentInChildren<Collider2D>();

        throwData = GameDataManager.Instance.THROWDATA;
    }

    public void Launch(Vector3 startPos, Vector3 initialVelocity, float attackerGravity, float attackerFallMultiplier)
    {
        transform.position = startPos;
        isStuck = false;
        hasRicocheted = false;
        isLanded = false;
        _fallMultiplier = attackerFallMultiplier;

        rb.isKinematic = false; // ★ 재사용 시 물리 다시 켜기
        col.enabled = true;     // ★ 재사용 시 충돌 다시 켜기

        Vector2 finalVelocity = initialVelocity * throwData.speedMultiplier;
        rb.velocity = finalVelocity;

        float standardGravity = Physics2D.gravity.y;
        float gravityRatio = attackerGravity / standardGravity;

        _baseGravityScale = gravityRatio * (throwData.speedMultiplier * throwData.speedMultiplier);
        rb.gravityScale = _baseGravityScale;

        LifetimeRoutine().Forget();
    }

    private void FixedUpdate()
    {
        if (isStuck) return;

        // 1. 가변 중력
        if (!hasRicocheted)
        {
            if (rb.velocity.y < 0) rb.gravityScale = _baseGravityScale * _fallMultiplier;
            else rb.gravityScale = _baseGravityScale;
        }
        else
        {
            rb.gravityScale = 3f;
        }

        // 2. 회전 처리
        if (!hasRicocheted && rb.velocity.sqrMagnitude > 0.1f)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            rb.rotation = angle;
        }

        // 3. 바닥 마찰
        if (hasRicocheted && !isStuck)
        {
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, Time.fixedDeltaTime * throwData.groundFriction), rb.velocity.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isStuck) return;

        int hitLayer = 1 << other.gameObject.layer;

        if ((hitLayer & enemyLayer) != 0)
        {
            if (!hasRicocheted) StickToTarget(other, true);
            else BounceOnGround(other); // ★ 인자 추가
        }
        else if ((hitLayer & blockLayer) != 0)
        {
            if (!hasRicocheted) Ricochet();
            else BounceOnGround(other); // ★ 인자 추가
        }
        else if ((hitLayer & groundLayer) != 0)
        {
            if (hasRicocheted) BounceOnGround(other); // ★ 인자 추가
            else StickToTarget(other, false);
        }
    }

    private void StickToTarget(Collider2D target, bool isSuccess)
    {
        if (isLanded) return;
        isLanded = true;
        isStuck = true;

        StopPhysics(true); // 물리 끄기

        transform.SetParent(target.transform);

        if (tipPoint != null)
        {
            Vector3 hitPoint = target.ClosestPoint(tipPoint.position);
            Vector3 tipOffset = transform.position - tipPoint.position;
            transform.position = hitPoint + tipOffset;
        }

        if (isSuccess)
        {
            EnemyDamage.Instance?.OnDamaged(throwData.damage, target);
            animator.SetTrigger("Success");
        }
        else
        {
            animator.SetTrigger("Fail");
        }
    }

    private void Ricochet()
    {
        if (hasRicocheted) return;
        hasRicocheted = true;
        // animator.SetTrigger("Fail");

        rb.velocity = new Vector2(
            -Mathf.Abs(transform.right.x) * 2f + throwData.ricochetForce.x,
            throwData.ricochetForce.y
        );
        rb.angularVelocity = throwData.ricochetTorque;
    }

    // ★ [핵심 수정] 바닥 트리거 처리
    private void BounceOnGround(Collider2D groundCollider)
    {
        // 1. 바닥 뚫고 들어가지 않게 위치 보정 (표면으로 올리기)
        // 현재 위치에서 가장 가까운 바닥 표면 점을 찾음
        // (주의: tipPoint가 아니라 몸체(transform) 기준으로 바닥 위에 안착시키는 느낌)
        Vector3 surfacePoint = groundCollider.ClosestPoint(transform.position);

        // 너무 깊이 박혔으면 위로 살짝 꺼내줌 (Y축 보정)
        if (transform.position.y < surfacePoint.y)
        {
            transform.position = new Vector3(transform.position.x, surfacePoint.y + 0.1f, transform.position.z);
        }

        // 2. 속도가 너무 느리면 -> 완전히 멈추고 물리 끄기 (안 그러면 뚫고 내려감)
        if (rb.velocity.magnitude < throwData.stopThreshold)
        {
            StopPhysics(false); // 물리를 꺼서 중력을 없앰
            isStuck = true;     // 로직 정지

            // 바닥에 누운 모양으로 회전 보정 (선택 사항)
             transform.rotation = Quaternion.Euler(0, 0, 0); 
            return;
        }

        // 3. 아직 빠르면 -> 튕기기 (Trigger라 물리 엔진이 안 해주므로 수동 계산)
        if (rb.velocity.y < 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Abs(rb.velocity.y) * throwData.groundBounciness);
            rb.angularVelocity *= 0.5f;
        }
    }

    // 물리 끄는 헬퍼 함수
    private void StopPhysics(bool disableCollider)
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic = true; // ★ 이게 핵심: 중력 영향 안 받게 설정!
        if (disableCollider) col.enabled = false;
    }

    private async UniTaskVoid LifetimeRoutine()
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(throwData.lifeTime), cancellationToken: this.GetCancellationTokenOnDestroy());
        if (this != null && gameObject != null) Destroy(gameObject);
    }
}