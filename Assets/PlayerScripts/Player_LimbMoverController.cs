using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_LimbMoverController : MonoBehaviour
{
    public Transform leftHandMover;
    public Transform rightHandMover;

    //Hardcoded position vals
    private Vector3 lefthandCarryMoverPositionEnd = new Vector3(-0.35f, 1.35f, 0.26f);
    private Vector3 righthandCarryMoverPositionEnd = new Vector3(0.35f, 1.35f, 0.26f);

    private void Start()
    {
        //Set our hand default carry positions
        leftHandMover.position = lefthandCarryMoverPositionEnd;
        rightHandMover.position = righthandCarryMoverPositionEnd;
    }

    private void Update()
    {
        
    }
}
