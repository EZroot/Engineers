using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;
using Photon.Pun;

public class Task_AirFiltration : MonoBehaviourPun, ITask, IPowered
{
    public bool isSabotaged = false;
    private float sabotageTimer = 60f;
    private float sabotageCounter = 0f;

    public bool dirtyFilter = true;
    private bool isPowered = false;

    private Outline outline;

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

            //maintains dirty filter so it cant be fixed
            dirtyFilter = true;

            sabotageCounter += 5f * Time.deltaTime;
            if (sabotageCounter >= sabotageTimer)
            {
                sabotageCounter = 0f;
                isSabotaged = false;
            }

            return;
        }

        if (!dirtyFilter && isPowered)
            outline.enabled = false;
        else
            outline.enabled = true;
    }

    public string GetInfo()
    {
        string returnText = "AIR_FILTRATION_STATUS\n";
        if (!dirtyFilter)
            returnText += "Filter Status: Clean\n";
        else
            returnText += "Filter Status: DIRTY\n";

        if(isPowered)
        {
            returnText += "Status: Powered\n";
        }
        else
        {
            returnText += "ERR; Power Outage\n";
        }

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

    [PunRPC]
    public void Sabotage()
    {
        isSabotaged = true;
    }

    [PunRPC]
    public void CleanFilter()
    {
        dirtyFilter = false;
    }

    public void SetPower(bool powered)
    {
        isPowered = powered;
    }

    public bool IsPowered()
    {
        return isPowered;
    }
}
