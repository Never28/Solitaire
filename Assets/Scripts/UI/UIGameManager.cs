using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Solitaire
{
    //classe per la gestione del panel e dei bottoni del menu principale
    public class UIGameManager : MonoBehaviour
    {
        public Transform uiVictory;
        public Text moves;
        public Text scores;
        public Text timer;
        public Text victoryMoves;
        public Text victoryScores;
        public Text victoryTimer;
        
        public static UIGameManager singleton;
        void Awake() {
            singleton = this;
        }

        void Update() {
            if(moves != null)
                moves.text = GameManager.singleton.gameState.moves.ToString();
            if(scores != null)
                scores.text = GameManager.singleton.gameState.scores.ToString();
            int minutes = Mathf.FloorToInt(GameManager.singleton.gameState.timer / 60F);
            int seconds = Mathf.FloorToInt(GameManager.singleton.gameState.timer - minutes * 60);
            if(timer != null)
                timer.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        }

        public void OpenVictory() {
            uiVictory.gameObject.SetActive(true);
        }

        void LoadUIVictory() {
            victoryMoves.text = GameManager.singleton.gameState.moves.ToString();
            victoryScores.text = GameManager.singleton.gameState.scores.ToString();
            int minutes = Mathf.FloorToInt(GameManager.singleton.gameState.timer / 60F);
            int seconds = Mathf.FloorToInt(GameManager.singleton.gameState.timer - minutes * 60);
            victoryTimer.text = string.Format("{0:0}:{1:00}", minutes, seconds);

        }

        public void Options()
        {
            UISessionManager.singleton.OpenOptions();
        }

        public void Home()
        {
            SessionManager.singleton.LoadScene("MainMenu");
        }

        public void Game()
        {

        }

        public void Undo()
        {
            GameManager.singleton.UndoCommandList();
        }

        public void Help()
        {

        }
    }
}
