using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class eenmy : MonoBehaviour
{
    float hp = 100f;

    public GameObject player;
    public Slider hpslider;
    public SpriteRenderer img;
    public GameObject endPanel;

    private void Start()
    {
        player.GetComponent<playerr>().GetPoison();
    }

    IEnumerator OnDamaged(float dmg)
    {
        hp -= dmg;
        hpslider.value = hp / 100f;
        img.color = Color.red;

        if ( hp <= 0f)
        {
            Destroy(gameObject);
            endPanel.SetActive(true);
        }

        yield return new WaitForSeconds(0.2f);

        img.color = Color.white;
        yield return null;
    }

    public void damage(float dmg)
    {
        StartCoroutine(OnDamaged(dmg));
    }
}
