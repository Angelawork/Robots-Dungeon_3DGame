using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class Teleport : MonoBehaviour
{
    public GameUI_Manager UIgm;
    public VisualEffect teleport;
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && UIgm.currentState == GameUI_Manager.GameUI_State.GameLevelUp)
        {
            teleport.Play();
            StartCoroutine(Delayed(0.7f));
            
        }
    }

    private IEnumerator Delayed(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene("GameLevel02");
    }
}
