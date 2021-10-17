using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace A.Minimap
{
    public class AMiniMap : Singleton<AMiniMap>
    {
        public Transform target;

        [SerializeField] BoxCollider worldSpaceArea;
        [SerializeField] RectTransform screenSpaceArea;

        [SerializeField] RectTransform visibleArea;
        System.ValueTuple<Vector2, Vector2>[] visibleAreaBorders = new System.ValueTuple<Vector2, Vector2>[4];

        [SerializeField] Color defaultMapColor;
        [SerializeField] RawImage mapRawImage;

        [SerializeField] RectTransform targetPin;
        [SerializeField] RectTransform mapPinContainer;

        [SerializeField] Image pinPrefab;
        [SerializeField] MapPin.PinStyle defaultPinStyle;

        ObservableCollection<MapPin> pins { get { UpdatePinPositions(); return mapPins_; } set => mapPins_ = value; }
        ObservableCollection<MapPin> mapPins_ = new ObservableCollection<MapPin>();

        WaitForSeconds updateDelay = new WaitForSeconds(0.1f);

        public class MapPin
        {
            public string id;
            public string name;
            public Vector3 worldSpacePosition;

            public Image pinImage;
            public Image arrowImage;

            public static PinStyle defaultStyle;
            public PinStyle style;

            [System.Serializable]
            public class PinStyle
            {
                public Color color;
                public Sprite pinSprite;
                public Sprite arrowSprite;
                public Vector2 size;

                public void Override(PinStyle overrideStyle)
                {
                    color = overrideStyle.color == default(Color) ? color : overrideStyle.color;
                    pinSprite = overrideStyle.pinSprite == null ? pinSprite : overrideStyle.pinSprite;
                    arrowSprite = overrideStyle.arrowSprite == null ? arrowSprite : overrideStyle.arrowSprite;
                    size = overrideStyle.size == default(Vector2) ? size : overrideStyle.size;
                }
            }

            public MapPin(string id, string name, Vector3 wsPosition, PinStyle overrideStyle = null)
            {
                this.id = id;
                this.name = name;
                worldSpacePosition = wsPosition;

                style = defaultStyle;
                if (overrideStyle != null)
                    style.Override(overrideStyle);
            }

            public void SetImage(Image image)
            {
                pinImage = image;
                arrowImage = image.transform.GetChild(0).GetComponent<Image>();

                ApplyStyle();
            }

            public void ApplyStyle()
            {
                if (pinImage == null) return;
                pinImage.color = style.color;
                pinImage.sprite = style.pinSprite;
                arrowImage.sprite = style.arrowSprite;
                pinImage.rectTransform.sizeDelta = style.size;
            }
        }

        void Awake()
        {
            pins.CollectionChanged += OnMapPinsChanged;
            MapPin.defaultStyle = defaultPinStyle;
            visibleAreaBorders[0] = new System.ValueTuple<Vector2, Vector2>(new Vector2(visibleArea.rect.xMin, visibleArea.rect.yMin), new Vector2(visibleArea.rect.xMax, visibleArea.rect.yMin));
            visibleAreaBorders[1] = new System.ValueTuple<Vector2, Vector2>(new Vector2(visibleArea.rect.xMax, visibleArea.rect.yMin), new Vector2(visibleArea.rect.xMax, visibleArea.rect.yMax));
            visibleAreaBorders[2] = new System.ValueTuple<Vector2, Vector2>(new Vector2(visibleArea.rect.xMax, visibleArea.rect.yMax), new Vector2(visibleArea.rect.xMin, visibleArea.rect.yMax));
            visibleAreaBorders[3] = new System.ValueTuple<Vector2, Vector2>(new Vector2(visibleArea.rect.xMin, visibleArea.rect.yMax), new Vector2(visibleArea.rect.xMin, visibleArea.rect.yMin));

            StartCoroutine(SlowUpdate());
        }

        void LateUpdate()
        {
            if (target == null)
            {
                targetPin.gameObject.SetActive(false);
                return;
            }
            targetPin.gameObject.SetActive(true);
            screenSpaceArea.anchoredPosition = TransformPointFromWorldToMap(target.position) * -screenSpaceArea.localScale;
            targetPin.localEulerAngles = new Vector3(0, 0, -target.localEulerAngles.y);
        }

        public void AddPin(MapPin pin)
        {
            if(pin != null)
                pins.Add(pin);
            UpdatePinPositions();
        }

        public bool Remove(string id)
        {
            var pin = pins.First((pin) => pin.id == id);
            return pins.Remove(pin);
        }

        public bool Remove(MapPin pin)
        {
            return pins.Remove(pin);
        }

        public MapPin GetPin(string id)
        {
            return pins.First((pin) => pin.id == id);
        }

        public void SetMapSprite(Texture tex)
        {
            mapRawImage.texture = tex;
            if (tex != null)
                mapRawImage.color = Color.white;
            else
                mapRawImage.color = defaultMapColor;
        }

        void OnMapPinsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var pin in e.NewItems.Cast<MapPin>())
                {
                    var image = Instantiate(pinPrefab, mapPinContainer);
                    pin.SetImage(image);
                    image.rectTransform.anchoredPosition = TransformPointFromWorldToMap(pin.worldSpacePosition);
                }
            }

            if (e.OldItems != null)
            {
                foreach (var pin in e.OldItems.Cast<MapPin>())
                {
                    Destroy(pin.pinImage.gameObject);
                }
            }
        }

        void UpdatePinPositions()
        {
            for (int i = 0; i < mapPins_.Count; i++)
            {
                var pin = mapPins_[i];
                pin.pinImage.rectTransform.anchoredPosition = TransformPointFromWorldToMap(pin.worldSpacePosition);
                pin.arrowImage.gameObject.SetActive(false);

                var isVisible = RectTransformUtility.RectangleContainsScreenPoint(visibleArea, pin.pinImage.rectTransform.position);
                if (!isVisible)
                {
                    for (int x = 0; x < visibleAreaBorders.Length; x++)
                    {
                        var intersection = AMath.LineIntersect(visibleArea.rect.center, pin.pinImage.rectTransform.anchoredPosition, visibleAreaBorders[x].Item1, visibleAreaBorders[x].Item2);
                        if(intersection == null)
                        {
                            continue;
                        }
                        else
                        {
                            var point = (Vector2)intersection;
                            pin.pinImage.rectTransform.anchoredPosition = point + new Vector2(pin.pinImage.rectTransform.sizeDelta.x / 2 * (point.x > 0 ? -1 : 1), pin.pinImage.rectTransform.sizeDelta.y / 2 * (point.y > 0 ? -1 : 1));
                            pin.arrowImage.gameObject.SetActive(true);
                            pin.arrowImage.transform.eulerAngles = Vector3.forward * Mathf.Atan2(pin.pinImage.rectTransform.position.x - visibleArea.position.x, pin.pinImage.rectTransform.position.y - visibleArea.position.y) * Mathf.Rad2Deg;
                            break;
                        }
                    }
                }
            }
        }

        IEnumerator SlowUpdate()
        {
            while (true)
            {
                UpdatePinPositions();
                yield return updateDelay;
            }
        }

        public Vector2 TransformPointFromWorldToMap(Vector3 position) => new Vector2(AMath.RemapValue(position.x, -worldSpaceArea.size.x / 2, worldSpaceArea.size.x / 2, -screenSpaceArea.rect.width/2, screenSpaceArea.rect.width/2), AMath.RemapValue(position.z, -worldSpaceArea.size.z / 2, worldSpaceArea.size.z / 2, -screenSpaceArea.rect.height / 2, screenSpaceArea.rect.height / 2));
        public Vector3 TransformPointFromMapToWorld(Vector2 position) => new Vector3(AMath.RemapValue(position.x, -screenSpaceArea.rect.width / 2, screenSpaceArea.rect.width / 2, -worldSpaceArea.size.x / 2, worldSpaceArea.size.x / 2), 0f, AMath.RemapValue(position.y, -screenSpaceArea.rect.height / 2, screenSpaceArea.rect.height / 2, -worldSpaceArea.size.z / 2, worldSpaceArea.size.z / 2));
    }
}
