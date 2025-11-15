using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "rune_", menuName = "Data/Rune Data")]
public class RuneData : ScriptableObject
{
    [Header("·é ±âº» Á¤º¸")]
    [SerializeField] private string rune_icon;
    [SerializeField] private RuneType rune_type;
    [SerializeField] private string rune_name;
    [TextArea] [SerializeField] private string rune_description;

    public string GetRuneIcon() => rune_icon;
}

public enum RuneType
{
    FIRE,
    FROST,
    WIND,
    GROUND,
    ROCK
}
