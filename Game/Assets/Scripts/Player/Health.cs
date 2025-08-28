using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth = 100f;
    private Animator animator;

    void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public int GetHP()
    {
         return (int)currentHealth;
    }

    public float GetMAXHP()
    {
        return maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        animator.SetTrigger("doDamaged");

        UIManager.Instance.UpdateHPUI();

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        animator.SetTrigger("doDie");
        Destroy(gameObject, 3f);
        GameManager.Instance.GameEnd(gameObject.tag);
    }
}
