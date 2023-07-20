using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfirmGoToMainMenu : MonoBehaviour
{
    public GameObject MenuCanvas { get; set; }

    public void No()
    {
        this.MenuCanvas.SetActive(true);
        Object.Destroy(this.gameObject);
    }

    public void Yes()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}