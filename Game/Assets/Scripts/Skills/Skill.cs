using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : ScriptableObject
{
    public string skillName;
    public Sprite icon;
    public float cooldown;
    protected float lastUseTime;

    public virtual bool CanUse()
    {
        return true/*Time.time >= lastUseTime + cooldown*/;
    }

    public void Use(GameObject user)
    {
        if (CanUse())
        {
            Activate(user);
            lastUseTime = Time.time;
        }
    }

    protected abstract void Activate(GameObject user);
}
