using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.MainMenu
{
    public class MainMenuVM : MonoBehaviour
    {
        public GameObject MainPanel;
        public GameObject RoPanel;
        public GameObject ForeignPanel;

        private void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (this.MainPanel.activeInHierarchy)
                {
                    this.Exit();
                }
                else
                {
                    this.GoBack();
                }
            }
        }

        public void SelectRo()
        {
            this.MainPanel.SetActive(false);
            this.RoPanel.SetActive(true);
        }

        public void SelectForegin()
        {
            this.MainPanel.SetActive(false);
            this.ForeignPanel.SetActive(true);
        }

        public void SelectStore()
        {
            SceneManager.LoadScene("Store");
        }

        public void Exit()
        {
            Application.Quit();
        }

        public void GoBack()
        {
            this.MainPanel.SetActive(true);
            this.RoPanel.SetActive(false);
            this.ForeignPanel.SetActive(false);
        }

        public void LoadIntro(string config)
        {
            SceneHelper.IntroConfigFileName = config;
            SceneManager.LoadScene("Intro");
        }
    }
}
