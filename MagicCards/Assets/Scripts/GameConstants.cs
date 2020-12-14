using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    public static Stack<string> steps;
    public static Stack<CellSlot> usedCellSlots;
    public static int gameMode;
    public static int levelNumber = 0;
    public static string playerName;

    public static List<string> levelsText = new List<string>
        {
        "0",

        $"Приветствую тебя {playerName}. Добро пожаловать в игру, " +
        $"посвященную теме теории вероятности  ”Зависимые события и условная вероятность”." +
        $" Давай ознакомимся с правилами игры.",

        "Уровень2"
    };

}
