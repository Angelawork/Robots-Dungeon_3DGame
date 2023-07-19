using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float HorizontalInput;
    public float VerticalInput;
 
    void Update()
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");
    }

    //stop update when player/gameobject is dead, make it uncontrollable
    private void OnDisable() {
        HorizontalInput = 0;
        VerticalInput = 0;
    }
}
