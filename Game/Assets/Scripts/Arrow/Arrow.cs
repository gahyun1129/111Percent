using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float lifeTime = 5f;       // 자동 파괴 시간
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime); // 일정 시간 뒤 자동 삭제
    }

    public void Launch(Vector2 direction)
    {
        rb.AddForce(direction, ForceMode2D.Impulse);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void Update()
    {
        if (rb.velocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("적 맞음!");
            // 적 체력 깎는 로직 연결 (Enemy 스크립트의 TakeDamage 호출 등)
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true; // 물리 영향 안 받음
            rb.simulated = false;  // 물리 계산 멈춤 (옵션)
        }
    }
}
