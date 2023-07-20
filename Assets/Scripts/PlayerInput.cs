using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float HorizontalInput;
    public float VerticalInput;
    public bool MouseButtonDown;
 
    void Update()
    {
        if(!MouseButtonDown && Time.timeScale !=0){
            MouseButtonDown = Input.GetMouseButtonDown(0);
        }

        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");
    }

    //stop update when player/gameobject is dead, make it uncontrollable
    private void OnDisable() {
        MouseButtonDown = false;
        HorizontalInput = 0;
        VerticalInput = 0;
    }
}
