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
    private Animator _animator;

    //Enemy
    public bool IsPlayer = true;
    private UnityEngine.AI.NavMeshAgent _navMeshAgent;
    private Transform TargetPlayer;

    //State machine
    public enum CharacterState{
        Normal, 
        Attacking
    }
    public CharacterState CurrentState;
    private float attackStartTime; //player slide while attack
    public float AttackSlideDuration = 0.35f;
    public float AttackSlideSpeed = 0.07f;

    private void Awake() {
        _cc = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        if(!IsPlayer){
            _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            TargetPlayer = GameObject.FindWithTag("Player").transform;
            _navMeshAgent.speed = MoveSpeed;
        }else{
            _playerinput = GetComponent<PlayerInput>();
        }
    }

    private void CalculatePlayerMovement(){
        if(_playerinput.MouseButtonDown && _cc.isGrounded){
            SwitchStateTo(CharacterState.Attacking);
            return;
        }

        _movementVelocity.Set(_playerinput.HorizontalInput, 0f, _playerinput.VerticalInput);
        _movementVelocity.Normalize();
        _movementVelocity = Quaternion.Euler(0,-45f,0) * _movementVelocity;

        if(Input.GetKey(KeyCode.LeftShift)){
            _animator.SetFloat("Speed",_movementVelocity.magnitude*5);
            _movementVelocity *= MoveSpeed * Time.deltaTime * 1.5f;
        } else{
            _animator.SetFloat("Speed",_movementVelocity.magnitude);
            _movementVelocity *= MoveSpeed * Time.deltaTime;
        }

        if(_movementVelocity != Vector3.zero){
            transform.rotation = Quaternion.LookRotation(_movementVelocity);
        }

        _animator.SetBool("AirBorne", !_cc.isGrounded);
    }

    private void CalculateEnemyMovement(){
        if(Vector3.Distance(TargetPlayer.position, transform.position) >= _navMeshAgent.stoppingDistance){
           _navMeshAgent.SetDestination(TargetPlayer.position);
           _animator.SetFloat("Speed",1.0f);
        }else{
            _navMeshAgent.SetDestination(transform.position);
           _animator.SetFloat("Speed",0f);
           SwitchStateTo(CharacterState.Attacking);
        }

    }

    private void FixedUpdate() {
        switch(CurrentState){
            case CharacterState.Normal:
                if(IsPlayer){
                    CalculatePlayerMovement();
                }
                else
                    CalculateEnemyMovement();
                break;

            case CharacterState.Attacking:

                if(IsPlayer){
                    _movementVelocity = Vector3.zero;
                    if(Time.time < attackStartTime + AttackSlideDuration){
                        float timePassed = Time.time - attackStartTime;
                        float lerpTime = timePassed / AttackSlideDuration;
                        _movementVelocity=Vector3.Lerp(transform.forward * AttackSlideSpeed, Vector3.zero, lerpTime);
                    }
                }
                break;
        }


        
        if(IsPlayer){
            if(!_cc.isGrounded){
                _verticalVelocity = Gravity;
            }else{
                _verticalVelocity = 0.25f * Gravity;
            }
            _movementVelocity += _verticalVelocity * Vector3.up * Time.deltaTime;

            _cc.Move(_movementVelocity);
        }
    }

    private void SwitchStateTo(CharacterState newState){
        if(IsPlayer){
        //clear cache
            _playerinput.MouseButtonDown=false;
        }

        //prepare for exisitng 
        switch(CurrentState){
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
                break;
        }

        //entering new state
        switch(newState){
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
                if(!IsPlayer){
                    Quaternion newRotation = Quaternion.LookRotation((TargetPlayer.position - transform.position).normalized);
                    transform.rotation = Quaternion.Slerp(transform.rotation , newRotation, 1f);
                    //transform.rotation = newRotation;
                }
                _animator.SetTrigger("Attack");

                if(IsPlayer){
                    attackStartTime = Time.time;
                }
                break;
        }

        CurrentState = newState;
    }

    public void AttackAnimationEnds(){
        SwitchStateTo(CharacterState.Normal);
    }
}
