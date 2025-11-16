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

    public string GetRuneName() => rune_name;

    public RuneType GetRuneType() => rune_type;

    public string GetRuneDescription() => rune_description;
}

public enum RuneType
{
    FIRE,
    FROST,
    WIND,
    GROUND,
    ROCK
}
