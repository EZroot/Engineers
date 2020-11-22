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

    public bool cleanFilter = false;
    private bool isPowered = false;

    //public Outline outline;

    AudioSource windAudioSource;

    private void Start()
    {
        //outline = GetComponent<Outline>();
       // outline.enabled = false;
        windAudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        //sabotaged
        if (isSabotaged)
        {
            //show outline
           // if (!outline.enabled)
            //    outline.enabled = true;
            if (windAudioSource.isPlaying)
                windAudioSource.Stop();
            //maintains dirty filter so it cant be fixed
            cleanFilter = false;

            sabotageCounter += 5f * Time.deltaTime;
            if (sabotageCounter >= sabotageTimer)
            {
                sabotageCounter = 0f;
                isSabotaged = false;
            }

            return;
        }

        if (cleanFilter && isPowered)
        {
          //  outline.enabled = false;
            if (!windAudioSource.isPlaying)
                windAudioSource.Play();
        }
        else
        {
          //  outline.enabled = true;
            if (windAudioSource.isPlaying)
                windAudioSource.Stop();
        }
    }

    public string GetInfo()
    {
        string returnText = "AIR_FILTRATION_STATUS\n";
        if (cleanFilter)
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
        if (!isPowered)
            return true;
        if (isPowered && !cleanFilter)
            return true;
        return false;
    }

    public void OutlineTaskOn()
    {
       // outline.enabled = true;
    }

    public void OutlineTaskOff()
    {
      //  outline.enabled = false;
    }

    [PunRPC]
    public void Sabotage()
    {
        isSabotaged = true;
    }

    [PunRPC]
    public void CleanFilter()
    {
        cleanFilter = true;
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
