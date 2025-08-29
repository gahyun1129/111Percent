using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    [Header("Arrow")]
    public GameObject arrowPrefab;
    public Transform firePoint;

    private Animator animator;
    private PlayerController playerController;

    private bool isAttack = false;
    private bool isUseSkill = false;

    private float minForce = 10f;
    private float maxForce = 11f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    public void Attack()
    {
        if ( !isAttack && !isUseSkill )
        {
            isAttack = true;
            animator.SetTrigger("doAttack");
        }
    }

    public void Shoot()
    {
        if (!isUseSkill)
        {
            transform.localScale = new Vector3(-1, 1, 1);

            GameObject arrowObj = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
            Arrow arrow = arrowObj.GetComponent<Arrow>();

            float normalized = Mathf.Clamp01(playerController.HoldTime / 2f);
            float power = Mathf.Lerp(minForce, maxForce, normalized);

            Vector2 direction = new Vector2(-transform.localScale.x, 1f).normalized;
            arrow.shooter = gameObject;
            arrow.Launch(direction * power, Mathf.CeilToInt(power));

            isAttack = false;
        }
        else
        {
            GameObject enemy = GameManager.Instance.GetEnemy();

            Vector2 start = firePoint.position;
            Vector2 target = enemy.transform.position;
            Vector2 dir = (target - start + new Vector2(0, 1f)).normalized;

            GameObject arrow = Instantiate(arrowPrefab, start, Quaternion.identity);
            var arrowRb = arrow.GetComponent<Rigidbody2D>();

            arrowRb.gravityScale = 0f;
            arrowRb.velocity = dir * 25f;

            float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            arrow.transform.rotation = Quaternion.AngleAxis(ang, Vector3.forward);

            var arrowComp = arrow.GetComponent<Arrow>();
            if (arrowComp != null)
            {
                arrowComp.SetDamage(20);
                arrowComp.shooter = gameObject;
            }
        }
    }

    public void UseSkill()
    {
        isUseSkill = true;
    }

    public void StopSkill()
    {
        isUseSkill = false;
    }

    public Transform FirePoint => firePoint;
    public bool IsAttack => isAttack;
    public bool IsUseSkill => isUseSkill;
}
