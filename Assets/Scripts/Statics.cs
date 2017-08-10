using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solitaire {
    //Classe contenente i metodi comuni
    public static class Statics
    {
        public enum Suit { hearts, clubs, diamonds, spades };
        public enum Color { red, black };

        public static Color GetSuitColor(Suit suit) {
            if (suit == Suit.hearts || suit == Suit.diamonds) {
                return Color.red;
            }
            return Color.black;
        }

    }

}
