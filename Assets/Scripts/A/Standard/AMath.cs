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

        public static Vector2 CircleIntersect(Vector2 center, Vector2 point, float radius)
        {
            //phi = atan2(y2 - y1, x2 - x1)
            //x = x1 + r * cos(phi)
            //y = y1 + r * sin(phi)
            var intersection = new Vector2();
            var phi = Mathf.Atan2(point.y - center.y, point.x - center.x);
            intersection.x = center.x + radius * Mathf.Cos(phi);
            intersection.y = center.y + radius * Mathf.Sin(phi);
            return intersection;
        }

        public static Vector2 GetPointAtDistance(Vector2 ls, Vector2 le, float dist)
        {
            var u = (ls - le).normalized;
            return ls - u * dist;
        }

        public static Vector2 Clamp(this Vector2 val, Vector2 min, Vector2 max)
        {
            return new Vector2(Mathf.Clamp(val.x, min.x, max.x), Mathf.Clamp(val.y, min.y, max.y));
        }

        //public struct Quadratic
        //{
        //    public float a, b, c;

        //    public Quadratic(float a, float b, float c)
        //    {
        //        this.a = a;
        //        this.b = b;
        //        this.c = c;
        //    }

        //    public Quadratic(Vector2 p1, Vector2 p2)
        //    {
        //        a = p1.y - p2.y;
        //        b = p2.x - p1.x;
        //        c = (p1.x - p2.x) * p1.y + (p2.y - p1.y) * p1.x;
        //    }

        //    public static Quadratic operator -(Quadratic lhs, Quadratic rhs)
        //    {
        //        lhs.a -= rhs.a;
        //        lhs.b -= rhs.b;
        //        lhs.c -= rhs.c;
        //        return lhs;
        //    }
        //}

        //public static System.ValueTuple<Vector2, Vector2> SolveQuadratic(Quadratic left, Quadratic right)
        //{
        //    var result = new System.ValueTuple<Vector2, Vector2>();
        //    var e = left - right;
        //    var d = Mathf.Pow(e.b, 2) - 4 * e.a * e.c;
        //    var x1 = (-e.b + Mathf.Sqrt(d))/2*e.a;
        //    var x2 = (-e.b - Mathf.Sqrt(d))/2*e.a;
        //    result.Item1 = new Vector2(x1, (e.a * Mathf.Pow(x1, 2)) + (e.b * x1) + (e.c));
        //    result.Item2 = new Vector2(x2, (e.a * Mathf.Pow(x2, 2)) + (e.b * x2) + (e.c));

        //    return result;
        //}
    }
}
