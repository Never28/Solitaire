using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solitaire {
    //Classe che contiene le immagini delle carte associate dinamicamente
    public class ResourcesManager : MonoBehaviour
    {
        public GameObject cardPrefab;
        public GameObject placeholderPrefab;
        Dictionary<int, Sprite> numberSprites = new Dictionary<int, Sprite>();
        Dictionary<Statics.Suit, Sprite> suitSprites = new Dictionary<Statics.Suit, Sprite>();
        Sprite backgroundSprite;
        Sprite frontSprite;

        public static ResourcesManager singleton;
        void Awake()
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
            LoadSprites();
        }

        void LoadSprites() {
            LoadNumberSprites();
            LoadSuitSprites();
            LoadFrontSprite();
            LoadBackgroundSprite();
        }

        void LoadNumberSprites()
        {
            string path = "carte/numeri carte/new";
            Sprite[] sprites = Resources.LoadAll<Sprite>(path);
            foreach (Sprite sprite in sprites)
            {
                int number;
                switch (sprite.name)
                {
                    case "A":
                        number = 1;
                        break;
                    case "J":
                        number = 11;
                        break;
                    case "Q":
                        number = 12;
                        break;
                    case "K":
                        number = 13;
                        break;
                    default:
                        number = int.Parse(sprite.name);
                        break;
                }
                try
                {
                    numberSprites.Add(number, sprite);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("There is already a Number Sprite associated with the number" + number);
                }
            }
        }

        void LoadSuitSprites()
        {
            string path = "carte/semi/new";
            Sprite[] sprites = Resources.LoadAll<Sprite>(path);
            foreach (Sprite sprite in sprites)
            {
                Statics.Suit suit;
                switch (sprite.name)
                {
                    case "cuori":
                        suit = Statics.Suit.hearts;
                        break;
                    case "fiori":
                        suit = Statics.Suit.clubs;
                        break;
                    case "picche":
                        suit = Statics.Suit.spades;
                        break;
                    case "quadri":
                        suit = Statics.Suit.diamonds;
                        break;
                    default:
                        suit = Statics.Suit.hearts;
                        break;
                }
                try
                {
                    suitSprites.Add(suit, sprite);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("There is already a Suit Sprite associated with the suit" + suit.ToString());
                }
            }
        }

        void LoadFrontSprite(){
            string path = "carte/fronte";
            frontSprite = Resources.Load<Sprite>(path);
        }

        void LoadBackgroundSprite(){        
            string path = "carte/retro-carte";
            backgroundSprite = Resources.Load<Sprite>(path);
        }

        public Sprite GetNumberSpriteFromNumber(int number)
        {
            Sprite sprite;
            if (numberSprites.TryGetValue(number, out sprite))
            {
                return sprite;
            }
            return null;
        }

        public Sprite GetSuitSpriteFromSuit(Statics.Suit suit)
        {
            Sprite sprite;
            if (suitSprites.TryGetValue(suit, out sprite))
            {
                return sprite;
            }
            return null;
        }

        public Sprite GetBackgroundSprite() {
            return backgroundSprite;
        }

        public Sprite GetFrontSprite() {
            return frontSprite;
        }
    }
}

