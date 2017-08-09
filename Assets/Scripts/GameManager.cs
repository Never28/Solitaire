using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solitaire {
    public class GameManager : MonoBehaviour
    {
        public GameState gameState;
        List<Card> cards = new List<Card>();
        public Transform canvas;

        //references
        public Hand hand;
        public Zone deckZone;
        public Zone drawZone;
        public Zone discardZone;
        public Zone heartsZone;
        public Zone diamondsZone;
        public Zone clubsZone;
        public Zone spadesZone;
        public Zone firstColumnZone;
        public Zone secondColumnZone;
        public Zone thirdColumnZone;
        public Zone fourthColumnZone;
        public Zone fifthColumnZone;
        public Zone sixthColumnZone;
        public Zone seventhColumnZone;
        public List<ColumnZone> columnZones = new List<ColumnZone>();

        public Stack<CommandList> commandListStack = new Stack<CommandList>();
        public Queue<CommandList> commandListQueue = new Queue<CommandList>();
        public CommandList runningCommandList;

        public bool startGame = true;

        public static GameManager singleton;
        void Awake() {
            singleton = this;
            if (!SessionManager.singleton.newGame)
            {
                gameState = Serializer.singleton.LoadGame();
            }
            if (gameState == null)
            {
                gameState = new GameState();
            }
            columnZones.Add((ColumnZone)firstColumnZone);
            columnZones.Add((ColumnZone)secondColumnZone);
            columnZones.Add((ColumnZone)thirdColumnZone);
            columnZones.Add((ColumnZone)fourthColumnZone);
            columnZones.Add((ColumnZone)fifthColumnZone);
            columnZones.Add((ColumnZone)sixthColumnZone);
            columnZones.Add((ColumnZone)seventhColumnZone);
        }

        // Use this for initialization
        void Start()
        {
            ShuffleCardsInDeck();
        }

        void ShuffleCardsInDeck()
        {
            List<Card> deck = new List<Card>(ObjectPool.singleton.cards);
            while (deck.Count > 0) {
                int index = UnityEngine.Random.Range(0, deck.Count);
                Card card = deck[index];
                card.parentZone = deckZone;
                //card.AddToParentZone();
                Move move = new Move(card, null, deckZone, false, false);
                Flip flip = new Flip(card, false);
                CommandList commands = new CommandList();
                commands.AddCommand(move);
                commands.AddCommand(flip);
                commands.undo = false;
                commandListQueue.Enqueue(commands);
                deck.RemoveAt(index);
            }
        }

        void Update() {
            if (startGame && commandListQueue.Count == 0 && runningCommandList == null) {
                bool sortCard = false;
                foreach (ColumnZone column in columnZones)
                {
                    if (column.startingCards > column.cards.Count) {
                        sortCard = true;
                        Card card = deckZone.cards[deckZone.cards.Count - 1];
                        card.parentZone = column;
                        Move move = new Move(card, card.prevZone, card.parentZone, true, false);
                        CommandList commands = new CommandList();
                        commands.AddCommand(move);
                        commands.undo = false;
                        commandListQueue.Enqueue(commands);
                        if (column.startingCards == column.cards.Count + 1) {
                            Flip flip = new Flip(card, true);
                            CommandList commands2 = new CommandList();
                            commands2.AddCommand(flip);
                            commands2.undo = false;
                            commandListQueue.Enqueue(commands2);
                        }
                        break;
                    }
                }
                startGame = sortCard;
            }

            CheckRunningCommandList();
            if (runningCommandList == null) {
                ExecuteNextCommandList();
                if (runningCommandList == null) {
                    if (Input.GetKeyDown(KeyCode.Z))
                        UndoCommandList();
                }
            }
        }

        void CheckRunningCommandList()
        {
            if (runningCommandList != null)
            {
                bool running = false;
                foreach (ICommand command in runningCommandList.commands)
                {
                    running = running || command.Running;
                }
                if (!running)
                {
                    runningCommandList = null;
                }
            }
        }

        void ExecuteNextCommandList() {
            if (commandListQueue.Count > 0) {
                runningCommandList = commandListQueue.Dequeue();
                if (runningCommandList.undo)
                    commandListStack.Push(runningCommandList);
                foreach (ICommand command in runningCommandList.commands)
                {
                    StartCoroutine(command.Execute());
                }
            }

        }

        public void UndoCommandList() {
            if (commandListStack.Count > 0)
            {
                runningCommandList = commandListStack.Pop();
                foreach (ICommand command in runningCommandList.commands)
                {
                    StartCoroutine(command.Undo());
                }
            }
        }

        void PlaceStartingCardsInColumn(ColumnZone column)
        {
            for (int i = 1; i <= column.startingCards; i++)
            {
                Card card = deckZone.cards[deckZone.cards.Count - i];
                card.parentZone = column;
                Move move = new Move(card, card.prevZone, card.parentZone, true, false);
                CommandList commands = new CommandList();
                commands.AddCommand(move);
                commandListQueue.Enqueue(commands);
                //card.AddToParentZone();
            }
            //column.FlipLastCard();
        }

        void OnApplicationQuit() {
            UpdateGameState();
            Serializer.singleton.SaveGame(gameState);
        }

        void UpdateGameState() {
            gameState.cardState = new List<CardState>();
            foreach (Card card in ObjectPool.singleton.cards)
            {
                CardState state = new CardState();
                state.suit = card.suit;
                state.number = card.number;
                state.frontSide = card.frontSide;
                state.zone = card.parentZone;
                state.position = card.transform.GetSiblingIndex();
            }
        }

    }

    [System.Serializable]
    public class GameState{
        public int moves = 0;
        public int scores = 0;
        public float timer = 0;

        public List<CardState> cardState = new List<CardState>();
    }

    [System.Serializable]
    public class CardState {
        public Statics.Suit suit;
        public int number;
        public bool frontSide;
        public Zone zone;
        public int position;        
    }

    public class CommandList {
        public List<ICommand> commands = new List<ICommand>();
        public bool undo = true;

        public void AddCommand(ICommand command)
        {
            commands.Add(command);
        }

    }
}

