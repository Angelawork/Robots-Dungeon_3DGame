using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropWeapons : MonoBehaviour
{
    public List<GameObject> Weapons;

    public void DropSwords(){
        foreach (GameObject weapon in Weapons){
            weapon.AddComponent<Rigidbody>();
            weapon.AddComponent<BoxCollider>();
            weapon.transform.parent = null;
        }
    }
}
