using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private Text rune_num_text;

    [SerializeField] private Image current_rune_icon;

    private string current_rune_name_none = "current_rune_name_none";

    public void UpdateRuneNumText(int num)
    {
        rune_num_text.text = num.ToString();
    }

    public void UpdateCurrentRuneIcon(RuneData rune)
    {
        if ( rune != null)
        {
            current_rune_icon.sprite = SpriteManager.Instance.GetSprite(rune.GetRuneIcon());
        }
        else
        {
            current_rune_icon.sprite = SpriteManager.Instance.GetSprite(current_rune_name_none);
        }

    }

    public void OnClickedSpawnRuneButton()
    {
        RuneManager.Instance.SpawnRune();
    }
    
    public void OnClickedNextRuneButton()
    {
        RuneManager.Instance.SkipToNextRune();
    }
}
