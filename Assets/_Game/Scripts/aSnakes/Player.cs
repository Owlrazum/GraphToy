using System;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour
{
    public static Func<Transform> FuncGetTransform;

    [SerializeField]
    private FixedJoystick _joystick;

    private PlayerMovement _movement;

    private bool _shouldRespondToJoystick;

    private void Awake()
    { 
        FuncGetTransform += GetTransform;
    }

    private void OnDestroy()
    {
        FuncGetTransform -= GetTransform;
    }

    private Transform GetTransform()
    {
        return transform;
    }

    private void Start()
    {
        _movement = GetComponent<PlayerMovement>();
        _joystick.SnapX = false;
        _joystick.SnapY = false;
        _shouldRespondToJoystick = true;
    }

    public void Update()
    {
        if (_shouldRespondToJoystick)
        {
            _movement.UpdateDirection(new Vector3(_joystick.Horizontal, 0, _joystick.Vertical));
        }

        _movement.Move();
    }
}