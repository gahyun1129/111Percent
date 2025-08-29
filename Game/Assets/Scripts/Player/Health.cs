using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth = 100f;
    private Animator animator;

    bool hasReviveChance = true;
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

        if (gameObject.CompareTag("player"))
        {
            StartCoroutine(UIManager.Instance.ShowDamagedImage());
        }

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        animator.SetTrigger("doDie");

        if (gameObject.CompareTag("Enemy") || !hasReviveChance)
        {
            UIManager.Instance.StopGameTimer();
            GameManager.Instance.GameOver(gameObject.tag);
            Destroy(gameObject, 3f);
        }

    }

    public bool HasReviveChance() => hasReviveChance;

    public void Revive(int reviveHealth)
    {
        animator.SetTrigger("doRevive");

        currentHealth = reviveHealth;
        UIManager.Instance.UpdateHPUI();
        hasReviveChance = false;
    }
}
