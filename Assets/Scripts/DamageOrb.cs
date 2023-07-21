using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOrb : MonoBehaviour
{
    public float Speed = 2.4f;
    public int Damage = 10;
    public ParticleSystem HitVFX;
    private Rigidbody _rb;

    private void Awake() {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        _rb.MovePosition(transform.position + transform.forward*Speed*Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other) {
        Character cc = other.gameObject.GetComponent<Character>();

        if(cc!=null && cc.IsPlayer){
            cc.ApplyDamage(Damage, transform.position);
        }
        Instantiate(HitVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
