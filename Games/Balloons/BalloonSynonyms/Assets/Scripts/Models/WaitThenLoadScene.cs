using UnityEngine;
using UnityEngine.SceneManagement;

public class WaitThenLoadScene : MonoBehaviour
{
    private bool firstFrame = true;

    public float SecondsToWait;

    public string SceneName;

    // Start is called before the first frame update
    private void Start()
    {
    }

    private void LateUpdate()
    {
        if (this.firstFrame)
        {
            this.firstFrame = false;
            this.WaitThenExecuteCoroutine(this.SecondsToWait, () => SceneManager.LoadScene(this.SceneName));
        }
    }
}