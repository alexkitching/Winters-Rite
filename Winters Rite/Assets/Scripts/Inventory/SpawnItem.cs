using UnityEngine;
using System.Collections;
public class SpawnItem : MonoBehaviour
{
    public Terrain terrain;
    // number of each object to place
    public int numberOfObjects; 
    public int numberOfObjects2;
    public int numberOfObjects3;
    // number of placed objects
    private int currentObjects;
    private int currentObjects2;
    private int currentObjects3;
    // GameObjects to place
    public GameObject objectToPlace; 
    public GameObject objectToPlace2; 
    public GameObject objectToPlace3; 
    private int terrainWidth; // terrain size (x)
    private int terrainLength; // terrain size (z)
    private int terrainPosX; // terrain position x
    private int terrainPosZ; // terrain position z
    void Start()
    {
        // terrain size x
        terrainWidth = (int)terrain.terrainData.size.x;
        // terrain size z
        terrainLength = (int)terrain.terrainData.size.z;
        // terrain x position
        terrainPosX = (int)terrain.transform.position.x;
        // terrain z position
        terrainPosZ = (int)terrain.transform.position.z;
    }
    // Update is called once per frame
    void Update()
    {
        // generate objects
        if (currentObjects <= numberOfObjects)
        {
            // generate random x position
            int posx = Random.Range(terrainPosX, terrainPosX + terrainWidth);
            // generate random z position
            int posz = Random.Range(terrainPosZ, terrainPosZ + terrainLength);
            // get the terrain height at the random position
            float posy = Terrain.activeTerrain.SampleHeight(new Vector3(posx, 0, posz));
            // create new gameObject on random position
            GameObject newObject = (GameObject)Instantiate(objectToPlace, new Vector3(posx, posy + 0.5f, posz), Quaternion.identity);
            currentObjects += 1;
        }

        if (currentObjects2 <= numberOfObjects2)
        {
            // generate random x position
            int posx = Random.Range(terrainPosX, terrainPosX + terrainWidth);
            // generate random z position
            int posz = Random.Range(terrainPosZ, terrainPosZ + terrainLength);
            // get the terrain height at the random position
            float posy = Terrain.activeTerrain.SampleHeight(new Vector3(posx, 0, posz));
            // create new gameObject on random position
            GameObject newObject = (GameObject)Instantiate(objectToPlace2, new Vector3(posx, posy + 0.5f, posz), Quaternion.identity);
            currentObjects2 += 1;
        }

        if (currentObjects3 <= numberOfObjects3)
        {
            // generate random x position
            int posx = Random.Range(terrainPosX, terrainPosX + terrainWidth);
            // generate random z position
            int posz = Random.Range(terrainPosZ, terrainPosZ + terrainLength);
            // get the terrain height at the random position
            float posy = Terrain.activeTerrain.SampleHeight(new Vector3(posx, 0, posz));
            // create new gameObject on random position
            GameObject newObject = (GameObject)Instantiate(objectToPlace3, new Vector3(posx, posy + 0.5f, posz), Quaternion.identity);
            currentObjects3 += 1;
        }

    }
}