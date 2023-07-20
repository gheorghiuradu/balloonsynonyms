using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Level
{
    public class WinCanvasVM : MonoBehaviour
    {
        public bool IsLastLevel { get; set; }
        public TextMeshProUGUI Correct;
        public TextMeshProUGUI Mistakes;
        public TextMeshProUGUI Score;
        public AudioSource Audio;

        public void Show(int correct, int mistakes, int score)
        {
            this.Audio.Play();
            this.Correct.text = this.Correct.text.Replace("correct", correct.ToString());
            this.Mistakes.text = this.Mistakes.text.Replace("mistakes", mistakes.ToString());
            this.Score.text = this.Score.text.Replace("score", score.ToString());
            this.gameObject.SetActive(true);
        }

        public void Continue()
        {
            if (SceneHelper.CurrentLevel == 2
                && Application.platform == RuntimePlatform.WebGLPlayer)
            {
                SceneManager.LoadScene("DemoInfo");
            }
            else
            {
                if (this.IsLastLevel)
                {
                    SceneManager.LoadScene("Credits");
                }
                else
                {
                    SceneHelper.CurrentLevel++;
                    Time.timeScale = 1;
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
        }

        public void MainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}