using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Task_DoorNumpad : MonoBehaviourPun, IInteractive
{
    public Rigidbody[] doors;
    public bool unlocked = false;
    public Task_DoorNumpad otherNumpad;

    public bool IsOn { get { return unlocked; } set { unlocked = value; } }

    public void On()
    {
        if(otherNumpad!=null)
        {
            otherNumpad.unlocked = unlocked;
        }

        foreach (Rigidbody rb in doors)
        {
            rb.isKinematic = false;
        }
    }

    public void Off()
    {
        if (otherNumpad != null)
        {
            otherNumpad.unlocked = unlocked;
        }

        foreach (Rigidbody rb in doors)
        {
            rb.isKinematic = true;
        }
    }

    [PunRPC]
    void PressedButton(bool isLocked)
    {
        unlocked = isLocked;

        if (unlocked)
        {
            On();
        }
        else
        {
            Off();
        }
    }
}
