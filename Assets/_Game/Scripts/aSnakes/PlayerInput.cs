using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveMobile))]
public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private FixedJoystick joystick;

    [SerializeField]
    private float bumpRecoveryTime;

    [SerializeField]
    private Transform playerSpace;

    private MoveMobile movementComponent;

    private bool shouldRespondToJoystick;
    private bool isJumping;

    private bool isBumped;
    private Vector2 bumpNormal;
    private bool isRecoveringFromBump;

    private float moveX, moveZ;

    private void Start()
    {
        movementComponent = GetComponent<MoveMobile>();
        joystick.SnapX = false;
        joystick.SnapY = false;
        shouldRespondToJoystick = true;
    }
    public void Update()
    {
        movementComponent.UpdateMoveControl(moveX, moveZ);

        if (shouldRespondToJoystick)
        {
            moveX = joystick.Horizontal;
            moveZ = joystick.Vertical;
        }
    }
}