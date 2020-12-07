using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_HeadLook : MonoBehaviour
{
    public GameObject headMover;

    private void FixedUpdate()
    {
        //gonna try camera pivot approach first (nvm cause cam gets disabled)
        RaycastHit hit;
        Ray ray = Camera.allCameras[0].ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            headMover.transform.position = hit.point;
        }
    }
}
