using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyVFXManager : MonoBehaviour
{
    public VisualEffect footstep;
    public VisualEffect AttackVFX;
    public ParticleSystem BeingHitVFX;
    public void BurstFootStep(){
        footstep.SendEvent("OnPlay");//same as set it by .Play()
    }
    public void PlayAttackVFX(){
        AttackVFX.Play();
    }

    public void PlayBeingHitVFX(Vector3 attackerPos){
        Vector3 forceForward = transform.position - attackerPos;
        forceForward.Normalize();
        forceForward.y=0;
        BeingHitVFX.transform.rotation = Quaternion.LookRotation(forceForward);
        BeingHitVFX.Play();
    }
}
