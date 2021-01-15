using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class BestBattle : MonoBehaviour, IPointerDownHandler, IEndDragHandler, IDragHandler, IPointerUpHandler
{
    private static Deck DECK;
    [SerializeField] GameObject MagicBallCard;
    [SerializeField] GameObject MagicBallBet;
    [SerializeField] GameObject MagicBallMainBet;
    [SerializeField] GameObject MagicBallChangeTurn;
    [SerializeField] GameObject unknownCardPref;
    [SerializeField] GameObject DeckImage;
    [SerializeField] GameObject Field;
    [SerializeField] Canvas Canvas;
    [SerializeField] Sprite SpadesPref;
    [SerializeField] Sprite HeartPref;
    [SerializeField] Sprite ClubPref;
    [SerializeField] Sprite DiamondPref;
    [SerializeField] TextMeshProUGUI ValueText;
    [SerializeField] GameObject BetButton;
    [SerializeField] GameObject OppText;
    [SerializeField] GameObject PlayerBetText;
    [SerializeField] GameObject BetText;
    [SerializeField] GameObject RateGO;
    [SerializeField] GameObject AnswerButtonsGO;
    [SerializeField] GameObject BetButtonsGO;
    [SerializeField] GameObject WinText;
    [SerializeField] GameObject WinTable;
    [SerializeField] GameObject LoseTable;
    [SerializeField] TextMeshProUGUI ScoreText;
    [SerializeField] TextMeshProUGUI OppScoreText;
    [SerializeField] TextMeshProUGUI SpadesText;
    [SerializeField] TextMeshProUGUI HeartsText;
    [SerializeField] TextMeshProUGUI ClubsText;
    [SerializeField] TextMeshProUGUI DiamondsText;
    [SerializeField] TextMeshProUGUI Higher10Text;
    [SerializeField] TextMeshProUGUI Lower10Text;
    [SerializeField] TextMeshProUGUI Equal10Text;
    [SerializeField] TextMeshProUGUI WinChanceText;
    [SerializeField] TextMeshProUGUI WinValue;
    [SerializeField] TextMeshProUGUI OppWinValue;



    private GameObject draggedCard;
    private GameObject oppDragCard;
    private bool isCardDown = false;
    private Card currentCard;
    private int? betValue;
    private Card.Suits? betSuit;
    private int? OppBetValue;
    private Card.Suits? OppBetSuit;
    private float rate;
    private float winChance;
    private Button currentSuitButton;
    private Button currentValueButton;
    private Button currentAnswerButton;
    private bool isTutorialEnd = false;
    private GameStates gameState = GameStates.BetState;
    enum GameStates
    {
        BetState,
        AnswerState
    }
    private float Rate
    {
        get { return rate; } 
        set
        {
            rate = (float)Math.Round(value, 2);
            BetText.GetComponent<TextMeshProUGUI>().text = rate.ToString();
            if (rate != 1)
            {
                winChance = (float)Math.Round(Mathf.Pow(rate, -1), 2);
                WinChanceText.text = Math.Round(Mathf.Pow(rate, -1) * 100, 2) + "%";
            }
            else WinChanceText.text = "0%";
        }
    }
    private float valueRate;
    private float suitRate;
    private float OppRate;
    private float OppvalueRate;
    private float OppsuitRate;
    private float Bank;
    private float Score;
    private float OppScore;
    private bool Answer;
    private bool OppAnswer;
    private bool Win;
    private bool OppWin;
    private bool firstTurn = true;
    private List<string> AvailableButons;
    class Card
    {
        public Cards? value { get; private set; }
        public Suits? suit { get; private set; }

        public Card(Cards value,Suits suit)
        {
            this.value = value;
            this.suit = suit;
        }

        public enum Cards
        {
            two = 2,
            three = 3,
            four = 4,
            five = 5,
            six = 6,
            seven = 7,
            eight = 8,
            nine = 9,
            ten = 10,
            J = 11,
            Q = 12,
            K = 13,
            A = 14
        }
        public enum Suits
        {
            Heart,
            Spade,
            Club,
            Diamond
        }
    }

    class Deck
    {
        public List<Card> cards { get; private set; }
        public int count => cards.Count;
        public float spadesChance => (float)spades / count;
        public float heartsChance => (float)hearts / count;
        public float clubsChance => (float)clubs / count;
        public float diamondsChance => (float)diamonds / count;
        public float higher10Chance => (float)higher10 / count;
        public float lower10Chance => (float)lower10 / count;
        public float equal10Chance => (float)equal10 / count;

        public int spades => cards.Where(card => card.suit == Card.Suits.Spade).Count();
        public int hearts => cards.Where(card => card.suit == Card.Suits.Heart).Count();
        public int clubs => cards.Where(card => card.suit == Card.Suits.Club).Count();
        public int diamonds => cards.Where(card => card.suit == Card.Suits.Diamond).Count();
        public int higher10 => cards.Where(card => (int)card.value > 10).Count();
        public int lower10 => cards.Where(card => (int)card.value < 10).Count();
        public int equal10 => cards.Where(card => (int)card.value == 10).Count();


        public Deck()
        {
            cards = new List<Card>();
            for (int i = 0; i < Enum.GetNames(typeof(Card.Suits)).Length; i++)
            {
                for (int j = 2; j < 15; j++)
                {
                    cards.Add(new Card((Card.Cards)j, (Card.Suits)i));
                }
            }
        }

        public void RandomizeDeck()
        {
            var rndCount = UnityEngine.Random.Range(17,23);
            Debug.Log(rndCount);
            for(int i = 0; i < 52 - rndCount; i++)
            {
                cards.RemoveAt(UnityEngine.Random.Range(0, count));
            }
            Debug.Log(count);
        }

        public bool CanBet(Card.Suits? suit,int? value)
        {
            var valueCond = (value != null) && (((int)value == 9 && lower10 != 0) || ((int)value == 10 && equal10 != 0) || ((int)value == 11 && higher10 != 0));
            return valueCond || (suit != null && cards.Where(card => card.suit == suit).Count() != 0);
        }

        public float GetRate(Card.Suits? suit,int? value)
        {
            var valueRate = 1f;
            var suitRate = 1f;
            float suitCount = cards.Where(card => card.suit == suit).Count();
            if (value != null && value == 9) valueRate = 1f / lower10Chance;
            if (value != null && value == 10) valueRate = 1f / equal10Chance;
            if (value != null && value == 11) valueRate = 1f / higher10Chance;
            if (suit != null) suitRate = 1f/(suitCount/count);
            return valueRate * suitRate;
        }

        public void RemoveCard(Card card) => cards.Remove(card);
        
    }
    void Start()
    {
        DECK = new Deck();
        DECK.RandomizeDeck();
        StartTurn();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isCardDown || gameState == GameStates.AnswerState) return;
        var data = PointerRaycast(eventData.position); // пускаем луч

        if (data != null && data.tag != "Cell")
        {
            Destroy(draggedCard);
        }
        else
        {
            draggedCard.transform.position = data.transform.position;
            draggedCard.transform.localScale = new Vector3(3f, 3f, 3f);
            isCardDown = true;
            CreateCard(); 
            ActivateBetButtons();
            if (!isTutorialEnd)
            {
                MagicBallCard.SetActive(false);
                MagicBallBet.SetActive(true);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (draggedCard) OnEndDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isCardDown || gameState == GameStates.AnswerState) return;
        draggedCard.GetComponent<RectTransform>().anchoredPosition += eventData.delta / Canvas.scaleFactor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isCardDown || gameState == GameStates.AnswerState) return; 
        draggedCard = Instantiate(unknownCardPref, DeckImage.transform);
        draggedCard.transform.position = eventData.pointerCurrentRaycast.worldPosition;
            //DeckImage.transform.position;
        draggedCard.GetComponent<Image>().raycastTarget = false; 
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

    private void CreateCard()
    {
        var number = UnityEngine.Random.Range(0, DECK.count - 1);
        currentCard = DECK.cards[number];
        
        if (currentCard.suit == Card.Suits.Club || currentCard.suit == Card.Suits.Spade)
            ValueText.color = Color.black;
        else
            ValueText.color = new Color(0.973f, 0f, 0.455f);
        if ((int)currentCard.value <= 10)
            ValueText.text = "" + (int)currentCard.value;
        else
            ValueText.text = currentCard.value.ToString();
    }

    public void ShowCard()
    {
        switch (currentCard.suit)
        {
            case Card.Suits.Spade:
                draggedCard.GetComponent<Image>().sprite = SpadesPref;
                break;
            case Card.Suits.Heart:
                draggedCard.GetComponent<Image>().sprite = HeartPref;
                break;
            case Card.Suits.Club:
                draggedCard.GetComponent<Image>().sprite = ClubPref;
                break;
            case Card.Suits.Diamond:
                draggedCard.GetComponent<Image>().sprite = DiamondPref;
                break;
            default: break;
        }

        ValueText.enabled = true;
    }

    private void ActivateMainBetButton()
    {
        BetButton.GetComponent<Button>().enabled = true;
        var BetText = BetButton.GetComponentInChildren<TextMeshProUGUI>();
        BetText.faceColor = new Color32(BetText.faceColor.r, BetText.faceColor.g, BetText.faceColor.b,255);
        BetText.outlineColor = new Color32(BetText.outlineColor.r, BetText.outlineColor.g, BetText.outlineColor.b, 255);
    }

    private void DeactivateMainBetButton()
    {
        BetButton.GetComponent<Button>().enabled = false;
        var BetText = BetButton.GetComponentInChildren<TextMeshProUGUI>();
        BetText.faceColor = new Color32(BetText.faceColor.r, BetText.faceColor.g, BetText.faceColor.b, 100);
        BetText.outlineColor = new Color32(BetText.outlineColor.r, BetText.outlineColor.g, BetText.outlineColor.b, 100);
    }

    private void ActivateBetButtons()
    {
        BetButtonsGO.GetComponent<CanvasGroup>().alpha = 1f;
        BetButtonsGO.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
    private void DeactivateBetButtons()
    {
        BetButtonsGO.GetComponent<CanvasGroup>().alpha = 0.5f;
        BetButtonsGO.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    private void GetAIAnswer()
    {
        if (OppAnswer)
        {
            OppText.GetComponentInChildren<TextMeshProUGUI>().text = "Верю";
            OppText.GetComponent<Image>().color = new Color(0.4f, 1f, 0.4f);
        }
        else
        {
            OppText.GetComponentInChildren<TextMeshProUGUI>().text = "Не верю";
            OppText.GetComponent<Image>().color = new Color(1f, 0.4f, 0.4f);
        }
    }

    private void GetPlayerAnswer()
    {
        if (Answer)
        {
            PlayerBetText.GetComponentInChildren<TextMeshProUGUI>().text = "Верю";
            PlayerBetText.GetComponent<Image>().color = new Color(0.4f, 1f, 0.4f);
        }
        else
        {
            PlayerBetText.GetComponentInChildren<TextMeshProUGUI>().text = "Не верю";
            PlayerBetText.GetComponent<Image>().color = new Color(1f, 0.4f, 0.4f);
        }
    }

    public void BetToSpade()
    {
        Rate = valueRate;
        betSuit = Card.Suits.Spade;
        suitRate = 1f / DECK.spadesChance;
        Rate *= suitRate;
        BetText.GetComponent<TextMeshProUGUI>().text = Rate.ToString();
    }
    public void BetToHeart()
    {
        Rate = valueRate;
        betSuit = Card.Suits.Heart;
        suitRate = 1f / DECK.heartsChance;
        Rate *= suitRate;
        BetText.GetComponent<TextMeshProUGUI>().text = Rate.ToString();
    }
    public void BetToClub()
    {
        Rate = valueRate;
        betSuit = Card.Suits.Club;
        suitRate = 1f / DECK.clubsChance;
        Rate *= suitRate;
        BetText.GetComponent<TextMeshProUGUI>().text = Rate.ToString();
    }
    public void BetToDiamond()
    {
        Rate = valueRate;
        betSuit = Card.Suits.Diamond;
        suitRate = 1f / DECK.diamondsChance;
        Rate *= suitRate;
        BetText.GetComponent<TextMeshProUGUI>().text = Rate.ToString();
    }
    public void BetTo10()
    {
        Rate = suitRate;
        betValue = 10;
        valueRate = 1f/DECK.equal10Chance;
        Rate *= valueRate;
    }
    public void BetToLower10()
    {
        Rate = suitRate;
        betValue = 9;
        valueRate = 1f/DECK.lower10Chance;
        Rate *= valueRate;
    }

    public void BetToHigher10()
    {
        Rate = suitRate;
        betValue = 11;
        valueRate = 1f/DECK.higher10Chance;
        Rate *= valueRate;
    }

    public void WeDoBet()
    {
        if (!isTutorialEnd)
        {
            MagicBallMainBet.SetActive(false);
        }

        if (gameState == GameStates.BetState)
        {
            OppAnswer = UnityEngine.Random.Range(0, 2) == 1;
            Invoke(nameof(SayBet), 0.3f);
            Invoke(nameof(GetAIAnswer), 1.75f);
            Invoke(nameof(ShowCard), 3.25f);
            WeDoBetLogic();
            Invoke(nameof(CalcScores), 4f);
            Invoke(nameof(ShowTurnResult), 5f);
            DeactivateMainBetButton();
            DeactivateBetButtons();
        }

        if (gameState == GameStates.AnswerState)
        {
            Invoke(nameof(GetPlayerAnswer), 0.3f);
            Invoke(nameof(ShowCard), 1.5f);
            OppDoBetLogic();
            Invoke(nameof(CalcScores), 3f);
            Invoke(nameof(ShowTurnResult), 4f);
            DeactivateMainBetButton();
            DeactivateBetButtons();
        }
    }

    private void WeDoBetLogic()
    {
        Bank = Rate;
        if(valueRate > 1 && suitRate == 1)
        {
            Win = betValue > 10 && (int)currentCard.value > 10 || betValue < 10 && (int)currentCard.value < 10 || betValue == 10 && (int)currentCard.value == 10;
        }
        if(valueRate == 1 && suitRate > 1)
        {
            Win = betSuit == currentCard.suit;
        }
        if(valueRate > 1 && suitRate > 1)
        {
            Win = (betValue > 10 && (int)currentCard.value > 10 || betValue < 10 && (int)currentCard.value < 10 || betValue == 10 && (int)currentCard.value == 10) && betSuit == currentCard.suit; 
        }
        OppWin = Win == OppAnswer;
        
        DECK.RemoveCard(currentCard);
    }

    private void OppDoBetLogic()
    {
        OppRate = DECK.GetRate(OppBetSuit,OppBetValue);
        Bank = OppRate;
        if (OppBetValue != null && OppBetSuit == null)
        {
            Win = OppBetValue > 10 && (int)currentCard.value > 10 || OppBetValue < 10 && (int)currentCard.value < 10 || OppBetValue == 10 && (int)currentCard.value == 10;
        }
        if (OppBetValue == null && OppBetSuit != null)
        {
            Win = OppBetSuit == currentCard.suit;
        }
        if (OppBetValue != null && OppBetSuit != null)
        {
            Win = (OppBetValue > 10 && (int)currentCard.value > 10 || OppBetValue < 10 && (int)currentCard.value < 10 || OppBetValue == 10 && (int)currentCard.value == 10) 
                && OppBetSuit == currentCard.suit;
        }
        OppWin = Win == Answer;

        DECK.RemoveCard(currentCard);
    }

    private void CalcScores()
    {
        var winValue = 0f;
        var oppWinValue = 0f;
        if (Win && OppWin)
        {
            winValue = (float)Math.Round(Bank * 0.7f, 2);
            oppWinValue = (float)Math.Round(Bank * 0.3f, 2);
            
            Field.GetComponent<Image>().color = new Color(0.6f, 1f, 0.6f);
            Bank = 0;
        }
        if (Win && !OppWin)
        {
            winValue = (float)Math.Round(Bank, 2);
            oppWinValue = 0;

            Field.GetComponent<Image>().color = new Color(0.6f, 1f, 0.6f);
            Bank = 0;
        }
        if (!Win && OppWin)
        {
            oppWinValue = (float)Math.Round(1 / (1 - Mathf.Pow(Bank, -1f)), 2);
            winValue = 0;

            Field.GetComponent<Image>().color = new Color(1f, 0.6f, 0.6f);
            Bank = 0;
        }
        if (!Win && !OppWin)
        {
            winValue = 0;
            oppWinValue = 0;
            
            Field.GetComponent<Image>().color = new Color(1f, 0.6f, 0.6f);
            Bank = 0;
        }
        if (gameState == GameStates.BetState)
        {
            Score += winValue;
            OppScore += oppWinValue;
            WinValue.text = winValue.ToString();
            OppWinValue.text = oppWinValue.ToString();
        }
        if (gameState == GameStates.AnswerState)
        {
            Score += oppWinValue;
            OppScore += winValue;
            WinValue.text = oppWinValue.ToString();
            OppWinValue.text = winValue.ToString();
        }
    }

    private void ShowTurnResult()
    {
        if (draggedCard) Destroy(draggedCard);
        ValueText.enabled = false;
        WinText.SetActive(true);
        ScoreText.text = Score.ToString();
        OppScoreText.text = OppScore.ToString();
        Invoke(nameof(StartTurn), 2.5f);
    }

    public void StartTurn()
    {
        Rate = 1f;
        SpadesText.text = DECK.spades.ToString();
        HeartsText.text = DECK.hearts.ToString();
        ClubsText.text = DECK.clubs.ToString();
        DiamondsText.text = DECK.diamonds.ToString();
        Lower10Text.text = DECK.lower10.ToString();
        Higher10Text.text = DECK.higher10.ToString();
        Equal10Text.text = DECK.equal10.ToString();
        WinText.SetActive(false);
        Field.GetComponent<Image>().color = new Color(0.6f, 0.6f, 1f);
        OffUnableButtons();
        if (EndGameCondition()) EndGame();
        isCardDown = false;
        
        valueRate = 1f;
        suitRate = 1f;
        OppBetSuit = null;
        OppBetValue = null;
        betSuit = null;
        betValue = null;
        if (currentValueButton)
        {
            currentValueButton.gameObject.GetComponent<Image>().color = Color.white;
            currentValueButton = null;
        }
        if (currentSuitButton)
        {
            currentSuitButton.gameObject.GetComponent<Image>().color = Color.white;
            currentSuitButton = null;
        }
        if (currentAnswerButton)
        {
            currentAnswerButton.gameObject.GetComponent<Image>().color = Color.white;
            currentAnswerButton = null;
            AnswerButtonsGO.GetComponent<CanvasGroup>().alpha = 0.5f;
            AnswerButtonsGO.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        OppText.GetComponentInChildren<TextMeshProUGUI>().text = "...";
        OppText.GetComponent<Image>().color = Color.white;
        PlayerBetText.GetComponentInChildren<TextMeshProUGUI>().text = "...";
        PlayerBetText.GetComponent<Image>().color = Color.white;

        if (!firstTurn) ChangeTurn();
        firstTurn = false;
    }

    public void OnClick(Button btn)
    {
        if (!isTutorialEnd && !btn.name.Contains("Answer"))
        {
            MagicBallBet.SetActive(false);
            MagicBallMainBet.SetActive(true);
        }

        if (btn.name.Contains("Answer"))
        {
            if (!isTutorialEnd)
            {
                MagicBallChangeTurn.SetActive(false);
                isTutorialEnd = true;
            }

            if (currentAnswerButton && btn == currentAnswerButton)
            {
                currentAnswerButton.gameObject.GetComponent<Image>().color = Color.white;
                currentAnswerButton = null;
                DeactivateMainBetButton();
                return;
            }
            if (!currentAnswerButton) currentAnswerButton = btn;
            currentAnswerButton.gameObject.GetComponent<Image>().color = Color.white;
            currentAnswerButton = btn;
            currentAnswerButton.gameObject.GetComponent<Image>().color = new Color(0.3f, 1f, 0.35f);
            if (currentAnswerButton.name.Contains("Yes"))
            {
                var winchance = Mathf.Pow(DECK.GetRate(OppBetSuit, OppBetValue), -1);
                WinChanceText.text = Math.Round(winchance * 100, 2) + "%";
                Answer = true;
            }
            if (currentAnswerButton.name.Contains("No"))
            {
                var winchance = 1 - Mathf.Pow(DECK.GetRate(OppBetSuit, OppBetValue), -1);
                WinChanceText.text = Math.Round(winchance * 100, 2) + "%";
                Answer = false;
            }
            
            ActivateMainBetButton();
            return;
        }

        if (btn.name.Contains("10"))
        {
            if(currentValueButton && btn == currentValueButton)
            {
                currentValueButton.gameObject.GetComponent<Image>().color = Color.white;
                valueRate = 1f;
                Rate = suitRate;
                currentValueButton = null;
                if (!currentSuitButton || Rate == 1) DeactivateMainBetButton();
                return;
            }
            if (!currentValueButton) currentValueButton = btn;
            currentValueButton.gameObject.GetComponent<Image>().color = Color.white;
            currentValueButton = btn;
            currentValueButton.gameObject.GetComponent<Image>().color = new Color(0.3f, 1f, 0.35f);
            if (Rate != 1) ActivateMainBetButton();
            else DeactivateMainBetButton();
        }
        else 
        {
            if (currentSuitButton && btn == currentSuitButton)
            {
                currentSuitButton.gameObject.GetComponent<Image>().color = Color.white;
                suitRate = 1f;
                Rate = valueRate;
                currentSuitButton = null;
                if (!currentValueButton || Rate == 1) DeactivateMainBetButton();
                return;
            }
            if (!currentSuitButton) currentSuitButton = btn;
            currentSuitButton.gameObject.GetComponent<Image>().color = Color.white;
            currentSuitButton = btn;
            currentSuitButton.gameObject.GetComponent<Image>().color = new Color(0.3f, 1f, 0.35f);
            if (Rate != 1) ActivateMainBetButton();
            else DeactivateMainBetButton();
        }
    }

    private void ChangeTurn()
    {
        if (gameState == GameStates.BetState)
        {
            gameState = GameStates.AnswerState;
            DeckImage.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
            RateGO.SetActive(false);
            BetButtonsGO.SetActive(false);
            AnswerButtonsGO.SetActive(true);
            Invoke(nameof(DoOppTurn), 1f);
            return;
        }
        if (gameState == GameStates.AnswerState)
        {
            gameState = GameStates.BetState;
            DeckImage.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            RateGO.SetActive(true);
            BetButtonsGO.SetActive(true);
            AnswerButtonsGO.SetActive(false);
            return;
        }
    }

    private void SetCardBigScale()
    {
        draggedCard.transform.localScale = new Vector3(3f, 3f, 3f);
    }
    private void DoOppTurn()
    {
        if (UnityEngine.Random.Range(0, 2) == 1)
            OppBetSuit = (Card.Suits)UnityEngine.Random.Range(0, 4);
        else
        {
            OppBetSuit = null;
            OppBetValue = 10 + UnityEngine.Random.Range(-1, 2);
        }
        if (UnityEngine.Random.Range(0, 2) == 1)
            OppBetValue = 10 + UnityEngine.Random.Range(-1, 2);
        else
        {
            OppBetValue = null;
            OppBetSuit = (Card.Suits)UnityEngine.Random.Range(0, 4);
        }

        if (!DECK.CanBet(OppBetSuit, OppBetValue))
        {
            DoOppTurn();
            return;
        }

        draggedCard = Instantiate(unknownCardPref, DeckImage.transform);
        draggedCard.transform.localPosition = new Vector3(0f, 0f, 0f);
        draggedCard.transform.DOLocalMove(new Vector3(-575f, 25f, 0f), 1f);
        Invoke(nameof(SetCardBigScale), 1.2f);

        CreateCard();

        Invoke(nameof(SayOppBet), 2f);
    }

    private void SayOppBet()
    {
        string SuitText = "";
        string ValueText = "";
        if (OppBetSuit != null && OppBetSuit == Card.Suits.Spade) SuitText = " Пики";
        if (OppBetSuit != null && OppBetSuit == Card.Suits.Heart) SuitText = " Черви";
        if (OppBetSuit != null && OppBetSuit == Card.Suits.Club) SuitText = " Крести";
        if (OppBetSuit != null && OppBetSuit == Card.Suits.Diamond) SuitText = " Буби";

        if (OppBetValue != null && OppBetValue == 11) ValueText = "Больше Десяти ";
        if (OppBetValue != null && OppBetValue == 10) ValueText = "Десять ";
        if (OppBetValue != null && OppBetValue == 9) ValueText = "Меньше Десяти ";

        OppText.GetComponentInChildren<TextMeshProUGUI>().text = $"Ставлю на то, что эта Карта{SuitText} {ValueText}! ";
        AnswerButtonsGO.GetComponent<CanvasGroup>().alpha = 1;
        AnswerButtonsGO.GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (!isTutorialEnd)
        {
            MagicBallChangeTurn.SetActive(true);
        }
    }
    private void SayBet()
    {
        string SuitText = "";
        string ValueText = "";
        if (betSuit != null && betSuit == Card.Suits.Spade) SuitText = " Пики";
        if (betSuit != null && betSuit == Card.Suits.Heart) SuitText = " Черви";
        if (betSuit != null && betSuit == Card.Suits.Club) SuitText = " Крести";
        if (betSuit != null && betSuit == Card.Suits.Diamond) SuitText = " Буби";

        if (betValue != null && betValue == 11) ValueText = "Больше Десяти ";
        if (betValue != null && betValue == 10) ValueText = "Десять ";
        if (betValue != null && betValue == 9) ValueText = "Меньше Десяти ";

        PlayerBetText.GetComponentInChildren<TextMeshProUGUI>().text = $"Ставлю на то, что эта Карта{SuitText} {ValueText}! ";
    }

    public void OffUnableButtons()
    {
        AvailableButons = BetButtonsGO.GetComponentsInChildren<Transform>().Select(comp => comp.gameObject.name).ToList();

        if (DECK.spades == 0 && AvailableButons.Contains("BetSpade"))
        {
            BetButtonsGO.GetComponentsInChildren<Transform>().Where(go => go.name == "BetSpade").Last().gameObject.SetActive(false);
            AvailableButons.Remove("BetSpade");
        }
        if (DECK.hearts == 0 && AvailableButons.Contains("BetHeart"))
        {
            BetButtonsGO.GetComponentsInChildren<Transform>().Where(go => go.name == "BetHeart").Last().gameObject.SetActive(false);
            AvailableButons.Remove("BetSHeart");
        }
        if (DECK.clubs == 0 && AvailableButons.Contains("BetClub"))
        {
            BetButtonsGO.GetComponentsInChildren<Transform>().Where(go => go.name == "BetClub").Last().gameObject.SetActive(false);
            AvailableButons.Remove("BetClub");
        }
        if (DECK.diamonds == 0 && AvailableButons.Contains("BetDiamond"))
        {
            BetButtonsGO.GetComponentsInChildren<Transform>().Where(go => go.name == "BetDiamond").Last().gameObject.SetActive(false);
            AvailableButons.Remove("BetDiamond");
        }
        if (DECK.lower10 == 0 && AvailableButons.Contains("BetLower10"))
        {
            BetButtonsGO.GetComponentsInChildren<Transform>().Where(go => go.name == "BetLower10").Last().gameObject.SetActive(false);
            AvailableButons.Remove("BetLower10");
        }
        if (DECK.equal10 == 0 && AvailableButons.Contains("Bet10"))
        {
            BetButtonsGO.GetComponentsInChildren<Transform>().Where(go => go.name == "Bet10").Last().gameObject.SetActive(false);
            AvailableButons.Remove("Bet10");
        }
        if (DECK.higher10 == 0 && AvailableButons.Contains("BetHigher10"))
        {
            BetButtonsGO.GetComponentsInChildren<Transform>().Where(go => go.name == "BetHigher10").Last().gameObject.SetActive(false);
            AvailableButons.Remove("BetHigher10");
        }
    }

    private void EndGame()
    {
        if(Score <= OppScore)
        {
            LoseTable.GetComponentsInChildren<TextMeshProUGUI>().Where(comp => comp.name == "OppScore").Last().text = $"Счет Противника:\n{OppScore}";
            LoseTable.GetComponentsInChildren<TextMeshProUGUI>().Where(comp => comp.name == "PlayerScore").Last().text = $"Ваш Счет:\n{Score}";
            LoseTable.SetActive(true);
        }
        else
        {
            WinTable.GetComponentsInChildren<TextMeshProUGUI>().Where(comp => comp.name == "PlayerScore").Last().text = $"Ваш Счет:\n{Score}";
            WinTable.GetComponentsInChildren<TextMeshProUGUI>().Where(comp => comp.name == "OppScore").Last().text = $"Счет Противника:\n{OppScore}";
            WinTable.SetActive(true);
        }
    }

    private bool EndGameCondition()
    {
        var variantCount = 7;
        if (DECK.equal10 == 0) variantCount--;
        if (DECK.lower10 == 0) variantCount--;
        if (DECK.higher10 == 0) variantCount--;
        if (DECK.spades == 0) variantCount--;
        if (DECK.hearts == 0) variantCount--;
        if (DECK.clubs == 0) variantCount--;
        if (DECK.diamonds == 0) variantCount--;
        return variantCount <= 2;
    }
}
