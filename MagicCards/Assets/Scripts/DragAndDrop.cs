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
    public string handler;
    public delegate void OnCardDrag();
    public static event OnCardDrag OnCardDragBegin;
    public static event OnCardDrag OnCardDragEnd;

    private RectTransform rectTransform;
    public CanvasGroup canvasGroup;
    private Canvas canvas;

    //public static DragAndDrop singleton;
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
        //singleton = this;
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        OnCardDragBegin -= OffBlockRaycast;
        OnCardDragBegin += OffBlockRaycast;
        OnCardDragEnd -= OnBlockRaycast;
        OnCardDragEnd += OnBlockRaycast;
    }

    private void OnEnable()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //if(!Battle.IsPlayerTurn) return;

        startPosition = gameObject.transform;
        if (!IsInCell)
        {
            probability = suitsManager.CalculateProbability(gameObject);
            numerator.text = suitsManager.GetCurrentSuit(gameObject).ToString();
            denominator.text = suitsManager.GetCurrentPack().ToString();
            decimLine.SetActive(true);
        }

        if (GameConstants.gameMode == 2)
            handler = "player";

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.7f;
        SendBeginDragEvent();
    }

    public void SendBeginDragEvent()
    {
        OnCardDragBegin -= OffBlockRaycast;
        OnCardDragBegin += OffBlockRaycast;
        OnCardDragBegin?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //if (!Battle.IsPlayerTurn) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        //Debug.Log(PointerRaycast(eventData.position));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //if (!Battle.IsPlayerTurn) return;

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

        OnCardDragEnd?.Invoke();

        if (GameConstants.gameMode == 1)
            canvasGroup.blocksRaycasts = true;
        else if (!IsInCell)
            canvasGroup.blocksRaycasts = true;
        else
            canvasGroup.blocksRaycasts = false;

        canvasGroup.alpha = 1f;
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
            //cellSlot.trainingManager.PlusStep();
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
        if (GameConstants.gameMode == 2 && IsInCell) return;
        canvasGroup.blocksRaycasts = true;
    }

    private void OnDestroy()
    {
        SendEndDragEvent();
        OnCardDragBegin -= OffBlockRaycast;
        OnCardDragEnd -= OnBlockRaycast;
    }

    public void SendEndDragEvent()
    {
        OnCardDragEnd?.Invoke();
    }
}