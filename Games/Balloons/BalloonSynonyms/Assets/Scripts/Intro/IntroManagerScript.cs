using Assets.Scripts.Serialization;
using Assets.Scripts.Services;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Intro
{
    public class IntroManagerScript : MonoBehaviour
    {
        private bool toggled;
        private string instructions;
        private string explanation;
        private int loadedLevel;

        public GameObject Title;
        public GameObject Instructions;
        public GameObject ResumeGameButton;
        public GameObject ToggleButton;

        private void Start()
        {
            SceneHelper.IntroSettingsLoaded += this.SetupIntro;

            this.StartCoroutine(SceneHelper.LoadIntroConfig());
        }

        private void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                this.GoBack();
            }
        }

        private void OnGameModeLoaded()
        {
            SceneManager.LoadScene("Level");
        }

        private void OnDestroy()
        {
            SceneHelper.IntroSettingsLoaded -= this.SetupIntro;
            SceneHelper.GameModeSettingsLoaded -= this.OnGameModeLoaded;
        }

        private void SetupIntro(IntroSettings settings)
        {
            this.instructions = settings.Instructions;
            this.explanation = settings.Explanation;
            this.loadedLevel = new SaveLoadService().GetLastLevel(settings.GameMode);
            SceneHelper.DictionaryFileName = settings.DictionaryFileName;
            SceneHelper.GameModeFileName = settings.ConfigFileName;
            SceneHelper.GameModeSettingsLoaded += this.OnGameModeLoaded;

            this.Title.GetComponent<TextMeshProUGUI>().text = $"{settings.Name} - Instrucțiuni";
            this.Instructions.GetComponent<TextMeshProUGUI>().text = settings.Instructions;

            if (this.loadedLevel > 0)
            {
                this.ResumeGameButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Continuă de la nivelul {this.loadedLevel}";
                this.ResumeGameButton.SetActive(true);
            }
        }

        public void GoBack()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void StartGame()
        {
            SceneHelper.CurrentLevel = 1;
            this.StartCoroutine(SceneHelper.LoadLevelConfigs());
        }

        public void ResumeGame()
        {
            SceneHelper.CurrentLevel = this.loadedLevel;
            this.StartCoroutine(SceneHelper.LoadLevelConfigs());
        }

        public void ToggleExplanation()
        {
            switch (this.toggled)
            {
                case false:
                    this.Instructions.GetComponent<TextMeshProUGUI>().text = this.explanation;
                    this.toggled = true;
                    break;

                case true:
                    this.Instructions.GetComponent<TextMeshProUGUI>().text = this.instructions;
                    this.toggled = false;
                    break;
            }
        }
    }
}