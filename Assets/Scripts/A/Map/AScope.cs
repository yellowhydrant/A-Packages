using UnityEngine;

namespace A.Map
{
    public abstract class AScope
    {
        public AMap map;
        public Vector2 ssArea;

        public AScope(AMap map, Vector2 ssArea)
        {
            this.map = map;
            this.ssArea = ssArea;
        }

        public abstract bool IsWithinScope(APin pin);
        public abstract Vector2 Intersect(APin pin);
    }
}
