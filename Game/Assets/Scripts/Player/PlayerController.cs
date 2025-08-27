using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private float moveX;
    private Rigidbody2D rb;
    private Animator animator;

    private bool isAttack = false;

    public GameObject projectilePrefab;
    public Transform firePoint;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isAttack)
            Move();

        if (Input.GetKeyDown(KeyCode.Space))
            Attack();
    }

    void Move()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        animator.SetBool("isWalk", moveX != 0);

        if (moveX != 0)
        {
            transform.localScale = new Vector3(-Mathf.Sign(moveX), 1, 1);
        }
    }

    void Attack()
    {
        isAttack = true;
        animator.SetTrigger("doAttack");
        // Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + new Vector2(moveX, 0) * moveSpeed * Time.fixedDeltaTime);
    }
}
