using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PathWayNavMesh : MonoBehaviour
{
    [SerializeField]
    public ePathType ePathType;
    public bool isNavPath;

    [HideInInspector]
    public List<Vector3> WayPoints;
    [HideInInspector]
    public List<Vector3> Path;
    [HideInInspector, Range(0.1f,1f)]
    public float NavDistance = 0.3f;
}

public enum ePathType
{
    PingPong,
    Round
}

