using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rigidbody_DistanceLimit : MonoBehaviour
{
    public LayerMask rbDragLayerMask;
    public int maxDistanceBetweenObjects = 4;
    public Transform otherObject;

    private bool checkDist = false;
    private Rigidbody_Drag rbDrag;

    private void Start()
    {
        rbDrag = GetComponent<Rigidbody_Drag>();
    }

    // Update is called once per frame
    void Update()
    {
        //check our rb_drag objs distance from player
        if (Input.GetMouseButtonDown(0))
        {
            checkDist = true;
        }

        //checking distance between player obj and obj
        if (checkDist)
        {
            Vector3 vecBetweenTwoPoints = otherObject.transform.position - transform.position;
            float mag = vecBetweenTwoPoints.magnitude;
            if (mag > maxDistanceBetweenObjects)
            {
                rbDrag.StopDragging();
                //checkDist = false;
            }
        }
        //reset out rbDrag for the next obj
        if (Input.GetMouseButtonUp(0))
        {
            checkDist = false;
        }
    }
}
