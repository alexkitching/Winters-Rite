////////////////////////////////////////////////////////////
// File: Pickup.cs
// Author: Chris Deere
// Date Created: 10/03/17
// Brief: Respawns Items after pickup
////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {

	public GameObject inventoryPanel;
	public GameObject[] inventoryIcons;

	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		// shows the amount of items the players has next to the inventory icon
		foreach(Transform child in inventoryPanel.transform)
		{
			if(child.gameObject.tag == hit.gameObject.tag)
			{
				string c = child.Find("Text").GetComponent<Text>().text;
				int tcount = System.Int32.Parse(c) + 1;
				child.Find("Text").GetComponent<Text>().text = "" + tcount;
				return;
			}
		}

		// Adds item into the inventory
		GameObject i;
		if(hit.gameObject.tag == "common")
		{
			i = Instantiate(inventoryIcons[0]);
			i.transform.SetParent(inventoryPanel.transform);
		}
		else if (hit.gameObject.tag == "uncommon")
		{
			i = Instantiate(inventoryIcons[1]);
			i.transform.SetParent(inventoryPanel.transform);
		}
		else if (hit.gameObject.tag == "rare")
		{
			i = Instantiate(inventoryIcons[2]);
			i.transform.SetParent(inventoryPanel.transform);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
