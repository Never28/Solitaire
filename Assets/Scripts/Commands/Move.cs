using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solitaire{
    public class Move : ICommand {

        List<Card> cards = new List<Card>();
        Zone fromZone;
        Zone toZone;
        bool animation;
        bool countMove;
        bool running;

        int score = 0;

        public Move(Card card, Zone fromZone, Zone toZone, bool animation, bool countMove) {
            this.cards.Add(card);
            this.fromZone = fromZone;
            this.toZone = toZone;
            this.animation = animation;
            this.countMove = countMove;
        }
        public Move(List<Card> cards, Zone fromZone, Zone toZone, bool animation, bool countMove) {
            this.cards = cards;
            this.fromZone = fromZone;
            this.toZone = toZone;
            this.animation = animation;
            this.countMove = countMove;
        }

        public IEnumerator Execute()
        {
            Running = true;
            yield return MoveCardsToZone(cards, fromZone, toZone);
            if (countMove)
            {
                score = CountMoveAndScore();
                GameManager.singleton.gameState.moves += 1;
                GameManager.singleton.gameState.scores += score;
            }

            Running = false;
        }

        public IEnumerator Undo()
        {
            Running = true;
            yield return MoveCardsToZone(cards, toZone, fromZone);
            if (countMove) {
                GameManager.singleton.gameState.scores -= score;
            }
            Running = false;
        }

        IEnumerator MoveCardsToZone(List<Card> cards, Zone fromZone, Zone toZone) {
            foreach (Card card in cards)
            {
                //Se la carta è ancora in una Zona, la sposto nella Hand
                if (card.transform.parent && card.transform.parent.GetComponent<Zone>())
                    card.MoveCardToHand();
            }

            Transform placeholder = toZone.placeHolder.transform;

            Vector3 startingPosition = GameManager.singleton.hand.transform.position;
            Vector3 endingPosition = placeholder.position;
            //Animazione di transizione della carta dalla posizione attuale al placeholder della Zona parent
            if (animation)
            {
                float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
                float increment = cards[0].smoothness / cards[0].transitionDuration; //The amount of change to apply.

                while (progress < 1)
                {
                    GameManager.singleton.hand.transform.position = Vector3.Lerp(startingPosition, endingPosition, progress);
                    progress += increment;
                    yield return new WaitForSeconds(cards[0].smoothness);
                }
            }
            GameManager.singleton.hand.transform.position = endingPosition;

            foreach (Card card in cards)
            {
                //Rimuovo la carta dalla Hand
                GameManager.singleton.hand.RemoveCard(card);

                //Imposto la nuova ParentZone
                card.parentZone = toZone;
                //Imposto la Parent Zone come parent della carta
                card.transform.SetParent(card.parentZone.transform);
                card.transform.localPosition = Vector3.zero;
                card.transform.localScale = Vector3.one;
                //Riposizione la carta alla posizione corretta dei figli
                int index = placeholder.GetSiblingIndex();
                card.transform.SetSiblingIndex(index);
                //Aggiunto la carta alla lista della Parent Zone
                card.parentZone.AddCard(card);
                //Aggiornamento della Zona da cui proviene la carta
                if (card.prevZone is ColumnZone)
                {
                    ColumnZone zone = (ColumnZone)card.prevZone;
                    //zone.FlipLastCard();
                }
                if (card.prevZone is DrawZone)
                {
                    DrawZone zone = (DrawZone)card.prevZone;
                    zone.AddCardFromDiscardZone();
                }
            }
        }

        int CountMoveAndScore() {

            if (SessionManager.singleton.options.vegas)
            {
                if (toZone is SuitZone)                                    
                    return 5;

            }
            else {
                if (fromZone is DrawZone && toZone is DeckZone)
                    return 5;
                if (fromZone is ColumnZone && toZone is SuitZone)
                    return 10;
                if (fromZone is DrawZone && toZone is SuitZone)
                    return 15;
                if (fromZone is SuitZone && toZone is ColumnZone)
                    return -15;
            }
            return 0;
        }


        public bool Running
        {
            get
            {
                return running;
            }
            set
            {
                running = value;
            }
        }
    }
}

