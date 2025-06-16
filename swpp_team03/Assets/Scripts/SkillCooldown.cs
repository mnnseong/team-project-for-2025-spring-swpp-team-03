using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillCooldown : MonoBehaviour
{
    public Image cooldownOverlay;
    //public TextMeshProUGUI cooldownText;
    public float cooldownDuration;

    private float remainingTime = 0f;

    public void TriggerCooldown()
    {
        remainingTime = cooldownDuration;
        cooldownOverlay.fillAmount = 1f;
        //cooldownText.gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
		cooldownOverlay.fillAmount = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            cooldownOverlay.fillAmount = remainingTime / cooldownDuration;
            //cooldownText.text = Mathf.Ceil(remainingTime).ToString("0");

            if (remainingTime <= 0f)
            {
                cooldownOverlay.fillAmount = 0f;
                //cooldownText.gameObject.SetActive(false);
            }
        }
    }

    public bool IsOnCooldown()
    {
        return remainingTime > 0f;
    }

}
