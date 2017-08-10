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
        public List<SuitZone> suitZones = new List<SuitZone>();

        public Stack<CommandList> commandListStack = new Stack<CommandList>();
        public Queue<CommandList> commandListQueue = new Queue<CommandList>();
        public CommandList runningCommandList;


        public bool startGame = true;
        public bool win = false;

        float time = 0;

        public static GameManager singleton;
        void Awake() {
            singleton = this;
            //se Continua carico lo stato del gioco precedente
            if (!SessionManager.singleton.newGame)
            {
                gameState = Serializer.singleton.LoadGame();
            }
            if (gameState == null)
            {
                gameState = new GameState();
                if (SessionManager.singleton.options.vegas)
                    gameState.scores = -52;
            }
            columnZones.Add((ColumnZone)firstColumnZone);
            columnZones.Add((ColumnZone)secondColumnZone);
            columnZones.Add((ColumnZone)thirdColumnZone);
            columnZones.Add((ColumnZone)fourthColumnZone);
            columnZones.Add((ColumnZone)fifthColumnZone);
            columnZones.Add((ColumnZone)sixthColumnZone);
            columnZones.Add((ColumnZone)seventhColumnZone);

            suitZones.Add((SuitZone)heartsZone);
            suitZones.Add((SuitZone)diamondsZone);
            suitZones.Add((SuitZone)clubsZone);
            suitZones.Add((SuitZone)spadesZone);
        }

        // Use this for initialization
        void Start()
        {
            if (SessionManager.singleton.newGame)
            {
                NewGame();
            }
            else {
                ContinueGame();
            }
        }

        void NewGame() {
            //mescolo le carte casualmente nel mazzo
            ShuffleCardsInDeck();
        }

        void ContinueGame() { 
            //TODO dal gameState caricato posizione le carte nelle zone corrette
        }

        void RestartGame() { 
            //TODO riavvio la partita azzerando il gameState ma mantendente lo shuffle iniziale
        }

        void ShuffleCardsInDeck()
        {
            List<Card> deck = new List<Card>(ObjectPool.singleton.cards);
            List<Card> shuffle = new List<Card>();
            //ricavo la lista di carte ordinata casualmente da inserire nel mazzo
            while (deck.Count > 0) {
                int index = UnityEngine.Random.Range(0, deck.Count);
                Card card = deck[index];
                card.parentZone = deckZone;
                shuffle.Add(card);
                deck.RemoveAt(index);
            }
            gameState.startingShuffle = shuffle;
            //muovo le carte nel mazzo
            Move move = new Move(shuffle, null, deckZone, true, false, false);
            //giro le carte a faccia coperta
            Flip flip = new Flip(shuffle, false);
            CommandList commands = new CommandList();
            commands.AddCommand(move);
            commands.AddCommand(flip);
            commands.undo = false;
            commandListQueue.Enqueue(commands);
        }

        void Update() {
            //Controllo se le condizioni di vittoria sono soddisfatte
            if (CheckWin())
                Win() ;
            
            //se inizio game, sposto le carte dal deck alle colonne
            if (SessionManager.singleton.newGame && startGame && commandListQueue.Count == 0 && runningCommandList == null)
            {
                PlaceStartingCardsInColumn();
            }

            if (!startGame && !win) {
                if (SessionManager.singleton.options.timer && !SessionManager.singleton.options.vegas) {
                    time += Time.deltaTime;
                    if (time > 10) {
                        time = 0;
                        gameState.scores -= 2;
                    }
                }
                gameState.timer += Time.deltaTime;

                //Controllo se le condizioni per il completamento automatico della partita sono soddisfatte
                //if (CheckCompleteGame())
                    //CompleteGame();
            }

            //controllo se il comando in esecuzione è terminato, nel caso svuoto la variabile
            CheckRunningCommandList();
            if (runningCommandList == null)
            {
                //se non si stanno eseguendo comando, prendo il primo delle coda da eseguiro e lo esgue
                ExecuteNextCommandList();
            }

        }

        void PlaceStartingCardsInColumn()
        {
            bool sortCard = false;
            foreach (ColumnZone column in columnZones)
            {
                if (column.startingCards > column.cards.Count)
                {
                    sortCard = true;
                    Card card = deckZone.cards[deckZone.cards.Count - 1];
                    card.parentZone = column;
                    Move move = new Move(card, card.prevZone, card.parentZone, true, true, false);
                    CommandList commands = new CommandList();
                    commands.AddCommand(move);
                    commands.undo = false;
                    commandListQueue.Enqueue(commands);
                    //se è l'ultima carta, la giro
                    if (column.startingCards == column.cards.Count + 1)
                    {
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

        bool CheckWin() {
            bool win = true;
            foreach (SuitZone suit in suitZones)
            {
                win = win && suit.cards.Count == 13;
            }
            return win;
        }

        void Win() {
            win = true;
            UIGameManager.singleton.OpenVictory();
        }

        //bugged
        bool CheckCompleteGame() {
            //Se non ci sono carte coperta o al di fuori delle colonne, il gioco si completa da solo
            if(deckZone.cards.Count > 0 || drawZone.cards.Count > 0 || discardZone.cards.Count > 0 || hand.cards.Count > 0)
                return false;
            
            bool frontSide = true;
            foreach (ColumnZone column in columnZones)
            {
                foreach (Card card in column.cards)
                {
                    if (!card.frontSide) {
                        frontSide = false;
                        break;
                    }
                }
                if (!frontSide)
                    break;
            }
            return frontSide;
        }

        void CompleteGame() {
            //sposto le carte dalle colonne alla zone dei segni
            foreach (ColumnZone column in columnZones)
            {
                if (column.cards.Count > 0)
                {
                    Card card = column.cards[column.cards.Count - 1];
                    foreach (SuitZone suit in suitZones)
                    {
                        if (suit.CheckValidDrop(card, suit.cards.Count - 1))
                        {
                            card.prevZone = card.parentZone;
                            card.parentZone = suit;
                            Move move = new Move(card, card.prevZone, card.parentZone, true, true, true);
                            CommandList commands = new CommandList();
                            commands.AddCommand(move);
                            commands.undo = false;
                            commandListQueue.Enqueue(commands);
                        }
                    }
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
                //se il comando prevede l'undo, lo aggiungo allo stack delle operazione eseguite
                if (runningCommandList.undo)
                    commandListStack.Push(runningCommandList);
                //eseguo la lista dei comandi
                foreach (ICommand command in runningCommandList.commands)
                {
                    StartCoroutine(command.Execute());
                }
            }

        }

        //se non ci sono comandi da eseguire, eseguo il comando di undo dell'ultima operazione eseguita
        public void UndoCommandList() {
            if (runningCommandList == null)
            {
                if (commandListStack.Count > 0)
                {
                    runningCommandList = commandListStack.Pop();
                    foreach (ICommand command in runningCommandList.commands)
                    {
                        StartCoroutine(command.Undo());
                    }
                }
            }
        }

        public void AutoMove(Card card) {
            //posizione direttamente la carta nella zona corretta, dando priorità ai segni e poi alle colonne
            if (card.parentZone is DrawZone || card.parentZone is ColumnZone) {
                foreach (SuitZone suit in suitZones)
                {
                    if (suit.CheckValidDrop(card, suit.cards.Count - 1))
                    {
                        card.prevZone = card.parentZone;
                        card.parentZone = suit;
                        Move move = new Move(card, card.prevZone, card.parentZone, true, true, true);
                        CommandList commands = new CommandList();
                        commands.AddCommand(move);
                        commands.undo = true;
                        //se la muovo dalla colonne e c'è una carta coperta, la giro
                        if (card.prevZone is ColumnZone && card.prevZone.cards.Count > 1)
                        {
                            Card lastCard = card.prevZone.cards[card.prevZone.cards.Count - 2];
                            if (!lastCard.frontSide)
                            {
                                Flip flip = new Flip(lastCard, true);
                                commands.AddCommand(flip);
                            }
                        }

                        commandListQueue.Enqueue(commands);
                        return;
                    } 
                }
            }
            if (card.parentZone is DrawZone) {
                foreach (ColumnZone column in columnZones)
                {
                    if (column.CheckValidDrop(card, column.cards.Count - 1))
                    {
                        card.prevZone = card.parentZone;
                        card.parentZone = column;
                        Move move = new Move(card, card.prevZone, card.parentZone, true, true, true);
                        CommandList commands = new CommandList();
                        commands.AddCommand(move);
                        commands.undo = true;
                        commandListQueue.Enqueue(commands);
                        return;
                    } 
                }
            }
        }

        //all'uscita dell'applicazione salvo lo stato del gioco
        void OnApplicationQuit() {
            //UpdateGameState();
            //Serializer.singleton.SaveGame(gameState);
        }

        //memorizzo la lista delle carte per ogni posizione
        void UpdateGameState() {
            gameState.cardsDeckZone.Clear();
            foreach (Card card in deckZone.cards)
            {
                gameState.cardsDeckZone.Add(new CardState(card));
            }
            gameState.cardsDrawZone.Clear();
            foreach (Card card in drawZone.cards)
            {
                gameState.cardsDrawZone.Add(new CardState(card));
            }
            gameState.cardsDiscardZone.Clear();
            foreach (Card card in discardZone.cards)
            {
                gameState.cardsDiscardZone.Add(new CardState(card));
            }
            gameState.cardsHeartsZone.Clear();
            foreach (Card card in heartsZone.cards)
            {
                gameState.cardsHeartsZone.Add(new CardState(card));
            }
            gameState.cardsDiamondsZone.Clear();
            foreach (Card card in diamondsZone.cards)
            {
                gameState.cardsDiamondsZone.Add(new CardState(card));
            }
            gameState.cardsClubsZone.Clear();
            foreach (Card card in clubsZone.cards)
            {
                gameState.cardsDiamondsZone.Add(new CardState(card));
            }
            gameState.cardsSpadesZone.Clear();
            foreach (Card card in spadesZone.cards)
            {
                gameState.cardsSpadesZone.Add(new CardState(card));
            }
            gameState.cardsFirstColumnZone.Clear();
            foreach (Card card in firstColumnZone.cards)
            {
                gameState.cardsFirstColumnZone.Add(new CardState(card));
            }
            gameState.cardsSecondColumnZone.Clear();
            foreach (Card card in secondColumnZone.cards)
            {
                gameState.cardsSecondColumnZone.Add(new CardState(card));
            }
            gameState.cardsThirdColumnZone.Clear();
            foreach (Card card in thirdColumnZone.cards)
            {
                gameState.cardsThirdColumnZone.Add(new CardState(card));
            }
            gameState.cardsFourthColumnZone.Clear();
            foreach (Card card in fourthColumnZone.cards)
            {
                gameState.cardsFourthColumnZone.Add(new CardState(card));
            }
            gameState.cardsFifthColumnZone.Clear();
            foreach (Card card in fifthColumnZone.cards)
            {
                gameState.cardsFifthColumnZone.Add(new CardState(card));
            }
            gameState.cardsSixthColumnZone.Clear();
            foreach (Card card in sixthColumnZone.cards)
            {
                gameState.cardsSixthColumnZone.Add(new CardState(card));
            }
            gameState.cardsSeventhColumnZone.Clear();
            foreach (Card card in seventhColumnZone.cards)
            {
                gameState.cardsSeventhColumnZone.Add(new CardState(card));
            }
        }

    }

    [System.Serializable]
    public class GameState{
        public List<Card> startingShuffle = new List<Card>();
        public int moves = 0;
        public int scores = 0;
        public float timer = 0;

        public List<CardState> cardsDeckZone = new List<CardState>();
        public List<CardState> cardsDrawZone = new List<CardState>();
        public List<CardState> cardsDiscardZone = new List<CardState>();
        public List<CardState> cardsHeartsZone = new List<CardState>();
        public List<CardState> cardsDiamondsZone = new List<CardState>();
        public List<CardState> cardsClubsZone = new List<CardState>();
        public List<CardState> cardsSpadesZone = new List<CardState>();
        public List<CardState> cardsFirstColumnZone = new List<CardState>();
        public List<CardState> cardsSecondColumnZone = new List<CardState>();
        public List<CardState> cardsThirdColumnZone = new List<CardState>();
        public List<CardState> cardsFourthColumnZone = new List<CardState>();
        public List<CardState> cardsFifthColumnZone = new List<CardState>();
        public List<CardState> cardsSixthColumnZone = new List<CardState>();
        public List<CardState> cardsSeventhColumnZone = new List<CardState>();
    }

    [System.Serializable]
    public class CardState {
        public Statics.Suit suit;
        public int number;
        public bool frontSide;

        public CardState(Card card) {
            this.suit = card.suit;
            this.number = card.number;
            this.frontSide = card.frontSide;
        }
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

