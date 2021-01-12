using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    private GameObject BattleModeBtn;
    private GameObject ForecasterModeBtn;

    private void Awake()
    {
        if(SceneManager.GetActiveScene().name == "ChooseMode")
        {
            BattleModeBtn = GameObject.Find("Mode2");
            ForecasterModeBtn = GameObject.Find("Mode3");
            if (GameConstants.isModesAvailable && BattleModeBtn && ForecasterModeBtn)
            {
                BattleModeBtn.GetComponent<Button>().interactable = true;
                BattleModeBtn.GetComponent<CanvasGroup>().alpha = 1f;
                ForecasterModeBtn.GetComponent<Button>().interactable = true;
                ForecasterModeBtn.GetComponent<CanvasGroup>().alpha = 1f;
            }
        }
    }
    public static void ToMainMenu()
    {
        SceneManager.LoadSceneAsync("Menu");
    }

    public static void Retry()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public static void ToChooseModeScene()
    {
        SceneManager.LoadSceneAsync("ChooseMode");
    }
}
