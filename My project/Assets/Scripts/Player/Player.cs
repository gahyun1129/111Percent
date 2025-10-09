using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int attackPower = 50;
    private float attackTime = 1f;

    private void Start()
    {
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        while (true)
        {
            GameObject go = EnemyManager.instance.GetEnemy();
            if ( go != null)
            {
                go.GetComponent<Enemy>().OnDamaged(attackPower);
            }
            yield return new WaitForSeconds(attackTime);
        }
    }
}
