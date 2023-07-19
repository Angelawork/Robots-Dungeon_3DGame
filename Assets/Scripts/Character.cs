using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private CharacterController _cc;
    public float MoveSpeed = 6f;
    private Vector3 _movementVelocity;
    private PlayerInput _playerinput;
    private float _verticalVelocity;
    public float Gravity = -9.8f;
    private void Awake() {
        _cc = GetComponent<CharacterController>();
        _playerinput = GetComponent<PlayerInput>();
    }

    private void CalculatePlayerMovement(){
        _movementVelocity.Set(_playerinput.HorizontalInput, 0f, _playerinput.VerticalInput);
        _movementVelocity.Normalize();
        _movementVelocity = Quaternion.Euler(0,-45f,0) * _movementVelocity;
        _movementVelocity *= MoveSpeed * Time.deltaTime;

        if(_movementVelocity != Vector3.zero){
            transform.rotation = Quaternion.LookRotation(_movementVelocity);
        }
    }
    private void FixedUpdate() {
        CalculatePlayerMovement();

        if(!_cc.isGrounded){
            _verticalVelocity = Gravity;
        }else{
            _verticalVelocity = 0.25f * Gravity;
        }
        _movementVelocity += _verticalVelocity * Vector3.up * Time.deltaTime;

        _cc.Move(_movementVelocity);
    }
}
