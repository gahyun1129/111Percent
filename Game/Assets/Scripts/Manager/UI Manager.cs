using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
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
        timeUI.text = (90 - (int)Time.time).ToString();

        if (timeUI.text == "0")
        {
            GameManager.Instance.GameEnd(Enemy.tag);
        }

        UpdateActiveSkills();
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

}
