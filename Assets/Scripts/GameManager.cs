using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Character playerCharacter;
    private bool gameOver;
    private void Awake() {
        playerCharacter = GameObject.FindWithTag("Player").GetComponent<Character>();
    }
    private void GameOver(){
        Debug.Log("gameover");
    }
    private void GameFinished(){
        Debug.Log("gamefin");
    }
    void Update()
    {
        if(gameOver)
            return;
        
        if(playerCharacter.CurrentState == Character.CharacterState.Dead){
            gameOver = true;
            GameOver();
        }
    }
}
