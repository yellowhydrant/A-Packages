using UnityEngine;

namespace A.Map
{
    public class ACircleScope : AScope
    {
        float radius;
        Vector2 center => rectTransform.rect.center;

        public ACircleScope(RectTransform rect, Vector2 ssArea, float radius) : base(rect, ssArea)
        {
            this.radius = radius;
        }

        public override bool IsWithinScope(Vector2 pos)
        {
            return Vector2.Distance(center, pos) < radius;
        }

        public override Vector2 Intersect(Vector2 pos)
        {
            //var pos = map.TransformPointFromWorldToMap(pin.position, ssArea);
            return AMath.CircleIntersect(center, pos, radius);
        }
    }
}
