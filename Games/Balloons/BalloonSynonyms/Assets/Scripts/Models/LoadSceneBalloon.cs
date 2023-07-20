using Assets.Scripts.Models;
using UnityEngine.SceneManagement;

public class LoadSceneBalloon : Animated
{
    public string SceneName;
    public string ConfigFile;
    public bool FitText;

    private void Start()
    {
        this.SetTextColor();
    }

    private void LateUpdate()
    {
        if (this.FitText)
        {
            //this.ScaleTofitText();
            this.transform.GetChild(0).position = this.transform.position;
        }
    }

    private void OnMouseDown()
    {
        this.PopAndContinueWith(() =>
        {
            SceneHelper.IntroConfigFileName = this.ConfigFile;
            SceneManager.LoadScene(this.SceneName);
        });
    }
}