using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Training : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelText;
    private int steps = 0;

    public GameObject[] correct;
    public int maxCardsInCells;
    private CellSlot[] CellSlotsOnMap;

    void Start()
    {
        CellSlotsOnMap = FindObjectsOfType<CellSlot>();
        if (GameConstants.gameMode == 1)
            levelText.text = "Уровень " + GameConstants.levelNumber;
        else
            UpdateRoundNumber();
        //if(GameConstants.levelNumber == 0) helperText.text = GameConstants.welcomeText;
    }

    public void UpdateRoundNumber()
    {
        levelText.text = "Раунд " + GameConstants.roundNumber;
    }

    public void Undo()
    {
        var steps = GameConstants.steps;
        if (steps.Count != 0)
        {
            if (steps.Peek() == "Insert")
            {
                var lastSlot = GameConstants.usedCellSlots.Pop();
                lastSlot.items[lastSlot.items.Count - 1].GetComponent<DragAndDrop>().DeleteCard();
                steps.Pop();
            }
            else if (steps.Peek() == "NextLevel")
            {
                PreviousLevel();
                steps.Pop();
            }
        }
    }

    public void NextLevel()
    {
        GameConstants.steps.Push("NextLevel");
        GameConstants.levelNumber++;
        var s = "Level" + GameConstants.levelNumber;
        SceneManager.LoadSceneAsync(s);
    }
    public void PreviousLevel()
    {
        GameConstants.levelNumber--;
        var s = "Level" + GameConstants.levelNumber;
        SceneManager.LoadSceneAsync(s);
    }

    public void ShowCorrect()
    {
        foreach (var obj in correct)
        {
            obj.SetActive(true);
        }
        if (GameConstants.levelNumber == 5) GameConstants.isModesAvailable = true;
    }
    public void CloseCorrect()
    {
        foreach (var obj in correct)
        {
            obj.SetActive(false);
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public bool CheckWin()
    {
        foreach (var e in CellSlotsOnMap)
        {
            if (!e.rightFill.activeSelf) return false;
        }
        if (GameConstants.levelNumber == 5 && steps >= 8)
            return false;
        return true;
    }

    public void PlusStep()
    {
        steps++;
    }

    public void ToMenuAndShowChoose()
    {
        SceneChanger.ToChooseModeScene();
    }
}
