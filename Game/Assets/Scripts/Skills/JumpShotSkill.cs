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

    private Coroutine currentRoutine;

    protected override void Activate(GameObject user)
    {
        // 이전 스킬 코루틴이 돌고 있으면 멈추기
        if (currentRoutine != null)
        {
            user.GetComponent<PlayerController>().StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        user.GetComponent<PlayerAttack>().UseSkill();
        currentRoutine = user.GetComponent<PlayerController>().StartCoroutine(FireDelayed(user));
    }

    private System.Collections.IEnumerator FireDelayed(GameObject user)
    {

        yield return new WaitForSeconds(0.2f);

        Rigidbody2D rb = user.GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.1f);

        rb.velocity = Vector2.zero;

        user.GetComponent<Animator>().SetTrigger("doAttack");

        yield return new WaitForSeconds(1.5f);

        rb.gravityScale = 1f;
        rb.AddForce(Vector2.down * jumpForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.1f);

        user.GetComponent<PlayerAttack>().StopSkill();
        lastUseTime = GameManager.Instance.GameTime;
    }
}
