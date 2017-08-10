using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solitaire {
    public class DrawZone : Zone
    {

        void Update() {
            if (GameManager.singleton.hand.cards.Count == 0) {
                //se nella draw zone ci sono meno di tre carte e sono presenti nella discard, sposto l'ultima delle discard nella prima posizione delle draw
                if (cards.Count < 3 && GameManager.singleton.discardZone.cards.Count > 0)
                {
                    Card topDiscard = GameManager.singleton.discardZone.cards[GameManager.singleton.discardZone.cards.Count - 1];
                    Move move = new Move(topDiscard, topDiscard.parentZone, GameManager.singleton.drawZone, false, false, false);
                    CommandList commands = new CommandList();
                    commands.AddCommand(move);
                    commands.undo = false;
                    GameManager.singleton.commandListQueue.Enqueue(commands);
                }
                //se ci sono piu di tre carte nella zona di draw, sposto la prima nella zona di discard
                if (cards.Count > 3)
                {
                    Card card = cards[0];
                    Move move = new Move(card, this, GameManager.singleton.discardZone, true, false, false);
                    CommandList commands = new CommandList();
                    commands.AddCommand(move);
                    commands.undo = false;
                    GameManager.singleton.commandListQueue.Enqueue(commands);
                }
            }

        }

        public override bool CheckValidDrop(Card card, int position)
        {
            return false;
        }
    }
}

