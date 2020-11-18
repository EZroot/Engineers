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
    public bool isGrabbed = false;

    private Transform jointTrans;
    private float dragDepth;
    public Rigidbody thisRb;

    //for slottables
    private Task_Plugin pluginTask;

    public bool playThumpSound = true;
    private AudioSource audioSource;

    private void Start()
    {
        pluginTask = GetComponent<Task_Plugin>();
        thisRb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (audioSource == null || !playThumpSound)
            return;
        //depending on velocity mag we should player louder sounds
        //if(thisRb.velocity.magnitude>1f)
            audioSource.Play();
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

    public void StartDragging(RaycastHit hit)
    {
        //If its a plugin
        if(pluginTask!=null)
        {
            if(pluginTask.IsSlotted)
            {
                Debug.Log("Should be unslotted in 1 second.");
                pluginTask.UnSlot();
            }
        }

        //check to see if its a plugin/battery
        photonView.RPC("IsGrabbedRPC", RpcTarget.AllBufferedViaServer, true);

        //transform network ownership
        //this glitches when it collides with another owner
        photonView.TransferOwnership(PhotonNetwork.LocalPlayer);

        dragDepth = Player_CameraPlane.CameraToPointDepth(Camera.allCameras[0], hit.point);
        jointTrans = AttachJoint(hit.rigidbody, hit.point);
    }

    public void StopDragging()
    {
        isGrabbed = false;
        if (!isGrabbed)
        {
            photonView.RPC("IsGrabbedRPC", RpcTarget.AllBufferedViaServer, false);

            if (jointTrans != null)
                Destroy(jointTrans.gameObject);
        }
    }

    public void HandleInputBegin(Vector3 screenPosition)
    {
        if (isGrabbed)
            return;

        var ray = Camera.allCameras[0].ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 4.0f))
        {
            StartDragging(hit);
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
        //tell our clients its now not grabbed anymore
        StopDragging();

    }

    [PunRPC]
    void IsGrabbedRPC(bool isgrabbed)
    {
        isGrabbed = isgrabbed;
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
