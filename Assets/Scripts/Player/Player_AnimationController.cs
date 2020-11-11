using CMF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AnimationController : MonoBehaviour
{
    public Transform characterMeshTransform;

    //animation smooth value
    public float smoothingLerpValue = 10f;

    //Walker
    private AdvancedWalkerController controller;

    //Config, if were imposter, animate the monster prefab
    private Player_Config config;

    //turnspeed
    private Vector3 characterMeshDir;

    //smoothing
    private float currentTurnSpeed = 0f;
    private Vector3 currentMovementVelocity;
    private Vector3 currentMomentum;

    private void Start()
    {
        controller = GetComponent<AdvancedWalkerController>();
        config = GetComponent<Player_Config>();
    }

    private void Awake()
    {
        characterMeshDir = characterMeshTransform.forward;
    }

    private void LateUpdate()
    {
        bool _isGrounded = controller.IsGrounded();
        //set animation isgrounded, _isgrounded

        Vector3 _movementVelocity = controller.GetMovementVelocity();
        if(!config.isImposter)
            config.humanAnimator.SetFloat("TotalMovementSpeed", _movementVelocity.magnitude);
        else
            config.monsterAnimator.SetFloat("TotalMovementSpeed", _movementVelocity.magnitude);

        //Smooth movement
        currentMovementVelocity = Vector3.Lerp(currentMovementVelocity, _movementVelocity, Time.deltaTime * smoothingLerpValue);

        //Smooth hand blending
        if (config.handsAnimator != null)
            config.handsAnimator.SetFloat("TotalMovementSpeed", currentMovementVelocity.magnitude);

        float _forwardSpeed = VectorMath.GetDotProduct(currentMovementVelocity, characterMeshTransform.forward);
        float _sidewardSpeed = VectorMath.GetDotProduct(currentMovementVelocity, characterMeshTransform.right);

        if (!config.isImposter)
        {
            config.humanAnimator.SetFloat("ForwardSpeed", _forwardSpeed);
            config.humanAnimator.SetFloat("SidewardSpeed", _sidewardSpeed);
        }
        else
        {
            config.monsterAnimator.SetFloat("ForwardSpeed", _forwardSpeed);
            config.monsterAnimator.SetFloat("SidewardSpeed", _sidewardSpeed);
        }
        Vector3 _momentum = controller.GetMomentum();

        //smooth momentum
        currentMomentum = Vector3.Lerp(currentMomentum, _momentum, Time.deltaTime * smoothingLerpValue);

        float _verticalSpeed = VectorMath.GetDotProduct(currentMomentum, characterMeshTransform.up);
        //animator setfloat speedvertical, _verticalspeed

        //Turning
        Vector3 _newMeshDir = characterMeshTransform.forward;
        float _angle = VectorMath.GetAngle(_newMeshDir, characterMeshDir, characterMeshTransform.up);
        float _turnSpeed = 0f;
        if (Time.deltaTime != 0f)
            _turnSpeed = _angle / Time.deltaTime;

        //smoothing
        currentTurnSpeed = Mathf.Lerp(currentTurnSpeed, _turnSpeed, Time.deltaTime * smoothingLerpValue);
        characterMeshDir = _newMeshDir;

        if(!config.isImposter)
            config.humanAnimator.SetFloat("TurnSpeed", currentTurnSpeed);
        else
            config.monsterAnimator.SetFloat("TurnSpeed", currentTurnSpeed);
    }

    public void TriggerPunch()
    {
        config.handsAnimator.SetTrigger("Punch");
    }

    public void ResetTriggerPunch()
    {
        config.handsAnimator.ResetTrigger("Punch");
    }
}
