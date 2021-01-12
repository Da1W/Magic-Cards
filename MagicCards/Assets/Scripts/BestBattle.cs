using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BestBattle : MonoBehaviour, IPointerDownHandler, IEndDragHandler, IDragHandler, IPointerUpHandler
{
    private static Deck DECK;
    [SerializeField] GameObject unknownCardPref;
    [SerializeField] GameObject DeckImage;
    [SerializeField] GameObject Field;
    [SerializeField] Canvas Canvas;
    [SerializeField] Sprite SpadesPref;
    [SerializeField] Sprite HeartPref;
    [SerializeField] Sprite ClubPref;
    [SerializeField] Sprite DiamondPref;
    [SerializeField] TextMeshProUGUI ValueText;
    [SerializeField] GameObject MagicBall;
    [SerializeField] GameObject BetButton;
    [SerializeField] GameObject OppText;
    [SerializeField] GameObject BetText;
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
    private bool isCardDown = false;
    private Card currentCard;
    private int betValue;
    private Card.Suits betSuit;
    private int OppBetValue;
    private Card.Suits OppBetSuit;
    private float rate;
    private float winChance;
    private Button currentSuitButton;
    private Button currentValueButton;

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
    private List<string> AbleButons;
    class Card
    {
        public Cards value { get; private set; }
        public Suits suit { get; private set; }

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
        if (isCardDown) return;
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
            MagicBall.SetActive(false);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (draggedCard) OnEndDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isCardDown) return;
        draggedCard.GetComponent<RectTransform>().anchoredPosition += eventData.delta / Canvas.scaleFactor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isCardDown) return; 
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
        OppAnswer = UnityEngine.Random.Range(0, 2) == 1;
        Invoke(nameof(GetAIAnswer), 0.75f);
        Invoke(nameof(ShowCard), 1.5f);
        WeDoBetLogic();
        Invoke(nameof(CalcScores), 2f);
        Invoke(nameof(ShowTurnResult), 3f);
        DeactivateMainBetButton();
        DeactivateBetButtons();
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

    private void CalcScores()
    {
        if (Win && OppWin)
        {
            Score += (float)Math.Round(Bank * 0.7f,2);
            WinValue.text = Math.Round(Bank * 0.7f,2).ToString();
            OppScore += (float)Math.Round(Bank * 0.3f,2);
            OppWinValue.text = Math.Round(Bank * 0.3f,2).ToString();

            Field.GetComponent<Image>().color = new Color(0.6f, 1f, 0.6f);
            Bank = 0;
        }
        if (Win && !OppWin)
        {
            Score += (float)Math.Round(Bank,2);
            WinValue.text = Math.Round(Bank,2).ToString();
            OppWinValue.text = "0";

            Field.GetComponent<Image>().color = new Color(0.6f, 1f, 0.6f);
            Bank = 0;
        }
        if (!Win && OppWin)
        {
            OppScore += (float)Math.Round(1 / (1 - Mathf.Pow(Bank, -1f)),2);
            OppWinValue.text = ((float)Math.Round(1 / (1 - Mathf.Pow(Bank, -1f)), 2)).ToString();
            WinValue.text = "0";

            Field.GetComponent<Image>().color = new Color(1f, 0.6f, 0.6f);
            Bank = 0;
        }
        if (!Win && !OppWin)
        {
            WinValue.text = "0";
            OppWinValue.text = "0";
            Field.GetComponent<Image>().color = new Color(1f, 0.6f, 0.6f);
            Bank = 0;
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
        OppText.GetComponentInChildren<TextMeshProUGUI>().text = "Думаю...";
        OppText.GetComponent<Image>().color = Color.white;
    }

    public void OnClick(Button btn)
    {
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

    public void OffUnableButtons()
    {
        AbleButons = BetButtonsGO.GetComponentsInChildren<Transform>().Select(comp => comp.gameObject.name).ToList();

        if (DECK.spades == 0 && AbleButons.Contains("BetSpade"))
        {
            BetButtonsGO.GetComponentsInChildren<Transform>().Where(go => go.name == "BetSpade").Last().gameObject.SetActive(false);
            AbleButons.Remove("BetSpade");
        }
        if (DECK.hearts == 0 && AbleButons.Contains("BetHeart"))
        {
            BetButtonsGO.GetComponentsInChildren<Transform>().Where(go => go.name == "BetHeart").Last().gameObject.SetActive(false);
            AbleButons.Remove("BetSHeart");
        }
        if (DECK.clubs == 0 && AbleButons.Contains("BetClub"))
        {
            BetButtonsGO.GetComponentsInChildren<Transform>().Where(go => go.name == "BetClub").Last().gameObject.SetActive(false);
            AbleButons.Remove("BetClub");
        }
        if (DECK.diamonds == 0 && AbleButons.Contains("BetDiamond"))
        {
            BetButtonsGO.GetComponentsInChildren<Transform>().Where(go => go.name == "BetDiamond").Last().gameObject.SetActive(false);
            AbleButons.Remove("BetDiamond");
        }
        if (DECK.lower10 == 0 && AbleButons.Contains("BetLower10"))
        {
            BetButtonsGO.GetComponentsInChildren<Transform>().Where(go => go.name == "BetLower10").Last().gameObject.SetActive(false);
            AbleButons.Remove("BetLower10");
        }
        if (DECK.equal10 == 0 && AbleButons.Contains("Bet10"))
        {
            BetButtonsGO.GetComponentsInChildren<Transform>().Where(go => go.name == "Bet10").Last().gameObject.SetActive(false);
            AbleButons.Remove("Bet10");
        }
        if (DECK.higher10 == 0 && AbleButons.Contains("BetHigher10"))
        {
            BetButtonsGO.GetComponentsInChildren<Transform>().Where(go => go.name == "BetHigher10").Last().gameObject.SetActive(false);
            AbleButons.Remove("BetHigher10");
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
