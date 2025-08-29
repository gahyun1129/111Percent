using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Skills/ArrowRain")]
public class ArrowRainSkill : Skill
{
    public GameObject arrowPrefab;
    public GameObject arrowRainPrefab;

    public int damage = 20;
    public int arrowCount = 10;
    public float delayBetween = 0.2f;
    
    public float center;
    public float size = 50f;
    public float spawnHeight = 0;

    protected override void Activate(GameObject user)
    {
        user.GetComponent<PlayerController>().StartCoroutine(ShootArrowRain(user));
    }

    private System.Collections.IEnumerator ShootArrowRain(GameObject user)
    {
        Transform firePoint = user.GetComponent<PlayerAttack>().FirePoint;
        GameObject arrow = Instantiate(arrowRainPrefab, firePoint.position, Quaternion.Euler(0, 0, 90));
        arrow.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 30f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.5f);

        Destroy(arrow);

        for (int i = 0; i < arrowCount; i++)
        {
            float randomX = Random.Range(center - size / 2f, center + size / 2f);
            Vector2 spawnPos = new Vector2(randomX, spawnHeight);

            arrow = Instantiate(arrowPrefab, spawnPos, Quaternion.Euler(0, 0, -90));
            arrow.GetComponent<Arrow>().shooter = user;

            yield return new WaitForSeconds(delayBetween);
        }

        yield return new WaitForSeconds(2f);
        
        lastUseTime = GameManager.Instance.GameTime;
    }
}
