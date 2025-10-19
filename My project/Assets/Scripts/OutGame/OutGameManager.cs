using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutGameManager : MonoBehaviour
{
    [SerializeField] private GameObject stageInfoPanel;

    private bool isOpend = false;

    public void OnClickedStageButton()
    {
        if (!isOpend)
        {
            stageInfoPanel.SetActive(true);
            isOpend = true;
        }
    }

    public void OnClickedGamePlayButton()
    {
        // ¾À ÀüÈ¯
    }

    public void OnClickedCancleButton()
    {
        stageInfoPanel.SetActive(false);
        isOpend = false;
    }
}
