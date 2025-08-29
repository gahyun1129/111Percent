using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Revive")]
public class ReviveSkill : Skill
{
    public int reviveHealth = 40;

    protected override void Activate(GameObject user)
    {
        Health hp = user.GetComponent<Health>();
        if (hp != null && hp.GetHP() <= 0)
        {
            hp.Revive(reviveHealth);
            user.GetComponent<PlayerController>().ReviveParticle.SetActive(true);
        }
    }

}
