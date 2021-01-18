using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SuitsManager : MonoBehaviour
{
    //spades, hearts, clubs, and diamonds

    [HideInInspector] public Dictionary<string, int> suits = new Dictionary<string, int>();
    public int pack;

    public TextMeshProUGUI packText;
    public TextMeshProUGUI spadesText;
    public TextMeshProUGUI heartsText;
    public TextMeshProUGUI clubsText;
    public TextMeshProUGUI diamondsText;

    void Start()
    {
        suits.Add("spade", 9);
        suits.Add("heart", 9);
        if (GameConstants.levelNumber != 2)
        {
            suits.Add("club", 9);
            pack = 36;
        }
        else
        {
            suits.Add("club", 8);
            pack = 35;
        }
        suits.Add("diamond", 9);

        packText.text = "В колоде: " + pack;
        spadesText.text = "Пики: " + suits["spade"];
        heartsText.text = "Черви: " + suits["heart"];
        clubsText.text = "Крести: " + suits["club"];
        diamondsText.text = "Бубны: " + suits["diamond"];
    }

    void Update()
    {
        
    }

    public double CalculateProbability(GameObject card)
    {
        return (double)suits[card.tag] / (double)pack;
    }

    public int GetCurrentSuit(GameObject card)
    {
        return suits[card.tag];
    }

    public int GetCurrentPack()
    {
        return pack;
    }

    public void UpdadeSuits(GameObject card, int add)
    {
        pack += add;
        suits[card.tag] += add;
        UpdateText();
    }

    public void UpdateText()
    {

        packText.text = "В колоде: " + pack;
        spadesText.text = "Пики: " + suits["spade"];
        heartsText.text = "Черви: " + suits["heart"];
        clubsText.text = "Крести: " + suits["club"];
        diamondsText.text = "Бубны: " + suits["diamond"];
    }
}
