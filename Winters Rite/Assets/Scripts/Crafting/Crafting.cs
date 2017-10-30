////////////////////////////////////////////////////////////
// File: Crafting.cs
// Author: Chris Deere
// Date Created: 12/04/17
// Brief: Allows user to craft items
////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

public class Crafting : MonoBehaviour
{


    GUISkin MenuSkin;

    //References
    GameObject player;
    GameObject mainCamera;
    GameObject arms;

    //Icons
    Texture campfireIcon;
    Texture tentIcon;
    Texture spareIcon;

    //Player prefabs
    GameObject campFire;
    GameObject tent;
    GameObject spare;


    private bool showGUI = false;

    private Inventory invScript;

    void Start()
    {
        invScript = GetComponent<Inventory>();
    }

    void Update()
    {
        // Opens crafting menu
        if (Input.GetKeyDown("c"))
        {
            showGUI = !showGUI;
        }

        if (showGUI == true)
        {
            Time.timeScale = 0;
            player.GetComponent<FPSInputController>().enabled = false;
            player.GetComponent<MouseLook>().enabled = false;
            mainCamera.GetComponent<MouseLook>().enabled = false;
            arms.GetComponent<PlayerControl>().enabled = false;
        }

        if (showGUI == false)
        {
            Time.timeScale = 1;
            player.GetComponent<FPSInputController>().enabled = true;
            player.GetComponent<MouseLook>().enabled = true;
            mainCamera.GetComponent<MouseLook>().enabled = true;
            arms.GetComponent<PlayerControl>().enabled = true;
        }
    }

    void OnGUI()
    {
        if (showGUI == true)
        {
            //Creates items if they have enough items in their inventory
            GUI.skin = MenuSkin;
            GUI.BeginGroup(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 150, 300, 300));
            GUI.Box(new Rect(0, 0, 300, 300), "Crafting System");

            if (GUI.Button(new Rect(10, 50, 50, 50), GUIContent(campfireIcon, "Build a campfire")))
            {
                if (invScript.wood >= 6 && invScript.stone >= 3)
                {
                    campFire.SetActive(true);
                    invScript.wood -= 6;
                    invScript.stone -= 3;
                }
            }

            if (GUI.Button(new Rect(10, 120, 50, 50), GUIContent(tentIcon, "Build a tent?")))
            {
                if (invScript.wood >= 10 && invScript.stone >= 5 && invScript.clay >= 3)
                {
                    tent.SetActive(true);
                    invScript.wood -= 10;
                    invScript.stone -= 5;
                }
            }

            if (GUI.Button(new Rect(10, 190, 50, 50), GUIContent(spareIcon, "Spare icon tooltip!")))
            {
                if (invScript.wood >= 10 && invScript.stone >= 5)
                {
                    spare.SetActive(true);
                    invScript.wood -= 10;
                    invScript.stone -= 5;
                }
            }

            GUI.Label(new Rect(100, 250, 100, 40), GUI.tooltip);
            GUI.EndGroup();
        }
    }
}

