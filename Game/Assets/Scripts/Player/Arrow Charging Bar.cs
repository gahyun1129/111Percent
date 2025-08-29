using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowChargingBar : MonoBehaviour
{
    public GameObject ChargingBar;
    

    public void ShowChargingBar()
    {
        ChargingBar.SetActive(true);
    }

    public void HideChargingBar()
    {
        ChargingBar.SetActive(false);
    }

    private void Update()
    {
        PlayerController controller = GetComponent<PlayerController>();
        float value = Mathf.Clamp01(controller.holdTime / 2f);
        UIManager.Instance.UpdateChargingBar(value);
    }

}
