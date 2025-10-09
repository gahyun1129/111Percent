using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartUI : MonoBehaviour
{
    [SerializeField] GameObject StartPanel;

    public void OnClickedStartButton()
    {
        StartPanel.SetActive(false);
        InGameManager.Instance.SetGameState(true);
    }
}
