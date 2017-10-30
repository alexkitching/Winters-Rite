//////////////////////////////////////////////////////////////////////////
// File: <Util.cs>
// Author: <Alex Kitching>
// Date Created: <8/03/17>
// Brief: <Script Containing Utility functions.>
/////////////////////////////////////////////////////////////////////////

using UnityEngine;

public class Util
{
    public static void SetLayerRecursively(GameObject obj, int newLayer) // Sets Layer for object and all children
    {
        if (obj == null) // If object does not exist, exit
            return;

        obj.layer = newLayer; // This gameobject layer set to new layer

        foreach (Transform child in obj.transform) // For each transform child of game object
        {
            if (child == null) // If child does not exist, exit
                continue;
            SetLayerRecursively(child.gameObject, newLayer); // Repeat Script
        }
    }
}
