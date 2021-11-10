using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class DebugRectCorners : MonoBehaviour
{
    Transform canvas;

    private void OnDrawGizmos()
    {
        var corners = new Vector3[4];
        (transform as RectTransform).GetLocalCorners(corners);

        for (int i = 0; i < corners.Length; i++)
        {
            if (canvas == null)
                canvas = transform.GetComponentInParent<Canvas>().transform;
            Gizmos.matrix = canvas.localToWorldMatrix;
            //Gizmos.DrawIcon(corners[i], "d_console.erroricon.sml");
            Gizmos.DrawCube(corners[i] + transform.localPosition, Vector3.one * 10);
        }
    }
}
