using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Solitaire{
    public class ColumnZone : Zone
    {
        Transform placeholder;
        public int startingCards;

        //è possibile posizionarci una carta solo se segue l'ordine crescente ed ha un seme di colore diverso
        public override bool CheckValidDrop(Card card, int position)
        {
            if (cards.Count > 0)
            {
                Card lastCard = cards[position];
                if (lastCard.number - 1 != card.number)
                {
                    return false;
                }
                if (Statics.GetSuitColor(lastCard.suit) == Statics.GetSuitColor(card.suit))
                {
                    return false;
                }
            }
            else {
                if (card.number != 13) {
                    return false;
                }
            }

            return true;
        }

    }
}
