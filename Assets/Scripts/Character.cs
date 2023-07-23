using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float cameraAngle = -45f;
    private CharacterController _cc;
    public float MoveSpeed = 6f;
    private Vector3 _movementVelocity;
    private PlayerInput _playerinput;
    private float _verticalVelocity;
    public float Gravity = -9.8f;
    private Animator _animator;
    public GameObject ItemToDrop;

    public float SlideSpeed = 10f;

    public int Coin;

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
        BeingHit,
        Slide,
        Spawn
    }
    public CharacterState CurrentState;


    //spawn
    public float SpawnDuration = 2.2f;
    private float currentSpawnTime;

    //player slide while attack
    private float attackStartTime; 
    public float AttackSlideDuration = 0.35f;
    public float AttackSlideSpeed = 0.07f;
    private Vector3 ImpactOnCharacter;

    private float attackAnimationDuration;

    //invincible time
    public bool IsInvincible;
    public float invincibleDuration = 2.3f;

    //material blink
    private MaterialPropertyBlock _materialPropertyBlock;
    private SkinnedMeshRenderer _skinnedMeshRenderer;


    //Audio manager
    private AudioSource _AudioSource;
    public AudioClip[] _stepSounds;
    public AudioClip[] AttackSounds;
    public AudioClip[] SlideSounds;
    public AudioClip[] DeathSounds;
    private bool isPlayingFootstep;
    public float footstepRate = 0.1f;
    private float nextFootstepTime;
    private void Footsteps(){
        if (_stepSounds.Length > 0)
        {
            int number = Random.Range(0, _stepSounds.Length);
            _AudioSource.PlayOneShot(_stepSounds[number]);
            isPlayingFootstep = true;
            Invoke("ResetFootstepStatus", _stepSounds[number].length);
        }
    }
    private void ResetFootstepStatus()
    {
        isPlayingFootstep = false;
    }


    private void Awake() {
        _cc = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _damageCaster = GetComponentInChildren<DamageCaster>();
        _AudioSource = GetComponent<AudioSource>();

        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();
        _skinnedMeshRenderer.GetPropertyBlock(_materialPropertyBlock);

        if(!IsPlayer){
            _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            TargetPlayer = GameObject.FindWithTag("Player").transform;
            _navMeshAgent.speed = MoveSpeed;

            SwitchStateTo(CharacterState.Spawn);
        }else{
            _playerinput = GetComponent<PlayerInput>();
        }
    }

    private void CalculatePlayerMovement(){
        if(_playerinput.MouseButtonDown && _cc.isGrounded){
            SwitchStateTo(CharacterState.Attacking);
            return;
        }else if(_playerinput.SpaceKeyDown && _cc.isGrounded){
            SwitchStateTo(CharacterState.Slide);
            return;
        }

        _movementVelocity.Set(_playerinput.HorizontalInput, 0f, _playerinput.VerticalInput);
        _movementVelocity.Normalize();

        //for camera perspective
        _movementVelocity = Quaternion.Euler(0,cameraAngle,0) * _movementVelocity;

        //run or walk
        if(Input.GetKey(KeyCode.LeftShift)){
            _animator.SetFloat("Speed",_movementVelocity.magnitude*5);
            _movementVelocity *= MoveSpeed * Time.deltaTime * 1.5f;
        } else{
            _animator.SetFloat("Speed",_movementVelocity.magnitude);
            _movementVelocity *= MoveSpeed * Time.deltaTime;
        }
        //run or walk

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

                
                //audio
                if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S)|| Input.GetKey(KeyCode.D)){
                    //controller is grounded and moving but we are not playing footsteps
                    if (Time.time >= nextFootstepTime && _cc.isGrounded && !isPlayingFootstep &&_movementVelocity.magnitude!=0)
                    {
                        Footsteps();
                        nextFootstepTime = Time.time + footstepRate;
                    }
                }
            
                break;

            case CharacterState.Attacking:


                if(IsPlayer){
                    
                    
                    if(Time.time < attackStartTime + AttackSlideDuration){
                        float timePassed = Time.time - attackStartTime;
                        float lerpTime = timePassed / AttackSlideDuration;
                        _movementVelocity=Vector3.Lerp(transform.forward * AttackSlideSpeed, Vector3.zero, lerpTime);
                    }

                    if(_playerinput.MouseButtonDown && _cc.isGrounded){
                        string currentClip = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                        attackAnimationDuration = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

                        if(currentClip != "LittleAdventurerAndie_ATTACK_03" && attackAnimationDuration > 0.5f && attackAnimationDuration <0.7f){
                            _playerinput.MouseButtonDown = false;
                            SwitchStateTo(CharacterState.Attacking);

                            CalculatePlayerMovement();
                        }
                
                    }
                }
                break;
            case CharacterState.Dead:
                return;
            case CharacterState.BeingHit:
                
                break;

            case CharacterState.Slide:
                _movementVelocity = transform.forward * SlideSpeed * Time.deltaTime;
                break;

            case CharacterState.Spawn:
                currentSpawnTime -= Time.deltaTime;
                if(currentSpawnTime<=0){
                    SwitchStateTo(CharacterState.Normal);
                }
                break;
        }

        if(ImpactOnCharacter.magnitude > 0.2f){
            _movementVelocity = ImpactOnCharacter*Time.deltaTime;
        }
        ImpactOnCharacter = Vector3.Lerp(ImpactOnCharacter, Vector3.zero, Time.deltaTime*5);

        
        if(IsPlayer){
            if(!_cc.isGrounded){
                _verticalVelocity = Gravity;
            }else{
                _verticalVelocity = 0.25f * Gravity;
            }
            _movementVelocity += _verticalVelocity * Vector3.up * Time.deltaTime;

            _cc.Move(_movementVelocity);
            _movementVelocity = Vector3.zero;
        }else{
            if(CurrentState!=CharacterState.Normal){
                _cc.Move(_movementVelocity);
                _movementVelocity = Vector3.zero;//enemy is pushed back when hit
            }
        }
    }

    private IEnumerator PlayAudioWithDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        int number = Random.Range(0, AttackSounds.Length);
        _AudioSource.PlayOneShot(AttackSounds[number]);
    }
    public void SwitchStateTo(CharacterState newState){
        if(IsPlayer){
            _playerinput.clearCache();
        }

        //prepare for exisitng 
        switch(CurrentState){
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:

                if(_damageCaster!=null){
                    DisableDamageCaster();
                }

                if(IsPlayer){
                    GetComponent<PlayerVFXManager>().StopBlade();
                }
                break;
            case CharacterState.Dead:
                return;
            case CharacterState.BeingHit:
                break;
            case CharacterState.Slide:
                break;
            case CharacterState.Spawn:
                IsInvincible = false;
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

                    //RotateToCursor();
                }

                if(AttackSounds.Length>0){
                    if(!IsPlayer){
                        StartCoroutine(PlayAudioWithDelay(1.5f));
                    }else{
                        _AudioSource.PlayOneShot(AttackSounds[Random.Range(0, AttackSounds.Length)]);
                    }
                }
                break;
            case CharacterState.Dead:
                _cc.enabled = false;
                _animator.SetTrigger("Dead");
                StartCoroutine(MaterialDissolve());

                Transform childTransform = transform.Find("Lights");
                if (childTransform != null)
                {
                    Destroy(childTransform.gameObject);
                }

                if(!IsPlayer){
                    SkinnedMeshRenderer mesh = GetComponentInChildren<SkinnedMeshRenderer>();
                    mesh.gameObject.layer=0;
                }

                if(IsPlayer){
                    _AudioSource.PlayOneShot(DeathSounds[Random.Range(0, DeathSounds.Length)]);
                }
                
                break;
            case CharacterState.BeingHit:
                _animator.SetTrigger("BeingHit");

                if(IsPlayer){
                    IsInvincible = true;
                    StartCoroutine(DelayCancelInvincible());
                }
                break;
            case CharacterState.Slide:
                _AudioSource.PlayOneShot(SlideSounds[Random.Range(0, SlideSounds.Length)]);
                _animator.SetTrigger("Slide");
                break;
            case CharacterState.Spawn:
                IsInvincible = true;
                currentSpawnTime = SpawnDuration;
                StartCoroutine(MaterialAppear());
                break;
        }

        CurrentState = newState;
    }

    public void RotateToTarget(){
        if(CurrentState != CharacterState.Dead){
            transform.LookAt(TargetPlayer, Vector3.up);
        }
    }

    public void AttackAnimationEnds(){
        SwitchStateTo(CharacterState.Normal);
    }

    public void BeingHitAnimationEnds(){
        SwitchStateTo(CharacterState.Normal);
    }

    public void SlideAnimationEnds(){
        SwitchStateTo(CharacterState.Normal);
    }

    public void ApplyDamage(int Damage, Vector3 attackerPos = new Vector3()){
        if(IsInvincible){
            return;
        }


        if(_health!=null){
            _health.ApplyDamage(Damage);
        }

        if(!IsPlayer){
            GetComponent<EnemyVFXManager>().PlayBeingHitVFX(attackerPos);
        }

        StartCoroutine(MaterialBlink());

        if(IsPlayer){
            SwitchStateTo(CharacterState.BeingHit);
            CalculateImpact(attackerPos, 8f);
        }else{
            CalculateImpact(attackerPos, 6f);
        }
    }
    IEnumerator DelayCancelInvincible(){
        yield return new WaitForSeconds(invincibleDuration);
        IsInvincible = false;

    }

    private void CalculateImpact(Vector3 attackerPos, float force){
        Vector3 impactDir = transform.position - attackerPos;
        impactDir.Normalize();
        impactDir.y=0;
        ImpactOnCharacter = impactDir*force;
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
public AudioClip[] CoinSounds;
public AudioClip[] HealSounds;
    public void PickUpItem(PickUp item){
        switch(item.type){
            case PickUp.PickUpType.Heal:
                _AudioSource.PlayOneShot(HealSounds[0]);
                AddHealth(item.Value);
                break;

            case PickUp.PickUpType.Coin:
                int number = Random.Range(0, CoinSounds.Length);
                _AudioSource.PlayOneShot(CoinSounds[number]);
                AddCoin(item.Value);
                break;
        }

    }

    private void AddHealth(int value){
        _health.AddHealth(value);
        GetComponent<PlayerVFXManager>().PlayHealVFX();
    }
    private void AddCoin(int value){
        Coin+=value;
    }

    IEnumerator MaterialAppear(){
        float dissolveTimeDuration = SpawnDuration;
        float currentTime = 0;
        float height_start = -10f;
        float height_target = 20f;
        float current_height;

        _materialPropertyBlock.SetFloat("_enableDissolve", 1f);//means set enabled, turn it on
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        while(currentTime < dissolveTimeDuration){
            currentTime += Time.deltaTime;
            current_height = Mathf.Lerp(height_start, height_target, currentTime/dissolveTimeDuration);
            _materialPropertyBlock.SetFloat("_dissolve_height", current_height);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;//wait for 1 frame untill next loop
        }

        _materialPropertyBlock.SetFloat("_enableDissolve", 0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }


    //finetune
    private void RotateToCursor(){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit HitResult;

        if(Physics.Raycast(ray, out HitResult, 1000, 1<< LayerMask.NameToLayer("CursorTest"))){
            Vector3 cursorPos = HitResult.point;
            transform.rotation = Quaternion.LookRotation(cursorPos - transform.position, Vector3.up);
        }
    }
}
