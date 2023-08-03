using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private Renderer render;
    MeshCollider col;
    public Map_V2 map_V2;

    void Start()
    {
        render = GetComponent<Renderer>();
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
