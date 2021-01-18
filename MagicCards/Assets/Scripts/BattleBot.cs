using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleBot : MonoBehaviour
{
    public GameObject hand;
    public Battle battle;
    private SuitsManager suitsManager;
    public List<GameObject> previewCards;
    private List<Vector2> previewCardsPos = new List<Vector2>();
    public static BattleBot singleton;

    void Awake()
    {
        singleton = this;
    }

    void Start()
    {
        battle = FindObjectOfType<Battle>();
        suitsManager = FindObjectOfType<SuitsManager>();
        for (var i = 0; i < previewCards.Count; i++)
        {
            previewCardsPos.Add(previewCards[i].transform.position);
        }
    }

    public void DeleteRandomPreviewCard()
    {
        var activePrevCards = previewCards.Where(comp => comp.activeInHierarchy).ToArray();
        var cardToSet = activePrevCards[Random.Range(0, activePrevCards.Length - 1)];
        Destroy(cardToSet);
    }
    public void BotTurn()
    {
        var emptyCells = battle.allCells.Where(comp => comp.IsCellEmpty).ToArray();

        var notEmptySlots = hand.GetComponentsInChildren<Transform>()
            .Where(comp => comp.gameObject.tag == "Slot")
            .Select(comp => comp.gameObject)
            .Where(comp => comp.transform.childCount != 0)
            .ToArray();

        if (emptyCells.Length == 0 || notEmptySlots.Length == 0)
        {
            //battle.CheckEndOfTurns();
            return;
        }

        var activePrevCards = previewCards.Where(comp => comp.activeInHierarchy).ToArray();
        var cardToSet = activePrevCards[Random.Range(0, activePrevCards.Length - 1)];
        //StartCoroutine(WaitForThink());
        SetCard(emptyCells, notEmptySlots, cardToSet);
    }

    public void ReloadPreviewCards()
    {
        for (var i = 0; i < previewCards.Count; i++)
        { 
            previewCards[i].SetActive(true);
            previewCards[i].transform.position = previewCardsPos[i];
        }
    }

    private void SetCard(CellSlot[] emptyCells, GameObject[] slots, GameObject prevCard)
    {
        var card = slots[Random.Range(0, slots.Length - 1)].transform.GetChild(0).gameObject;
        var cardProp = card.GetComponent<DragAndDrop>();
        cardProp.SendBeginDragEvent();
        var playerHand = battle.playerHand.GetComponent<CanvasGroup>();
        playerHand.blocksRaycasts = false;
        playerHand.alpha = 0.5f;

        cardProp.handler = "bot";
        cardProp.probability = suitsManager.CalculateProbability(card);
        cardProp.numerator.text = suitsManager.GetCurrentSuit(card).ToString();
        cardProp.denominator.text = suitsManager.GetCurrentPack().ToString();
        cardProp.decimLine.SetActive(true);
        var cellToDrop = emptyCells[Random.Range(0, emptyCells.Length - 1)];
        battle.MoveCard(prevCard, cellToDrop.gameObject);
        cellToDrop.StartCoroutine(cellToDrop.DropCardFromBot(card));
        StartCoroutine(HideAfterSeconds(prevCard));
    }

    private IEnumerator HideAfterSeconds(GameObject card)
    {
        yield return new WaitForSeconds(2f);
        card.SetActive(false);
    }
    public void TurnOnPreview()
    {
        foreach (var prevCard in previewCards)
            prevCard.SetActive(true);
    }
}