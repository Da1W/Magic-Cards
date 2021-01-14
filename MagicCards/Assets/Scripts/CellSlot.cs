using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class CellSlot : MonoBehaviour, IDropHandler
{
    public List<GameObject> items;

    public List<string> decimals;
    public double rightDecimal = 1;
    public GameObject rightFill;
    //[SerializeField] Text decimals1;
    //[SerializeField] Text decimals2;
    public Training trainingManager;
    public bool IsCellEmpty = true;

    //private List<Text> decimalsText;
    private bool rightCell = false;
    private SuitsManager suitsManager;
    private GameObject cellSlotFake;
    private GameObject[] cellSlotFakes;
    private Battle battle;
    public CellSlot[] neighbours;
    public CellSlot singleton;

    void Awake()
    {
        singleton = this;
    }

    void Start()
    {
        //if (GameConstants.gameMode == 2)
        //{
        //    IsCellEmpty = true;
        //}
        if (GameConstants.steps == null)
        {
            GameConstants.steps = new Stack<string>();
        }
        GameConstants.usedCellSlots = new Stack<CellSlot>();

        suitsManager = FindObjectOfType<SuitsManager>();
        trainingManager = FindObjectOfType<Training>();
        battle = FindObjectOfType<Battle>();

        cellSlotFake = GameObject.Find("CellSlotFake");

        var ChildsTransforms = GetComponentsInChildren<Transform>();
        cellSlotFakes = ChildsTransforms
            .Where(comp => comp.gameObject.tag == "CellSlotFake")
            .Select(comp => comp.gameObject).ToArray();
        //decimalsText = new List<Text>();
        //decimalsText.Add(decimals1);
        //decimalsText.Add(decimals2);

        if (decimals.Count != 0)
        {
            int i = 0;
            foreach (var e in decimals)
            {
                var nums = e.Split('/');
                rightDecimal *= double.Parse(nums[0]) / double.Parse(nums[1]);
                //text_dec.text = nums[0] + "\n—" + nums[1];

                //decimalsText[i].text = nums[0] + "\n—\n" + nums[1];
                cellSlotFakes[i].transform.GetChild(0).GetComponent<Text>().text = nums[0];
                cellSlotFakes[i].transform.GetChild(1).GetComponent<Text>().text = nums[1];
                i++;
            }

            //decimalsText.text = decimals[0] + " * " + decimals[1];
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        var droppedObject = eventData.pointerDrag.GetComponent<DragAndDrop>();

        if (eventData.pointerDrag != null && items.Count < trainingManager.maxCardsInCells
            && !items.Contains(eventData.pointerDrag))
        {
            eventData.pointerDrag.transform.position = transform.position;
            eventData.pointerDrag.transform.SetParent(transform);


            if (droppedObject.cellSlot != null)
            {
                droppedObject.cellSlot.items.Remove(eventData.pointerDrag);
                droppedObject.cellSlot.SetCorrectPositions();
                droppedObject.cellSlot.CheckWin();
            }
            else
            {
                suitsManager.UpdadeSuits(eventData.pointerDrag, -1);
            }
            droppedObject.cellSlot = this;
            droppedObject.IsInCell = true;
            items.Add(eventData.pointerDrag);

            if (GameConstants.gameMode == 2)
            {
                Battle.IsPlayerTurn = false;
                IsCellEmpty = false;
                battle.CheckEndOfTurns();
                Merge(eventData.pointerDrag, droppedObject);
                BattleBot.singleton.BotTurn();
            }

            eventData.pointerDrag.GetComponent<RectTransform>().sizeDelta =
                cellSlotFake.GetComponent<RectTransform>().sizeDelta;
            SetCorrectPositions();

            if (GameConstants.gameMode == 2)
            {
                if (int.Parse(droppedObject.numerator.text) == 1)
                    battle.ClearParadox(eventData.pointerDrag);
            }

            CheckWin();
            GameConstants.usedCellSlots.Push(this);
            GameConstants.steps.Push("Insert");
            //trainingManager.PlusStep();
        }
        else if (items.Contains(eventData.pointerDrag))
        {
            eventData.pointerDrag.transform.position = transform.position;
            eventData.pointerDrag.transform.SetParent(transform);
            SetCorrectPositions();
        }
        else
        {
            if (GameConstants.gameMode == 1)
            {
                items.Remove(eventData.pointerDrag);
                Destroy(eventData.pointerDrag);
            }
            else
            {
                droppedObject.BackIntoPos();
            }
        }
    }

    private void Merge(GameObject eventData, DragAndDrop droppedObject)
    {
        foreach (var cellSlot in neighbours)
        {
            if (cellSlot.items.Count != 0 && cellSlot.items[0].tag == eventData.tag)
            {
                var otherCard = cellSlot.items[0].GetComponent<DragAndDrop>();
                if (otherCard.handler != droppedObject.handler)
                {
                    eventData.GetComponent<Image>().color = new Color(0.77f, 1f, 0.75f);
                    droppedObject.probability += otherCard.probability;
                    droppedObject.probability /= 2;
                    WaitForThink();
                    cellSlot.ClearCards();
                }
            }
        }
    }

    public IEnumerator DropCardFromBot(GameObject card)
    {
        yield return new WaitForSeconds(2f);
        var droppedObject = card.GetComponent<DragAndDrop>();
        card.transform.position = transform.position;
        card.transform.SetParent(transform);
        suitsManager.UpdadeSuits(card, -1);

        droppedObject.cellSlot = this;
        droppedObject.IsInCell = true;
        items.Add(card);

        Battle.IsPlayerTurn = true;
        IsCellEmpty = false;

        card.GetComponent<RectTransform>().sizeDelta =
            cellSlotFake.GetComponent<RectTransform>().sizeDelta;
        SetCorrectPositions();

        Merge(card, droppedObject);
        if (int.Parse(droppedObject.numerator.text) == 1)
            battle.ClearParadox(card);

        battle.CheckEndOfTurns();
    }
    public void CheckWin()
    {
        double res = 1;
        foreach (var e in items)
        {
            res *= e.GetComponent<DragAndDrop>().probability;
        }

        if (res == rightDecimal && res != 1)
        {
            rightCell = true;
            rightFill.SetActive(true);
            if (trainingManager.CheckWin())
                trainingManager.ShowCorrect();
        }
        else
        {
            rightCell = false;
            rightFill.SetActive(false);
            trainingManager.CloseCorrect();
        }
    }

    public void SetCorrectPositions()
    {
        var rectTransform = GetComponent<RectTransform>();
        switch (items.Count)
        {
            case 1:
                items[0].transform.localPosition = new Vector3(0, 0, 0);
                break;
            case 2:
                items[0].transform.localPosition = new Vector3(-rectTransform.rect.width / 4,
                    rectTransform.rect.height / 4, 0);
                items[1].transform.localPosition = new Vector3(rectTransform.rect.width / 4,
                    -rectTransform.rect.height / 4, 0);
                break;
            case 3:
                items[0].transform.localPosition = new Vector3(-rectTransform.rect.width / 4,
                    rectTransform.rect.height / 4, 0);
                items[1].transform.localPosition = new Vector3(rectTransform.rect.width / 4,
                    rectTransform.rect.height / 4, 0);
                items[2].transform.localPosition = new Vector3(0,
                    -rectTransform.rect.height / 4, 0);
                break;
            case 4:
                items[0].transform.localPosition = new Vector3(-rectTransform.rect.width / 4,
                    rectTransform.rect.height / 4, 0);
                items[1].transform.localPosition = new Vector3(rectTransform.rect.width / 4,
                    rectTransform.rect.height / 4, 0);
                items[2].transform.localPosition = new Vector3(-rectTransform.rect.width / 4,
                    -rectTransform.rect.height / 4, 0);
                items[3].transform.localPosition = new Vector3(rectTransform.rect.width / 4,
                    -rectTransform.rect.height / 4, 0);
                break;
            default:
                break;
        }
    }

    public void ClearCards()
    {
        foreach (var card in items)
        {
            Destroy(card);
        }
        items.Clear();
        IsCellEmpty = true;
    }

    private IEnumerator WaitForThink()
    {
        yield return new WaitForSeconds(1f);
        rightDecimal = 1;
    }
}