using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private float playerScore;
    private float botScore;

    Dictionary<int, GameObject> suits = new Dictionary<int, GameObject>(4);

    void Start()
    {
        if (Random.Range(0, 1) == 0)
            IsPlayerTurn = true;
        else
            IsPlayerTurn = false;

        suits.Add(0, heartPref);
        suits.Add(1, clubPref);
        suits.Add(2, diamondPref);
        suits.Add(3, spadePref);

        allCells = GameObject.FindGameObjectsWithTag("Cell").Select(comp => comp.GetComponent<CellSlot>()).Where(comp => comp != null).ToArray();
        suitsManager = FindObjectOfType<SuitsManager>();

        GameConstants.gameMode = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && suitsManager.pack >= 4)
        {
            DealTheCards(playerHand);
            DealTheCards(botHand);
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
        GameObject[] slots = new GameObject[handSize];

        slots = hand.GetComponentsInChildren<Transform>().Where(comp => comp.gameObject.tag == "Slot").Select(comp => comp.gameObject).ToArray();

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

    private GameObject GetRandomCard()
    {
        var card = suits[Random.Range(0, 4)];

        if (suitsManager.suits[card.tag] == 1)
            return GetRandomCard();

        return card;
    }

    public void RoundOver()
    {
        Debug.Log("Round Over");
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