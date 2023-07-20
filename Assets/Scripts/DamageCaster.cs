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

            if(targetCC != null)
                targetCC.ApplyDamage(Damage);

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
}
