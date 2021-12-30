using UnityEngine;

namespace A.Map
{
    public class ACircleScope : AScope
    {
        float radius;
        Vector2 center;

        public ACircleScope(AMap map, Vector2 ssArea, Vector2 center, float radius) : base(map, ssArea)
        {
            this.radius = radius;
            this.center = center;
        }

        public override bool IsWithinScope(APin pin)
        {
            return Vector2.Distance(center, map.TransformPointFromWorldToMap(pin.position, ssArea)) < radius;
        }

        public override Vector2 Intersect(APin pin)
        {
            var pos = map.TransformPointFromWorldToMap(pin.position, ssArea);
            return AMath.CircleIntersect(center, pos, radius);
        }
    }
}
