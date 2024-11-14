#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShapeSO))]
public class ShapeSOEditor : Editor
{
    public override void OnInspectorGUI()
    {

        ShapeSO shapeSO = (ShapeSO)target;


        base.OnInspectorGUI();


        if (shapeSO.blockedCells.Count != shapeSO.rows * shapeSO.columns)
        {
            shapeSO.InitializeGrid();
        }


        GUILayout.Label("Blocked Cells", EditorStyles.boldLabel);

        for (int i = 0; i < shapeSO.rows; i++)
        {
            EditorGUILayout.BeginHorizontal();

            for (int j = 0; j < shapeSO.columns; j++)
            {
                bool isBlocked = shapeSO.IsBlocked(i, j);
                bool newBlockedStatus = EditorGUILayout.Toggle(isBlocked, GUILayout.Width(20), GUILayout.Height(20));
                if (newBlockedStatus != isBlocked)
                {
                    shapeSO.SetBlocked(i, j, newBlockedStatus);
                }
            }

            EditorGUILayout.EndHorizontal();
        }


        if (GUI.changed)
        {
            EditorUtility.SetDirty(shapeSO);
        }
    }
}
#endif