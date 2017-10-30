using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuButtons : MonoBehaviour {

	public Button menuButton;		// So we can target a button
	public bool quit;				// SO we can use a button to quit the game
	public int sceneIndex;	// So we can target a scene

	void Start ()
	{
		menuButton.onClick.AddListener (OnClick);	// Checks for the tageted button to be clicked
	}
	
	void OnClick()
	{
		if (quit)
			Application.Quit ();
		else
			SceneManager.LoadScene (sceneIndex);	// Loads the targetted scene
	}
}
