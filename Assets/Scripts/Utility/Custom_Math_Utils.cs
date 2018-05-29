using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Custom_Math_Utils {
    public static float nfmod(float a, float b)
    {
        return a - b * Mathf.Floor(a / b);
    }

    public static float Calculate_Kinetic_Energy(GameObject obj)
    {
        float mass = obj.GetComponent<Rigidbody>().mass;
        float vel = obj.GetComponent<Rigidbody>().velocity.magnitude;
        return Mathf.Pow((mass * vel), 2) * 0.5f;
    }

    public Vector2 PointAroundACircle(float cx, float cy, float rad, float ang)
    {
        float x = cx + rad * Mathf.Cos(ang);
        float y = cy + rad * Mathf.Sin(ang);
        return new Vector2(x, y);
    }
    public static Vector3 FindTargetAngle(Vector3 target_pos, Vector3 src_pos)
    {
        return Vector3.Normalize(target_pos - src_pos);
    }
}