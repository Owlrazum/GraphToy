using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(Animator))]
public class MoveMobile : MonoBehaviour
{
    Vector3 directionOfMove;

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float fallSpeed;

    [SerializeField]
    private Transform playerSpace;

    private bool isFalling;
 

    private void Start()
    {
        initialForward = transform.forward;
        isFalling = false;
        directionOfMove = transform.forward;
    }

    public void UpdateMoveControl(float moveX, float moveZ) // , bool isLocalSpace
    {
        directionOfMove = Quaternion.Euler(0, 0, 0) * (new Vector3(moveX, 0, moveZ)).normalized;
    }

    public void UpdateMoveControl(Vector3 direction)
    {
        directionOfMove = direction;
    }

    #region MoveProcessing

    private Vector3 initialForward;
    private Vector3 rotationDirection;
    bool isRotating = false;
    private void Move(Vector3 direction)
    {
        Vector3 movement = direction * moveSpeed * Time.deltaTime;
        Vector3 pos = playerSpace.position + movement;
        playerSpace.position = pos;
        //Debug.Log("Moved " + moveX + " " + moveZ);
        if (isRotating == false)
        {
            if (rotationDirection != direction)
            {
                isRotating = true;
                rotationDirection = direction;
                RotateTo(initialForward, rotationDirection, true);
            }
        }
        else
        {
            RotateTo(initialForward, rotationDirection, true);
        }
    }
    void RotateTo(Vector3 from, Vector3 to, bool isGradual = false)
    {
        float angle = Vector3.SignedAngle(from, to, transform.up);
        Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
        if (isGradual)
        {
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, targetRotation, 360);
        }
        else
        {
            transform.rotation *= targetRotation;
        }

        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1)
        {
            isRotating = false;
        }
    }
    #endregion
    private void Update()
    {
        Move(directionOfMove);
        if (!isFalling)
        {
            return;
        }
        Vector3 fallTrans = new Vector3(0, fallSpeed * Time.deltaTime, 0);
        transform.position -= fallTrans;
    }
}
