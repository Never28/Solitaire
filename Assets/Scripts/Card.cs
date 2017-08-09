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

        public void AddToParentZone(bool animation = false, bool lastPosition = true) {
            StartCoroutine(MoveToParentZone(animation));
        }

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

        IEnumerator MoveToParentZone(bool animation, bool lastPosition = true)
        {
            //Se la carta è ancora in una Zona, la sposto nella Hand
            if (transform.parent && transform.parent.GetComponent<Zone>())
                MoveCardToHand();

            Transform placeholder = parentZone.placeHolder.transform;

            //Animazione di transizione della carta dalla posizione attuale al placeholder della Zona parent
            if (animation && lastPosition) {
                float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
                float increment = smoothness / transitionDuration; //The amount of change to apply.
                Vector3 startingPosition = transform.position;
                Vector3 offset = GameManager.singleton.hand.cards[0].transform.position - transform.position;
                Vector3 endingPosition = placeholder.position + offset;
                while (progress < 1)
                {
                    transform.position = Vector3.Lerp(startingPosition, endingPosition, progress);
                    progress += increment;
                    yield return new WaitForSeconds(smoothness);
                }
            }

            //Rimuovo la carta dalla Hand
            GameManager.singleton.hand.RemoveCard(this);

            //Imposto la Parent Zone come parent della carta
            transform.SetParent(parentZone.transform);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            //Riposizione la carta alla posizione corretta dei figli
            int index = (lastPosition) ? placeholder.GetSiblingIndex() : 0;
            transform.SetSiblingIndex(index);
            //Aggiunto la carta alla lista della Parent Zone
            parentZone.AddCard(this);
            //Aggiornamento della Zona da cui proviene la carta
            if (prevZone is ColumnZone) {
                ColumnZone zone = (ColumnZone)prevZone;
                zone.FlipLastCard();
            }
            if (prevZone is DrawZone) {
                DrawZone zone = (DrawZone)prevZone;
                zone.AddCardFromDiscardZone();
            }
        }

        public void RemoveFromParentZone() {
            if(transform.parent)
                prevZone = transform.parent.GetComponent<Zone>();
            //GameManager.singleton.hand.transform.position = this.transform.position;
            transform.SetParent(GameManager.singleton.hand.transform);
            GameManager.singleton.hand.AddCard(this);
            if(prevZone)
                prevZone.RemoveCard(this);
        }

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

            //StartCoroutine(FlipSide(frontSide, animation));
        }

        IEnumerator FlipSide(bool frontSide, bool animation) {
            Quaternion startingRotation = Quaternion.Euler(transform.localEulerAngles);
            Quaternion endingRotation;
            if (frontSide)
            {
                endingRotation = Quaternion.Euler(Vector3.zero);
            }
            else
            {
                endingRotation = Quaternion.Euler(transform.localEulerAngles.x, 180, transform.localEulerAngles.z);
            }
            if (animation)
            {
                float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
                float increment = smoothness / transitionDuration; //The amount of change to apply.
                while (progress < 1)
                {
                    if (progress > 0.5) {
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
                    transform.localRotation = Quaternion.Slerp(startingRotation, endingRotation, progress);
                    progress += increment;
                    yield return new WaitForSeconds(smoothness);
                }
            }
            transform.localRotation = endingRotation;

        }

        #region Drag Event
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (CheckInteractable())
            {
                GameManager.singleton.hand.drag = true;
                List<Card> cardsDragged = new List<Card>();
                for (int i = parentZone.cards.IndexOf(this); i < parentZone.cards.Count; i++)
                {
                    cardsDragged.Add(parentZone.cards[i]);
                }
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

        public void OnDrag(PointerEventData eventData)
        {
            if (GameManager.singleton.hand.drag)
                GameManager.singleton.hand.transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (GameManager.singleton.hand.drag)
            {
                List<Card> cardsDragged = new List<Card>(GameManager.singleton.hand.cards);
                CommandList commands = new CommandList();
                foreach (Card card in cardsDragged)
                {
                    //card.AddToParentZone(!(prevZone == parentZone));
                    card.canvasGroup.blocksRaycasts = true;
                }
                bool newZone = prevZone != parentZone;
                Move move = new Move(cardsDragged, prevZone, parentZone, newZone, newZone);
                commands.AddCommand(move);
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
            if (frontSide && parentZone.cards[parentZone.cards.Count - 1] == this) {
                return true;
            }
            if (frontSide && parentZone is ColumnZone) {
                int index = parentZone.cards.IndexOf(this);
                bool valid = true;
                for (int i = index + 1; i < parentZone.cards.Count; i++)
                {
                    valid = valid && parentZone.CheckValidDrop(parentZone.cards[i], i - 1);
                    Debug.Log(valid);
                }
                return valid;
            }

            return false;
        }
        #endregion  

        #region Click Event
        public void OnPointerClick(PointerEventData eventData)
        {
            //GameManager.singleton.hand.transform.position = eventData.position;
            /*targetRotation = Quaternion.LookRotation(-transform.forward, Vector3.up);
            flipping = true;*/
            if (parentZone is DeckZone) {
                /*if (!frontSide)
                {
                    SetSide(true, true);                
                }*/
                prevZone = parentZone;
                parentZone = GameManager.singleton.drawZone;
                //AddToParentZone(true);
                Move move = new Move(this, prevZone, parentZone, true, true);
                Flip flip = new Flip(this, true);
                CommandList commands = new CommandList();
                commands.AddCommand(move);
                commands.AddCommand(flip);
                GameManager.singleton.commandListQueue.Enqueue(commands);
            }
        }
        #endregion
    }
}

