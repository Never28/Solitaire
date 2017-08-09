using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solitaire {
    public class Hand : MonoBehaviour
    {
        public bool drag;
        public List<Card> cards = new List<Card>();

        void Update(){
            //transform.position = Input.mousePosition;
        }

        public void AddCard(Card card) {
            cards.Add(card);
        }

        public void RemoveCard(Card card) {
            cards.Remove(card);
        }

    }

}
