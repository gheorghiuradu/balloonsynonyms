using System;
using UnityEngine;

public class MenuButtonScript : MonoBehaviour
{
    public event Action MouseDown;

    private void OnMouseDown()
    {
        this.MouseDown?.Invoke();
    }
}