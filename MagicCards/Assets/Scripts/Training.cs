using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Training : MonoBehaviour
{
    [SerializeField] Text stepsCount;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Text helperText;
    public int steps = 0;

    public GameObject correct;

    private CellSlot[] CellSlotsOnMap;

    void Start()
    {
        CellSlotsOnMap = FindObjectsOfType<CellSlot>();
        stepsCount.text = "Количество ходов: 0";
        levelText.text = "Уровень " + GameConstants.levelNumber;
        if(GameConstants.levelNumber == 0) helperText.text = GameConstants.welcomeText;
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
        correct.SetActive(true);
    }
    public void CloseCorrect()
    {
        correct.SetActive(false);
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
        return true;
    }

    public void PlusStep()
    {
        steps++;
        stepsCount.text = "Количество ходов: " + steps.ToString();
    }
}
