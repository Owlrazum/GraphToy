using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed;

    [SerializeField]
    private float _rotationSpeedDeg;

    private Vector3 _targetDirection;
    private Vector3 _moveDirection;

    private CharacterController _characterController;

    private void Awake()
    {
        TryGetComponent(out _characterController);

        _moveDirection = transform.forward;
    }

    public void UpdateDirection(Vector3 direction)
    {
        _targetDirection = direction;
    }

    public void Move()
    {
        Quaternion targetRotation = Quaternion.LookRotation(_targetDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeedDeg * Time.deltaTime);
        Vector3 movement = transform.forward * _moveSpeed * Time.deltaTime;
        _characterController.Move(movement);
    }
}
