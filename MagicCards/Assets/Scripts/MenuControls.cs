using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MenuControls : MonoBehaviour
{
    public Text playerNameField;
    public void PlayButton()
    {
        //string userName = playerNameField.text == "" ? "Мой друг" : playerNameField.text;
        //GameConstants.welcomeText =  string.Format("Приветствую тебя, {0}. Добро пожаловать в игру," +
        //    " посвященную теме теории вероятности ”Зависимые события и условная вероятность”." +
        //    " Давай ознакомимся с правилами игры. (Нажми на меня чтобы продолжить)", userName); 
        GameConstants.gameMode = 1;
        
        SceneManager.LoadSceneAsync("Level" + GameConstants.levelNumber);
    }

    public void PlayBattleButton()
    {
        GameConstants.gameMode = 2;
        SceneManager.LoadSceneAsync("Battle");
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
