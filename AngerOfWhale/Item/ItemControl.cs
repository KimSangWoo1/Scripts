using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemControl : MonoBehaviour
{
    // Update is called once per frame
    public float x;
    public float y;
    public float z;

    public float rotationSpeed;
    void Update()
    {
        transform.Rotate(new Vector3(x, y, z) * Time.deltaTime * this.rotationSpeed);
    }
}
