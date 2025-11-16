using System;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI; // Text, Image 등을 사용한다면 필요

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }

    public GameObject tooltipPopup;

    public Text itemNameText;
    public Text itemDescriptionText;

    private ListedRuneSlot currentSlot;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (tooltipPopup != null)
        {
            tooltipPopup.SetActive(false);
        }
    }

    public void ShowTooltip(ListedRuneSlot slot, RuneData _data)
    {
        HideTooltip();

        currentSlot = slot;

        if (itemNameText != null) itemNameText.text = _data.GetRuneName();

        switch (_data.GetRuneType())
        {
            case RuneType.FIRE:
                {
                    itemDescriptionText.text = "<color=red>[불 속성]";
                    break;
                }
            case RuneType.ROCK:
                {
                    itemDescriptionText.text = "<color=gray>[바위 속성]";
                    break;
                }
            case RuneType.FROST:
                {
                    itemDescriptionText.text = "<color=blue>[얼음 속성]";
                    break;
                }
            case RuneType.GROUND:
                {
                    itemDescriptionText.text = "<color=yellow>[땅 속성]";
                    break;
                }
            case RuneType.WIND:
                {
                    itemDescriptionText.text = "<color=green>[바람 속성]";
                    break;
                }
        }
        itemDescriptionText.text += "</color> " + _data.GetRuneDescription();

        if (tooltipPopup != null)
        {
            tooltipPopup.SetActive(true);
        }
    }

    public void HideTooltip()
    {
        if (tooltipPopup != null && tooltipPopup.activeSelf)
        {
            tooltipPopup.SetActive(false);
            currentSlot = null;
        }
    }
}