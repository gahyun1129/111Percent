using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Shield")]
public class ShieldSkill : Skill
{

    private float holdingTime = 4f;

    protected override void Activate(GameObject user)
    {
        user.GetComponent<PlayerController>().StartCoroutine(GetShield(user));
    }

    private System.Collections.IEnumerator GetShield(GameObject user)
    {

        user.transform.Find("Shield").gameObject.SetActive(true);

        yield return new WaitForSeconds(holdingTime);

        user.transform.Find("Shield").gameObject.SetActive(false);
        lastUseTime = Time.time;
    }
}
