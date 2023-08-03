using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

public partial class DeployPointGroupEditor : Editor
{
    private GUIStyle _txtStyle;
    public GUIStyle TxtStyle
    {
        get
        {
            if (_txtStyle == null)
            {
                _txtStyle = new GUIStyle(EditorStyles.label);
                _txtStyle.alignment = TextAnchor.MiddleCenter;
                _txtStyle.fontSize = 13;
                _txtStyle.fontStyle = FontStyle.BoldAndItalic;
                _txtStyle.normal.textColor = Color.black;
                _txtStyle.wordWrap = true;
                _txtStyle.hover = new GUIStyleState() { textColor = Color.red };
            }
            return _txtStyle;
        }
    }

    override public void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ShowOption();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("DeployPoints");

        bool isRepaint = false;

        if (_deployPoints != null)
        {
            for (int i = 0; i < _deployPoints.Count; i++)
            {
                Color guiColor = GUI.color;

                DeployPoint currentDP = _deployPoints[i];

                if (currentDP == _selectedDeployPoint)
                    GUI.color = Color.green;

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("S", GUILayout.Width(20)))
                {
                    SelectDeployPoint(currentDP);
                    isRepaint = true;
                }
                ControlTypeEvent(currentDP);

                if(_deployPointGroup.CanSwitch)
                    isRepaint |= SwitchDeployPoint(i);

                if (_deployPointGroup.ShowSticky)
                    GUILayout.Toggle(currentDP.isStickGround, "", GUILayout.Width(20));

                EditorGUI.BeginChangeCheck();
                Vector3 oldV = currentDP.GetPosition(_deployPointGroup.XYZConstraint);
                Vector3 newV = EditorGUILayout.Vector3Field("", oldV);

                if (EditorGUI.EndChangeCheck())
                {
                    Debug.Log("Change");
                    Undo.RecordObject(_deployPointGroup, "DeployPoint Moved");
                    currentDP.UpdatePosition(newV - oldV, _deployPointGroup.XYZConstraint);
                    isRepaint = true;
                }

                if (GUILayout.Button("D", GUILayout.Width(25)))
                {
                    DeleteDeployPoint(i);
                    isRepaint = true;
                }

                GUI.color = guiColor;
                EditorGUILayout.EndHorizontal();
            }
        }

        if (GUILayout.Button("Add"))
        {
            OnAddClick();
            isRepaint = true;
        }

        if (GUILayout.Button("Parent Center Point"))
        {
            OnCenterPointClick();
        }

        EditorGUILayout.EndVertical();
        if (isRepaint)
        {
            SceneView.RepaintAll();
        }
    }
    
    private void ShowOption()
    {
        bool optionValue = serializedObject.FindProperty("_option").boolValue;
        // _objectType 필드를 Inspector에 노출 시켜줍니다.
        if (optionValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CanSwitch"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_showCollision"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ShowSticky"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Radius"));
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void ControlSelectedDeployPoint()
    {
        Color c = Handles.color;
        Handles.color = Color.green;

        EditorGUI.BeginChangeCheck();
        Vector3 oldpos = _selectedDeployPoint.GetPosition(_deployPointGroup.XYZConstraint);
        Vector3 newPos = Handles.DoPositionHandle(oldpos, Quaternion.identity);
        float handleSize = HandleUtility.GetHandleSize(_selectedDeployPoint.GetPosition());

        Handles.SphereHandleCap(-1, newPos, Quaternion.identity, 0.25f * handleSize, EventType.Repaint);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_deployPointGroup, "DeployPoint Moved");
            _selectedDeployPoint.UpdatePosition(newPos - oldpos, _deployPointGroup.XYZConstraint);

            ControlTypeHandles();
        }
        Handles.color = c;
    }

    #region Type Chagne
    private void ControlTypeHandles()
    {
        switch (_deployPointGroup.DeployControlType)
        {
            case DeployPointGroup.eDeployControlType.Hand:

                break;
            case DeployPointGroup.eDeployControlType.Auto:
                CheckPhysics();
                break;
        }
    }

    private void ControlTypeEvent(DeployPoint currentDP)
    {
        switch (_deployPointGroup.DeployControlType)
        {
            case DeployPointGroup.eDeployControlType.Hand:
                if (GUILayout.Button("C", GUILayout.Width(20)))
                {
                    _selectedDeployPoint = currentDP;
                    CheckPhysics();
                    SceneView.RepaintAll();
                }
                break;
            case DeployPointGroup.eDeployControlType.Auto:
                break;
        }
    }
    #endregion

    #region Click
    // S 
    private void SelectDeployPoint(DeployPoint currentDP)
    {
        if (_selectedDeployPoint == currentDP)
        {
            _selectedDeployPoint = null;
        }
        else
        {
            _selectedDeployPoint = currentDP;
        }
    }

    // ▲ ▼
    private bool SwitchDeployPoint(int index)
    {
        bool isClick = false;
        int switchIndex = 0;

        if (GUILayout.Button("▲", GUILayout.Width(20)))
        {
            switchIndex = index -1;
            if (switchIndex < 0) 
                switchIndex = _deployPoints.Count-1;

            isClick = true;
        }
        if (GUILayout.Button("▼", GUILayout.Width(20)))
        {
            switchIndex = index +1;
            if (switchIndex > _deployPoints.Count - 1) 
                switchIndex = 0;
            isClick = true;
        }

        if (isClick)
        {
            DeployPoint switchDP = _deployPoints[index];
            _deployPoints[index] = _deployPoints[switchIndex];
            _deployPoints[switchIndex] = switchDP;
        }
        return isClick;
    }

    //D
    private void DeleteDeployPoint(int deleteIndex)
    {
        if (deleteIndex > -1)
        {
            if (_deployPoints[deleteIndex] == _selectedDeployPoint)
                _selectedDeployPoint = null;
            _deployPoints.RemoveAt(deleteIndex);
        }
    }

    // Add
    private void OnAddClick()
    {
        Undo.RecordObject(_deployPointGroup, "DeployPoint Added");
        int nextIndex = GetNextDeployPointIndex();

        DeployPoint deployPoint = new DeployPoint();
        deployPoint.CopyOther(_selectedDeployPoint);
        _deployPointGroup.AddDeployPoint(deployPoint, nextIndex);
        _selectedDeployPoint = deployPoint;
    }
    // Center Move
    private void OnCenterPointClick()
    {
        if (_deployPoints == null || _deployPoints.Count == 0)
        {
            Debug.LogError("Object list is null or empty.");
            return;
        }

        Undo.RecordObject(_deployPointGroup, "Group Move to center with Points");

        Vector3 centerPoint = Vector3.zero;

        foreach (var dp in _deployPoints)
        {
            centerPoint += dp.GetPosition();
        }

        centerPoint /= _deployPoints.Count;
        _deployPointGroup.transform.position = centerPoint;
    }
    #endregion

    private int GetNextDeployPointIndex()
    {
        int currentIndex = -1;

        if (_selectedDeployPoint != null)
        {
            currentIndex = _deployPoints.IndexOf(_selectedDeployPoint);
        }
        else
        {
            _selectedDeployPoint = GetLastDeployPoint();
            currentIndex = _deployPoints.IndexOf(_selectedDeployPoint);
        }

        if (currentIndex != -1)
        {
            return currentIndex + 1;
        }

        return currentIndex;
    }

    private DeployPoint GetLastDeployPoint()
    {
        return _deployPoints.LastOrDefault(dp => dp != null);
    }
}
