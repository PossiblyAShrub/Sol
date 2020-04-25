using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public float N; // longitude of ascending node
    public float i; // inclination
    public float w; // argument of perihelion
    public float a; // semi-major axis
    public float e; // eccentricity
    public float M; // mean anomaly

    public float NStep;
    public float iStep;
    public float wStep;
    public float aStep;
    public float eStep;
    public float MStep;

    public Vector3 GetHelioPos(float d)
    {
        N += NStep * d;
        i += iStep * d;
        w += wStep * d;
        a += aStep * d;
        e += eStep * d;
        M += MStep * d;

        N = rev(N);
        i = rev(i);
        w = rev(w);
        M = rev(M);

        float E = M;//GetE(3);
        float planeX = a * (Cos(E) - e);
        float planeY = a * Mathf.Sqrt(1 - Mathf.Pow(e, 2)) * Sin(E);

        float r = Mathf.Sqrt(Mathf.Pow(planeX, 2) + Mathf.Pow(planeY, 2));
        float v = Mathf.Atan2(planeY, planeX);

        // float x = r * (Cos(N) * Cos(v + w) - Sin(N) * Sin(v + w) * Cos(i));
        // float y = r * (Sin(N) * Cos(v + w) + Cos(N) * Sin(v + w) * Cos(i));
        // float z = r * Sin(v + w) * Sin(i);

        float x = r * Cos(M) * Cos(i);
        float y = r * Sin(M) * Cos(i);
        float z = r * Sin(M);

        return new Vector3(x, z, y);
    }

    private float rev(float deg)
    {
        int excess = (int)Mathf.Floor(deg / 360) * 360;
        return deg - excess;
    }

    private float GetE(int iterations)
    {
        float E = M;
        for (int i = 0; i < iterations; i++)
        {
            E = E - (E - (Mathf.Rad2Deg) * e * Sin(E) - M) / (1 - e * Cos(E));
        }
        return E;
    }

    private float Cos(float f)
    {
        return Mathf.Cos(f * Mathf.Deg2Rad);
    }

    private float Sin(float f)
    {
        return Mathf.Sin(f * Mathf.Deg2Rad);
    }

    public float d = 0;

    private void LateUpdate()
    {
        this.transform.position = GetHelioPos(0.1f);

        d += 0.1f;
    }
}
