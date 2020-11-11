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

    //dont lock it if the hinge joint doesnt exist (door torn off)
    private HingeJoint[] doorHinges;

    private void Start()
    {
        doorHinges = new HingeJoint[doors.Length];

        for(int i =0; i < doors.Length;i++)
        {
            doorHinges[i] = doors[i].gameObject.GetComponent<HingeJoint>();
        }
    }

    //unlocked
    public void On()
    {
        if(otherNumpad!=null)
        {
            otherNumpad.unlocked = unlocked;
        }

        for(int i = 0; i < doors.Length;i++)
        {
                doors[i].isKinematic = false;
        }
    }

    //locked
    public void Off()
    {
        if (otherNumpad != null)
        {
            otherNumpad.unlocked = unlocked;
        }

        for (int i = 0; i < doors.Length; i++)
        {
            if (doorHinges[i] != null)
                doors[i].isKinematic = true;
            else
                doors[i].isKinematic = false;

            
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
