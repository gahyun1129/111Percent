using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerr : MonoBehaviour
{
    float hp = 100f;

    public bool getAntidote = false;

    public GameObject enemy;
    public Slider hpslider;
    public GameObject ment;
    public GameObject poison;
    public GameObject endPanel;

    public void GetPoison()
    {
        StartCoroutine(Poison());
    }

    IEnumerator Poison()
    {
        while(true)
        {
            if (getAntidote)
            {
                yield break;
            }

            OnDamaged(10f);
            poison.SetActive(true);

            yield return new WaitForSeconds(1f);

            poison.SetActive(false);
            
            yield return new WaitForSeconds(1f);
        }
    }

    private void Start()
    {
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        while (enemy != null)
        {
            enemy.GetComponent<eenmy>().damage(10f);
            yield return new WaitForSeconds(1f);
        }
    }

    public void OnDamaged(float dmg)
    {
        hp -= dmg;
        hpslider.value = hp / 100f;

        if ( hp <= 0f)
        {
            Destroy(gameObject);
            endPanel.SetActive(true);
        }
    }

    public void OnClickedAntidote(Button btn)
    {
        getAntidote = true;
        btn.interactable = false;
        StartCoroutine(ShowMent());
    }

    IEnumerator ShowMent()
    {
        ment.SetActive(true);

        yield return new WaitForSeconds(2f);

        ment.SetActive(false);
    }
}
