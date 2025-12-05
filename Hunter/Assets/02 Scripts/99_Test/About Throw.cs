using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AboutThrow : MonoBehaviour
{
    [Header("조준 영역")]
    public GameObject aimArea;
    public TextMeshProUGUI aim_area_scale;

    private GameData gameData;

    void Start()
    {
        if (gameData == null)
        {
            gameData = GameDataManager.Instance.GAMEDATA;
        }

        aim_area_scale.text = $"{gameData.aimAreaSize:F2}";
        aimArea.transform.localScale = Vector3.one * gameData.aimAreaSize;

        aimArea.transform.localPosition = gameData.aimAreaPos;
    }

    public void AimAreaScale(float degree)
    {
        Vector3 currentScale = aimArea.transform.localScale;
        currentScale += Vector3.one * degree;
        aimArea.transform.localScale = currentScale;
        gameData.aimAreaSize = currentScale.x;

        aim_area_scale.text = $"{currentScale.x:F2}";
    }

    public void AimAreaMoveX(float degree)
    {
        Vector2 currentPos = aimArea.transform.localPosition;
        currentPos.x += degree;

        aimArea.transform.localPosition = currentPos;
        gameData.aimAreaPos.x = currentPos.x;
    }

    public void AimAreaMoveY(float degree)
    {
        Vector2 currentPos = aimArea.transform.localPosition;
        currentPos.y += degree;

        aimArea.transform.localPosition = currentPos;
        gameData.aimAreaPos.y = currentPos.y;
    }

}
