using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/JumpShot")]
public class JumpShotSkill : Skill
{
    public GameObject arrowPrefab;
    public float jumpForce = 20f;
    public int damage = 20;
    public float arrowSpeed = 25f;

    protected override void Activate(GameObject user)
    {
       
        user.GetComponent<PlayerController>().StartCoroutine(FireDelayed(user));
    }

    private System.Collections.IEnumerator FireDelayed(GameObject user)
    {
        Rigidbody2D rb = user.GetComponent<Rigidbody2D>();


        if (rb != null)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(0.1f);

        rb.gravityScale = 0f;   // 중력 꺼서 공중 정지
        rb.velocity = Vector2.zero; // 혹시 남은 속도 제거

        // user.GetComponent<PlayerController>().UseSkill();

        user.GetComponent<Animator>().SetTrigger("doAttack");

        yield return new WaitForSeconds(1f);

        Transform firePoint = user.GetComponent<PlayerController>().GetFirePoint();
        GameObject enemy = GameManager.Instance.GetEnemy();

        Vector2 start = firePoint.position;
        Vector2 target = enemy.transform.position;
        Vector2 dir = (target - start + new Vector2(0, 1f)).normalized;

        GameObject arrow = Instantiate(arrowPrefab, start, Quaternion.identity);
        var arrowRb = arrow.GetComponent<Rigidbody2D>();

        arrowRb.gravityScale = 0f;
        arrowRb.velocity = dir * arrowSpeed;

        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.AngleAxis(ang, Vector3.forward);

        var arrowComp = arrow.GetComponent<Arrow>();
        if (arrowComp != null)
        {
            arrowComp.SetDamage(damage);
            arrowComp.shooter = user;
        }

        // user.GetComponent<PlayerController>().StopSkill();
        yield return new WaitForSeconds(0.1f);

        rb.gravityScale = 1f;

        lastUseTime = GameManager.Instance.GameTime;

    }
}
