using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameUI_Manager : MonoBehaviour
{
    public GameManager GM;
    public TMPro.TextMeshProUGUI CoinText;
    public Slider HealthSlider;
    public GameObject UI_Pause;
    public GameObject UI_GameOver;
    public GameObject UI_GameFinished;

    public enum GameUI_State{
        GamePlay,Pause,GameOver,GameFinished,GameLevelUp
    }

    public GameUI_State currentState;

    private void Start() {
        SwitchUIState(GameUI_State.GamePlay);
    }

    void Update()
    {
        HealthSlider.value = GM.playerCharacter.GetComponent<Health>().CurrentHealthPrecentage;
        CoinText.text = GM.playerCharacter.Coin.ToString();
    }

    private IEnumerator DelayedPause(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Time.timeScale = 0;
    }
    private void SwitchUIState(GameUI_State state){
        UI_GameFinished.SetActive(false);
        UI_GameOver.SetActive(false);
        UI_Pause.SetActive(false);
        Time.timeScale = 1;

        switch(state){
            case GameUI_State.GamePlay:
                break;
            case GameUI_State.Pause:
                Time.timeScale = 0;
                UI_Pause.SetActive(true);
                break;
            case GameUI_State.GameOver:
                StartCoroutine(DelayedPause(2.5f));
                
                UI_GameOver.SetActive(true);
                break;
            case GameUI_State.GameFinished:
                StartCoroutine(DelayedPause(2.5f));
                UI_GameFinished.SetActive(true);
                break;
            case GameUI_State.GameLevelUp:
                break;
        }

        currentState = state;
    }

    public void TogglePauseUI(){
        if(currentState == GameUI_State.GamePlay)
            SwitchUIState(GameUI_State.Pause);
        else if(currentState == GameUI_State.Pause)
            SwitchUIState(GameUI_State.GamePlay);
    }

    public void Button_MainMenu(){
        GM.return_MainMenu();
    }

    public void Button_Restart(){
        GM.Restart();
    }

    public void show_GameOver(){
        SwitchUIState(GameUI_State.GameOver);
    }

    public void show_GameFinished(){
        SwitchUIState(GameUI_State.GameFinished);
    }
    public void show_GameLevelUp(){
        SwitchUIState(GameUI_State.GameLevelUp);
    }
}
