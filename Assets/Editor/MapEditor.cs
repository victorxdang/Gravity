
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapParser))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if ((DrawDefaultInspector() || GUILayout.Button("Refresh")) && !GameManager.Instance)
        {
            (target as MapParser).CreateMap(true, 0);
        }
    }
}
