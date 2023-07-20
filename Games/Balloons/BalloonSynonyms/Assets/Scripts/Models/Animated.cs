using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Models
{
    public abstract class Animated : MonoBehaviour
    {
        public string Color { get; set; }

        internal void PopAndContinueWith(Action action)
        {
            this.GetComponent<CircleCollider2D>().enabled = false;
            var animator = this.GetComponent<Animator>();
            var popAnimation = animator.runtimeAnimatorController.animationClips.FirstOrDefault(a => a.name.Contains("Pop"));
            animator.Play(popAnimation.name);
            this.GetComponent<AudioSource>().Play();

            this.WaitThenExecuteCoroutine(popAnimation.length, action);
        }

        internal void SetTextColor()
        {
            if (this.Color.StartsWith("blue", StringComparison.OrdinalIgnoreCase)
            || this.Color.StartsWith("yellow", StringComparison.OrdinalIgnoreCase)
            || this.Color.StartsWith("green", StringComparison.OrdinalIgnoreCase)
            || this.Color.StartsWith("mixed", StringComparison.OrdinalIgnoreCase)
            || this.Color.StartsWith("white", StringComparison.OrdinalIgnoreCase)
            || this.Color.StartsWith("mustard", StringComparison.OrdinalIgnoreCase)
                )
            {
                this.GetComponentInChildren<TextMeshProUGUI>().color = UnityEngine.Color.black;
                this.GetComponentInChildren<TextMeshProUGUI>().outlineColor = UnityEngine.Color.white;
                this.GetComponentInChildren<TextMeshProUGUI>().outlineWidth = 200;
            }
        }

        internal void ScaleTofitText2()
        {
            // Make ballon bigger to fit text on it
            var tmPro = this.GetComponentInChildren<TextMeshProUGUI>();
            var image = this.GetComponent<Image>();
            if (tmPro.textBounds.size.x > image.sprite.bounds.size.x)
            {
                var scale = this.transform.localScale;
                scale.x = (tmPro.textBounds.size.x * scale.x / image.sprite.bounds.size.x) + .05f;

                // Detach children before scaling to avoid scaling the children
                var child = this.transform.GetChild(0);
                this.transform.DetachChildren();
                this.transform.localScale = scale;
                // Reattach child;
                child.SetParent(this.transform);
            }
        }
    }
}