using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerVFXManager : MonoBehaviour
{
    public VisualEffect footstep;
    public VisualEffect walkstep;
    public VisualEffect Slash;
    public ParticleSystem Blade01;
    public ParticleSystem Blade02;
    public ParticleSystem Blade03;
    public VisualEffect Heal;

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
        CameraShake.Instance.ShakeCamera();
    }
    public void PlayBlade02(){
        Blade02.Play();
        CameraShake.Instance.ShakeCamera();
    }
    public void PlayBlade03(){
        Blade03.Play();
        CameraShake.Instance.ShakeCamera();
    }

    public void StopBlade(){
        Blade01.Simulate(0);
        Blade01.Stop();
        
        Blade02.Simulate(0);
        Blade02.Stop();
        
        Blade03.Simulate(0);
        Blade03.Stop();
    }

    public void PlaySlash(Vector3 pos){
        Slash.transform.position = pos;
        Slash.Play();
    }
    public void PlayHealVFX(){
        Heal.Play();
    }
}
    
