using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Solitaire {
    public class SessionManager : MonoBehaviour
    {

        public Options options;
        public bool newGame;

        public static SessionManager singleton;
        void Awake()
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }

        // Use this for initialization
        void Start()
        {
            options = Serializer.singleton.LoadOptions();
            if (options == null)
            {
                options = new Options();
                Serializer.singleton.SaveOptions(options);
            }

            StartCoroutine("StartGame");
        }

        void SaveOptions() {
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
            yield return LoadResources();
            yield return new WaitForSeconds(0.4f);
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
            yield return new WaitForSeconds(0.5f);
            yield return SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
            SceneManager.UnloadSceneAsync("Loading");
        }

        IEnumerator LoadLoadingScene()
        {
            yield return SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);

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
