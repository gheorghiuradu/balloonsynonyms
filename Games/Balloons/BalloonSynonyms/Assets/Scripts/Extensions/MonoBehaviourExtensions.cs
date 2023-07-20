using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MonoBehaviourExtensions
{
    public static void WaitThenExecuteCoroutine(this MonoBehaviour monoBehaviour, float seconds, Action action)
    {
        monoBehaviour.StartCoroutine(ReturnWaitThenExecute(seconds, action));
    }

    public static void Wait(this MonoBehaviour monoBehaviour, float seconds)
    {
        ReturnWait(seconds);
    }

    public static GameObject GetClickedObject()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    public static List<GameObject> GetClickedObjects()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray);

        return hits.Select(h => h.collider.gameObject).ToList();
    }

    private static IEnumerator ReturnWaitThenExecute(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action();
    }

    private static IEnumerable ReturnWait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}