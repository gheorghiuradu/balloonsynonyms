using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class ResponsiveService
    {
        public bool ZoomToFit(RectTransform reference)
        {
            if (!reference.IsFullyVisibleFrom(Camera.main))
            {
                Camera.main.orthographicSize++;
                return true;
            }
            return false;
        }

        public void RepositionItems(List<(Vector3 desiredPosition, Transform transform)> objects)
        {
            objects.ForEach(o => o.transform.position = o.desiredPosition);
        }

        public void ResizeBackgroundTofitScreen(Renderer background)
        {
            var scale = new Vector3(1, 1, 1);

            var width = background.bounds.size.x;
            var height = background.bounds.size.y;

            var worldScreenHeight = Camera.main.orthographicSize * 2;
            var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

            scale.x = worldScreenWidth / width + (1.4f * Camera.main.aspect);
            scale.y = worldScreenHeight / height + (2.4f * Camera.main.aspect);

            background.transform.localScale = scale;
        }

        private MinMaxPoints GetMinMaxPoints()
        {
            var maxY = Camera.main.ViewportToWorldPoint(Vector2.up).y - .5f;
            var minY = Camera.main.ViewportToWorldPoint(new Vector2(0, 0)).y + 1;
            var maxX = Camera.main.ViewportToWorldPoint(Vector2.right).x - 1.1f;
            var minX = Camera.main.ViewportToWorldPoint(new Vector2(0, 0)).x + 1;
            return new MinMaxPoints
            {
                MaxX = maxX,
                MaxY = maxY,
                MinX = minX,
                MinY = minY
            };
        }

        public Vector2 BottomRight()
        {
            var points = this.GetMinMaxPoints();
            return new Vector2(points.MaxX, points.MaxY);
        }

        public float Bottom()
        {
            return this.GetMinMaxPoints().MinY;
        }

        public Vector2 BottomLeft()
        {
            var points = this.GetMinMaxPoints();
            return new Vector2(points.MinX, points.MaxX);
        }

        public float Top()
        {
            return this.GetMinMaxPoints().MaxY;
        }

        public Vector2 TopLeft()
        {
            var points = this.GetMinMaxPoints();
            return new Vector2(points.MinX, points.MaxY);
        }

        public Vector2 TopRight()
        {
            var points = this.GetMinMaxPoints();
            return new Vector2(points.MaxX, points.MaxY);
        }

        public float Left()
        {
            return this.GetMinMaxPoints().MinX;
        }

        public float Right()
        {
            return this.GetMinMaxPoints().MaxX;
        }

        public float HorizontalCenter()
        {
            var points = this.GetMinMaxPoints();
            return (points.MaxX + points.MinX) / 2;
        }

        public float VerticalCenter()
        {
            var points = this.GetMinMaxPoints();
            return (points.MaxY + points.MinY) / 2;
        }
    }
}