using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace A.UI
{
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    [AddComponentMenu("A/UI/Fit Content")]
    public class AFitContent : MonoBehaviour
    {
        public Vector2 padding;

        [ContextMenu("Dsds")]
        private void Start()
        {
            var children = GetComponentsInChildren<RectTransform>().Where((t) => t != transform).ToArray();
            var size = padding;
            var positionsX = new float[children.Length * 4];
            var positionsY = new float[children.Length * 4];
            for (int i = 0; i < children.Length; i++)
            {
                var corners = new Vector3[4];
                children[i].GetLocalCorners(corners);

                positionsX[0 + i * 4] = corners[0].x + children[i].localPosition.x;
                positionsX[1 + i * 4] = corners[1].x + children[i].localPosition.x;
                positionsX[2 + i * 4] = corners[2].x + children[i].localPosition.x;
                positionsX[3 + i * 4] = corners[3].x + children[i].localPosition.x;

                positionsY[0 + i * 4] = corners[0].y + children[i].localPosition.y;
                positionsY[1 + i * 4] = corners[1].y + children[i].localPosition.y;
                positionsY[2 + i * 4] = corners[2].y + children[i].localPosition.y;
                positionsY[3 + i * 4] = corners[3].y + children[i].localPosition.y;
            }
            var minX = Mathf.Min(positionsX);
            var maxX = Mathf.Max(positionsX);
            var minY = Mathf.Min(positionsY);
            var maxY = Mathf.Max(positionsY);
            Debug.Log($"{nameof(minX)}:{minX}, {nameof(maxX)}:{maxX}, {nameof(minY)}:{minY}, {nameof(maxY)}:{maxY}");

            size.x += Mathf.Abs(minX - maxX);
            size.y += Mathf.Abs(minY - maxY);

            (transform as RectTransform).sizeDelta = size;
            (transform as RectTransform).anchoredPosition = (new Vector2(minX, minY) + new Vector2(maxX, maxY)) / 2;
            for (int i = 0; i < children.Length; i++)
            {
                children[i].anchoredPosition -= (new Vector2(minX, minY) + new Vector2(maxX, maxY)) / 2;
            }
        }
    }
}
