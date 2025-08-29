using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    private float moveX;
    public float moveSpeed = 5f;
    public GameObject ReviveParticle;

    private Rigidbody2D rb;
    private Animator animator;
    private PlayerAttack attack;

    private bool isCharging = false;

    private float pressTime;
    private float holdTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        attack = GetComponent<PlayerAttack>();
    }

    void Update()
    {
        if (!attack.IsAttack && !attack.IsUseSkill)
        {
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
                gameObject.GetComponent<PlayerAttack>().Attack();
                Invoke("HideChargingBar", 2f);
            }
            if (isCharging)
            {
                holdTime = GameManager.Instance.GameTime - pressTime;
            }
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

    void FixedUpdate()
    {
        if ( !attack.IsAttack && !attack.IsUseSkill)
        {
            rb.MovePosition(rb.position + new Vector2(moveX, 0) * moveSpeed * Time.fixedDeltaTime);
        }
    }

    public void WinPerformance()
    {
        animator.SetTrigger("doVictory");
    }

    public float HoldTime => holdTime;
}
