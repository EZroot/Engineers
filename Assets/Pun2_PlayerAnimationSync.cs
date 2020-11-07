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
            //stream.SendNext(animator.Trigg)
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
            animator.SetFloat("ForwardSpeed", Mathf.Lerp(animator.GetFloat("ForwardSpeed"), forwardSpeed, 5 * Time.deltaTime));
            animator.SetFloat("SidewardSpeed", Mathf.Lerp(animator.GetFloat("SidewardSpeed"), sideSpeed, 5 * Time.deltaTime));
            animator.SetFloat("TurnSpeed", turnSpeed);
            if(totalMovementSpeed>0.1f)
                animator.SetFloat("TotalMovementSpeed", Mathf.Lerp(animator.GetFloat("TotalMovementSpeed"), totalMovementSpeed, 5 * Time.deltaTime));
            else
                animator.SetFloat("TotalMovementSpeed", 0);
        }
    }
}