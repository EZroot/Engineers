using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using CMF;

public class Pun2_PlayerAnimationSync : MonoBehaviourPun, IPunObservable
{
    public Animator animator;

    float forwardSpeed;
    float sideSpeed;
    float turnSpeed;
    float totalMovementSpeed;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(animator.GetFloat("ForwardSpeed"));
            stream.SendNext(animator.GetFloat("SidewardSpeed"));
            stream.SendNext(animator.GetFloat("TurnSpeed"));
            stream.SendNext(animator.GetFloat("TotalMovementSpeed"));

            //stream.SendNext(animatorState);

        }
        else
        {
            //Network player, receive data
            forwardSpeed = (float)stream.ReceiveNext();
            sideSpeed = (float)stream.ReceiveNext();
            turnSpeed = (float)stream.ReceiveNext();
            totalMovementSpeed = (float)stream.ReceiveNext();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Remote player
        if (!photonView.IsMine)
        {
            animator.SetFloat("ForwardSpeed", forwardSpeed);
            animator.SetFloat("SidewardSpeed", sideSpeed);
            animator.SetFloat("TurnSpeed", turnSpeed);
            animator.SetFloat("TotalMovementSpeed", totalMovementSpeed);
        }
    }
}