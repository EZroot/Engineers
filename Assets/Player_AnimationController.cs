using CMF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AnimationController : MonoBehaviour
{
    public Animator bodyAnimator;
    public Animator handsAnimator;

    public AdvancedWalkerController controller;
    public Transform characterMeshTransform;

    //animation smooth value
    public float smoothingLerpValue = 10f;

    //turnspeed
    Vector3 characterMeshDir;

    //smoothing
    float currentTurnSpeed = 0f;
    Vector3 currentMovementVelocity;
    Vector3 currentMomentum;

    private void Awake()
    {
        characterMeshDir = characterMeshTransform.forward;
    }

    private void LateUpdate()
    {
        bool _isGrounded = controller.IsGrounded();
        //set animation isgrounded, _isgrounded

        Vector3 _movementVelocity = controller.GetMovementVelocity();
        bodyAnimator.SetFloat("TotalMovementSpeed",_movementVelocity.magnitude);

        //Smooth movement
        currentMovementVelocity = Vector3.Lerp(currentMovementVelocity, _movementVelocity, Time.deltaTime*smoothingLerpValue);

        //Smooth hand blending
        handsAnimator.SetFloat("TotalMovementSpeed", currentMovementVelocity.magnitude);

        float _forwardSpeed = VectorMath.GetDotProduct(currentMovementVelocity, characterMeshTransform.forward);
        float _sidewardSpeed = VectorMath.GetDotProduct(currentMovementVelocity, characterMeshTransform.right);

        bodyAnimator.SetFloat("ForwardSpeed", _forwardSpeed);
        bodyAnimator.SetFloat("SidewardSpeed", _sidewardSpeed);

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
        bodyAnimator.SetFloat("TurnSpeed", currentTurnSpeed);
    }
}
