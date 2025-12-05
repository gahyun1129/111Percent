using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    [Header("Weapon Flying")]
    [SerializeField] private GameObject flyingBirdFeather;
    [SerializeField] private float feather_scale;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    public void PlayFeather()
    {
        flyingBirdFeather.transform.localScale = Vector3.one * feather_scale;
        flyingBirdFeather.SetActive(true);
    }

    public void StopFeather()
    {
        flyingBirdFeather.SetActive(false);
    }
}
