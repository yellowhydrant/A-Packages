using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    public static class AMath
    {
        public static float RemapValue(float value, float from1, float to1, float from2, float to2) => (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        public static float RemapValue(float value, Vector2 from, Vector2 to) => RemapValue(value, from.x, to.x, from.y, to.y);

        public static Vector2 RemapVector(Vector2 value, Vector2 from1, Vector2 to1, Vector2 from2, Vector2 to2)
        {
            value.x = RemapValue(value.x, from1, to1);
            value.y = RemapValue(value.y, from2, to2);
            return value;
        }

        public static Vector3 RemapVector(Vector3 value, Vector2 from1, Vector2 to1, Vector2 from2, Vector2 to2, Vector2 from3, Vector2 to3)
        {
            value.x = RemapValue(value.x, from1, to1);
            value.y = RemapValue(value.y, from2, to2);
            value.z = RemapValue(value.z, from3, to3);
            return value;
        }

        public static Vector2 XY(this Vector3 vec) => new Vector2(vec.x, vec.y);
        public static Vector2 YX(this Vector3 vec) => new Vector2(vec.y, vec.x);
        public static Vector2 YZ(this Vector3 vec) => new Vector2(vec.y, vec.z);
        public static Vector2 ZY(this Vector3 vec) => new Vector2(vec.z, vec.y);
        public static Vector2 ZX(this Vector3 vec) => new Vector2(vec.z, vec.x);
        public static Vector2 XZ(this Vector3 vec) => new Vector2(vec.x, vec.z);

        public static Vector2 XX(this Vector3 vec) => new Vector2(vec.x, vec.x);
        public static Vector2 YY(this Vector3 vec) => new Vector2(vec.y, vec.y);
        public static Vector2 ZZ(this Vector3 vec) => new Vector2(vec.z, vec.z);

        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) => Quaternion.Euler(angles) * (point - pivot) + pivot;

        public static Vector2? LineIntersect(Vector2 L1s, Vector2 L1e, Vector2 L2s, Vector2 L2e)
        {
            var den = (L1s.x - L1e.x) * (L2s.y - L2e.y) - (L1s.y - L1e.y) * (L2s.x - L2e.x);
            if (den == 0) return null;

            var t = ((L1s.x - L2s.x) * (L2s.y - L2e.y) - (L1s.y - L2s.y) * (L2s.x - L2e.x)) / den;
            var u = ((L1s.x - L2s.x) * (L1s.y - L1e.y) - (L1s.y - L2s.y) * (L1s.x - L1e.x)) / den;

            if (t < 1 && t > 0 && u < 1 && u > 0)
                return new Vector2(L1s.x + t * (L1e.x - L1s.x), L1s.y + t * (L1e.y - L1s.y));
            else
                return null;
        }

        public static Vector3 RandomPointInBounds(this Bounds bounds)
        {
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }

        public static GameObject[] GetAllChildren(this Transform transform)
        {
            var GOs = new GameObject[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                GOs[i] = transform.GetChild(i).gameObject;
            }
            return GOs;
        }

        public static Vector3[] CreateUniformPointsAroundPivot(int num, Vector3 pivot, float radius, Vector3 offset = new Vector3())
        {
            var points = new Vector3[num];
            for (int i = 0; i < num; i++)
            {
                var radians = 2 * Mathf.PI / num * i;

                var vertical = Mathf.Sin(radians);
                var horizontal = Mathf.Cos(radians);

                var spawnDir = new Vector3(horizontal, 0, vertical);

                var spawnPos = pivot + spawnDir * radius;

                spawnPos += offset;

                points[i] = spawnPos;
            }
            return points;
        }

        public static Vector3[] CreateRandomPointsAroundPivot(int num, Vector3 pivot, float radius, Vector3 offset = new Vector3())
        {
            var points = new Vector3[num];
            for (int i = 0; i < num; i++)
            {
                var radians = 2 * Mathf.PI / Random.Range(0, num);

                var vertical = Mathf.Sin(radians);
                var horizontal = Mathf.Cos(radians);

                var spawnDir = new Vector3(horizontal, 0, vertical);

                var spawnPos = pivot + spawnDir * radius;

                spawnPos += offset;

                points[i] = spawnPos;
            }
            return points;
        }

        public static Vector3 CreateRandomPointAroundPivot(Vector3 pivot, float radius)
        {
            var radians = 2 * Mathf.PI / Random.Range(0.1f, 360f);

            var vertical = Mathf.Sin(radians);
            var horizontal = Mathf.Cos(radians);

            var spawnDir = new Vector3(horizontal, 0, vertical);

            var spawnPos = pivot + spawnDir * radius;

            return spawnPos;
        }
    }
}
