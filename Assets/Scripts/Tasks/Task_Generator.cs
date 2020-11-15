using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class Task_Generator : MonoBehaviour, ITask
{
    public Light[] lights;
    public GameObject[] emergencyLights; //for emergency spotlight gameobjects only; need to disable the object bc it shows unlit texture if we just disbale light

    public Task_PluginSlot pluginSlot;
    public Task_Button button;

    private Outline outline;

    public bool isSabotaged = false;
    private float sabotageTimer = 60f;
    private float sabotageCounter = 0f;

    private void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    private void Update()
    {
        //sabotaged
        if(isSabotaged)
        {
            //show outline
            if (!outline.enabled)
                outline.enabled = true;

            //turn off lights
            foreach (Light l in lights)
            {
                l.enabled = false;
            }

            //turn on emergency lights
            foreach (GameObject o in emergencyLights)
            {
                o.SetActive(true);
            }

            sabotageCounter += 5f * Time.deltaTime;
            if (sabotageCounter >= sabotageTimer)
            {
                sabotageCounter = 0f;
                isSabotaged = false;
            }

            return;
        }

        if(pluginSlot.isPluggedIn)
        {
            //turn off/on lights if button is on
            if (button.IsOn)
            {
                foreach (Light l in lights)
                {
                    l.enabled = true;
                }
            }
            else
            {
                foreach (Light l in lights)
                {
                    l.enabled = false;
                }
            }

            //turn off emergency lights
            foreach (GameObject o in emergencyLights)
            {
                o.SetActive(false);
            }
        }
        else
        {
            //turn off lights
            foreach (Light l in lights)
            {
                l.enabled = false;
            }
            //turn on emergency lights
            foreach (GameObject o in emergencyLights)
            {
                o.SetActive(true);
            }
        }
    }

    public string GetInfo()
    {
        string returnText = "GENERATOR STATUS\n";
        if (pluginSlot.isPluggedIn)
            returnText += "ACTIVE\n";
        else
            returnText += "!ERROR!\n";

        if (pluginSlot.isPluggedIn)
        {
            if (button.IsOn)
                returnText += "STATUS: ON\n";
            else
                returnText += "STATUS: OFF\n";
        }

        if(isSabotaged)
        {
            returnText += "Sabotage (60) - (" + sabotageCounter + ")";
        }
        return returnText;
    }

    public bool IsBroken()
    {
        if(!pluginSlot.isPluggedIn)
        {
            return true;
        }
        return false;
    }

    public void Sabotage()
    {
        isSabotaged = true;
    }

    public void OutlineTaskOn()
    {
        outline.enabled = true;
    }
    public void OutlineTaskOff()
    {
        outline.enabled = false;
    }
}
