using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private readonly MenuPanelFactory menuFactory = new MenuPanelFactory();

    public GameObject ContinueButton;
    public Slider SoundSlider;
    public System.Action OnContinue { get; set; }

    public void Continue()
    {
        OnContinue();
        //Object.Destroy(this.gameObject);
    }

    public void ShowMainMenuConfirmation()
    {
        var confirmation = this.menuFactory.CreateConfirmatioMenu();
        confirmation.GetComponent<ConfirmGoToMainMenu>().MenuCanvas = this.gameObject;
        this.gameObject.SetActive(false);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene("Level");
    }

    public void SetVolume()
    {
        AudioListener.volume = this.SoundSlider.value;
    }

    private void Start()
    {
        this.SoundSlider.value = AudioListener.volume;
    }

    private void LateUpdate()
    {
        var canContinue = !(this.OnContinue is null);
        this.ContinueButton.SetActive(canContinue);
    }
}