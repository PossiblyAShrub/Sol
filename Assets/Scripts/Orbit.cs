using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    // <summary>
    // Semi-Major Axis
    //     The average distance (r) between the secondary and the barycenter.
    //     Ranges from 0 to infinity.
    // <summary>
    public float a;

    // <summary>
    // Eccentricity
    //     A value that describes the orbit's shape (circle, ellipse, hyperbolic, parabolic)
    //     Ranges from 0 to 1 for a circular to elliptical orbit, 
    //     and 1 to infinity for hyperbolic/parabolic orbits.
    // <summary>
    public float e;

    public float i; // inclination

    // <summary>
    // Longitude of the Ascending Node
    //     Angle between the vernal point and the ascending node.
    //     (the ascending node is where the orbit path crosses the 
    //     plane of reference at an 'ascending' directrion)
    //     Ranges from 0 to 360.
    // <summary>
    public float N;

    // <summary>
    // Argument of periapsis
    //     The angle from the ascending node to the periapsis.
    //     Ranges from 0 to 360.
    // <summary>
    public float w;

    // <summary>
    // Time at Perihelion
    //     The time stamp where the secondary is at the perihelion.
    // <summary>
    public float T; // Time at perihelion

    public const float u = 3.986004418E14f; // std. gravitational parameter

    public int EApproxLvl;

    // <summary>
    // Returns the position of the secondary at time d in rectangular coordinates.
    //     Param: t
    //            Time stamp to evaluate for.
    // <summary>
    public Vector3 EvalPosition(float t)
    {
        float M = this.M(t);
        float E = this.E(M, EApproxLvl);
        float v = this.v(E);

        float r = a * ((1 - Mathf.Pow(e, 2)) / 1 + e * Mathf.Cos(v));

        float x = r * (Cos(N) * Cos(w + v) - Sin(N) * Cos(i) * Sin(w + v));
        float y = r * (Sin(N) * Cos(w + v) + Cos(N) * Cos(i) * Sin(w + v));
        float z = r * Sin(i) * Sin(w + v);

        return new Vector3(x, z, y);
    }

    public float Sin(float n)
    {
        return Mathf.Sin(n);
    }

    public float Cos(float n)
    {
        return Mathf.Cos(n);
    }

    public float v(float E)
    {
        return Mathf.Atan2(Mathf.Sqrt(1 - Mathf.Pow(e, 2)) * Mathf.Sin(E), Mathf.Cos(E) - e);
    }

    // <summary>
    // Returns the Eccentric Anomaly as a float angle in radians.
    //     Param: M
    //            The Mean Anomaly, a float angle in degrees
    //     Param: k
    //            The amount of times to approximate E - more approximations = more accuracy but less performance
    // <summary>
    public float E(float M, int k)
    {
        float EAnomaly = M;
        for (int i = 0; i < k; i++)
        {
            EAnomaly = M + e * Mathf.Sin(EAnomaly);
        }
        return EAnomaly;
    }

    // <summary>
    // Returns the Mean Anomaly as a float angle in degrees.
    //     Param: M
    //            The Mean Anomaly, a float angle in degrees
    //     Param: k
    //            The amount of times to approximate E - more approximations = more accuracy but less performance
    // <summary>
    public float M(float t)
    {
        return (t - T) * Mathf.Sqrt(u / Mathf.Pow(a, 3));
    }

    public float time = 0;

    private void FixedUpdate()
    {
        this.transform.position = EvalPosition(time);
        time += 0.0000001f;
    }


}
