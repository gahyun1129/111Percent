using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Freeze")]
public class FreezeSkill : Skill
{

    private float holdingTime = 3f;

    protected override void Activate(GameObject user)
    {
        user.GetComponent<PlayerController>().StartCoroutine(FreezeEnemy(user));
    }

    private System.Collections.IEnumerator FreezeEnemy(GameObject user)
    {
        GameManager.Instance.GetEnemy().GetComponent<EnemyAI>().SetFreezeState(true);

        yield return new WaitForSeconds(holdingTime);

        GameManager.Instance.GetEnemy().GetComponent<EnemyAI>().SetFreezeState(false);
        lastUseTime = GameManager.Instance.GameTime;

    }
}
