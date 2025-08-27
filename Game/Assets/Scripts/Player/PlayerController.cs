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
        // ��/�� �Է� �ޱ� (A/D Ű or ȭ��ǥ �� ��)
        moveX = Input.GetAxisRaw("Horizontal");

        // �ִϸ��̼� ���� (�̵� ����)
        animator.SetBool("isWalk", moveX != 0);

        // ���� ���� ó�� (���� �̵� �� ĳ���� flip)
        if (moveX != 0)
        {
            transform.localScale = new Vector3(-Mathf.Sign(moveX), 1, 1);
        }
    }

    void FixedUpdate()
    {
        // �¿� �̵��� ó��
        rb.MovePosition(rb.position + new Vector2(moveX, 0) * moveSpeed * Time.fixedDeltaTime);
    }
}
