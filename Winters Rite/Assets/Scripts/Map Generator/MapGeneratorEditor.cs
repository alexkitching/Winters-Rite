 ////////////////////////////////////////////////////////////
// File: MapGeneratorEditor.cs
// Author: Chris Deere
// Date Created: 12/05/17
// Brief: Allows editing of height maps within the unity editor
////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        // Auto updates the generated map as the parameters are changed
        MapGenerator mapGen = (MapGenerator)target;

        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.GenerateMap();
            }
        }

        // Creates a button that generates a map when clicked
        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap();
        }
    }
}
