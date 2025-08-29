using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI countDownText;

    private bool isTimerActive = false;
    private float gameDuration = 90f;

    public Slider playerHPBarUI;
    public Slider enemyHPBarUI;

    public Text playerHPUI;
    public Text enemyHPUI;

    public GameObject Player;
    public GameObject Enemy;

    public Text timeUI;

    public Text[] ActiveSkillsText;
    public Slider[] ActiveSkillsSlider;

    public GameObject PassiveSkillImage;

    public Slider ChargingBar;
    public GameObject ChargingMax;

    public GameObject damagedImg;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        playerHPBarUI.value = Player.GetComponent<Health>().GetHP() / Player.GetComponent<Health>().GetMAXHP();
        enemyHPBarUI.value = Enemy.GetComponent<Health>().GetHP() / Enemy.GetComponent<Health>().GetMAXHP();

        playerHPUI.text = Player.GetComponent<Health>().GetHP().ToString();
        enemyHPUI.text = Enemy.GetComponent<Health>().GetHP().ToString();

        timeUI.text = "90";

    }

    private void Update()
    {

        if (!isTimerActive) return;

        float remaining = gameDuration - GameManager.Instance.GameTime;
        timeUI.text = Mathf.CeilToInt(remaining).ToString();

        if ( remaining <= 0)
        {
            isTimerActive = false;
            GameManager.Instance.GameOver("Enemy");
        }

        UpdateActiveSkills();
    }

    public IEnumerator ShowCountdown(int seconds)
    {
        int count = seconds;

        while( count > 0)
        {
            countDownText.text = count.ToString();
            yield return new WaitForSeconds(1f);
            count--;
        }

        countDownText.text = "Start!";
        yield return new WaitForSecondsRealtime(1f);

        countDownText.gameObject.SetActive(false);

        GameManager.Instance.StartGameRoutine();
    }

    public void StartGameTimer()
    {
        isTimerActive=true;
    }

    public void UpdateHPUI()
    {
        playerHPBarUI.value = Player.GetComponent<Health>().GetHP() / Player.GetComponent<Health>().GetMAXHP();
        enemyHPBarUI.value = Enemy.GetComponent<Health>().GetHP() / Enemy.GetComponent<Health>().GetMAXHP();

        playerHPUI.text = Player.GetComponent<Health>().GetHP().ToString();
        enemyHPUI.text = Enemy.GetComponent<Health>().GetHP().ToString();
    }

    public void OnSkillButtonClick(int index)
    {
        SkillManager.Instance.UseSkill(index);
    }

    public void UpdateActiveSkills()
    {
        for ( int i = 0; i < ActiveSkillsSlider.Length; ++i)
        {
            float coolTime = SkillManager.Instance.GetCoolTime(i);
            float remainTime = Mathf.CeilToInt(SkillManager.Instance.GetRemainTime(i));
            if ( coolTime <= -1f) continue;

            if (remainTime == 0)
            {
                ActiveSkillsSlider[i].value = remainTime / coolTime;
                ActiveSkillsText[i].enabled = false;
            }
            else
            {
                ActiveSkillsText[i].enabled = true;
                ActiveSkillsSlider[i].value = remainTime / coolTime;
                ActiveSkillsText[i].text = remainTime.ToString();
            }


        }
    }

    public void UpdatePassiveSkill()
    {
        PassiveSkillImage.SetActive(true);
    }

    public void UpdateChargingBar(float value)
    {
        ChargingBar.value = value;

        if ( value >= 1f)
        {
            ChargingMax.SetActive(true);   
        }
        else
        {
            ChargingMax.SetActive(false);
        }
    }

    public IEnumerator ShowDamagedImage()
    {
        damagedImg.SetActive(true);

        Vector3 originalScale = damagedImg.transform.localScale;
        Vector3 originalPosition = playerHPBarUI.transform.localPosition;
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float scale = Mathf.Sin(elapsed) + 0.9f;
            float newPos = Mathf.Sin(elapsed * 20) * 2;

            playerHPBarUI.gameObject.transform.localPosition = originalPosition + new Vector3(0, newPos, 0); 
            damagedImg.transform.localScale = originalScale * scale;

            yield return null;
        }

        // 원래 크기로 복구
        damagedImg.transform.localScale = originalScale;
        playerHPBarUI.transform.localPosition = originalPosition;

        damagedImg.SetActive(false);
    }
}
