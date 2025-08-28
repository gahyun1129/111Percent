using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public Skill[] equippedSkills;
    Health hp;

    private void Start()
    {
        hp = gameObject.GetComponent<Health>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            gameObject.GetComponent<PlayerController>().UseSkill();
            UseSkill(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // gameObject.GetComponent<PlayerController>().UseSkill();
            UseSkill(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // gameObject.GetComponent<PlayerController>().UseSkill();
            UseSkill(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            // gameObject.GetComponent<PlayerController>().UseSkill();
            UseSkill(3);
        }
        if ( hp.GetHP() <= 0 && hp.HasReviveChance())
        {
            UseSkill(4);
        }
    }

    void UseSkill(int index)
    {
        if (index < equippedSkills.Length && equippedSkills[index] != null)
        {
            equippedSkills[index].Use(gameObject);
        }
    }
}
