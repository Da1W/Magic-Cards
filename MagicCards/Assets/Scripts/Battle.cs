using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;
using UnityEngine;

public class Battle : MonoBehaviour
{
    public GameObject playerHand;
    public GameObject botHand;

    public GameObject heartPref;
    public GameObject clubPref;
    public GameObject diamondPref;
    public GameObject spadePref;
    public bool IsPlayerTurn;

    private int handSize = 4;
    [HideInInspector] public CellSlot[] allCells;
    private SuitsManager suitsManager;
    private float playerScore = 0;
    private float botScore = 0;
    private BattleBot bot;
    [SerializeField] Text playerScoreText;
    [SerializeField] Text botScoreText;

    Dictionary<int, GameObject> suits = new Dictionary<int, GameObject>(4);

    void Start()
    {
        UpdateScore();
        GameConstants.roundNumber = 1;
        if (UnityEngine.Random.Range(0, 2) == 0)
            IsPlayerTurn = true;
        else
            IsPlayerTurn = false;

        suits.Add(0, heartPref);
        suits.Add(1, clubPref);
        suits.Add(2, diamondPref);
        suits.Add(3, spadePref);

        allCells = GameObject.FindGameObjectsWithTag("Cell")
            .Select(comp => comp.GetComponent<CellSlot>())
            .Where(comp => comp != null).ToArray();

        suitsManager = FindObjectOfType<SuitsManager>();
        bot = FindObjectOfType<BattleBot>();

        GameConstants.gameMode = 2;
    }

    private void UpdateScore()
    {
        playerScore = (float)Math.Round(playerScore, 3);
        botScore = (float)Math.Round(botScore, 3);
        playerScoreText.text = $"Вы: {playerScore}  очков";
        botScoreText.text = $"Противник: {botScore} очков";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && suitsManager.pack >= 4)
        {
            DealTheCards(playerHand);
            DealTheCards(botHand);
            bot.ReloadPreviewCards();
        }
        if (Input.GetKeyDown(KeyCode.C))
            ClearMap();
    }

    void ClearMap()
    {
        foreach (var cell in allCells)
            cell.ClearCards();
    }

    void DealTheCards(GameObject hand)
    {
        var slots = GetSlotsFromHand(hand);

        for (int i = 0; i < handSize; i++)
        {
            if (slots[i].transform.childCount == 0)
            {
                var currentCard = Instantiate(GetRandomCard(), slots[i].transform);
                currentCard.transform.localPosition = Vector3.zero;
                currentCard.GetComponent<RectTransform>().sizeDelta = new Vector2(115, 150);
            }
        }
    }

    private GameObject[] GetSlotsFromHand(GameObject hand)
    {
        GameObject[] slots = new GameObject[handSize];

        slots = hand.GetComponentsInChildren<Transform>()
            .Where(comp => comp.gameObject.tag == "Slot")
            .Select(comp => comp.gameObject).ToArray();

        return slots;
    }

    private GameObject GetRandomCard()
    {
        var card = suits[UnityEngine.Random.Range(0, 4)];

        if (suitsManager.suits[card.tag] == 0)
            return GetRandomCard();

        return card;
    }

    public void CheckEndOfTurns()
    {
        var IsAllCellsFull = true;

        foreach (var cell in allCells)
            if (cell.IsCellEmpty) IsAllCellsFull = false;

        if (IsAllCellsFull)
        {
            SumScoreOnMap();

            return;
        }

        var IsHandEmpty = true;

        var playerSlots = GetSlotsFromHand(playerHand);
        playerSlots.Where(comp => comp.transform.childCount != 0);

        if (playerSlots == null)
        {
            SumScoreOnMap();
            return;
        }
        var botSlots = GetSlotsFromHand(botHand);
        botSlots.Where(comp => comp.transform.childCount != 0);

        if (botSlots == null)
        {
            SumScoreOnMap();
            return;
        }

    }

    private void SumScoreOnMap()
    {
        foreach (var cell in allCells)
        {
            var cardInCell = cell.items[0].GetComponent<DragAndDrop>();
            if (cardInCell != null)
            {
                if (cardInCell.handler == "bot")
                    botScore += (float)cardInCell.probability;
                else
                    playerScore += (float)cardInCell.probability;
            }
        }
        UpdateScore();
    }

    public void RoundOver()
    {
        Debug.Log("Round Over");
    }

    public void MoveCard(GameObject card, GameObject target)
    {
        while (card.transform.position != target.transform.position)
        { 
        Vector2.MoveTowards(card.transform.position, target.transform.position, 5);
        }
    }

    public void ClearParadox(GameObject card)
    {
        var cardsToDelete = playerHand.GetComponentsInChildren<Transform>().Where(comp => comp.gameObject.tag == card.tag).Select(comp => comp.gameObject).ToArray();
        for (var i = 0; i < cardsToDelete.Length; i++)
        {
            Destroy(cardsToDelete[i]);
        }

        cardsToDelete = botHand.GetComponentsInChildren<Transform>().Where(comp => comp.gameObject.tag == card.tag).Select(comp => comp.gameObject).ToArray();
        for (var i = 0; i < cardsToDelete.Length; i++)
        {
            Destroy(cardsToDelete[i]);
        }
    }
}