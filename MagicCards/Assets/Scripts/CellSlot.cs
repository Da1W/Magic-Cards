using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    //private List<Text> decimalsText;
    private bool rightCell = false;
    private SuitsManager suitsManager;
    private GameObject cellSlotFake;
    private GameObject[] cellSlotFakes;

    void Start()
    {
        if (GameConstants.steps == null)
        {
            GameConstants.steps = new Stack<string>();
        }
        GameConstants.usedCellSlots = new Stack<CellSlot>();

        suitsManager = FindObjectOfType<SuitsManager>();
        trainingManager = FindObjectOfType<Training>();
        cellSlotFake = GameObject.Find("CellSlotFake");
        cellSlotFakes = GameObject.FindGameObjectsWithTag("CellSlotFake");

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
        if (eventData.pointerDrag != null && items.Count < 4 && !items.Contains(eventData.pointerDrag))
        {
            eventData.pointerDrag.transform.position = transform.position;
            eventData.pointerDrag.transform.SetParent(transform);
            if (eventData.pointerDrag.GetComponent<DragAndDrop>().cellSlot != null)
            {
                eventData.pointerDrag.GetComponent<DragAndDrop>().cellSlot.items.Remove(eventData.pointerDrag);
                eventData.pointerDrag.GetComponent<DragAndDrop>().cellSlot.SetCorrectPositions();
                eventData.pointerDrag.GetComponent<DragAndDrop>().cellSlot.CheckWin();
            }
            else 
            {
                suitsManager.UpdadeSuits(eventData.pointerDrag, -1);
            }
            eventData.pointerDrag.GetComponent<DragAndDrop>().cellSlot = this;
            eventData.pointerDrag.GetComponent<DragAndDrop>().IsInCell = true;
            items.Add(eventData.pointerDrag);
            eventData.pointerDrag.GetComponent<RectTransform>().sizeDelta =
                cellSlotFake.GetComponent<RectTransform>().sizeDelta;
            SetCorrectPositions();
            CheckWin();
            GameConstants.usedCellSlots.Push(this);
            GameConstants.steps.Push("Insert");
            trainingManager.PlusStep();
        }
        else if (items.Contains(eventData.pointerDrag))
        {
            eventData.pointerDrag.transform.position = transform.position;
            eventData.pointerDrag.transform.SetParent(transform);
            SetCorrectPositions();
        }
        else
        {
            items.Remove(eventData.pointerDrag);
            Destroy(eventData.pointerDrag);
        }
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
            if(trainingManager.CheckWin())
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
}
