using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MenuControls : MonoBehaviour
{
    //public Text playerNameField;

    public void PlayButton()
    {
        GameConstants.gameMode = 1;
        
        SceneManager.LoadSceneAsync("Level" + GameConstants.levelNumber);
    }

    public void PlayBattleButton()
    {
        GameConstants.gameMode = 2;
        SceneManager.LoadSceneAsync("Battle");
    }

    public void PlayForecastersBattle()
    {
        GameConstants.gameMode = 3;
        SceneManager.LoadSceneAsync("ForeCasterBattle");
    }

    public void DropProgressButton()
    {
        GameConstants.levelNumber = 0;
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
