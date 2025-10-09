using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour
{
    [SerializeField] Text time;
    private float stageTime = 30f; 

    private void Update()
    {
        if ( stageTime - InGameManager.Instance.GameTime >= 0f)
        {
            time.text = (stageTime - InGameManager.Instance.GameTime).ToString("F2");
        }
        else
        {
            // 게임 오버(lose)
        }
    }

}
