using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace A
{
    [AddComponentMenu("A/Standard/Object Spawner")]
    public class AObjectSpawner : MonoBehaviour
    {
        [SerializeField] DistributionType distribution;

        [SerializeField] RotationType rotation;
        [SerializeField] Vector3Int rotationAxis;

        [SerializeField] Vector3 offset = Vector3.up;

        [SerializeField] float radius;
        [SerializeField] float radiusStep;

        [SerializeField] BoxCollider spawnArea;

        [SerializeField] int objectCount;
        [SerializeField] bool forceCount;
        [SerializeField] GameObject prefab;

        List<Vector3> spawnPoints = new List<Vector3>();

        enum DistributionType
        {
            circleOutsideUniform,
            circleOutsideRandom,
            circleInsideUniform,
            circleInsideRandom,
            circleInsideUniRandom,
            rectangleInsideUniform,
            rectangleInsideRandom
        }

        enum RotationType
        {
            none,
            random,
            uniform,
            inwards,
            outwards
        }

        // merge with grid spawner
        // TODO: Add amount limiter and more randomization in circle distribution

        //[MyBox.ButtonMethod]
        [ContextMenu("Start")]
        void Start()
        {
            rotationAxis.Clamp(Vector3Int.zero, Vector3Int.one);
            CleanUp();
            CreatePoints();
            SpawnObjects();
        }

        void CleanUp()
        {
            spawnPoints.Clear();
            var GOs = spawnArea.transform.GetAllChildren();
            for (int i = GOs.Length - 1; i >= 0; i--)
            {
                if (Application.isPlaying)
                    Destroy(GOs[i].gameObject);
                else
                    DestroyImmediate(GOs[i].gameObject);
            }
        }

        void CreatePoints()
        {
            switch (distribution)
            {
                case DistributionType.circleOutsideUniform:
                    CreatePointsInCircleOutsideUniform();
                    break;
                case DistributionType.circleOutsideRandom:
                    CreatePointsInCircleOutsideRandom();
                    break;
                case DistributionType.circleInsideUniform:
                    CreatePointsInCircleInsideUniform();
                    break;
                case DistributionType.circleInsideRandom:
                    CreatePointsInCircleInsideRandom();
                    break;
                case DistributionType.circleInsideUniRandom:
                    CreatePointsInCircleInsideUniRandom();
                    break;
                case DistributionType.rectangleInsideUniform:
                    CreatePointsInRectangleInsideUniform();
                    break;
                case DistributionType.rectangleInsideRandom:
                    CreatePointsInRectangleInsideRandom();
                    break;
                default:
                    break;
            }
        }

        void CreatePointsInRectangleInsideUniform()
        {
            for (int x = 0; x < spawnArea.bounds.size.x; x++)
            {
                for (int z = 0; z < spawnArea.bounds.size.z; z++)
                {
                    var pos = new Vector3(x, 0, z);
                    pos += spawnArea.bounds.center - new Vector3((spawnArea.bounds.size.x / 2), 0, (spawnArea.bounds.size.z / 2)) + new Vector3(.5f, 0, .5f);
                    pos += offset;
                    spawnPoints.Add(pos);
                }
            }
        }

        void CreatePointsInRectangleInsideRandom()
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < objectCount; i++)
            {
            makePoint:
                var pos = spawnArea.bounds.RandomPointInBounds();
                pos.y = 0;
                pos += offset;
                if (timer.ElapsedMilliseconds > 100)
                {
                    Debug.LogError("Too much time elapsed process has been terminated!");
                    return;
                }
                if (spawnPoints.Any((p) => (new Bounds(pos, Vector3.one + Vector3.up)).Intersects(new Bounds(p, Vector3.one + Vector3.up))))
                    goto makePoint;
                spawnPoints.Add(pos);
            }
        }

        void CreatePointsInCircleOutsideUniform()
        {
            spawnPoints = AMath.CreateUniformPointsAroundPivot((int)Mathf.Clamp(objectCount, 0, radius * 6), transform.position, radius, offset).ToList();
        }

        void CreatePointsInCircleOutsideRandom()
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < objectCount; i++)
            {
            makePoint:
                var pos = AMath.CreateRandomPointAroundPivot(transform.position, radius);
                pos += offset;
                if (timer.ElapsedMilliseconds > 100)
                {
                    Debug.LogError("Too much time elapsed process has been terminated!");
                    return;
                }
                if (spawnPoints.Any((p) => (new Bounds(pos, Vector3.one + Vector3.up)).Intersects(new Bounds(p, Vector3.one + Vector3.up))))
                    goto makePoint;
                spawnPoints.Add(pos);
            }
        }

        void CreatePointsInCircleInsideUniform()
        {
            for (float r = 0; r <= radius; r += radiusStep)
            {
                spawnPoints.AddRange(AMath.CreateUniformPointsAroundPivot(Mathf.Clamp(objectCount, 0, (int)r * 6), transform.position, r, offset));
            }
        }

        void CreatePointsInCircleInsideRandom()
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < objectCount; i++)
            {
            makePoint:
                var rndPos = Random.insideUnitCircle * radius;
                var pos = new Vector3(rndPos.x, 0, rndPos.y);
                pos += offset;
                if (timer.ElapsedMilliseconds > 100)
                {
                    Debug.LogError("Too much time elapsed process has been terminated!");
                    return;
                }
                if (spawnPoints.Any((p) => (new Bounds(pos, Vector3.one + Vector3.up)).Intersects(new Bounds(p, Vector3.one + Vector3.up))))
                    goto makePoint;
                spawnPoints.Add(pos);
            }
        }

        void CreatePointsInCircleInsideUniRandom()
        {
            for (float r = 0; r <= radius; r += radiusStep)
            {
                var timer = System.Diagnostics.Stopwatch.StartNew();

                for (int i = 0; i < Mathf.Clamp(objectCount - (forceCount ? ((r - 1) * 6) : 0), 0, r * 6); i++)
                {
                makePoint:
                    var pos = AMath.CreateRandomPointAroundPivot(transform.position, r);
                    pos += offset;
                    if (timer.ElapsedMilliseconds > 100)
                    {
                        Debug.LogError("Too much time elapsed process has been terminated!");
                        break;
                    }
                    if (spawnPoints.Any((p) => (new Bounds(pos, Vector3.one + Vector3.up)).Intersects(new Bounds(p, Vector3.one + Vector3.up))))
                        goto makePoint;
                    spawnPoints.Add(pos);
                }
            }
        }

        void SpawnObjects()
        {
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                var go = Instantiate(prefab, spawnPoints[i], prefab.transform.rotation, spawnArea.transform);
                var rot = Vector3.zero;
                switch (rotation)
                {
                    case RotationType.none:
                        break;
                    case RotationType.random:
                        rot = Random.rotation.eulerAngles;
                        rot.Scale(rotationAxis);
                        go.transform.eulerAngles = rot;
                        break;
                    case RotationType.uniform:
                        rot = Random.rotation.eulerAngles;
                        rot.Scale(rotationAxis);
                        go.transform.eulerAngles = rot;
                        break;
                    case RotationType.inwards:
                        go.transform.LookAt(transform.position);
                        break;
                    case RotationType.outwards:
                        go.transform.LookAt(2 * go.transform.position - transform.position);
                        break;
                    default:
                        break;
                }
            }
        }

        Vector3 InvertVector(Vector3Int vec)
        {
            vec.x = vec.x == 1 ? 0 : 1;
            vec.y = vec.y == 1 ? 0 : 1;
            vec.z = vec.z == 1 ? 0 : 1;
            return vec;
        }
    }
}
