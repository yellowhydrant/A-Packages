using UnityEngine;

namespace A.Map
{
    [System.Serializable]
    public class APin
    {
        [field: Header("Info")]
        [field: SerializeField]
        public string id { get; private set; }
        [field: SerializeField]
        public string name { get; private set; }
        [field: SerializeField]
        public Vector3 position { get; private set; }

        [field: Header("Appearence")]
        [field: SerializeField]
        public Sprite sprite { get; private set; }
        [field: SerializeField]
        public Color color { get; private set; }
        [field: SerializeField]
        public Visibility visibility { get; private set; }

        public enum Visibility
        {
            Always,
            OnlyInScope,
            Hidden
        }

        public APin(string id, string name, Vector3 position, Sprite sprite, Color color, Visibility visibility)
        {
            this.id = id;
            this.name = name;
            this.position = position;
            this.sprite = sprite;
            this.color = color;
        }

        public void SetSpriteAndColor(AMap map, Sprite sprite, Color color)
        {
            this.sprite = sprite;
            this.color = color;
            map.onPinsChanged.Invoke(this, AMap.ChangeType.SpriteAndColor);
        }

        public void SetPosition(AMap map, Vector3 position)
        {
            this.position = position;
            map.onPinsChanged.Invoke(this, AMap.ChangeType.Position);
        }

        public void SetVisibility(AMap map, Visibility visibility)
        {
            this.visibility = visibility;
            map.onPinsChanged.Invoke(this, AMap.ChangeType.Visibility);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
    }
}
