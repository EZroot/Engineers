using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Task_Button : MonoBehaviourPun, IInteractive
{
    public Material buttonOff;
    public Material buttonOn;

    //used to change teh color
    public Renderer buttonMeshRenderer;

    private bool isOn = false;
    public bool IsOn { get { return isOn; } set { isOn = value; } }

    private void Start()
    {
        buttonMeshRenderer.material = buttonOff;
    }

    public void On()
    {
        buttonMeshRenderer.material = buttonOn;
    }

    public void Off()
    {
        buttonMeshRenderer.material = buttonOff;
    }

    [PunRPC]
    public void PressedButton(bool ison)
    {
        isOn = ison;
        if(isOn)
        {
            On();
        }
        else
        {
            Off();
        }
    }
}
