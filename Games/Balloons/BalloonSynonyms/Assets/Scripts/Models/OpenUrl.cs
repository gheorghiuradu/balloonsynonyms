using UnityEngine;

public class OpenUrl : MonoBehaviour
{
    public string Url;

    private void OnMouseDown()
    {
        Application.OpenURL(this.Url);
    }
}