using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : ScriptableObject
{
    public string skillName;
    public float cooldown;
    protected float lastUseTime = 0f;

    public virtual bool CanUse()
    {
        return Time.time >= lastUseTime + cooldown;
    }

    public void Use(GameObject user)
    {
        if (CanUse())
        {
            Activate(user);
        }
    }

    public float GetCoolDown()
    {
        return cooldown;
    }

    public float GetRemainTime()
    {
        float elapsed = Time.time - lastUseTime;     // ���� �ð�
        float remaining = cooldown - elapsed;            // ���� �ð�
        return Mathf.Max(remaining, 0f);
    }

    public void ResetLastUseTime()
    {
        lastUseTime = Time.time;
    }
    protected abstract void Activate(GameObject user);
}
