using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task_Generator : MonoBehaviour
{
    public Light[] lights;
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
        }
        else
        {
            foreach (Light l in lights)
            {
                l.enabled = false;
            }
        }
    }
}
