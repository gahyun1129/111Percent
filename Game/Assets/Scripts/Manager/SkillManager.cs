using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public Skill[] equippedSkills;
    Health hp;

    public static SkillManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        hp = GetComponent<Health>();
        for ( int i = 0; i < equippedSkills.Length; ++i )
        {
            equippedSkills[i].ResetLastUseTime();
        }
    }

    void Update()
    {
        if ( hp.GetHP() <= 0 && hp.HasReviveChance())
        {
            UseSkill(4);
            UIManager.Instance.UpdatePassiveSkill();
        }
    }

    public void UseSkill(int index)
    {
        if (index < equippedSkills.Length && equippedSkills[index] != null && !GetComponent<PlayerAttack>().IsUseSkill)
        {
            equippedSkills[index].Use(gameObject);
        }
    }

    public float GetCoolTime(int index)
    {
        if (index < equippedSkills.Length && equippedSkills[index] != null)
        {
            return equippedSkills[index].GetCoolDown();
        }

        return -1f;
    }

    public float GetRemainTime(int index)
    {
        if (index < equippedSkills.Length && equippedSkills[index] != null)
        {
            return equippedSkills[index].GetRemainingTime();
        }

        return -1f;
    }
}
