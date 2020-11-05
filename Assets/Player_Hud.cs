using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player_Hud : MonoBehaviour
{
    public TextMeshProUGUI imposterHudText;
    public TextMeshProUGUI nameHudText;
    public TextMeshProUGUI staminaHudText;

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
}
