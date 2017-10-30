using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Splash : MonoBehaviour {

	public int Menu = 1; // Index value for the menu
	public float splashTime = 3.0f; // Time splash screen stays on for
	public float fadeTime = 2.0f; // Time the logo will start fading at
	public Light fadeLight; // Variable to fade the light
	public float zoomSpeed = 0.2f; // How fast the camera will zoom

	Camera c; // Camera variable
	float timer; // Timer variable

	// Use this for initialization
	void Start ()
	{
		c = GetComponent<Camera>(); // Set c as Camera
		timer = 0.0f; // Set timer to 0
	}
	
	// Update is called once per frame
	void Update ()
	{
		timer += Time.deltaTime; // Start the timer
		c.fieldOfView -= zoomSpeed; // Decrease the field of view to make it look like the camera is zooming in

		if (timer > fadeTime && timer < splashTime)
		{
			// After the fadeTime has passed, decrease the light intensity a the same rate as the camera zoom
			fadeLight.intensity -= zoomSpeed;
		}
		else if (timer > splashTime)
		{
			// After the splashTime has passed, load the main menu 
			SceneManager.LoadScene(Menu);
		}
	}
}
