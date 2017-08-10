using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Solitaire {
    //Classe per la gestione del panel e dei bottoni delle opzioni
    public class UISessionManager : MonoBehaviour
    {
        public Transform uiOptions;
        public Image draw3Button;
        public Image vegasButton;
        public Image timerButton;
        public Image helpsButton;

        public static UISessionManager singleton;
        void Awake() {
            singleton = this;
        }

        public void OpenOptions() { 
            uiOptions.gameObject.SetActive(true);
            LoadUIOptions();
        }

        void LoadUIOptions(){
            draw3Button.color = (SessionManager.singleton.options.draw3) ? Color.red : Color.white;
            vegasButton.color = (SessionManager.singleton.options.vegas) ? Color.red : Color.white;
            timerButton.color = (SessionManager.singleton.options.timer) ? Color.red : Color.white;
            helpsButton.color = (SessionManager.singleton.options.helps) ? Color.red : Color.white;
        }

        public void OnClickDraw3() {
            SessionManager.singleton.options.draw3 = !SessionManager.singleton.options.draw3;
            LoadUIOptions();
        }

        public void OnClickVegas() { 
            SessionManager.singleton.options.vegas = !SessionManager.singleton.options.vegas;
            LoadUIOptions();        
        }

        public void OnClickTimer() { 
            SessionManager.singleton.options.timer = !SessionManager.singleton.options.timer;
            LoadUIOptions();        
        }

        public void OnClickHelps() { 
            SessionManager.singleton.options.helps = !SessionManager.singleton.options.helps;
            LoadUIOptions();        
        }

        public void CloseOptions() {
            SessionManager.singleton.SaveOptions();
            uiOptions.gameObject.SetActive(false);
        }
    }

}
