using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : ScriptableObject
{
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

    public float GetRemainingTime()
    {
        float elapsed = GameManager.Instance.GameTime - lastUseTime;
        float remaining = cooldown - elapsed;
        return Mathf.Max(remaining, 0f);
    }

    public void ResetLastUseTime()
    {
        lastUseTime = GameManager.Instance.GameTime;
    }
    protected abstract void Activate(GameObject user);
}
