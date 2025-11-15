using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rune : MonoBehaviour
{
    [SerializeField] private RuneData data;
    [SerializeField] private Image icon;

    public void SetRuneData(RuneData _data)
    {
        data = _data;
        icon.sprite = SpriteManager.Instance.GetSprite(data.GetRuneIcon());
    }
}
