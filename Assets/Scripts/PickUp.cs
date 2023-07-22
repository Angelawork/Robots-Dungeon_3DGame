using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public enum PickUpType{
        Heal,Coin
    }
    public PickUpType type;
    public int Value = 25;
    public ParticleSystem collectedVFX;

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player"){
            other.gameObject.GetComponent<Character>().PickUpItem(this);

            if(collectedVFX!=null){
                Instantiate(collectedVFX, transform.position,Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
