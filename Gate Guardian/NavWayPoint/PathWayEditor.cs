using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using UnityEditorInternal;
using System;

[CustomEditor(typeof(PathWayNavMesh))]
public class PathWayEditor : Editor
{
    private ReorderableList _reorderableList;
    private PathWayNavMesh _pathWayNavMesh;

    private const int TXT_SIZE = 18;
    private const int SAFE_CAP_SIZE = 1;
    private const int UNSAFE_CAP_SIZE = 3;
    private const float PATH_CAP_SIZE = 0.5f;
    private const float RAY_DISTANCE = 2f;

    public const string FIELD_LABEL = "Point";
    public const string TITLE_LABEL = "Waypoints";

    private GUIStyle _txtStyle;
    private Color _capRedColor;
    private Color _capGreenColor;
    private Color _capBlueColor;
    private Color _lineColor;

    private void OnSceneGUI()
    {
        DrawGUI();
        DrawPath();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        if (_pathWayNavMesh.isNavPath)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("NavDistance"));
            var serializeList = serializedObject.FindProperty("Path");
            DrawPathList(serializeList, "Path");
        }
        _reorderableList.DoLayoutList();
        if (EditorGUI.EndChangeCheck())
        {
            UpdatePath();
            SceneView.RepaintAll();
            
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void Awake()
    {
        InitGUI();
    }

    private void OnEnable()
    {   
        _pathWayNavMesh = target as PathWayNavMesh;

        Undo.undoRedoPerformed += DoUndo;
        _reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("WayPoints"), true, true, true, true);
        _reorderableList.drawHeaderCallback += DrawHeader;
        _reorderableList.drawElementCallback += DrawElement;
        _reorderableList.onAddCallback += AddItem;
        _reorderableList.onRemoveCallback += RemoveItem;
        _reorderableList.onChangedCallback += ListModified;
        _reorderableList.onMouseDragCallback += DragItem;
        _reorderableList.onSelectCallback += Selected;
        SceneView.duringSceneGui += this.OnSceneGUI;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= DoUndo;
        _reorderableList.drawHeaderCallback -= DrawHeader;
        _reorderableList.drawElementCallback -= DrawElement;
        _reorderableList.onAddCallback -= AddItem;
        _reorderableList.onRemoveCallback -= RemoveItem;
        _reorderableList.onChangedCallback -= ListModified;
        _reorderableList.onMouseDragCallback -= DragItem;
        SceneView.duringSceneGui -= this.OnSceneGUI;
    }
    #region Inspector
    private void DrawHeader(Rect rect)
    {
        GUI.Label(rect, TITLE_LABEL);
    }

    private void DrawElement(Rect rect, int index, bool active, bool focused)
    {
        SerializedProperty item = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
        item.vector3Value = EditorGUI.Vector3Field(rect, FIELD_LABEL + index, item.vector3Value);
        
        //조작할 경우
        if (active)
        {
            UpdatePath();
        }
    }

    private void AddItem(ReorderableList list)
    {       
        int index = list.index;
        // Not Element Selected Add 
        if(index >= list.serializedProperty.arraySize)
        {
            index = list.serializedProperty.arraySize - 1;
            list.Select(index);
        }

        if (index > -1 && list.serializedProperty.arraySize >= 1)
        {
            list.serializedProperty.InsertArrayElementAtIndex(index+1 );
            Vector3 previous = list.serializedProperty.GetArrayElementAtIndex(index).vector3Value;
            list.serializedProperty.GetArrayElementAtIndex(index+1 ).vector3Value = new Vector3(previous.x + 2, previous.y, previous.z + 2);
        }
        else
        {
            list.serializedProperty.InsertArrayElementAtIndex(list.serializedProperty.arraySize);
            Vector3 previous = Vector3.zero;
            list.serializedProperty.GetArrayElementAtIndex(list.serializedProperty.arraySize - 1).vector3Value = new Vector3(previous.x + 2, previous.y, previous.z + 2);
        }
        list.index++;
    }

    private void RemoveItem(ReorderableList list)
    {
        int index = list.index;

        list.serializedProperty.DeleteArrayElementAtIndex(index);

        if (list.index == list.serializedProperty.arraySize)
        {
            list.index--;
        }
    }

    // Add, Sub, Drag 시만
    private void ListModified(ReorderableList list)
    {
        list.serializedProperty.serializedObject.ApplyModifiedProperties();
        UpdatePath();
    }

    private void DragItem(ReorderableList list)
    {

    }

    private void Selected(ReorderableList list)
    {
        
    }

    void DrawPathList(SerializedProperty property, string labelName)
    {
        EditorGUILayout.Space();
        for (int i = 0; i < property.arraySize; ++i)
        {
            EditorGUILayout.PropertyField(property.GetArrayElementAtIndex(i), new GUIContent(labelName + i));
        }
        EditorGUILayout.Space();
    }
    #endregion

    #region GUI
    private void OnSceneGUI(SceneView sceneView)
    {
        
    }
    
    void InitGUI()
    {
        if (_txtStyle == null)
        {
            _txtStyle = new GUIStyle();
            _txtStyle.normal.textColor = Color.white;
            _txtStyle.alignment = TextAnchor.MiddleCenter;
            _txtStyle.fontSize = TXT_SIZE;
        }
        _capRedColor = new Color(1f, 0f, 0f, 0.2f);
        _capGreenColor = new Color(0, 1f, 0, 0.5f);
        _capBlueColor = new Color(0f, 0f, 1f, 0.5f);
        _lineColor = new Color(0,0,1f,1f);
    }

    private void DrawGUI()
    {
        for (int i = 0; i < _pathWayNavMesh.WayPoints.Count; i++)
        {
            if(!_pathWayNavMesh.isNavPath) DrawGUILabel(_pathWayNavMesh.WayPoints[i], i);
            DrawHandleCap(_pathWayNavMesh.WayPoints[i]);
            _pathWayNavMesh.WayPoints[i] = DrawPostionHandle(_pathWayNavMesh.WayPoints[i]);
        }

        if (_pathWayNavMesh.isNavPath)
        {
            for (int i = 0; i < _pathWayNavMesh.Path.Count; i++)
            {
                DrawGUILabel(_pathWayNavMesh.Path[i], i);
                if (!_pathWayNavMesh.WayPoints.Contains(_pathWayNavMesh.Path[i]))
                {
                    DrawHandleCap(_pathWayNavMesh.Path[i], true);
                }
            }
        }
    }
    private void DrawGUILabel(Vector3 wayPointPos, int index)
    {
        Handles.Label(wayPointPos + Vector3.up, index.ToString(), _txtStyle);
    }

    private void DrawHandleCap(Vector3 PointPos, bool isPath =false)
    {
        if (!isPath)
        {
            if (DownSurface(PointPos + Vector3.up) != Vector3.zero)
            {
                Handles.color = _capGreenColor;
                Handles.SphereHandleCap(0, PointPos, Quaternion.identity, SAFE_CAP_SIZE, EventType.Repaint);
            }
            else
            {
                Handles.color = _capRedColor;
                Handles.SphereHandleCap(0, PointPos, Quaternion.identity, UNSAFE_CAP_SIZE, EventType.Repaint);
            }
        }
        else
        {
            Handles.color = _capBlueColor;
            Handles.SphereHandleCap(0, PointPos, Quaternion.identity, PATH_CAP_SIZE, EventType.Repaint);
        }
    }

    private Vector3 DrawPostionHandle(Vector3 wayPointPos)
    {
        EditorGUI.BeginChangeCheck();

        Vector3 newPos = Handles.PositionHandle(wayPointPos, Quaternion.identity);
        Vector3 hitPos = DownSurface(newPos);
        if (EditorGUI.EndChangeCheck())
        {
            UpdatePath();
            if (hitPos != Vector3.zero)
            {
                return hitPos;
            }
            else
            {
                return newPos;
            }
        }
        return wayPointPos;
    }

    #region Draw Path
    private void DrawPath()
    {
        if (!_pathWayNavMesh.isNavPath)
        {
            DrawLinePath();
        }
        else
        {
            DrawNavMeshLinePath();
        }
    }

    private void DrawLinePath()
    {
        Handles.color = _lineColor;

        if (_pathWayNavMesh.WayPoints.Count > 1)
        {
            for (int i = 1; i < _pathWayNavMesh.WayPoints.Count; i++)
            {
                Handles.DrawDottedLine(_pathWayNavMesh.WayPoints[i - 1], _pathWayNavMesh.WayPoints[i], 2);
            }
            if(_pathWayNavMesh.ePathType == ePathType.Round)
            {
                if (_pathWayNavMesh.WayPoints.Count > 2)
                {
                    Handles.DrawDottedLine(_pathWayNavMesh.WayPoints[0], _pathWayNavMesh.WayPoints[_pathWayNavMesh.WayPoints.Count - 1], 2);
                }
            }
        }
    }

    private void DrawNavMeshLinePath()
    {
        Handles.color = _lineColor;
        for (int i = 0; i < _pathWayNavMesh.Path.Count; i++)
        {
            if (i < _pathWayNavMesh.Path.Count - 1)
                Handles.DrawLine(_pathWayNavMesh.Path[i], _pathWayNavMesh.Path[i + 1]);
        }
        if(_pathWayNavMesh.ePathType == ePathType.Round) 
            Handles.DrawLine(_pathWayNavMesh.Path[0], _pathWayNavMesh.Path[_pathWayNavMesh.Path.Count-1]);
    }

    #endregion
    #endregion

    #region Physics
    private Vector3 DownSurface(Vector3 wayPointPos)
    {
        RaycastHit hit;
        if (Physics.Raycast(wayPointPos, Vector3.down, out hit))
        {
            Vector3 surfacePoint = hit.point;
            float distance = Vector3.Distance(wayPointPos, surfacePoint);

            if (distance < RAY_DISTANCE)
            {
                return surfacePoint;
            }
        }
        return Vector3.zero;
    }
    #endregion

    #region Nav
    private List<Vector3> GetPathCorners(int startIndex, int endIndex)
    {
        NavMeshPath navMeshPath = new NavMeshPath();
        if (NavMesh.CalculatePath(_pathWayNavMesh.WayPoints[startIndex], _pathWayNavMesh.WayPoints[endIndex], NavMesh.AllAreas, navMeshPath))
        {
            return navMeshPath.corners.ToList();
        }
        else
            return null;
    }

    private void UpdatePath()
    {
        _pathWayNavMesh.Path.Clear();
        for (int i = 1; i < _pathWayNavMesh.WayPoints.Count; i++)
        {
            AddConners(i);
        }

        if (_pathWayNavMesh.ePathType == ePathType.Round)
        {
            if (_pathWayNavMesh.Path.Count > 2)
            {
                AddConners(0, true);
            }
        }
    }

    private void AddConners(int i, bool isRoundTail = false)
    {
        List<Vector3> conners;
        if (!isRoundTail)
        {
            conners = GetPathCorners(i - 1, i);
        }
        else
        {
            conners = GetPathCorners(_pathWayNavMesh.WayPoints.Count - 1, i);
        }

        if (conners != null)
        {
            for (int j=0; j<conners.Count; j++)
            {
                if (isRoundTail && j == conners.Count - 1) continue;
                if (_pathWayNavMesh.Path.Count > 1)
                {
                    float distance = (_pathWayNavMesh.Path.Last() - conners[j]).sqrMagnitude; //Vector3.Distance(_pathWayNavMesh.Path.Last(), conners[j]);
                    if(distance > _pathWayNavMesh.NavDistance * _pathWayNavMesh.NavDistance)
                    {
                        _pathWayNavMesh.Path.Add(conners[j]);
                    }
                }
                else
                {
                    _pathWayNavMesh.Path.Add(conners[j]);
                }
            }
        }
    }
    #endregion

    #region Undo
    private void DoUndo()
    {
        UpdatePath();
    }
    #endregion
}
