using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private float moveX;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 좌/우 입력 받기 (A/D 키 or 화살표 ← →)
        moveX = Input.GetAxisRaw("Horizontal");

        // 애니메이션 제어 (이동 여부)
        animator.SetBool("isWalk", moveX != 0);

        // 방향 반전 처리 (왼쪽 이동 시 캐릭터 flip)
        if (moveX != 0)
        {
            transform.localScale = new Vector3(-Mathf.Sign(moveX), 1, 1);
        }
    }

    void FixedUpdate()
    {
        // 좌우 이동만 처리
        rb.MovePosition(rb.position + new Vector2(moveX, 0) * moveSpeed * Time.fixedDeltaTime);
    }
}
