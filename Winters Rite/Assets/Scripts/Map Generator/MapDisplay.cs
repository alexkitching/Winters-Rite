////////////////////////////////////////////////////////////
// File: MapGenerator.cs
// Author: Chris Deere
// Date Created: 12/05/17
// Brief: Displays map
////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;


public class MapDisplay : MonoBehaviour
{
    // Renders the mesh and texture
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public void DrawTexture(Texture2D texture)
    {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }

}
