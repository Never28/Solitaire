using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solitaire {
    //Classe per tener traccia delle carte nell'Hand
    public class Hand : MonoBehaviour
    {
        public bool drag;
        public List<Card> cards = new List<Card>();

        public void AddCard(Card card) {
            cards.Add(card);
        }

        public void RemoveCard(Card card) {
            cards.Remove(card);
        }

    }

}
