using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using CMF;

public class Pun2_PlayerAnimationSync : MonoBehaviourPun, IPunObservable
{
    private float forwardSpeed;
    private float sideSpeed;
    private float turnSpeed;
    private float totalMovementSpeed;

    private Player_Config config;

    private void Start()
    {
        //if NOT photonview.ismine
        config = GetComponent<Player_Config>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //We own this player: send the others our data
            if(config.isImposter)
            {
                stream.SendNext(config.monsterAnimator.GetFloat("ForwardSpeed"));
                stream.SendNext(config.monsterAnimator.GetFloat("SidewardSpeed"));
                stream.SendNext(config.monsterAnimator.GetFloat("TurnSpeed"));
                stream.SendNext(config.monsterAnimator.GetFloat("TotalMovementSpeed"));
            }
            else
            {
                stream.SendNext(config.humanAnimator.GetFloat("ForwardSpeed"));
                stream.SendNext(config.humanAnimator.GetFloat("SidewardSpeed"));
                stream.SendNext(config.humanAnimator.GetFloat("TurnSpeed"));
                stream.SendNext(config.humanAnimator.GetFloat("TotalMovementSpeed"));
            }
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
            //WARNING!!!!!!
            //THIS WILL GET LOCAL IMPOSTER, THEREFORE WERE APPLYING OUR OWN SHIT TO CLIENT
            //just a hack for now, fix this later
            //need to sync impsoter statusss
            if (config.isImposter)
            {
                config.monsterAnimator.SetFloat("ForwardSpeed", Mathf.Lerp(config.monsterAnimator.GetFloat("ForwardSpeed"), forwardSpeed, 5 * Time.deltaTime));
                config.monsterAnimator.SetFloat("SidewardSpeed", Mathf.Lerp(config.monsterAnimator.GetFloat("SidewardSpeed"), sideSpeed, 5 * Time.deltaTime));
                config.monsterAnimator.SetFloat("TurnSpeed", turnSpeed);
                if (totalMovementSpeed > 0.1f)
                    config.monsterAnimator.SetFloat("TotalMovementSpeed", Mathf.Lerp(config.monsterAnimator.GetFloat("TotalMovementSpeed"), totalMovementSpeed, 5 * Time.deltaTime));
                else
                    config.monsterAnimator.SetFloat("TotalMovementSpeed", 0);
            }
            else
            {
                config.humanAnimator.SetFloat("ForwardSpeed", Mathf.Lerp(config.humanAnimator.GetFloat("ForwardSpeed"), forwardSpeed, 5 * Time.deltaTime));
                config.humanAnimator.SetFloat("SidewardSpeed", Mathf.Lerp(config.humanAnimator.GetFloat("SidewardSpeed"), sideSpeed, 5 * Time.deltaTime));
                config.humanAnimator.SetFloat("TurnSpeed", turnSpeed);
                if (totalMovementSpeed > 0.1f)
                    config.humanAnimator.SetFloat("TotalMovementSpeed", Mathf.Lerp(config.humanAnimator.GetFloat("TotalMovementSpeed"), totalMovementSpeed, 5 * Time.deltaTime));
                else
                    config.humanAnimator.SetFloat("TotalMovementSpeed", 0);
            }
        }
    }
}