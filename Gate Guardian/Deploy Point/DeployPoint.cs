using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DeployPoint
{
    // OFFSET position to parent WaypointsGroup; 
    // Use GETPOSITION to read properly (as it depends on the PositionConstraint of the WaypointGroup parent)
    public Vector3 position;
    public bool isStickGround;

    [SerializeField]
    [HideInInspector]
    Vector3 xyzPosition;    // OFFSET position to parent WaypointsGroup

    [HideInInspector]
    [SerializeField]
    Vector3 xyPosition;     // OFFSET position to parent WaypointsGroup

    [HideInInspector]
    [SerializeField]
    Vector3 xzPosition;     // OFFSET position to parent WaypointsGroup

    DeployPointGroup _dpGroup;

    public Vector3 XY
    {
        get { return xyPosition; }
    }

    public Vector3 XYZ
    {
        get { return xyzPosition; }
    }
    public Vector3 XZ
    {
        get { return xzPosition; }
    }

    public void SetDeployPointGroup(DeployPointGroup dpGroup)
    {
        _dpGroup = dpGroup;
    }

    public void CopyOther(DeployPoint other)
    {
        if (other == null) return;

        xyPosition = other.XY;
        xzPosition = other.XZ;
        xyzPosition = other.XYZ;
    }

    public Vector3 GetPosition(PositionConstraint constraint = PositionConstraint.XYZ)
    {
        if (_dpGroup != null)
        {
            constraint = _dpGroup.XYZConstraint;
        }

        if (constraint == PositionConstraint.XY)
            position = xyPosition;
        else if (constraint == PositionConstraint.XZ)
            position = xzPosition;
        else
            position = xyzPosition;
        return position;
    }

    public void UpdatePosition(Vector3 newPos, PositionConstraint constraint)
    {
        xyPosition.x += newPos.x;
        xzPosition.x += newPos.x;
        xyzPosition.x += newPos.x;

        if (constraint == PositionConstraint.XY)
        {
            xyzPosition.y += newPos.y;
            xyPosition.y += newPos.y;
        }
        else if (constraint == PositionConstraint.XZ)
        {
            xzPosition.z += newPos.z;
            xyzPosition.z += newPos.z;
        }
        else if (constraint == PositionConstraint.XYZ)
        {
            xyzPosition.y += newPos.y;
            xyzPosition.z += newPos.z;

            xyPosition.y += newPos.y;
            xzPosition.z += newPos.z;
        }
    }

    public void SetClosetPosition(Vector3 newPos)
    {
        /*
        xyPosition.x = newPos.x;
        xyPosition.y = newPos.y;

        xzPosition.x = newPos.x;
        xzPosition.z = newPos.z;
        */
        xyzPosition.x = newPos.x;
        xyzPosition.y = newPos.y;
        xyzPosition.z = newPos.z;
    }
}

public enum PositionConstraint
{
    XYZ,        // 3D
    XY,         // 2D
    XZ          // 2D
}

public enum TravelDirection
{
    FORWARD,  //  0 to Length-1 
    REVERSE   // Length-1 to 0
}
public enum EndpointBehavior
{
    STOP,       // Movement stops when end position reached
    LOOP,       // Movement loops back to first position
    PINGPONG,   // Reverse direction through the the positions list
}
public enum MoveType
{
    LERP,                   // Uses the MoveLerpSimple function to update transform position
    FORWARD_TRANSLATE       // uses MoveForwardToNext function to translate position - ROTATION DEPENDENT!
}
