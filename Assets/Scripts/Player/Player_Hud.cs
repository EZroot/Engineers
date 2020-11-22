using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player_Hud : MonoBehaviour
{
    public TextMeshProUGUI imposterHudText;
    public TextMeshProUGUI nameHudText;
    public TextMeshProUGUI staminaHudText;
    public TextMeshProUGUI oxygenHudText;
    public TextMeshProUGUI temperatureHudText;
    public TextMeshProUGUI identifyHudText;
    public TextMeshProUGUI progressBar;
    public TextMeshProUGUI taskHudText;
    public TextMeshProUGUI batteryHudText;

    public GameObject minimapImg;

    public Image progressBarBackground;
    public Image progressBarFill;

    public void SetNameText(string text)
    {
        nameHudText.text = text;
    }

    public void SetStaminaText(string text)
    {
        staminaHudText.text = text;
    }

    public void SetOxygenText(string text)
    {
        oxygenHudText.text = text;
    }

    public void SetTemperatureText(string text)
    {
        temperatureHudText.text = text;
    }

    public void SetImposterText(string text)
    {
        imposterHudText.text = text;
    }

    public void SetIdentityText(string text)
    {
        identifyHudText.text = text;
    }

    public void SetProgressBarText(string text)
    {
        progressBar.text = text;
    }

    public void SetTaskText(string text)
    {
        taskHudText.text = text;
    }

    public void SetBatteryHudText(string text)
    {
        batteryHudText.text = text;
    }

    public void MinimapOn()
    {
        minimapImg.SetActive(true);
    }

    public void MinimapOff()
    {
        minimapImg.SetActive(false);
    }

    public void SetProgressBarOff()
    {
        progressBarBackground.enabled = false;
        progressBarFill.enabled = false;
    }

    public void SetProgressBarOn()
    {
        progressBarBackground.enabled = true;
        progressBarFill.enabled = true;
    }

    public void SetProgressBarFillAmount(float amount)
    {
        progressBarFill.fillAmount = amount;
    }
}
