using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;
using Photon.Pun;

public class Task_Generator : MonoBehaviourPun, ITask
{
    public Light[] lights;
    public GameObject[] emergencyLights; //for emergency spotlight gameobjects only; need to disable the object bc it shows unlit texture if we just disbale light
    
    //grab stuff like air filter which would require power
    public GameObject[] objectsToBePowered;
    private IPowered[] tasksToBePowered;

    public Task_PluginSlot pluginSlot;
    public Task_Button button;

    public Outline outline;

    public bool isSabotaged = false;
    private float sabotageTimer = 60f;
    private float sabotageCounter = 0f;

    private AudioSource audioSource;

    public Renderer[] decalLightRenderers;
    public Material onDecalLightMat;
    public Material offDecalLightMat;

    private void Start()
    {
        //outline = GetComponent<Outline>();
        outline.enabled = false;

        //power other task components
        tasksToBePowered = new IPowered[objectsToBePowered.Length];
        for(int i =0;i < objectsToBePowered.Length;i++)
        {
            tasksToBePowered[i] = objectsToBePowered[i].GetComponent<IPowered>();
        }

        audioSource = GetComponent<AudioSource>();
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

            foreach(Renderer r in decalLightRenderers)
            {
                if(r!=null)
                    r.material = offDecalLightMat;
            }

            foreach(IPowered poweredItem in tasksToBePowered)
            {
                poweredItem.SetPower(false);
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
            if (!audioSource.isPlaying)
                audioSource.Play();

            //turn off/on lights if button is on
            if (button.IsOn)
            {
                foreach (Light l in lights)
                {
                    l.enabled = true;
                }
                foreach (Renderer r in decalLightRenderers)
                {
                    if (r != null)
                        r.material = onDecalLightMat;
                }
            }
            else
            {
                foreach (Light l in lights)
                {
                    l.enabled = false;
                    foreach (Renderer r in decalLightRenderers)
                    {
                        if (r != null)
                            r.material = offDecalLightMat;
                    }
                }
            }

            foreach (IPowered poweredItem in tasksToBePowered)
            {
                poweredItem.SetPower(true);
            }

            //turn off emergency lights
            foreach (GameObject o in emergencyLights)
            {
                o.SetActive(false);
            }
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
            //turn off lights
            foreach (Light l in lights)
            {
                l.enabled = false;
            }
            foreach (Renderer r in decalLightRenderers)
            {
                if (r != null)
                    r.material = offDecalLightMat;
            }
            //turn on emergency lights
            foreach (GameObject o in emergencyLights)
            {
                o.SetActive(true);
            }

            foreach (IPowered poweredItem in tasksToBePowered)
            {
                poweredItem.SetPower(false);
            }
        }
    }

    public string GetInfo()
    {
        string returnText = "GENERATOR_STATUS\n";
        if (pluginSlot.isPluggedIn)
            returnText += "Status: Powering components\n";
        else
            returnText += "!ERROR! No power produced\n";

        if (pluginSlot.isPluggedIn)
        {
            if (button.IsOn)
                returnText += "Lights: ON\n";
            else
                returnText += "Lights: OFF\n";
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

    [PunRPC]
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
