using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Solitaire
{
    public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        public Statics.Suit suit{get; set;}
        public int number { get; set; }
        public bool frontSide;
        public float transitionDuration = 1f;
        public float rotationDuration = 1f;
        public float smoothness = 0.02f;

        public Zone prevZone;
        public Zone parentZone;

        //References
        public Image imageBigSuit;
        public Image imageSmallSuit;
        public Image imageNumber;
        public Image imageBackground;
        public GameObject front;
        CanvasGroup canvasGroup;

        Quaternion targetRotation;

        float lastClickTime = 0;
        float doubleClickTime = 0.25f;

        //inizializzo la carta con i valori e le immagini corrette
        public void Init(Statics.Suit suit, int number, bool frontSide)
        {
            this.suit = suit;
            this.number = number;
            SetSide(frontSide);

            foreach (Transform t in transform)
            {
                switch (t.name)
                {
                    case "Big Suit":
                        imageBigSuit = t.GetComponent<Image>();
                        break;
                    case "Small Suit":
                        imageSmallSuit = t.GetComponent<Image>();
                        break;
                    case "Number":
                        imageNumber = t.GetComponent<Image>();
                        break;
                    default:
                        break;
                }
            }
            this.canvasGroup = GetComponent<CanvasGroup>();

            InitSprites();
        }

        //a seconda dei valori, assegno le immagini corrette
        void InitSprites()
        {
            if (imageBigSuit) {
                imageBigSuit.sprite = ResourcesManager.singleton.GetSuitSpriteFromSuit(suit);
            }
            if (imageSmallSuit)
            {
                imageSmallSuit.sprite = ResourcesManager.singleton.GetSuitSpriteFromSuit(suit);            
            }
            if (imageNumber) {
                imageNumber.sprite = ResourcesManager.singleton.GetNumberSpriteFromNumber(number);
                if (Statics.GetSuitColor(suit) == Statics.Color.red) {
                    imageNumber.color = Color.red;
                }
            }
        }

        //sposto la carta dalla zona attuale alla mano
        public void MoveCardToHand() {
            //Imposto la Zona precedente
            if (transform.parent)
                prevZone = transform.parent.GetComponent<Zone>();

            //Aggiungo la carta alla lista della Hand
            GameManager.singleton.hand.AddCard(this);
            //Posizione l'Hand alla posizione della prima carta
            GameManager.singleton.hand.transform.position = GameManager.singleton.hand.cards[0].transform.position;
            //Imposto Hand come parent della carta
            transform.SetParent(GameManager.singleton.hand.transform);

            //Rimuovo la carta dalla lista della Zona precedente
            if (prevZone)
                prevZone.RemoveCard(this);
        }

        //imposto il lato della carta
        public void SetSide(bool frontSide, bool animation = false) {
            this.frontSide = frontSide;
            front.SetActive(frontSide);
            if (frontSide)
            {
                imageBackground.sprite = ResourcesManager.singleton.GetFrontSprite();
            }
            else
            {
                imageBackground.sprite = ResourcesManager.singleton.GetBackgroundSprite();
            }
        }

        #region Drag Event
        //Evento dell'inizio del drag
        public void OnBeginDrag(PointerEventData eventData)
        {
            //controllo che la carta sia valida da trascinare
            if (CheckInteractable())
            {
                //ricavo la carta e tutte quelle successive nella zona
                GameManager.singleton.hand.drag = true;
                List<Card> cardsDragged = new List<Card>();
                for (int i = parentZone.cards.IndexOf(this); i < parentZone.cards.Count; i++)
                {
                    cardsDragged.Add(parentZone.cards[i]);
                }
                //sposto le carte nella mano
                foreach (Card card in cardsDragged)
                {
                    int index = parentZone.cards.IndexOf(card);
                    parentZone.cards[index].canvasGroup.blocksRaycasts = false;
                    parentZone.cards[index].MoveCardToHand();
                }
            }
            else {
                GameManager.singleton.hand.drag = false;
            }
        }

        //muovo la mano in relazione al mouse
        public void OnDrag(PointerEventData eventData)
        {
            if (GameManager.singleton.hand.drag)
                GameManager.singleton.hand.transform.position = eventData.position;
        }

        //evento di fine drag
        public void OnEndDrag(PointerEventData eventData)
        {
            if (GameManager.singleton.hand.drag)
            {
                List<Card> cardsDragged = new List<Card>(GameManager.singleton.hand.cards);
                CommandList commands = new CommandList();
                foreach (Card card in cardsDragged)
                {
                    card.canvasGroup.blocksRaycasts = true;
                }
                bool newZone = prevZone != parentZone;
                //comando di movimento delle carte da dove si trovano alla nuova zona impostata nel parent dall'evento di drop della zone
                Move move = new Move(cardsDragged, prevZone, parentZone, true, newZone, newZone);
                commands.AddCommand(move);
                //se sto spostando da una colonna e l'ultima carta è coperta, la giro
                if (newZone && prevZone is ColumnZone && prevZone.cards.Count > 0)
                {
                    Card lastCard = prevZone.cards[prevZone.cards.Count - 1];
                    if (!lastCard.frontSide)
                    {
                        Flip flip = new Flip(lastCard, true);
                        commands.AddCommand(flip);
                    }
                }

                GameManager.singleton.hand.drag = false;
                GameManager.singleton.commandListQueue.Enqueue(commands);
            }
        }

        bool CheckInteractable()
        {
            //la carta è interagibile solo se è l'ultima della zona o se si trova nelle colonne e tutte quelle successive rispettano l'ordine
            if (frontSide && parentZone.cards[parentZone.cards.Count - 1] == this) {
                return true;
            }
            if (frontSide && parentZone is ColumnZone) {
                int index = parentZone.cards.IndexOf(this);
                bool valid = true;
                for (int i = index + 1; i < parentZone.cards.Count; i++)
                {
                    valid = valid && parentZone.CheckValidDrop(parentZone.cards[i], i - 1);
                }
                return valid;
            }

            return false;
        }
        #endregion  

        #region Click Event
        public void OnPointerClick(PointerEventData eventData)
        {

            //se clicco una carta nel mazzo, la sposto nella zona di draw
            if (parentZone is DeckZone && GameManager.singleton.drawZone.cards.Count <= 3) {
                //a seconda dell'opzione draw3 pesco una o tre carte
                int cardNumber = (SessionManager.singleton.options.draw3) ? 3 : 1;
                if (cardNumber > GameManager.singleton.deckZone.cards.Count)
                    cardNumber = GameManager.singleton.deckZone.cards.Count;

                for (int i = 0; i < cardNumber; i++)
                {
                    Card card = GameManager.singleton.deckZone.cards[(GameManager.singleton.deckZone.cards.Count - 1) - i];
                    card.prevZone = card.parentZone;
                    card.parentZone = GameManager.singleton.drawZone;
                    //sposto la carta nella zona di draw
                    Move move = new Move(card, card.prevZone, card.parentZone, true, true, true);
                    //giro la carta
                    Flip flip = new Flip(card, true);
                    CommandList commands = new CommandList();
                    commands.AddCommand(move);
                    commands.AddCommand(flip);
                    GameManager.singleton.commandListQueue.Enqueue(commands);      
                }

            }
            //se doppio click su una carta, la sposto automaticamente nella zona corretta
            if (Time.time - lastClickTime < doubleClickTime)
            {
                if (parentZone.cards.IndexOf(this) == parentZone.cards.Count - 1) {
                    GameManager.singleton.AutoMove(this);
                }
            }
            lastClickTime = Time.time;
        }
        #endregion
    }
}

