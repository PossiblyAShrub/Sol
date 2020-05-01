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
    [Range(0, 1)]
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
        // calculate anomalies
        float M = this.M(t);
        float E = this.E(M, EApproxLvl);

        // semi - minor axis
        float b = Mathf.Sqrt(((1 + e) * a) * ((1 - e) * a));

        // adjust for w
        E += w * Mathf.Deg2Rad;

        // planear coords
        float x = a * Cos(E);
        float y = b * Sin(E);

        // apply inclination rotation (also creates z data)
        x = x * CosD(i);
        y = y * CosD(i);
        float z = x * SinD(i);

        // apply longitude of the ascending node rotation (no new z axis data)
        x = (x * CosD(N)) - (y * SinD(N));
        y = (x * SinD(N)) + (y * CosD(N));

        return new Vector3(x, z, y); // x, y, z vars are in diff. order to correct for unity's axis rotation
    }

    public float Sin(float n)
    {
        return Mathf.Sin(n);
    }

    public float Cos(float n)
    {
        return Mathf.Cos(n);
    }

    public float SinD(float n)
    {
        return Mathf.Sin(n * Mathf.Deg2Rad);
    }

    public float CosD(float n)
    {
        return Mathf.Cos(n * Mathf.Deg2Rad);
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
        float EAnomaly = M + e * Mathf.Sin(M);
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
        return (t + T) * Mathf.Sqrt(u / Mathf.Pow(a, 3)) * Mathf.Deg2Rad;
    }

    public float time = 0;
    public float timeStep = 0.000001f;

    private void FixedUpdate()
    {
        this.transform.position = EvalPosition(time);
        time += timeStep;
    }


}
