using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float lifeTime = 5f;       // �ڵ� �ı� �ð�
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime); // ���� �ð� �� �ڵ� ����
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
            Debug.Log("�� ����!");
            // �� ü�� ��� ���� ���� (Enemy ��ũ��Ʈ�� TakeDamage ȣ�� ��)
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true; // ���� ���� �� ����
            rb.simulated = false;  // ���� ��� ���� (�ɼ�)
        }
    }
}
