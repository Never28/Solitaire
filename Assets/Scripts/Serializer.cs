using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Solitaire {
    public class Serializer : MonoBehaviour
    {
        string directoryName = "Data";
        string optionsFileName = "options";
        string gameFileName = "game";

        public static Serializer singleton;
        void Awake()
        {
            singleton = this;
        }

        public Options LoadOptions() {
            Options r = null;
            string saveFile = SaveLocation() + "/" + directoryName;

            if(!Directory.Exists(saveFile))
                return null;

            saveFile += "/" + optionsFileName;
            if(File.Exists(saveFile)){
                IFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(saveFile, FileMode.Open);

                Options save = (Options)formatter.Deserialize(stream);
                r = save;
                stream.Close();
            }
            return r;
        }

        public void SaveOptions(Options options) {
            string saveLocation = SaveLocation() + "/" + directoryName;

            if (!Directory.Exists(saveLocation)) {
                Directory.CreateDirectory(saveLocation);
            }

            saveLocation += "/" + optionsFileName;

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(saveLocation, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, options);
            stream.Close();
        }

        public GameState LoadGame()
        {
            GameState r = null;
            string saveFile = SaveLocation() + "/" + directoryName;

            if (!Directory.Exists(saveFile))
                return null;

            saveFile += "/" + gameFileName;
            if (File.Exists(saveFile))
            {
                IFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(saveFile, FileMode.Open);

                GameState save = (GameState)formatter.Deserialize(stream);
                r = save;
                stream.Close();
            }
            return r;
        }

        public void SaveGame(GameState game)
        {
            string saveLocation = SaveLocation() + "/" + directoryName;

            if (!Directory.Exists(saveLocation))
            {
                Directory.CreateDirectory(saveLocation);
            }

            saveLocation += "/" + gameFileName;

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(saveLocation, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, game);
            stream.Close();
        }

        static string SaveLocation() {
            string saveLocation = Application.streamingAssetsPath;

            if (!Directory.Exists(saveLocation)) {
                Directory.CreateDirectory(saveLocation);
            }

            return saveLocation;
        }
    }

}
