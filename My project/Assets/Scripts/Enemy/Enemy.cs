using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    private int HP = 100;
    private float speed = 5f;
    private Vector3 target;

    private void Start()
    {
        target = transform.position;
    }

    public void OnDamaged(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            EnemyManager.instance.DeleteEnemy(gameObject);
            // enemy death
        }
    }

    public void SetTarget(Vector3 _target)
    {
        target = _target;
    }

    private void FixedUpdate()
    {
        if ( transform.position != target) { 
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }
    }
}
