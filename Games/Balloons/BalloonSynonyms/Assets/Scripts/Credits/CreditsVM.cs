using Assets.Scripts.Services;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Credits
{
    public class CreditsVM : MonoBehaviour
    {
        private const string LevelCompleteText = "Nivelul {level} complet";
        private const string CurrentScoreText = "{score} <sprite=\"Hud/hud-elements\" index=0>";
        private const string HighScoreText = "{score} <sprite=\"Icons/trophy\" index=0>";

        private int level = 1;

        public AudioSource Audio;
        public Slider LevelCompleteSlider;
        public TextMeshProUGUI LevelCompleteTMPro;
        public TextMeshProUGUI CurrentScore;
        public GameObject Star;
        public TextMeshProUGUI HighScore;
        public GameObject CreditsPanel;
        public string StarAudioPath;
        public string HighScoreAudioPath;

        private void Start()
        {
            for (int i = 0; i < 10; i++)
            {
                var time = (i * 2.8f) / 10;
                this.WaitThenExecuteCoroutine(time, () =>
                {
                    this.LevelCompleteSlider.value++;
                    this.LevelCompleteTMPro.text = LevelCompleteText.Replace("{level}", level++.ToString());
                });
            }
            this.Audio.PlayDelayed(1.1f);

            var service = new SaveLoadService();
            var currentScore = service.GetLastScore(SceneHelper.GameModeSettings.GameMode);
            var highScore = service.GetHighScore(SceneHelper.GameModeSettings.GameMode);
            this.WaitThenExecuteCoroutine(3, () =>
            {
                this.CurrentScore.text = CurrentScoreText.Replace("{score}", currentScore.ToString());
                this.CurrentScore.gameObject.SetActive(true);
                this.Star.SetActive(false);
                if (currentScore >= highScore)
                {
                    highScore = currentScore;
                    this.WaitThenExecuteCoroutine(1, () =>
                    {
                        var clip = Resources.Load<AudioClip>(this.StarAudioPath);
                        this.Audio.PlayOneShot(clip);
                        this.Star.SetActive(true);
                        service.SaveHighScore(SceneHelper.GameModeSettings.GameMode, currentScore);
                    });
                }
            });

            this.WaitThenExecuteCoroutine(5, () =>
            {
                this.HighScore.text = HighScoreText.Replace("{score}", highScore.ToString());
                var clip = Resources.Load<AudioClip>(this.HighScoreAudioPath);
                this.Audio.PlayOneShot(clip);
                this.HighScore.gameObject.SetActive(true);
            });
        }

        public void OpenUrl(string url)
        {
            Application.OpenURL(url);
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void OpenCredits()
        {
            this.CreditsPanel.SetActive(true);
        }

        public void CloseCredits()
        {
            this.CreditsPanel.SetActive(false);
        }
    }
}