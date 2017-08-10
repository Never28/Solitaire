using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Solitaire {
    public class DeckZone : Zone, IPointerClickHandler
    {

        public override bool CheckValidDrop(Card card, int position) {
            return false;
        }

        #region Click Event
        //se non ci sono più carte, recupero quelle dalla discard e draw zone, le giro e le riposizione in ordine contrario nel deck
        public void OnPointerClick(PointerEventData eventData)
        {
            List<Card> cards = new List<Card>(GameManager.singleton.discardZone.cards);
            foreach (Card card in GameManager.singleton.drawZone.cards)
            {
                cards.Add(card);
            }
            cards.Reverse();
            Move move = new Move(cards, GameManager.singleton.discardZone, this, true, false, false);
            Flip flip = new Flip(cards, false);
            CommandList commands = new CommandList();
            commands.AddCommand(move);
            commands.AddCommand(flip);
            commands.undo = false;
            GameManager.singleton.commandListQueue.Enqueue(commands);
        }
        #endregion
    }

}
