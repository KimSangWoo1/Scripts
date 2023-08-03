
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StoryBoardManager))]
public class StroryBoardEditor : Editor
{
    StoryBoardManager storyBoardManager;

    private void OnEnable()
    {
        storyBoardManager = target as StoryBoardManager;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        
        GUILayout.Space(16);
        if (storyBoardManager.Story != null)
        {
            if (GUILayout.Button("Read Script"))
            {
                storyBoardManager.ReadScript();
            }
        }
        else
        {
            if (GUILayout.Button("Read Script"))
            {
                storyBoardManager.ReadScript();
            }
        }
        
    }
}
