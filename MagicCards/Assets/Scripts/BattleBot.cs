using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleBot : MonoBehaviour
{
    public GameObject hand;
    public Battle battle;
    private SuitsManager suitsManager;
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

            var card = slots[Random.Range(0, slots.Length - 1)].transform.GetChild(0).gameObject;
            card.GetComponent<DragAndDrop>().probability = suitsManager.CalculateProbability(card);
            card.GetComponent<DragAndDrop>().numerator.text = suitsManager.GetCurrentSuit(card).ToString();
            card.GetComponent<DragAndDrop>().denominator.text = suitsManager.GetCurrentPack().ToString();
            card.GetComponent<DragAndDrop>().decimLine.SetActive(true);
            emptyCells[Random.Range(0, emptyCells.Length - 1)].DropCardFromBot(card);
        }
    }
}
