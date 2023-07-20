using Assets.Scripts.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloaterFactory
{
    //private readonly GameObject balloonPrefab = Resources.Load<GameObject>("Prefabs/Level/BalloonPrefab");
    private GameObject GetPrefab(LevelType type)
    {
        return Resources.Load<GameObject>($"Prefabs/Level/{type}Prefab");
    }

    public GameObject SpawnFloater(
        LevelType type, 
        Vector3 position, 
        string color, 
        Transform parent, 
        Vector2 size, 
        string word)
    {
        color = color.ToUpperFirstChar();
        var animationController = Resources.Load<RuntimeAnimatorController>($"Animation/{color}{type}/{color}{type}");
        var sprite = Resources.Load<Sprite>($"Sprites/{color}{type}/1");

        var prefab = this.GetPrefab(type);
        prefab.GetComponent<Animator>().runtimeAnimatorController = animationController;
        prefab.GetComponent<Image>().sprite = sprite;

        var floater = Object.Instantiate(prefab, position, new Quaternion(0, 0, 0, 0), parent);

        floater.name = $"{color}Floater";
        floater.GetComponent<FloaterScript>().Color = color;

        var circleCollider = floater.GetComponent<CircleCollider2D>();
        circleCollider.radius = size.x / 2;

        var rt = floater.GetComponent<RectTransform>();
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);

        var image = floater.GetComponent<Image>();
        image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);

        var wordTmpGui = floater.GetComponentInChildren<TextMeshProUGUI>();
        wordTmpGui.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x / 2);
        wordTmpGui.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y / 2 * 3 /4);
        wordTmpGui.text = word;

        this.ScaleTofitText(floater);

        return floater;
    }

    internal void ScaleTofitText(GameObject floater)
    {
        // Make ballon bigger to fit text on it
        var tmPro = floater.GetComponentInChildren<TextMeshProUGUI>();
        var image = floater.GetComponent<Image>();
        if (tmPro.rectTransform.rect.size.x >= image.rectTransform.rect.size.x / 2)
        {
            var scale = tmPro.transform.localScale;
            scale.x = scale.y = (image.rectTransform.rect.size.x / 2 * scale.x / tmPro.rectTransform.rect.size.x);

            // Detach children before scaling to avoid scaling the children
            var child = floater.transform.GetChild(0);
            floater.transform.DetachChildren();
            floater.transform.localScale = scale;
            // Reattach child;
            child.SetParent(floater.transform);
        }
    }


    public float FloaterRadius(LevelType type)
    {
        return this.GetPrefab(type).GetComponent<CircleCollider2D>().radius;
    }

    public Rect GetRect(LevelType type)
    {
        return this.GetPrefab(type).GetComponent<RectTransform>().rect;
    }
}