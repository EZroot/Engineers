using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/*
 * Need to dissallow rigidbodysync from transfering on contact when its grabbed
 * Need to send a RPC saying its grabbed/isnt grabbed
 */

public class Rigidbody_Drag : MonoBehaviourPun
{
    public float force = 600;
    public float damping = 6;

    Transform jointTrans;
    float dragDepth;

	//public PUN2_RigidbodySync rigidbodySync;

    private Rigidbody thisRb;

    private bool canDrag = true;

    private void Start()
    {
        thisRb = GetComponent<Rigidbody>();
    }
    void OnMouseDown()
    {
        HandleInputBegin(Input.mousePosition);
    }

    void OnMouseUp()
    {
        HandleInputEnd(Input.mousePosition);
    }

    void OnMouseDrag()
    {
        HandleInput(Input.mousePosition);
    }

    public void StopDragging()
    {
        canDrag = false;
        if (!canDrag)
        {
            //tell our clients its now not grabbed anymore
            //rigidbodySync.LetGoOfObject();

            if (jointTrans != null)
                Destroy(jointTrans.gameObject);

            canDrag = true;
        }
    }

    public void HandleInputBegin(Vector3 screenPosition)
    {
        if (!canDrag)
            return;

        var ray = Camera.allCameras[0].ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 4.0f))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Interactive")) //&& !rigidbodySync.isGrabbed)
            {
				//tell our clients its grabbed
				//rigidbodySync.GrabbedObject();
                //transform network ownership
				photonView.TransferOwnership(PhotonNetwork.LocalPlayer);

                dragDepth = Player_CameraPlane.CameraToPointDepth(Camera.allCameras[0], hit.point);
                jointTrans = AttachJoint(hit.rigidbody, hit.point);
            }
        }
    }

    public void HandleInput(Vector3 screenPosition)
    {
        if (jointTrans == null)
            return;
        var worldPos = Camera.allCameras[0].ScreenToWorldPoint(screenPosition);
        jointTrans.position = Player_CameraPlane.ScreenToWorldPlanePoint(Camera.allCameras[0], dragDepth, screenPosition);

        //If distance from player is too great
        //we need to break the join, and make sure all clients know we let it go
    }

    public void HandleInputEnd(Vector3 screenPosition)
    {
        //if (rigidbodySync == null)
        //    return;

        //tell our clients its now not grabbed anymore
		//rigidbodySync.LetGoOfObject();

        if(jointTrans!=null)
            Destroy(jointTrans.gameObject);
    }

    Transform AttachJoint(Rigidbody rb, Vector3 attachmentPosition)
    {
        GameObject go = new GameObject("Attachment Point");
        go.hideFlags = HideFlags.HideInHierarchy;
        go.transform.position = attachmentPosition;

        var newRb = go.AddComponent<Rigidbody>();
        newRb.isKinematic = true;

        var joint = go.AddComponent<ConfigurableJoint>();
        joint.connectedBody = rb;
        joint.configuredInWorldSpace = true;
        joint.xDrive = NewJointDrive(force, damping);
        joint.yDrive = NewJointDrive(force, damping);
        joint.zDrive = NewJointDrive(force, damping);
        joint.slerpDrive = NewJointDrive(force, damping);
        joint.rotationDriveMode = RotationDriveMode.Slerp;
        return go.transform;
    }

    private JointDrive NewJointDrive(float force, float damping)
    {
        JointDrive drive = new JointDrive();
        drive.mode = JointDriveMode.Position;
        drive.positionSpring = force;
        drive.positionDamper = damping;
        drive.maximumForce = Mathf.Infinity;
        return drive;
    }
}
