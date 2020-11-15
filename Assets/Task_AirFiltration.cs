using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class Task_AirFiltration : MonoBehaviour, ITask
{
    
    private Outline outline;

    public bool isSabotaged = false;
    private float sabotageTimer = 60f;
    private float sabotageCounter = 0f;

    private bool dirtyFilter = true;

    private void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    private void Update()
    {
        //sabotaged
        if (isSabotaged)
        {
            //show outline
            if (!outline.enabled)
                outline.enabled = true;

            dirtyFilter = true;

            sabotageCounter += 5f * Time.deltaTime;
            if (sabotageCounter >= sabotageTimer)
            {
                sabotageCounter = 0f;
                isSabotaged = false;
            }

            return;
        }

        if (!dirtyFilter)
            outline.enabled = false;
        else
            outline.enabled = true;
    }

    public string GetInfo()
    {
        string returnText = "AIR FILTRATION STATUS\n";
        if (!dirtyFilter)
            returnText += "ACTIVE\n";
        else
            returnText += "!ERROR!\n";

        if (isSabotaged)
        {
            returnText += "Sabotaged (60) - (" + sabotageCounter + ")";
        }
        return returnText;
    }

    public bool IsBroken()
    {
        if (!dirtyFilter)
            return false;
        return true;
    }

    public void OutlineTaskOn()
    {
        outline.enabled = true;
    }

    public void OutlineTaskOff()
    {
        outline.enabled = false;
    }

    public void Sabotage()
    {
        isSabotaged = true;
    }
}
