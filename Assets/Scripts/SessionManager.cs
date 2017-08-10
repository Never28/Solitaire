using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Solitaire {
    //Classe che gestisce i dati di sessione e il caricamento delle scene
    public class SessionManager : MonoBehaviour
    {

        public Options options;
        public bool newGame;
        public Transform uiOptions;

        public static SessionManager singleton;
        void Awake()
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }

        // Use this for initialization
        void Start()
        {
            //carico le opzioni, se non le trovo utilizzo quelle di default
            options = Serializer.singleton.LoadOptions();
            if (options == null)
            {
                options = new Options();
                Serializer.singleton.SaveOptions(options);
            }
            //caricamento delle scene per far avviare il gioco
            StartCoroutine("StartGame");
        }

        public void SaveOptions() {
            Serializer.singleton.SaveOptions(options);
        }

        public void LoadScene(string scene) {
            if (string.IsNullOrEmpty(scene))
                return;
            if (scene == "MainMenu") {
                StartCoroutine("LoadMainMenu");
                return;
            }
            if (scene == "Game") {
                StartCoroutine("LoadGame");
                return;
            }

        }

        IEnumerator StartGame() {
            //carica la scena che contiene risorse comuni
            yield return LoadResources();
            yield return new WaitForSeconds(0.2f);
            //carica il menu principale
            yield return LoadMainMenu();
        }

        IEnumerator LoadResources() {
            yield return SceneManager.LoadSceneAsync("Dependencies", LoadSceneMode.Single);
        }

        IEnumerator LoadMainMenu() {
            yield return SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
            Time.timeScale = 1;
        }

        IEnumerator LoadGame() {
            //ResourcesManager.singleton.ResetResources();
            //yield return LoadLevelDependencies();
            yield return LoadLoadingScene();
            yield return new WaitForSeconds(0.1f);
            yield return SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
            SceneManager.UnloadSceneAsync("Loading");
        }

        IEnumerator LoadLoadingScene()
        {
            yield return SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Single);

        }
    }

    [System.Serializable]
    public class Options
    {
        public bool draw3;
        public bool vegas;
        public bool helps;
        public bool timer;
        public bool sounds;
        public bool blockOrientation;
        public string background;
        public int cardType = 0;
    }
}
