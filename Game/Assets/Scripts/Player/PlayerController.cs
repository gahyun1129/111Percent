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
    public float holdTime;
    private bool isCharging = false;
    


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
        {
            pressTime = GameManager.Instance.GameTime;
            gameObject.GetComponent<ArrowChargingBar>().ShowChargingBar();
            isCharging = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isCharging = false;
            holdTime = GameManager.Instance.GameTime - pressTime;
            Attack();
            Invoke("HideChargingBar", 2f);
        }
        if (isCharging)
        {
            holdTime = GameManager.Instance.GameTime - pressTime;
        }
    }

    void HideChargingBar()
    {
        gameObject.GetComponent<ArrowChargingBar>().HideChargingBar();
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

    public void Attack()
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
        if (!isUseSkill)
        {
            GameObject arrowObj = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
            Arrow arrow = arrowObj.GetComponent<Arrow>();

            float normalized = Mathf.Clamp01(holdTime / 2f);
            float power = Mathf.Lerp(minForce, maxForce, normalized);

            Vector2 direction = new Vector2(-transform.localScale.x, 1f).normalized;
            arrow.shooter = gameObject;
            arrow.Launch(direction * power, 10 /* 변경 필요!! */);

            isAttack = false;

            StopSkill();
        }
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
