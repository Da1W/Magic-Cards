using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Training : MonoBehaviour
{
    public Text stepsCount;
    public Text levelText;
    public Text helperText;
    public int steps = 0;

    public GameObject correct;

    public CellSlot[] CellSlotsOnMap;
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
                GameConstants.levelNumber--;
                var s = "Level" + GameConstants.levelNumber;
                Application.LoadLevel(s);
                steps.Pop();
            }
        }
    }

    public void NextLevel()
    {
        GameConstants.steps.Push("NextLevel");
        GameConstants.levelNumber++;
        var s = "Level" + GameConstants.levelNumber;
        Application.LoadLevel(s);
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
        Application.LoadLevel(0);
    }

    public bool CheckWin()
    {
        foreach (var e in CellSlotsOnMap)
        {
            if (!e.rightFill.activeSelf) return false;
        }
        return true;
    }
    void Start()
    {
        CellSlotsOnMap = FindObjectsOfType<CellSlot>();
        stepsCount.text = "Количество ходов: 0";
        levelText.text = "Уровень " + GameConstants.levelNumber;
        //helperText.text = GameConstants.levelsText[GameConstants.levelNumber];
    }

    public void PlusStep()
    {
        steps++;
        stepsCount.text = "Количество ходов: " + steps.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
