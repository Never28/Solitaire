using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solitaire {
    public class DrawZone : Zone
    {

        public override void AddCard(Card card)
        {
            base.AddCard(card);
            if (cards.Count > 3)
            {
                Debug.Log("3");
                Card discard = cards[0];
                discard.parentZone = GameManager.singleton.discardZone;
                discard.AddToParentZone();
            }
        }

        public void AddCardFromDiscardZone() {
            if (cards.Count < 3)
            {
                if (GameManager.singleton.discardZone.cards.Count > 0)
                {
                    Card discard = GameManager.singleton.discardZone.cards[GameManager.singleton.discardZone.cards.Count - 1];
                    discard.parentZone = this;
                    discard.AddToParentZone(false, false);
                }
            }
        }

        public override bool CheckValidDrop(Card card, int position)
        {
            return false;
        }
    }
}

