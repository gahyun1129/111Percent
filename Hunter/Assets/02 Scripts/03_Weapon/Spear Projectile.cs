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

    [SerializeField] private float fail_effect_scale = 3f;
    [SerializeField] private float success_effect_scale = 8f;

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

        StopPhysics(true);

        // 1. 꽂힐 위치 계산 (기존 로직)
        Vector3 hitPoint = target.ClosestPoint(tipPoint.position);

        if (tipPoint != null)
        {
            Vector3 tipOffset = transform.position - tipPoint.position;
            transform.position = hitPoint + tipOffset;
        }

        // ================================================================
        // ★ 추가된 로직: 표면 각도 계산 및 회전 적용
        // ================================================================

        // 팁 위치에서 타겟의 중심 쪽으로 향하는 방향 (혹은 현재 날아오던 방향의 반대)
        // 정확한 표면 각도를 얻기 위해 투사체 위치에서 타겟 쪽으로 레이를 쏩니다.
        Vector2 dirToTarget = (hitPoint - transform.position).normalized;

        // 타겟 레이어만 충돌 체크하도록 필터링 (자기 자신 등 제외)
        int targetLayerMask = 1 << target.gameObject.layer;

        // 약간 뒤에서 쏴야 표면을 정확히 감지함 (거리 1.0f 정도면 충분)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToTarget, 2.0f, targetLayerMask);

        if (hit.collider != null)
        {
            // hit.normal이 바로 '표면의 수직 벡터'입니다.
            // Atan2를 사용해 벡터를 각도(도)로 변환합니다.
            float angle = Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg;

            // [중요] 스프라이트가 오른쪽(0도)을 보고 있다고 가정할 때:
            // 표면이 위를 향하면(90도), 칼은 아래를 향해(270도/-90도) 꽂혀야 하므로 180도를 더해줍니다.
            // 여기에 랜덤값(-15 ~ +15도)을 추가합니다.
            float randomSpread = Random.Range(-15f, 15f);
            transform.rotation = Quaternion.Euler(0, 0, angle + 180f + randomSpread);
        }
        else
        {
            // 혹시 레이캐스트가 실패했다면 그냥 랜덤만 줍니다.
            transform.Rotate(0, 0, Random.Range(-20f, 20f));
        }
        // ================================================================

        transform.SetParent(target.transform);

        if (isSuccess)
        {
            EffectManager.Instance.PlayEffect(EffectType.Bird_Success, transform.position, Vector3.one * success_effect_scale);

            EnemyDamage.Instance?.OnDamaged(throwData.damage, target);
            animator.SetTrigger("Success");
        }
        else
        {
            EffectManager.Instance.PlayEffect(EffectType.Bird_Fail, transform.position, Vector3.one * fail_effect_scale);

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