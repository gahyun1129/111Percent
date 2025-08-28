using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float lifeTime = 5f;       // �ڵ� �ı� �ð�
    private Rigidbody2D rb;
    public GameObject shooter;

    public int damage = 10;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime); // ���� �ð� �� �ڵ� ����
    }

    public void Launch(Vector2 direction, int _damage)
    {
        rb.AddForce(direction, ForceMode2D.Impulse);
        damage = _damage;
    }

    public void RainArrow(Vector2 direction, int _damage)
    {
        rb.AddForce(direction, ForceMode2D.Impulse);
        damage = _damage;

        // ȸ�� ���� �� ������ ����
        transform.rotation = Quaternion.Euler(0, 0, 90);
    }

    void Update()
    {
        if (rb.velocity.magnitude > 0.1f && transform.rotation.eulerAngles.z != 90)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true; // ���� ���� �� ����
            rb.simulated = false;  // ���� ��� ���� (�ɼ�)
        }
        else if (shooter.CompareTag("player") && collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Health>().TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (shooter.CompareTag("Enemy") && collision.gameObject.CompareTag("player"))
        {
            collision.gameObject.GetComponent<Health>().TakeDamage(damage);
            Destroy(gameObject);
        }
        else if ( shooter.CompareTag("Enemy") && collision.gameObject.CompareTag("Shield"))
        {
            // �� ���⼭ ȿ���� �ָ� ���ڴ� ��~~ �ؼ� ���� ����?
            Destroy(gameObject);
        }
    }
}
