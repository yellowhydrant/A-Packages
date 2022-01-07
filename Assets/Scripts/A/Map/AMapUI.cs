using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using UnityEngine;

namespace A.Map
{
    //Assumption: all pins have the same size, are square and the ui elements don't change size at runtime
    //TODO: Constrain/Clamp visibleArea so it doesnt go ourside the corners of the full map
    public class AMapUI : MonoBehaviour
    {
        public Transform mainTarget;

        [SerializeField] ScopeType scopeType;

        [SerializeField] RectTransform mapArea;
        [SerializeField] RectTransform visibleArea;

        [SerializeField] APinUI pinPrefab;
        [SerializeField] RectTransform pinContainer;

        [SerializeField] float deltaTime = .1f;

        [SerializeField]RectTransform targetPin;
        Dictionary<APin, APinUI> mappedPins = new Dictionary<APin, APinUI>();

        AScope scope;

        AMap map;
        Vector2 ssArea;
        Vector2 offset;

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
            {
                scope = new ARectScope(visibleArea, ssArea);
                offset = (pinPrefab.icon.rectTransform.sizeDelta * .5f) + pinPrefab.arrow.rectTransform.sizeDelta;
            }
            else
            {
                scope = new ACircleScope(visibleArea, visibleArea.anchoredPosition, visibleArea.rect.width / 2);
                offset = (pinPrefab.icon.rectTransform.sizeDelta / 2) + pinPrefab.arrow.rectTransform.sizeDelta;
            }

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
            if (mappedPins.ContainsKey(pin))
            {
                switch (change)
                {
                    case AMap.ChangeType.Position:
                        UpdatePinPosition(pin);
                        return;
                    case AMap.ChangeType.SpriteAndColor:
                        UpdatePinSpriteAndColor(pin);
                        return;
                    case AMap.ChangeType.Visibility:
                        UpdatePinVisibility(pin);
                        return;
                    case AMap.ChangeType.Removal:
                        Destroy(mappedPins[pin].gameObject);
                        mappedPins.Remove(pin);
                        return;
                }
            }
            else if (change == AMap.ChangeType.Addition)
            {
                mappedPins.Add(pin, Instantiate(pinPrefab, pinContainer));
                UpdateAll(pin);
            }
            else
            {
                Debug.LogError("Error: This pin hasn't been mapped yet!");
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

        private void LateUpdate()
        {
            if(mainTarget == null)
            {
                targetPin.gameObject.SetActive(false);
                return;
            }
            targetPin.gameObject.SetActive(true);
            mapArea.anchoredPosition = -map.TransformPointFromWorldToMap(mainTarget.position, ssArea);
            //mapArea.anchoredPosition = -visibleArea.anchoredPosition;
            targetPin.localEulerAngles = new Vector3(0, 0, -mainTarget.localEulerAngles.y);
        }

        IEnumerator SlowUpdate()
        {
            while (true)
            {
                {
                    foreach (var pin in mappedPins)
                    {
                        UpdatePinPosition(pin.Key);
                        if (scope.IsWithinScope(pin.Value.position))
                        {
                            pin.Value.arrow.gameObject.SetActive(false);
                        }
                        else if(pin.Key.visibility == APin.Visibility.Always)
                        {
                            var intersection = scope.Intersect(pin.Value.position);
                            if (scopeType == ScopeType.Rect)
                                pin.Value.rect.anchoredPosition = AMath.Clamp(intersection, visibleArea.rect.min + offset, visibleArea.rect.max - offset);
                            else
                                pin.Value.rect.anchoredPosition = AMath.GetPointAtDistance(visibleArea.rect.center, intersection, (visibleArea.rect.width / 2) - ((offset.x + offset.y) / 2));
                            pin.Value.rect.anchoredPosition -= mapArea.anchoredPosition;
                            pin.Value.arrow.gameObject.SetActive(true);
                            var pos = pin.Value.icon.rectTransform.position;
                            pin.Value.arrow.transform.eulerAngles = Vector3.forward * (Mathf.Atan2(visibleArea.position.x - pos.x, pos.y - visibleArea.position.y) * Mathf.Rad2Deg + pinPrefab.angleOffset);
                        }
                    }
                }
                yield return timer;
            }
        }

    }
}
