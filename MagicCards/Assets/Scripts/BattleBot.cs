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
    private GameObject[] hidePreviewCards;

    void Start()
    {
        battle = FindObjectOfType<Battle>();
        suitsManager = FindObjectOfType<SuitsManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!battle.IsPlayerTurn)
        {
            var emptyCells = battle.allCells.Where(comp => comp.IsCellEmpty).ToArray();

            var slots = hand.GetComponentsInChildren<Transform>()
                .Where(comp => comp.gameObject.tag == "Slot")
                .Select(comp => comp.gameObject)
                .Where(comp => comp.transform.childCount != 0)
                .ToArray();

            if (emptyCells.Length == 0 || slots.Length == 0)
                return;

            var activePrevCards = previewCards.Where(comp => comp.activeInHierarchy).ToArray();
            var cardToSet = activePrevCards[Random.Range(0, activePrevCards.Length - 1)];
            SetCard(emptyCells, slots, cardToSet);
        }
    }

    public void ReloadPreviewCards()
    {
        foreach (var card in previewCards)
        {
            card.SetActive(true);
        }
    }

    private void SetCard(CellSlot[] emptyCells, GameObject[] slots, GameObject prevCard)
    {
        var card = slots[Random.Range(0, slots.Length - 1)].transform.GetChild(0).gameObject;
        var cardProp = card.GetComponent<DragAndDrop>();

        cardProp.handler = "bot";
        cardProp.probability = suitsManager.CalculateProbability(card);
        cardProp.numerator.text = suitsManager.GetCurrentSuit(card).ToString();
        cardProp.denominator.text = suitsManager.GetCurrentPack().ToString();
        cardProp.decimLine.SetActive(true);
        var cellToDrop = emptyCells[Random.Range(0, emptyCells.Length - 1)];
        battle.MoveCard(prevCard, cellToDrop.gameObject);
        cellToDrop.DropCardFromBot(card);
        prevCard.SetActive(false);
    }
}
