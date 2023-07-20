using Assets.Scripts.Extensions;
using Assets.Scripts.Level;
using Assets.Scripts.Models;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

public class FloaterScript : Animated, IPointerDownHandler
{
    private bool isRising;
    private float invisibleTopPoint;
    public Vector3 InitialPosition { get; set; }

    public bool GameIsPaused { get; set; }

    public event Action Popped;

    public event Action FailedToPop;

    // Update is called once per frame
    private void LateUpdate()
    {
        if (this.isRising)
        {
            var position = this.transform.position;
            this.transform.GetChildren().ForEach(c => c.position = new Vector3(position.x, position.y, c.position.z));
        }

        if (this.isRising && this.transform.position.y > invisibleTopPoint)
        {
            this.OnBecameInvisible();
        }
    }

    private void OnBecameInvisible()
    {
        this.GetComponent<Rigidbody2D>().position = this.InitialPosition;
    }

    public void OnPointerDown(PointerEventData e)
    {
        //var objectsClicked = MonoBehaviourExtensions.GetClickedObjects();
        if (this.GameIsPaused //||
                              //objectsClicked.Any(o => o.gameObject.name == "NextWord"
                              //|| o.gameObject.name == "PrevWord")
            )
        {
            return;
        }

        var clickedWord = this.GetComponentInChildren<TextMeshProUGUI>().text;
        var manager = this.GetComponentInParent<LevelVM>();
        var audioSource = this.GetComponent<AudioSource>();

        if (manager.IsSynonym(clickedWord))
        {
            this.PopAndContinueWith(() => Object.Destroy(this.gameObject));
            this.Popped?.Invoke();
            manager.SetNextWord();
        }
        else
        {
            var failClip = Resources.Load<AudioClip>("Audio/CasualGameSounds/DM-CGS-07");
            audioSource.PlayOneShot(failClip);
            this.FailedToPop?.Invoke();
        }
    }

    public void StartRising(float riseSpeed, float invisibleTopPoint)
    {
        this.GetComponent<Rigidbody2D>().velocity = Vector2.up * (riseSpeed * 50);
        this.isRising = true;

        this.InitialPosition = this.transform.position;
        this.invisibleTopPoint = invisibleTopPoint;
        this.SetTextColor();
        //this.ScaleTofitText();
    }
}