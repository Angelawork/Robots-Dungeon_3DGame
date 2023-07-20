using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyVFXManager : MonoBehaviour
{
    public VisualEffect footstep;
    public void BurstFootStep(){
        footstep.SendEvent("OnPlay");//same as set it by .Play()
    }
}
