using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float minimumRange = 7f;
    public float maximumRange = 15f; // 플레이어와 유지할 거리
    private float moveX = 0f;
    public Transform player;

    [Header("Attack")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float attackCooldown = 2f;

    private Rigidbody2D rb;
    private Animator animator;
    private bool canAttack = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();



        StartCoroutine(AIBehavior());
    }

    IEnumerator AIBehavior()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f); // 0.3초마다 거리 계산

            if (player == null) continue;
            if (!canAttack) continue;

            float distanceX = Mathf.Abs(player.position.x - transform.position.x);

            if (distanceX > maximumRange)
            {
                animator.SetBool("isWalk", true);
                moveX = -1;
                transform.localScale = new Vector3(-moveX, 1, 1);
            }
            else if (distanceX < minimumRange)
            {
                animator.SetBool("isWalk", true);
                moveX = 1;
                transform.localScale = new Vector3(-moveX, 1, 1);
            }
            else
            {
                animator.SetBool("isWalk", false);
                moveX = 0;
                
                if (canAttack)
                    StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        canAttack = false;
        transform.localScale = new Vector3(1, 1, 1);

        animator.SetTrigger("doAttack");

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void Shoot()
    {
        GameObject arrowObj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Arrow arrow = arrowObj.GetComponent<Arrow>();

        // 발사 방향 = 플레이어 바라보는 방향
        Vector2 direction = new Vector2(-1f, 1f).normalized;
        arrow.shooter = gameObject;
        arrow.Launch(direction * 10f);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + new Vector2(moveX / 5, 0) * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            moveX = 0;
            animator.SetBool("isWalk", false);
        }
    }
}
