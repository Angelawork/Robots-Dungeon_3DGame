using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerVFXManager : MonoBehaviour
{
    public VisualEffect footstep;
    public VisualEffect walkstep;
    public ParticleSystem Blade01;

    public void Update_FootStep(bool state){
        if(state){
            footstep.Play();
        }else{
            footstep.Stop();
        }
    }
    public void Update_WalkStep(bool state){
        walkstep.playRate = 0.45f;
        if(state){
            walkstep.Play();
        }else{
            walkstep.Stop();
        }
    }

    public void PlayBlade01(){
        Blade01.Play();
    }
}
    
