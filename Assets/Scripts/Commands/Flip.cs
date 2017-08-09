using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solitaire
{
    public class Flip : ICommand
    {

        List<Card> cards = new List<Card>();
        bool animation;
        bool running;

        public Flip(Card card, bool animation)
        {
            this.cards.Add(card);
            this.animation = animation;
        }

        public Flip(List<Card> cards, bool animation) {
            this.cards = cards;
            this.animation = animation;
        }

        public IEnumerator Execute()
        {
            yield return FlipCards(cards);
        }

        public IEnumerator Undo()
        {
            yield return FlipCards(cards);
        }

        IEnumerator FlipCards(List<Card> cards)
        {
            foreach (Card card in cards)
            {
                Quaternion startingRotation = Quaternion.Euler(card.transform.localEulerAngles);

                float y = card.transform.localEulerAngles.y + 180;
                if (y > 360)
                    y -= 360;

                Quaternion endingRotation = Quaternion.Euler(card.transform.localEulerAngles.x, y, card.transform.localEulerAngles.z);
                bool frontSide = !card.frontSide;
                if (animation)
                {
                    float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
                    float increment = card.smoothness / card.transitionDuration; //The amount of change to apply.
                    bool flip = false;
                    while (progress < 1)
                    {
                        if (progress > 0.5 && !flip)
                        {
                            card.SetSide(frontSide);
                            flip = true;
                        }
                        card.transform.localRotation = Quaternion.Slerp(startingRotation, endingRotation, progress);
                        progress += increment;
                        yield return new WaitForSeconds(card.smoothness);
                    }
                }
                card.SetSide(frontSide);
                card.transform.localRotation = endingRotation;                
            }
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

