using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Solitaire {
    public class SuitZone : Zone
    {
        public Statics.Suit suit;
        public Image imageSuit;

        void Start(){
            imageSuit.sprite = ResourcesManager.singleton.GetSuitSpriteFromSuit(suit);
        }

        //è possibile spostare una carta solo se segue l'ordine crescente del numero ed ha lo stesso seme
        public override bool CheckValidDrop(Card card, int position)
        {
            if (card.suit != suit)
                return false;

            if (cards.Count > 0)
            {
                Card lastCard = cards[position];
                if (lastCard.number + 1 != card.number)
                {
                    return false;
                }
            }
            else {
                if (card.number != 1)
                    return false;
            }
            return true;
        }
    }
}

