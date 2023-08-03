using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployPointGroup : MonoBehaviour
{
    public GameObject[] Monsters;

    [Header("Type")]
    [SerializeField]
    private PositionConstraint _xyzConstraint = PositionConstraint.XYZ;
    [SerializeField]
    private eDeployControlType _deployControlType;
    [SerializeField]
    private ePhysicType _physicType;
    [HideInInspector]
    private List<DeployPoint> _deployPoints;

    [Header("Options")]
    [SerializeField]
    private bool _option;
    [HideInInspector]
    public bool CanSwitch;
    [SerializeField, HideInInspector]
    private bool _showCollision;
    [HideInInspector]
    public bool ShowSticky; 
    [HideInInspector]
    public float Radius;
    Color color = new Color(0.5f, 1f, 0.5f, 0.6f);

    public PositionConstraint XYZConstraint { get => _xyzConstraint; set => _xyzConstraint = value; }
    public eDeployControlType DeployControlType { get => _deployControlType; set => _deployControlType =value; }
    public ePhysicType PhysicType { get => _physicType; set => _physicType = value; }
    public List<DeployPoint> DeployPoints { get => _deployPoints; }

    private void Awake()
    {
        if (_deployPoints != null)
        {
            foreach (DeployPoint deployPoint in _deployPoints)
                deployPoint.SetDeployPointGroup(this);
        }
        else
        {
            _deployPoints = GetDeployPointChildren();
        }
    }

    public List<DeployPoint> GetDeployPointChildren()
    {
        if (_deployPoints == null)
            _deployPoints = new List<DeployPoint>();
        return _deployPoints;
    }

    public void AddDeployPoint(DeployPoint dp, int ndx = -1)
    {
        if (_deployPoints == null) _deployPoints = new List<DeployPoint>();
        if (ndx == -1)
        {
            dp.UpdatePosition(transform.position + Vector3.up, XYZConstraint);
            _deployPoints.Add(dp);
        }
        else
            _deployPoints.Insert(ndx, dp);
        dp.SetDeployPointGroup(this);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (_showCollision && _deployPoints !=null && _deployPoints.Count > 0)
        {
            Gizmos.color = color;
            foreach (var dp in _deployPoints)
                Gizmos.DrawSphere(dp.GetPosition(), Radius);
        }
    }
#endif

    public enum eDeployControlType{
        Hand,
        Auto
    }

    [Flags]
    public enum ePhysicType
    {
        Ray = 1 << 0,
        Overlap = 1 << 1,
    }
}