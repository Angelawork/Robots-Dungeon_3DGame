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
    public GameObject ItemToDrop;

    //health
    private Health _health;

    //damage caster
    private DamageCaster _damageCaster;

    //Enemy
    public bool IsPlayer = true;
    private UnityEngine.AI.NavMeshAgent _navMeshAgent;
    private Transform TargetPlayer;

    //State machine
    public enum CharacterState{//everytime a new state is added, we need to update fixedUpdate() and SwitchStateTo()
        Normal, 
        Attacking,
        Dead,
        BeingHit
    }
    public CharacterState CurrentState;
    private float attackStartTime; //player slide while attack
    public float AttackSlideDuration = 0.35f;
    public float AttackSlideSpeed = 0.07f;

    //material blink
    private MaterialPropertyBlock _materialPropertyBlock;
    private SkinnedMeshRenderer _skinnedMeshRenderer;


    private void Awake() {
        _cc = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _damageCaster = GetComponentInChildren<DamageCaster>();

        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();
        _skinnedMeshRenderer.GetPropertyBlock(_materialPropertyBlock);

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
                    
                    if(Time.time < attackStartTime + AttackSlideDuration){
                        float timePassed = Time.time - attackStartTime;
                        float lerpTime = timePassed / AttackSlideDuration;
                        _movementVelocity=Vector3.Lerp(transform.forward * AttackSlideSpeed, Vector3.zero, lerpTime);
                    }
                }
                break;
            case CharacterState.Dead:
                return;
            case CharacterState.BeingHit:
                return;
        }


        
        if(IsPlayer){
            if(!_cc.isGrounded){
                _verticalVelocity = Gravity;
            }else{
                _verticalVelocity = 0.25f * Gravity;
            }
            _movementVelocity += _verticalVelocity * Vector3.up * Time.deltaTime;

            _cc.Move(_movementVelocity);
            _movementVelocity = Vector3.zero;
        }
    }

    public void SwitchStateTo(CharacterState newState){
        if(IsPlayer){
        //clear cache
            _playerinput.MouseButtonDown=false;
        }

        //prepare for exisitng 
        switch(CurrentState){
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:

                if(_damageCaster!=null){
                    DisableDamageCaster();
                }
                break;
            case CharacterState.Dead:
                return;
            case CharacterState.BeingHit:
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
            case CharacterState.Dead:
                _cc.enabled = false;
                _animator.SetTrigger("Dead");
                StartCoroutine(MaterialDissolve());
                break;
            case CharacterState.BeingHit:
                _animator.SetTrigger("BeingHit");
                break;
        }

        CurrentState = newState;
    }

    public void AttackAnimationEnds(){
        SwitchStateTo(CharacterState.Normal);
    }

    public void BeingHitAnimationEnds(){
        SwitchStateTo(CharacterState.Normal);
    }

    public void ApplyDamage(int Damage, Vector3 attackPos = new Vector3()){
        if(_health!=null){
            _health.ApplyDamage(Damage);
        }

        if(!IsPlayer){
            GetComponent<EnemyVFXManager>().PlayBeingHitVFX(attackPos);
        }

        StartCoroutine(MaterialBlink());

        if(IsPlayer){
            SwitchStateTo(CharacterState.BeingHit);
        }
    }

    public void EnableDamageCaster(){
        _damageCaster.EnableDamageCaster();
    }
    public void DisableDamageCaster(){
        _damageCaster.DisableDamageCaster();
    }

    IEnumerator MaterialBlink(){
        _materialPropertyBlock.SetFloat("_blink", 0.35f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        yield return new WaitForSeconds(0.2f);
        _materialPropertyBlock.SetFloat("_blink", 0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

    }

    IEnumerator MaterialDissolve(){
        yield return new WaitForSeconds(2.1f);

        float dissolveTimeDuration = 1.7f;
        float currentTime = 0;
        float height_start = 20f;
        float height_target = -10f;
        float current_height;

        _materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        while(currentTime < dissolveTimeDuration){
            currentTime += Time.deltaTime;
            current_height = Mathf.Lerp(height_start, height_target, currentTime/dissolveTimeDuration);
            _materialPropertyBlock.SetFloat("_dissolve_height", current_height);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;
        }

        DropItem();
    }

    public void DropItem(){
        if(ItemToDrop !=null){
            Instantiate(ItemToDrop,transform.position, Quaternion.identity);
        }
    }
}
