using A;
using A.Map;
using UnityEngine;
using UnityEngine.UI;

public class UITest : MonoBehaviour
{
    [SerializeField] AMapUI ui;
    [SerializeField] Test test;

    [SerializeField] float distance;
    [SerializeField] Image a, b, c;

    [ContextMenu("Create Line")]
    private void Update()
    {
        b.rectTransform.anchoredPosition = AMath.GetPointAtDistance(a.rectTransform.anchoredPosition, c.rectTransform.anchoredPosition, distance);
    }

    //private void Update()
    //{
    //    (transform as RectTransform).anchoredPosition = AMap.instance.TransformPointFromWorldToMap(test.pin.position, ui.ssArea);
    //}
}
