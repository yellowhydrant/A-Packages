using System.Collections.Generic;
using System.Linq;
//using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace A.Map
{
    //TODO:Add&Remove methods
    //TODO:AMapUI & AMapArea
    public class AMap : ASingleton<AMap>
    {
        [SerializeField] Transform cornerA, cornerB;
        public Vector2 wsArea;

        public System.Action<APin, ChangeType> onPinsChanged;

        List<APin> pins = new List<APin>();

        public enum ChangeType
        {
            Position,
            SpriteAndColor,
            Visibility,
            Addition,
            Removal
        }

        //private void OnValidate()
        //{
        //    if(transform.childCount < 2)
        //    {
        //        for (int i = 0; i <= 2 - transform.childCount; i++)
        //        {
        //            var go = new GameObject();
        //            go.transform.parent = transform;
        //        }
        //        cornerA = transform.GetChild(0);
        //        cornerA.name = "A";
        //        cornerB = transform.GetChild(1);
        //        cornerB.name = "B";
        //    }
        //}

        private void Awake()
        {
            wsArea.x = Mathf.Abs(cornerA.position.x - cornerB.position.x);
            wsArea.y = Mathf.Abs(cornerA.position.z - cornerB.position.z);
        }

        public void AddPin(APin pin)
        {
            pins.Add(pin);
            onPinsChanged?.Invoke(pin, ChangeType.Addition);
        }

        public bool RemovePin(APin pin)
        {
            if(pins.Remove(pin))
            {
                onPinsChanged?.Invoke(pin, ChangeType.Removal);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemovePin(string id)
        {
            return RemovePin(GetPin(id));
        }

        public APin GetPin(string id)
        {
            return pins.FirstOrDefault((p) => p.id == id);
        }

        public void SetPinPosition(APin pin, Vector3 position)
        {
            pin.SetPosition(this, position);
        }

        public void SetPinVisibility(APin pin, APin.Visibility visibility)
        {
            pin.SetVisibility(this, visibility);
        }

        public void SetPinSpriteAndColor(APin pin, Sprite sprite, Color color)
        {
            pin.SetSpriteAndColor(this, sprite, color);
        }

        public Vector2 TransformPointFromWorldToMap(Vector3 position, Vector2 ssArea) => new Vector2(AMath.RemapValue(position.x, -wsArea.x / 2, wsArea.x / 2, -ssArea.x / 2, ssArea.x / 2), AMath.RemapValue(position.z, -wsArea.y / 2, wsArea.y / 2, -ssArea.y / 2, ssArea.y / 2));
        public Vector3 TransformPointFromMapToWorld(Vector2 position, Vector2 ssArea) => new Vector3(AMath.RemapValue(position.x, -ssArea.x / 2, ssArea.x / 2, -wsArea.x / 2, wsArea.x / 2), 0f, AMath.RemapValue(position.y, -ssArea.y / 2, ssArea.y / 2, -wsArea.y / 2, wsArea.y / 2));
    }
}
