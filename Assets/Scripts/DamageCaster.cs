using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCaster : MonoBehaviour
{
    private Collider _damageCasterCollider;
    public int Damage = 30;
    public string TargetTag;
    private List<Collider> _damagedTargetList;//prevent damage same character multiple times

    private void Awake() {
        _damageCasterCollider = GetComponent<Collider>();
        _damageCasterCollider.enabled = false;
        _damagedTargetList = new List<Collider>();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == TargetTag && !_damagedTargetList.Contains(other)){
            Character targetCC = other.GetComponent<Character>();

            if(targetCC != null){
                targetCC.ApplyDamage(Damage, transform.parent.position);
                
                PlayerVFXManager _playerVFXmanager = transform.parent.GetComponent<PlayerVFXManager>();
                if(_playerVFXmanager!=null){
                    RaycastHit hit;
                    Vector3 originalPos = transform.position + (-_damageCasterCollider.bounds.extents.z)*transform.forward;
                    bool isHit = Physics.BoxCast(originalPos, _damageCasterCollider.bounds.extents/2, transform.forward, out hit, transform.rotation, _damageCasterCollider.bounds.extents.z, 1<<6);

                    if(isHit){
                        _playerVFXmanager.PlaySlash(hit.point + new Vector3(0, 0.5f, 0));
                    }
                }
            }

            _damagedTargetList.Add(other);
        }
    }

    public void EnableDamageCaster(){
        _damagedTargetList.Clear();
        _damageCasterCollider.enabled = true;
    }
    public void DisableDamageCaster(){
        _damagedTargetList.Clear();
        _damageCasterCollider.enabled = false;
    }

    /*
    private void OnDrawGizmos() {
        if(_damageCasterCollider == null)
            _damageCasterCollider = GetComponent<Collider>();

        RaycastHit hit;

        Vector3 originalPos = transform.position + (-_damageCasterCollider.bounds.extents.z)*transform.forward;
        bool isHit = Physics.BoxCast(originalPos, _damageCasterCollider.bounds.extents/2, transform.forward, out hit, transform.rotation, _damageCasterCollider.bounds.extents.z, 1<<6);

        if(isHit){
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(hit.point, 0.3f);
        }
    }
    */
}
