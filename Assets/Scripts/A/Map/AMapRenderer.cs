using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A.Map
{
    public class AMapRenderer : MonoBehaviour
    {
        public RenderTexture mapTex;

        private void Start()
        {
            var mapArea = GetComponent<BoxCollider>();
            var cam = new GameObject("Map Camera").AddComponent<Camera>();
            mapTex = mapTex == null ? mapTex : Resources.Load<RenderTexture>("MapTexture");

            cam.transform.parent = transform;
            cam.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
            cam.transform.localPosition += Vector3.up * 100f;

            cam.orthographic = true;
            cam.clearFlags = CameraClearFlags.Depth;
            cam.targetTexture = mapTex;
            cam.orthographicSize = mapArea.size.x / 2;

            AMiniMap.instance.SetMapTexture(mapTex);
        }
    }
}
