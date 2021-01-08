using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public bool IsInCell = false;
    public CellSlot cellSlot;
    public double probability; 
    public SuitsManager suitsManager;
    public Transform startPosition;
    public Text numerator;
    public Text denominator;
    public GameObject decimLine;

    public delegate void OnCardDrag();
    public static event OnCardDrag OnCardDragBegin;
    public static event OnCardDrag OnCardDragEnd;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;

    public void Start()
    {
        suitsManager = FindObjectOfType<SuitsManager>();
        canvas = FindObjectOfType<Canvas>();
        OnCardDragBegin -= OffBlockRaycast;
        OnCardDragBegin += OffBlockRaycast;
        OnCardDragEnd -= OnBlockRaycast;
        OnCardDragEnd += OnBlockRaycast;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = gameObject.transform;
        if (cellSlot == null)
        {
            probability = suitsManager.CalculateProbability(this.gameObject);
            numerator.text = suitsManager.GetCurrentSuit(this.gameObject).ToString();
            denominator.text = suitsManager.GetCurrentPack().ToString();
            decimLine.SetActive(true);
        }
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.7f;
        OnCardDragBegin?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        //Debug.Log(PointerRaycast(eventData.position));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var data = PointerRaycast(eventData.position); // пускаем луч

        if (data != null && data.tag != "Cell")
        {
            if (GameConstants.gameMode == 1)
                DeleteCard();
            else
                BackIntoPos();
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
        OnCardDragEnd?.Invoke();
    }

    public void BackIntoPos()
    {
        transform.localPosition = startPosition.position;
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
        if (!IsInCell && GameConstants.gameMode == 1)
        {
            Instantiate(gameObject, transform.parent);
        }
    }

    private void OffBlockRaycast()
    {
        canvasGroup.blocksRaycasts = false;
    }

    private void OnBlockRaycast()
    {
        canvasGroup.blocksRaycasts = true;
    }

    private void OnDestroy()
    {
        OnCardDragEnd?.Invoke();
        OnCardDragBegin -= OffBlockRaycast;
        OnCardDragEnd -= OnBlockRaycast;
    }
}
