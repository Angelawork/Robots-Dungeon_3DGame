using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy02_shoot : MonoBehaviour
{
    public Transform ShootingPoint;
    public GameObject DamageOrb;
    private Character _cc;
    private void Awake() {
        _cc = GetComponent<Character>();
    }
    public void ShootTheDamageOrb(){
        Instantiate(DamageOrb, ShootingPoint.position, Quaternion.LookRotation(ShootingPoint.forward));
    }
    private void Update() {
        _cc.RotateToTarget();
    }
}
