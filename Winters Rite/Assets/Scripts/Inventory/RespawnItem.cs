////////////////////////////////////////////////////////////
// File: RespawnItem.cs
// Author: Chris Deere
// Date Created: 19/03/17
// Brief: Respawns Items after pickup
////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

public class RespawnItem : MonoBehaviour {

	public int respawnTime;

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			// Disables the renderer and collider after the player collects the item
			this.GetComponent<SphereCollider>().enabled = false;
			this.GetComponent<MeshRenderer>().enabled = false;

			Invoke("Respawn", respawnTime);

		}
	}

	void Respawn()
	{
		// Enables the renderer and collider after the player the respawn time has passed
		this.GetComponent<SphereCollider>().enabled = false;
		this.GetComponent<MeshRenderer>().enabled = true;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
