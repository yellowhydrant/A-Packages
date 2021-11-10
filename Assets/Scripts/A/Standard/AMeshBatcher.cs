using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using A.Extensions;

namespace A
{
    [AddComponentMenu("A/Standard/Combine Children")]
    public class AMeshBatcher : MonoBehaviour
    {
        private void Start()
        {
            foreach (Transform child in transform)
                child.position += transform.position;

            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;

            var meshFilters = GetComponentsInChildren<MeshFilter>();

            var meshPairs = new Dictionary<Material, List<Tuple<Transform, Mesh>>>();

            for (int i = 0; i < meshFilters.Length; i++)
            {
                var renderer = meshFilters[i].GetComponent<MeshRenderer>();
                if (renderer == null || !renderer.enabled) continue;

                for (int m = 0; m < meshFilters[i].sharedMesh.subMeshCount; m++)
                {
                    if (!meshPairs.ContainsKey(renderer.sharedMaterials[m]))
                        meshPairs.Add(renderer.sharedMaterials[m], new List<Tuple<Transform, Mesh>>());
                    meshPairs[renderer.sharedMaterials[m]].Add(new Tuple<Transform, Mesh>(meshFilters[i].transform, meshFilters[i].sharedMesh.GetSubmesh(m)));
                    renderer.enabled = false;
                }
            }

            List<CombineInstance> combine = new List<CombineInstance>();

            foreach (var pair in meshPairs)
            {
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    var combineInstance = new CombineInstance();

                    combineInstance.mesh = pair.Value[i].Item2;
                    combineInstance.transform = pair.Value[i].Item1.localToWorldMatrix;

                    combine.Add(combineInstance);
                }

                var mesh = new Mesh();
                try
                {
                    mesh.CombineMeshes(combine.ToArray());
                }
                catch (System.ArgumentException)
                {
                    mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                    mesh.CombineMeshes(combine.ToArray());
                }

                var go = new GameObject();
                go.transform.parent = transform;
                go.AddComponent<MeshFilter>().mesh = mesh;
                go.AddComponent<MeshRenderer>().sharedMaterial = pair.Key;

                Debug.LogWarning("Saved by batching: " + combine.Count);

                combine.Clear();
            }
        }
    }
}

