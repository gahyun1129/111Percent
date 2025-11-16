using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rune : MonoBehaviour
{
    [SerializeField] private RuneData data;
    [SerializeField] private Image icon;

    [SerializeField] private UITweenBouncer bouncer;
    [SerializeField] private UITweenParabolaFlyer parabolaFlyer;

    public RectTransform target;

    public void SetRunePrefab(RuneData _data, RectTransform rect)
    {
        data = _data;
        icon.sprite = SpriteManager.Instance.GetSprite(data.GetRuneIcon());
        target = rect;
    }

    private void Update()
    {
        if (bouncer != null && bouncer.isDone)
        {
            parabolaFlyer.PlayFlyAnimation(target.position);
            bouncer = null;
        }

        if ( parabolaFlyer != null && parabolaFlyer.isDone)
        {
            RuneManager.Instance.SaveToListedRune(gameObject);
            Destroy(gameObject);
        }
    }

    public RuneData GetRuneData() => data;
}
