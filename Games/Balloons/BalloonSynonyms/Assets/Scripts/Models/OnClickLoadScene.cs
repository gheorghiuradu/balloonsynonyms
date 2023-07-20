using UnityEngine;
using UnityEngine.SceneManagement;

public class OnclickLoadScene : MonoBehaviour
{
    public string SceneName;

    public void OnMouseDown()
    {
        SceneManager.LoadScene(this.SceneName);
    }
}