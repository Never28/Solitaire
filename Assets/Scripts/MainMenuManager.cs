using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solitaire {
    public class MainMenuManager : MonoBehaviour
    {

        public void NewGame(){
            SessionManager.singleton.newGame = true;
            SessionManager.singleton.LoadScene("Game");
        }

        public void Continue() {
            SessionManager.singleton.newGame = false;
            SessionManager.singleton.LoadScene("Game");
        }
    }
}
