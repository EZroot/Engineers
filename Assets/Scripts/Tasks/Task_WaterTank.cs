using cakeslice;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task_WaterTank : MonoBehaviourPun, IPowered, ITask
{
    public bool isSabotaged = false;
    private float sabotageTimer = 60f;
    private float sabotageCounter = 0f;

    public bool clearWater = false;
    private bool isPowered = false;

    private GameObject valveWheel;

   // public Outline outline;

    AudioSource waterAudioSource;

    private void Start()
    {
        //outline.enabled = false;
        valveWheel = transform.GetChild(0).gameObject;
        waterAudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        //sabotaged
        if (isSabotaged)
        {
            //show outline
            //if (!outline.enabled)
            //    outline.enabled = true;

            if (waterAudioSource.isPlaying)
                waterAudioSource.Stop();

            //maintains dirty filter so it cant be fixed
            clearWater = false;

            sabotageCounter += 5f * Time.deltaTime;
            if (sabotageCounter >= sabotageTimer)
            {
                sabotageCounter = 0f;
                isSabotaged = false;
            }

            return;
        }

        if (clearWater && isPowered)
        {
            //outline.enabled = false;
            if (!waterAudioSource.isPlaying)
                waterAudioSource.Play();
        }
        else
        {
            //outline.enabled = true;
            if (waterAudioSource.isPlaying)
                waterAudioSource.Stop();
        }

    }

    public void SpinValve(float spinSpeed)
    {
        valveWheel.transform.Rotate(0, spinSpeed * Time.deltaTime,0); //rotates 50 degrees per second around z axis
    }

    public string GetInfo()
    {
        string returnText = "WATER_TANK_STATUS\n";
        if (clearWater)
            returnText += "Water Status: Flowing\n";
        else
            returnText += "Water Status: Stopped\n";

        if (isPowered)
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
        if (isPowered && !clearWater)
            return true;
        return false;
    }

    public void OutlineTaskOn()
    {
       // outline.enabled = true;
    }

    public void OutlineTaskOff()
    {
       // outline.enabled = false;
    }

    [PunRPC]
    public void Sabotage()
    {
        isSabotaged = true;
    }

    [PunRPC]
    public void ClearedWater()
    {
        clearWater = true;
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
