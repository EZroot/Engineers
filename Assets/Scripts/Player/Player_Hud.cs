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
    public TextMeshProUGUI identifyHudText;
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

    public void SetImposterText(string text)
    {
        imposterHudText.text = text;
    }

    public void SetIdentityText(string text)
    {
        identifyHudText.text = text;
    }

    public void MinimapToggle()
    {
        minimapCam.enabled = !minimapCam.enabled;
        minimapHud.enabled = !minimapHud.enabled;
    }
}
