using UnityEngine;

namespace A.Map
{
    public class ARectScope : AScope
    {
        System.ValueTuple<Vector2, Vector2>[] borders = new System.ValueTuple<Vector2, Vector2>[4];
        Rect rect => rectTransform.rect;

        public ARectScope(RectTransform rectTransform, Vector2 ssArea) : base(rectTransform, ssArea)
        {
            borders[0] = new System.ValueTuple<Vector2, Vector2>(new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMax, rect.yMin));
            borders[1] = new System.ValueTuple<Vector2, Vector2>(new Vector2(rect.xMax, rect.yMin), new Vector2(rect.xMax, rect.yMax));
            borders[2] = new System.ValueTuple<Vector2, Vector2>(new Vector2(rect.xMax, rect.yMax), new Vector2(rect.xMin, rect.yMax));
            borders[3] = new System.ValueTuple<Vector2, Vector2>(new Vector2(rect.xMin, rect.yMax), new Vector2(rect.xMin, rect.yMin));
        }

        public override bool IsWithinScope(Vector2 pos)
        {
            return pos.x <= rect.xMax && pos.x >= rect.xMin && pos.y <= rect.yMax && pos.y >= rect.yMin;
        }

        public override Vector2 Intersect(Vector2 pos)
        {
            //var pos = map.TransformPointFromWorldToMap(pin.position, ssArea);
            for (int x = 0; x < borders.Length; x++)
            {
                var intersection = AMath.LineIntersect(rect.center, pos, borders[x].Item1, borders[x].Item2);
                if (intersection == null)
                    continue;
                else
                    return (Vector2)intersection;
            }
            return default;
        }
    }
}
