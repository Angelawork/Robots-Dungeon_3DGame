using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyVFXManager : MonoBehaviour
{
    public VisualEffect footstep;
    public VisualEffect AttackVFX;
    public void BurstFootStep(){
        footstep.SendEvent("OnPlay");//same as set it by .Play()
    }
    public void PlayAttackVFX(){
        AttackVFX.Play();
    }
}
