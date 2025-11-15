using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rune : MonoBehaviour
{
    [SerializeField] private RuneData data;
    [SerializeField] private Image icon;
    [SerializeField] private UITweenBouncer bouncer;

    public void SetRuneData(RuneData _data)
    {
        data = _data;
        icon.sprite = SpriteManager.Instance.GetSprite(data.GetRuneIcon());
    }

    private void Update()
    {
        if (bouncer != null && bouncer.isDone)
        {
            RuneManager.Instance.SaveToListedRune(gameObject);
            Destroy(gameObject);
        }
    }

    public RuneData GetRuneData() => data;
}
