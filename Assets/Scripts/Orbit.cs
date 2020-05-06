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

    public GameObject satObj;

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

        return EvalPositionByAnomaly(E);
    }

    public Vector3 EvalPositionByAnomaly(float E)
    {
        // planear coords
        float px = a * (Cos(E) - e);
        float py = a * Mathf.Sqrt(1 - Mathf.Pow(e, 2)) * Sin(E);

        // distance and true anomaly
        float r = Mathf.Sqrt((px * px) + (py * py));
        float v = Mathf.Atan2(py, px) * Mathf.Rad2Deg;

        // convert to rect coords with steps:
        //      1. Apply rotations (N), (w), (i)
        //      2. Apply true anomaly (v)
        //      3. Apply distance from primary center (r)
        float x = r * (CosD(N) * CosD(v + w) - SinD(N) * SinD(v + w) * CosD(i));
        float y = r * (SinD(N) * CosD(v + w) + CosD(N) * SinD(v + w) * CosD(i));
        float z = r * SinD(v + w) * SinD(i);

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
    //            The amount of times to approximate E => more approximations = more accuracy but less performance
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

    public Vector3[] OrbitPath()
    {
        Vector3[] orbitPath = new Vector3[61];

        for (int i = 0; i <= 60; i++)
        {
            orbitPath[i] = EvalPositionByAnomaly(i * 6 * Mathf.Deg2Rad);
        }

        return orbitPath;
    }

    public LineRenderer orbitLine;

    public float time = 0;
    public float timeStep = 0.000001f;

    private void FixedUpdate()
    {
        this.transform.position = EvalPosition(time);
        time += timeStep;

        orbitLine.SetPositions(OrbitPath());
    }


}
