using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Battle : MonoBehaviour
{
    public GameObject playerHand;
    public GameObject botHand;

    public GameObject heartPref;
    public GameObject clubPref;
    public GameObject diamondPref;
    public GameObject spadePref;
    public static bool IsPlayerTurn;
    public bool IsGameStarted = false;

    private int handSize = 4;
    [HideInInspector] public CellSlot[] allCells;
    private SuitsManager suitsManager;
    private float playerScore = 0;
    private float botScore = 0;
    private BattleBot bot;
    private GameObject backSpawnPoint;
    [SerializeField] Text playerScoreText;
    [SerializeField] Text botScoreText;
    [SerializeField] GameObject WinTable;
    [SerializeField] GameObject LoseTable;
    [SerializeField] TextMeshProUGUI plusScorePrefab;
    [SerializeField] TextMeshProUGUI turnText;

    Dictionary<int, GameObject> suits = new Dictionary<int, GameObject>(4);

    void Start()
    {
        UpdateScore();
        GameConstants.roundNumber = 1;
        if (UnityEngine.Random.Range(0, 2) == 0)
        {
            IsPlayerTurn = true;
            turnText.text = "Ты ходишь первым в этот раз";
        }
        else
        {
            IsPlayerTurn = false;
            turnText.text = "Я хожу первым в этот раз";
        }

        suits.Add(0, heartPref);
        suits.Add(1, clubPref);
        suits.Add(2, diamondPref);
        suits.Add(3, spadePref);

        backSpawnPoint = GameObject.Find("Background");
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
            //StopAllCoroutines();
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
            Invoke(nameof(RoundOver), 2f);
            return;
        }

        var playerSlots = GetSlotsFromHand(playerHand);
        playerSlots = playerSlots.Where(comp => comp.transform.childCount != 0).ToArray();

        if (IsPlayerTurn && playerSlots.Length == 0)
        {
            SumScoreOnMap();
            Invoke(nameof(RoundOver), 2f);
            return;
        }
        var botSlots = GetSlotsFromHand(botHand);
        botSlots = botSlots.Where(comp => comp.transform.childCount != 0).ToArray();

        if (!IsPlayerTurn && botSlots.Length == 0)
        {
            SumScoreOnMap();
            Invoke(nameof(RoundOver), 2f);
            return;
        }

    }

    private void SumScoreOnMap()
    {
        var previousScorePlayer = playerScore;
        var previousScoreBot = botScore;
        foreach (var cell in allCells)
        {

            if (cell.items.Count != 0)
            {
                var cardInCell = cell.items[0].GetComponent<DragAndDrop>();
                if (cardInCell.handler == "bot")
                    botScore += (float)cardInCell.probability;
                else
                    playerScore += (float)cardInCell.probability;
            }
        }
        StartCoroutine(SpawnPlusText(previousScorePlayer, previousScoreBot));
        UpdateScore();
    }

    public void RoundOver()
    {
        if (suitsManager.pack >= 4)
        {
            //StopAllCoroutines();
            Debug.Log("NextRound");
            GameConstants.roundNumber += 1;
            FindObjectOfType<Training>().UpdateRoundNumber();
            StartGame();
        }
        else 
        {
            if (playerScore <= botScore)
            {
                LoseTable.GetComponentsInChildren<TextMeshProUGUI>().Where(comp => comp.name == "OppScore").Last().text = $"Счет Противника:\n{botScore}";
                LoseTable.GetComponentsInChildren<TextMeshProUGUI>().Where(comp => comp.name == "PlayerScore").Last().text = $"Ваш Счет:\n{playerScore}";
                LoseTable.SetActive(true);
            }
            else
            {
                WinTable.GetComponentsInChildren<TextMeshProUGUI>().Where(comp => comp.name == "PlayerScore").Last().text = $"Ваш Счет:\n{playerScore}";
                WinTable.GetComponentsInChildren<TextMeshProUGUI>().Where(comp => comp.name == "OppScore").Last().text = $"Счет Противника:\n{botScore}";
                WinTable.SetActive(true);
            }
        }
    }

    private IEnumerator SpawnPlusText(float previousScorePlayer, float previousScoreBot)
    {
        var playerPlusText = Instantiate(plusScorePrefab, backSpawnPoint.transform);
        playerPlusText.text = (playerScore - previousScorePlayer).ToString();
        MoveCard(playerPlusText.gameObject, playerScoreText.gameObject);

        var botPlusText = Instantiate(plusScorePrefab, backSpawnPoint.transform);
        botPlusText.text = (botScore - previousScoreBot).ToString();
        botPlusText.color = Color.white;
        MoveCard(botPlusText.gameObject, botScoreText.gameObject);
        yield return new WaitForSeconds(2f);

        Destroy(playerPlusText.gameObject);
        Destroy(botPlusText.gameObject);
    }
    
    public void StartGame()
    {
        IsGameStarted = true;
        bot.TurnOnPreview();
        DealAllCards();
        bot.ReloadPreviewCards();
        ClearMap();
        if (!IsPlayerTurn) bot.BotTurn();
    }

    public void ShowStartDialog()
    {
        if (IsGameStarted)
            GameObject.Find("EnemyDialog").SetActive(false);
    }
    public void DealAllCards()
    {
        DealTheCards(playerHand);
        DealTheCards(botHand);
    }

    public void MoveCard(GameObject card, GameObject target)
    {
        StartCoroutine(MoveCardCorutine(card, target));
    }
    public IEnumerator MoveCardCorutine(GameObject card, GameObject target)
    {
        card.transform.DOMove(target.transform.position, 2f);
        yield return null;
        //while (card.transform.localPosition != target.transform.localPosition)
        //{
        //    card.transform.position = Vector2.MoveTowards(card.transform.position,
        //        target.transform.position, Time.deltaTime * 3f);
        //    yield return null;
        //}
    }

    public void ClearParadox(GameObject card)
    {
        var cardsToDelete = playerHand.GetComponentsInChildren<Transform>()
            .Where(comp => comp.gameObject.tag == card.tag)
            .Select(comp => comp.gameObject)
            .ToArray();
        for (var i = 0; i < cardsToDelete.Length; i++)
        {
            Destroy(cardsToDelete[i]);
        }

        cardsToDelete = botHand.GetComponentsInChildren<Transform>()
            .Where(comp => comp.gameObject.tag == card.tag)
            .Select(comp => comp.gameObject)
            .ToArray();
        for (var i = 0; i < cardsToDelete.Length; i++)
        {
            Destroy(cardsToDelete[i]);
        }
        CheckEndOfTurns();
    }
}