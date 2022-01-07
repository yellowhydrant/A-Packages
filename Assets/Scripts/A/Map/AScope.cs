using UnityEngine;

namespace A.Map
{
    public abstract class AScope
    {
        public Vector2 ssArea;
        protected RectTransform rectTransform;

        public AScope(RectTransform rect, Vector2 ssArea)
        {
            this.ssArea = ssArea;
            this.rectTransform = rect;
        }

        public abstract bool IsWithinScope(Vector2 pos);
        public abstract Vector2 Intersect(Vector2 pos);
    }
}
