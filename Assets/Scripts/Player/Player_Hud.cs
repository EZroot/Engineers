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

    public RawImage minimapHud;
    public Camera minimapCam;

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

    public void MinimapToggle()
    {
        minimapCam.enabled = !minimapCam.enabled;
        minimapHud.enabled = !minimapHud.enabled;
    }
}
