using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class TransformExtensions
    {
        public static List<Transform> GetChildren(this Transform transform)
        {
            var children = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                children.Add(transform.GetChild(i));
            }

            return children;
        }

        public static T GetComponentInChildWithTag<T>(this GameObject parent, string tag) where T : Component
        {
            foreach (Transform tr in parent.transform)
            {
                if (tr.tag == tag)
                {
                    return tr.GetComponent<T>();
                }
            }
            return null;
        }
    }
}