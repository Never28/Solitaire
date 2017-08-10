using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solitaire {
    //classe per la gestione del panel e dei bottoni del menu principale
    public class UIMainMenuManager : MonoBehaviour
    {

        public void NewGame(){
            SessionManager.singleton.newGame = true;
            SessionManager.singleton.LoadScene("Game");
        }

        public void Continue() {
            SessionManager.singleton.newGame = false;
            SessionManager.singleton.LoadScene("Game");
        }

        public void Options() {
            UISessionManager.singleton.OpenOptions();
        }

        public void Quit() {
            Application.Quit();
        }
    }
}
