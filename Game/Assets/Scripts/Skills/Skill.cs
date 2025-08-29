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
        return GameManager.Instance.GameTime >= lastUseTime + cooldown;
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
        float elapsed = GameManager.Instance.GameTime - lastUseTime;     // 지난 시간
        float remaining = cooldown - elapsed;            // 남은 시간
        return Mathf.Max(remaining, 0f);
    }

    public void ResetLastUseTime()
    {
        lastUseTime = GameManager.Instance.GameTime;
    }
    protected abstract void Activate(GameObject user);
}
