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
 
    �ڷ�ƾ���� �׳� ������ �ҰԿ�

    coroutin
        ����
        ���� �� �߷� ���� // �������� ����
        
        enemy ��ġ ã�� �� �������� Ȱ ���� �� ��
        �̋� �ִϸ��̼ǵ� ������ ����Ǹ� ������
        
        �� �Ŀ� �ٽ� �߷� �ѱ�
        // ������
        
        ĳ���� stopskill �Լ� �θ���
 
 */
