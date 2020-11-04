using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Config : MonoBehaviour
{
    //Role
    public bool isImposter;
    public bool isEngineer;
    public bool isChemist;
    public bool isBotanist;
    public bool isInvestigator;

    //speed
    public int walkSpeed = 2;
    public int runSpeed = 5;

    //Stamina
    public int staminaLevel = 60;
    public int staminaLevelForRecover = 15;

    [HideInInspector]
    public bool canRun = true;
    [HideInInspector]
    public bool staminaExhausted = false;
    [HideInInspector]
    public float runStamina;
}
