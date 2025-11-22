using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance { get; private set; }

    private void Awake()
    {
        if ( Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }    

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void LoadSceneByName(string name)
    {
        SceneManager.LoadScene(name);
    }
}
