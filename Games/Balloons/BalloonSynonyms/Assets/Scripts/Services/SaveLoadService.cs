using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Services
{
    public class SaveLoadService
    {
        private const string LastLevel = nameof(LastLevel);
        private const string LastScore = nameof(LastScore);
        private const string HighScore = nameof(HighScore);

        public void SaveLevelAndScore(string gameMode, int level, int score)
        {
            PlayerPrefs.SetInt($"{gameMode}.{LastLevel}", level);
            PlayerPrefs.SetInt($"{gameMode}.{LastScore}", score);
            PlayerPrefs.Save();
        }

        public void LoadLevel(string scene, string gameMode)
        {
            var lastLevel = PlayerPrefs.GetInt($"{gameMode}.{LastLevel}");
            SceneHelper.CurrentLevel = lastLevel;

            SceneManager.LoadScene(scene);
        }

        public int GetLastScore(string gameMode)
        {
            return PlayerPrefs.GetInt($"{gameMode}.{LastScore}");
        }

        public int GetLastLevel(string gameMode)
        {
            return PlayerPrefs.GetInt($"{gameMode}.{LastLevel}");
        }

        public int GetHighScore(string gameMode)
        {
            return PlayerPrefs.GetInt($"{gameMode}.{HighScore}");
        }

        public void SaveHighScore(string gameMode, int score)
        {
            PlayerPrefs.SetInt($"{gameMode}.{HighScore}", score);
        }
    }
}