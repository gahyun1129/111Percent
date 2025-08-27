using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float desiredRange = 5f; // 플레이어와 유지할 거리
    public Transform player;

    [Header("Attack")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float attackCooldown = 2f;

    private Rigidbody2D rb;
    private Animator animator;
    private bool canAttack = true;

    void Start()
    {
        // player = GameObject.FindGameObjectWithTag("Player").transform;
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

            float distanceX = player.position.x - transform.position.x;

            if (Mathf.Abs(distanceX) > desiredRange)
            {
                Debug.Log("이동");
                animator.SetBool("isWalk", true);
                float moveDir = Mathf.Sign(distanceX); // -1: 왼쪽, 1: 오른쪽
                // rb.MovePosition(rb.position + new Vector2(moveDir * moveSpeed * Time.deltaTime, 0));
                transform.localScale = new Vector3(-moveDir, 1, 1);
            }
            else
            {
                Debug.Log("공격");
                if (canAttack)
                    StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        canAttack = false;

        // 화살 발사
        GameObject arrowObj = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        Arrow arrow = arrowObj.GetComponent<Arrow>();
        Vector2 direction = new Vector2(Mathf.Sign(player.position.x - transform.position.x), 0).normalized;
        // arrow.shooterTag = "Enemy";
        arrow.Launch(direction * 10f);

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
