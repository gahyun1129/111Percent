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
    private bool isUseSkill = false;

    public GameObject arrowPrefab;
    public Transform firePoint;

    private float minForce = 10f;
    private float maxForce = 11f;
    private float pressTime;
    private float holdTime;

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
            pressTime = Time.time;

        if (Input.GetKeyUp(KeyCode.Space))
        {
            holdTime = Time.time - pressTime;
            Attack();
        }
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
        
    }

    public void UseSkill()
    {
        isUseSkill = true;
    }

    public void StopSkill()
    {
        isUseSkill = false;
    }

    public void Shoot()
    {
        GameObject arrowObj = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
        Arrow arrow = arrowObj.GetComponent<Arrow>();

        float normalized = Mathf.Clamp01(holdTime / 2f);
        float power = Mathf.Lerp(minForce, maxForce, normalized);

        Vector2 direction = new Vector2(-transform.localScale.x, 1f).normalized;
        arrow.shooter = gameObject;
        arrow.Launch(direction * power, 10 /* 변경 필요!! */);

        isAttack = false;
    }

    void FixedUpdate()
    {
        if (!isUseSkill)
        {
            rb.MovePosition(rb.position + new Vector2(moveX, 0) * moveSpeed * Time.fixedDeltaTime);
        }
    }

    public void WinPerformance()
    {
        animator.SetTrigger("doVictory");
    }

    public Transform GetFirePoint() => firePoint;
}
