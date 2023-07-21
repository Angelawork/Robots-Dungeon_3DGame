using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyVFXManager : MonoBehaviour
{
    public VisualEffect footstep;
    public VisualEffect AttackVFX;
    public ParticleSystem BeingHitVFX;

    public VisualEffect BeingHitSplashVFX;
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

        Vector3 splashPos = transform.position;
        splashPos.y += 2f;
        VisualEffect newSplashVFX = Instantiate(BeingHitSplashVFX, splashPos, Quaternion.identity);
        newSplashVFX.SendEvent("OnPlay");
        Destroy(newSplashVFX, 10f);
    }
}
