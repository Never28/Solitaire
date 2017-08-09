using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Solitaire
{
    public abstract class Zone : MonoBehaviour, IDropHandler
    {
        public GameObject placeHolder;
        public List<Card> cards = new List<Card>();

        void Awake()
        {
            CreatePlaceholder();
        }

        void CreatePlaceholder() {
            GameObject go = (GameObject)Instantiate(ResourcesManager.singleton.placeholderPrefab);
            go.name = "Placeholder";
            go.transform.SetParent(this.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            placeHolder = go;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (GameManager.singleton.hand.cards.Count > 1 && !this is ColumnZone)
                return;

            Card card = eventData.pointerDrag.GetComponent<Card>();
            if (card)
            {
                if (CheckValidDrop(card, cards.Count - 1))
                {
                    foreach (Card draggedCard in GameManager.singleton.hand.cards)
                    {                        
                        draggedCard.parentZone = this;
                    }
                }
            }
        }

        public abstract bool CheckValidDrop(Card card, int position);

        public virtual void AddCard(Card card) {
            cards.Add(card);
        }

        public virtual void RemoveCard(Card card) {
            cards.Remove(card);
        }
    }

}
