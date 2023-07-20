using TMPro;
using UnityEngine;

public class CurrentWordScript : MonoBehaviour
{
    public void FailLevel()
    {
        this.GetComponent<TextMeshProUGUI>().text = "Nivel pierdut";
        var failClip = Resources.Load<AudioClip>("Audio/fail");
        this.GetComponent<AudioSource>().PlayOneShot(failClip);
        this.FloatUp();
    }

    private void FloatUp()
    {
        for (int i = 0; i < 200; i++)
        {
            this.WaitThenExecuteCoroutine(i / 80, () =>
                this.transform.Translate(Vector3.up * Time.deltaTime, Space.World));
        }
    }
}