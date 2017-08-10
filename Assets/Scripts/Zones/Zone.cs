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
            //creo un placeholder che avrà sempre la posizione in cima della lista delle carte della zone
            //allo spostamento di una carta nella zona, verrà preso come riferimento il placeholder
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

        //funzione che imposta la nuova parent zone della carta rilasciata se è una carta valida
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

        //metodo per la verifica che la carta inserita nella zona sia corretta
        public abstract bool CheckValidDrop(Card card, int position);

        //aggiunta di una carta all'inizio o alla fine della lista
        public virtual void AddCard(Card card, bool top = true) {
            if (top)
            {
                cards.Add(card);
            }
            else {
                cards.Insert(0, card);
            }
        }

        //rimozione della carta dalla lista
        public virtual void RemoveCard(Card card) {
            cards.Remove(card);
        }
    }

}
