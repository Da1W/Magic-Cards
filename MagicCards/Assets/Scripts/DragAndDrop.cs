using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    public bool IsInCell = false;
    public CellSlot cellSlot;

    public double probability; 

    public SuitsManager suitsManager;

    public void Start()
    {
        suitsManager = FindObjectOfType<SuitsManager>();
        canvas = FindObjectOfType<Canvas>();
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (cellSlot == null)
        {
            probability = suitsManager.CalculateProbability(this.gameObject);
        }
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.7f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var data = PointerRaycast(eventData.position); // пускаем луч

        if (data != null && data.tag != "Cell")
        {
            DeleteCard();
        }
        else
        {
            data.transform.parent.gameObject.GetComponent<CellSlot>().OnDrop(eventData);
        }
        //else
        //{
        //    if (IsInCell)
        //    {
        //        suitsManager.UpdadeSuits(this.gameObject, 1);
        //    }
        //}

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
    }

    public void DeleteCard()
    {
        if (IsInCell)
        {
            IsInCell = false;
            cellSlot.items.Remove(gameObject);
            suitsManager.UpdadeSuits(this.gameObject, 1);
            cellSlot.SetCorrectPositions();
            cellSlot.CheckWin();
            cellSlot.trainingManager.PlusStep();
        }
        Destroy(gameObject);
    }

    GameObject PointerRaycast(Vector2 position)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        List<RaycastResult> resultsData = new List<RaycastResult>();
        pointerData.position = position;
        EventSystem.current.RaycastAll(pointerData, resultsData);

        if (resultsData.Count > 0)
        {
            return resultsData[0].gameObject;
        }

        return null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsInCell)
        {
            Instantiate(gameObject, transform.parent);
        }
    }
}
