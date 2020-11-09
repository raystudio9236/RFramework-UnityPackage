using UnityEngine;

namespace RFramework.Common.Extension
{
    public static class MathExtension
    {
        public static Vector2 ToVector2(this int i)
        {
            return new Vector2(i, i);
        }

        public static Vector3 ToVector3(this int i)
        {
            return new Vector3(i, i, i);
        }

        public static Vector2 ToVector2(this float f)
        {
            return new Vector2(f, f);
        }

        public static Vector3 ToVector3(this float f)
        {
            return new Vector3(f, f, f);
        }

        public static Vector3 ToVector3(this Vector2 v)
        {
            return new Vector3(v.x, v.y, 0);
        }

        public static Vector3[] ToVector3(this Vector2[] v)
        {
            var ret = new Vector3[v.Length];
            for (var i = 0; i < v.Length; i++)
            {
                ret[i] = new Vector3(v[i].x, v[i].y);
            }

            return ret;
        }

        public static Vector2 ToVector2(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector2[] ToVector2(this Vector3[] v)
        {
            var ret = new Vector2[v.Length];
            for (var i = 0; i < v.Length; i++)
            {
                ret[i] = new Vector2(v[i].x, v[i].y);
            }

            return ret;
        }

        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            var sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            var cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            var tx = v.x;
            var ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }

        public static float Qua2Angle2D(this Quaternion quaternion)
        {
            return quaternion.eulerAngles.z;
        }

        public static Vector2 Angle2Vector2D(this float angle)
        {
            var a = (angle + 90) * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
        }

        public static Vector2 Qua2Vector2D(this Quaternion quaternion)
        {
            return quaternion.Qua2Angle2D().Angle2Vector2D();
        }

        public static float Vector2Angle2D(this Vector2 vector2)
        {
            return Vector2.SignedAngle(Vector2.up, vector2);
        }

        public static Quaternion Angle2Quaternion2D(this float angle)
        {
            return Quaternion.AngleAxis(angle, Vector3.forward);
        }

        public static Quaternion Vector2Quaternion2D(this Vector2 vector2)
        {
            return vector2.Vector2Angle2D().Angle2Quaternion2D();
        }
    }
}