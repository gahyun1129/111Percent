using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Player")]
    public Slider playerHPBarUI;
    public Text playerHPUI;
    public Text[] activeSkillsText;
    public Slider[] activeSkillsSlider;
    public GameObject passiveSkillImage;
    public Slider chargingBar;
    public GameObject chargingMax;

    [Header("Enemy")]
    public Slider enemyHPBarUI;
    public Text enemyHPUI;
    public GameObject enemyChasingMark;

    [Header("InGame")]
    public TextMeshProUGUI countDownText;
    public Text timeUI;
    public GameObject damagedImg;


    private bool isTimerActive = false;
    private float gameDuration = 90f;

    private GameObject Player;
    private GameObject Enemy;

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

        Player = GameManager.Instance.GetPlayer();
        Enemy = GameManager.Instance.GetEnemy();

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
        isTimerActive = true;
    }

    public void StopGameTimer()
    {
        isTimerActive = false;
    }

    public void UpdateHPUI()
    {
        playerHPBarUI.value = Player.GetComponent<Health>().GetHP() / Player.GetComponent<Health>().GetMAXHP();
        enemyHPBarUI.value = Enemy.GetComponent<Health>().GetHP() / Enemy.GetComponent<Health>().GetMAXHP();
        
        playerHPUI.text = Mathf.Max(Player.GetComponent<Health>().GetHP(), 0).ToString();
        enemyHPUI.text = Mathf.Max(Enemy.GetComponent<Health>().GetHP(), 0).ToString();
    }

    public void OnSkillButtonClick(int index)
    {
        SkillManager.Instance.UseSkill(index);
    }

    public void UpdateActiveSkills()
    {
        for ( int i = 0; i < activeSkillsSlider.Length; ++i)
        {
            float coolTime = SkillManager.Instance.GetCoolTime(i);
            float remainTime = Mathf.CeilToInt(SkillManager.Instance.GetRemainTime(i));
            if ( coolTime <= -1f) continue;

            if (remainTime == 0)
            {
                activeSkillsSlider[i].value = remainTime / coolTime;
                activeSkillsText[i].enabled = false;
            }
            else
            {
                activeSkillsText[i].enabled = true;
                activeSkillsSlider[i].value = remainTime / coolTime;
                activeSkillsText[i].text = remainTime.ToString();
            }


        }
    }

    public void UpdatePassiveSkill()
    {
        passiveSkillImage.SetActive(true);
    }

    public void UpdateChargingBar(float value)
    {
        chargingBar.value = value;

        if ( value >= 1f)
        {
            chargingMax.SetActive(true);   
        }
        else
        {
            chargingMax.SetActive(false);
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

        damagedImg.transform.localScale = originalScale;
        playerHPBarUI.transform.localPosition = originalPosition;

        damagedImg.SetActive(false);
    }

    public void ShowChasingMark()
    {
        enemyChasingMark.SetActive(true);
    }

    public void HideChasingMark()
    {
        enemyChasingMark.SetActive(false);
    }
}
