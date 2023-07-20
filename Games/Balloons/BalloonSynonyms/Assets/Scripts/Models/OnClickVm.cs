using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class OnClickVm : MonoBehaviour
    {
        private void OnMouseDown()
        {
            this.OnMouseDownActions.ForEach(a => a());
        }

        public List<Action> OnMouseDownActions { get; } = new List<Action>();
    }
}