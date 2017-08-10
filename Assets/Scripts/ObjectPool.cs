using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solitaire {
    //classe contentene l'elenco delle carte create
    public class ObjectPool : MonoBehaviour
    {

        public List<Card> cards = new List<Card>();

        public static ObjectPool singleton;
        void Awake() {
            singleton = this;
            CreateCards();
        }

        void CreateCards()
        {

            foreach (Statics.Suit suit in Enum.GetValues(typeof(Statics.Suit)))
            {
                for (int i = 1; i <= 13; i++)
                {
                    GameObject go = (GameObject)Instantiate(ResourcesManager.singleton.cardPrefab);
                    Card card = go.GetComponent<Card>();
                    if (card)
                    {
                        card.Init(suit, i, true);
                        cards.Add(card);                    
                    }
                    go.transform.SetParent(this.transform);
                }
            }
        }
        
    }

}
