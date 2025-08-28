using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/JumpShot")]
public class JumpShotSkill : Skill
{
    public GameObject arrowPrefab;
    public float jumpForce = 20f;
    public int damage = 20;

    protected override void Activate(GameObject user)
    {
        Rigidbody2D rb = user.GetComponent<Rigidbody2D>();
        if ( rb != null)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        user.GetComponent<PlayerController>().StartCoroutine(FireDelayed(user));
    }

    private System.Collections.IEnumerator FireDelayed(GameObject user)
    {
        yield return new WaitForSeconds(0.5f);

        Transform firePoint = user.transform.Find("Fire Point");
        if ( firePoint != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
            arrow.GetComponent<Arrow>().SetDamage(damage);
        }
    }
}

/*
 
    코루틴으로 그냥 만들기로 할게요

    coroutin
        점프
        점프 후 중력 끄기 // 떨어지지 않음
        
        enemy 위치 찾고 그 방향으로 활 조준 후 슛
        이떄 애니메이션도 위에서 실행되면 좋겠음
        
        그 후엔 다시 중력 켜기
        // 떨어짐
        
        캐릭터 stopskill 함수 부르기
 
 */
