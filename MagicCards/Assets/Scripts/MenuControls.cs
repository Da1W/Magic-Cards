using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MenuControls : MonoBehaviour
{
    private SceneManager sceneManager;
    public Text playerNameField;
    public void PlayButton()
    {
        GameConstants.playerName = playerNameField.text;
        GameConstants.gameMode = 1;
        Application.LoadLevel("Level" + GameConstants.levelNumber);
    }

    public void DropProgressButton()
    {
        GameConstants.levelNumber = 0;
    }

    public void TopicButton()
    {
        Application.OpenURL("http://www.mathprofi.ru/zavisimye_sobytija.html");
    }
    public void ExitButton()
    {
        Application.Quit();
    }

}
