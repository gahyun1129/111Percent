using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public Skill[] equippedSkills;

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
    }

    void UseSkill(int index)
    {
        if (index < equippedSkills.Length && equippedSkills[index] != null)
        {
            equippedSkills[index].Use(gameObject);
        }
    }
}
