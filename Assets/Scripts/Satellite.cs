using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Satellite : Planet
{
    // Start is called before the first frame update
    void Start()
    {
        this.transform.localScale = new Vector3(size, size, size);
    }

    private void LateUpdate()
    {
        this.transform.position = GetHelioPos(0.1f);

        d += 0.1f;
    }
}
