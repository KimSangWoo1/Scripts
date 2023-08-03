using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DeployPointGroup))]
public partial class DeployPointGroupEditor : Editor
{
    DeployPointGroup _deployPointGroup;
    List<DeployPoint> _deployPoints;
    DeployPoint _selectedDeployPoint = null;


    private void OnEnable()
    {
        _deployPointGroup = target as DeployPointGroup;
        _deployPoints = _deployPointGroup.GetDeployPointChildren();
    }

    private void OnSceneGUI()
    {
        ParentMove(); // working shortKey to T , Not W
        DrawDeployPoints(_deployPoints);
    }

    #region Draw
    public void DrawDeployPoints(List<DeployPoint> deployPoints)
    {
        if (deployPoints != null)
        {
            int cnt = 0;
            foreach (DeployPoint deployPoint in deployPoints)
            {
                DrawDeployPoint(deployPoint);
                Handles.Label(deployPoints[cnt].position, string.Format("DP{0}",cnt+1), TxtStyle);
                
                // Draw a pointer line 
                if (cnt < deployPoints.Count - 1)
                {
                    DeployPoint wpnext = deployPoints[cnt + 1];
                    Handles.DrawLine(deployPoint.GetPosition(_deployPointGroup.XYZConstraint), wpnext.GetPosition(_deployPointGroup.XYZConstraint));
                }
                cnt += 1;
            }
        }
    }

    public void DrawDeployPoint(DeployPoint deployPoint, int controlID = -1)
    {
        if (_selectedDeployPoint == deployPoint)
        {
            ControlSelectedDeployPoint();
        }
        else
        {
            DrawNormalDeployPoint(deployPoint);
        }
    }

    private void DrawNormalDeployPoint(DeployPoint deployPoint)
    {
        Vector3 currPos = deployPoint.GetPosition(_deployPointGroup.XYZConstraint);
        float handleSize = HandleUtility.GetHandleSize(currPos);

        if (Handles.Button(currPos, Quaternion.identity, 0.25f * handleSize, 0.25f * handleSize, Handles.SphereHandleCap))
        {
            _selectedDeployPoint = deployPoint;

            //************* SceneView.RepaintAll();
        }
    }
    #endregion

    #region Physics
    private bool DownSurface(DeployPoint deployPoint)
    {
        Vector3 pos = deployPoint.GetPosition();
        RaycastHit hit;
        if (Physics.Raycast(pos, Vector3.down, out hit))
        {
            Vector3 surfacePoint = hit.point;
            //Debug.DrawRay(hit.point, Vector3.up, Color.red, 1.2f);
            //Debug.DrawRay(pos, Vector3.up, Color.blue, 1.2f);
            float distance = Vector3.Distance(pos, surfacePoint);

            //Debug.Log($"�Ÿ� : {distance}");
            if (distance < 3.5f)
            {
                deployPoint.SetClosetPosition(surfacePoint);
                return true;
            }
        }
        return false;
    }
    
    private bool ClosetSurface(DeployPoint deployPoint)
    {
        Vector3 pos = deployPoint.GetPosition();

        
        Collider[] colliders = Physics.OverlapSphere(pos, _deployPointGroup.Radius);
        if (colliders.Length > 0)
        {
            deployPoint.SetClosetPosition(colliders[0].ClosestPointOnBounds(pos));
            deployPoint.isStickGround = true;
            return true;
        }

        /*
        Collider[] colliders = Physics.OverlapSphere(pos, _deployPointGroup.Radius);
        if (colliders.Length > 0)
        {
            // �ʱⰪ ����
            float closestDistance = float.MaxValue;
            Vector3 closestSurfacePoint = Vector3.zero;
            Collider closestCollider = null;

            foreach (Collider collider in colliders)
            {
                // �浹ü���� ǥ�鿡 ���� ����� ���� ã��
                Vector3 surfacePoint = collider.ClosestPoint(pos);
                float distance = Vector3.Distance(pos, surfacePoint);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestSurfacePoint = surfacePoint;
                    closestCollider = collider;
                }
            }

            if (closestCollider != null)
            {
                // ���� ����� ǥ������ �̵�
                Vector3 newNormal = closestSurfacePoint - closestCollider.transform.position;
                Vector3 newPos = closestSurfacePoint + newNormal.normalized; // offset�� ���ϴ� ��ġ ���� ���Դϴ�.

                // ��� ��ü �̵�
                deployPoint.SetClosetPosition(newPos);
                return true;
            }
        }
        */
        return false;
    }

    private void CheckPhysics()
    {
        bool isClosetSurface =false;
        bool isDownSurface =false;

        if ((_deployPointGroup.PhysicType & DeployPointGroup.ePhysicType.Ray) != 0 && (_deployPointGroup.PhysicType & DeployPointGroup.ePhysicType.Overlap) != 0)
        {
            isDownSurface = DownSurface(_selectedDeployPoint);
            if (!isDownSurface)
            {
                isClosetSurface = ClosetSurface(_selectedDeployPoint);
            }
        }
        else if (_deployPointGroup.PhysicType == DeployPointGroup.ePhysicType.Ray)
        {
            isDownSurface = DownSurface(_selectedDeployPoint);
        }
        else if(_deployPointGroup.PhysicType == DeployPointGroup.ePhysicType.Overlap){
            isClosetSurface = ClosetSurface(_selectedDeployPoint);
        }
        _selectedDeployPoint.isStickGround = isDownSurface | isClosetSurface;
    }
    #endregion

    private void ParentMove()
    {
        // ���� �����ӿ����� ��ġ ����
        Vector3 previousPosition = _deployPointGroup.transform.position;

        // DeployPointGroup�� Transform �ڵ��� �׸��� �̵��� ����
        EditorGUI.BeginChangeCheck();
        Vector3 newPosition = Handles.PositionHandle(previousPosition, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            // ��ġ ��ȭ�� ���
            Vector3 positionDelta = newPosition - previousPosition;

            // DeployPointGroup �̵�
            Undo.RecordObject(_deployPointGroup.transform, "DeployPointGroup Moved");
            _deployPointGroup.transform.position = newPosition;

            // DeployPoint�鵵 �Բ� �̵�
            foreach (DeployPoint deployPoint in _deployPoints)
            {
                deployPoint.UpdatePosition(positionDelta, _deployPointGroup.XYZConstraint);
            }
        }
    }
}