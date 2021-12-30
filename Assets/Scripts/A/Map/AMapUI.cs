using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using UnityEngine;

namespace A.Map
{
    public class AMapUI : MonoBehaviour
    {
        public Transform mainTarget;

        [SerializeField] ScopeType scopeType;

        [SerializeField] RectTransform mapArea;
        [SerializeField] RectTransform visibleArea;

        [SerializeField] APinUI pinPrefab;
        [SerializeField] RectTransform pinContainer;

        [SerializeField] float deltaTime = .1f;

        RectTransform targetPin;
        Dictionary<APin, APinUI> mappedPins = new Dictionary<APin, APinUI>();

        AScope scope;

        AMap map;
        Vector2 ssArea;

        Coroutine slowUpdate;
        WaitForSeconds timer;

        enum ScopeType
        {
            Rect,
            Circle
        }

        private void Start()
        {
            map = AMap.instance;
            timer = new WaitForSeconds(deltaTime);
            map.onPinsChanged += OnPinsChanged;
            ssArea = new Vector2(mapArea.rect.width, mapArea.rect.height);
            if (scopeType == ScopeType.Rect)
                scope = new ARectScope(map, ssArea, visibleArea.rect);
            else
                scope = new ACircleScope(map, ssArea, visibleArea.position, visibleArea.rect.width / 2);
        }

        private void OnEnable()
        {
            slowUpdate = StartCoroutine(SlowUpdate());
        }

        private void OnDisable()
        {
            StopCoroutine(slowUpdate);
        }

        void OnPinsChanged(APin pin, AMap.ChangeType change)
        {
            switch (change)
            {
                case AMap.ChangeType.Position:
                    UpdatePinPosition(pin);
                    break;
                case AMap.ChangeType.SpriteAndColor:
                    UpdatePinSpriteAndColor(pin);
                    break;
                case AMap.ChangeType.Visibility:
                    UpdatePinVisibility(pin);
                    break;
                case AMap.ChangeType.Addition:
                    mappedPins.Add(pin, Instantiate(pinPrefab, pinContainer));
                    UpdateAll(pin);
                    break;
                case AMap.ChangeType.Removal:
                    Destroy(mappedPins[pin].gameObject);
                    mappedPins.Remove(pin);
                    break;
            }
        }

        void UpdatePinPosition(APin pin)
        {
            mappedPins[pin].icon.rectTransform.anchoredPosition = map.TransformPointFromWorldToMap(pin.position, ssArea);
        }

        void UpdatePinSpriteAndColor(APin pin)
        {
            mappedPins[pin].icon.sprite = pin.sprite;
            mappedPins[pin].icon.color = pin.color;
        }

        void UpdatePinVisibility(APin pin)
        {
            mappedPins[pin].gameObject.SetActive(pin.visibility != APin.Visibility.Hidden);
        }

        void UpdateAll(APin pin)
        {
            UpdatePinPosition(pin);
            UpdatePinSpriteAndColor(pin);
            UpdatePinVisibility(pin);
        }

        IEnumerator SlowUpdate()
        {
            while (true)
            {
                {
                    foreach (var pin in mappedPins)
                    {
                        if (scope.IsWithinScope(pin.Key))
                        {
                            pin.Value.arrow.gameObject.SetActive(false);
                        }
                        else if(pin.Key.visibility == APin.Visibility.Always)
                        {
                            var intersection = scope.Intersect(pin.Key);
                            pin.Value.icon.rectTransform.anchoredPosition = intersection + new Vector2(pin.Value.icon.rectTransform.sizeDelta.x / 2 * (intersection.x > 0 ? -1 : 1), pin.Value.icon.rectTransform.sizeDelta.y / 2 * (intersection.y > 0 ? -1 : 1));
                            pin.Value.arrow.gameObject.SetActive(true);
                            pin.Value.arrow.transform.eulerAngles = Vector3.forward * Mathf.Atan2(pin.Value.icon.rectTransform.position.x - visibleArea.position.x, pin.Value.icon.rectTransform.position.y - visibleArea.position.x) * Mathf.Rad2Deg;
                        }
                    }
                }
                yield return timer;
            }
        }

    }
}
