using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task_Generator : MonoBehaviour, ITask
{
    public Light[] lights;
    //we use gameobject so we can easily disable the unlit texture that it shows when the spotlights turned off
    public GameObject[] emergencyLights;

    
    public Task_PluginSlot pluginSlot;
    public Task_Button button;

    private void Update()
    {
        if(pluginSlot.isPluggedIn)
        {
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

            foreach (GameObject o in emergencyLights)
            {
                o.SetActive(false);
            }
        }
        else
        {
            foreach (Light l in lights)
            {
                l.enabled = false;
            }
            foreach(GameObject o in emergencyLights)
            {
                o.SetActive(true);
            }
        }
    }

    public string GetInfo()
    {
        string returnText = "";
        if (pluginSlot.isPluggedIn)
            returnText += "ACTIVE\n";
        else
            returnText += "!ERROR!\n";

        if (pluginSlot.isPluggedIn)
        {
            if (button.IsOn)
                returnText += "STATUS: ON";
            else
                returnText += "STATUS: OFF";
        }
        return returnText;
    }
}
