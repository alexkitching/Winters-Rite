//////////////////////////////////////////////////////////////////////////
// File: <PlayerUI.cs>
// Author: <Alex Kitching>
// Date Created: <8/3/17>
// Brief: <Script handling In Game PlayerUI Functionality.>
/////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;


public class PlayerUI : MonoBehaviour
{
	#region Variables
	[SerializeField]
	private Image HealthFill;
	[SerializeField]
	private Image HungerFill;
	[SerializeField]
	private Image ThirstFill;

	[SerializeField]
	GameObject pauseMenu;

	private Player player;


	#endregion


	public void SetPlayer(Player a_player)
	{
        // Setup Player Reference.
		player = a_player;
	}

	void Start()
	{
        // When Player is loaded in, Pause Menu is Disabled.
		PauseMenu.bIsOn = false;
	}

	void Update()
	{
        // Update Stat Bars each frame with current stat amounts.
		UpdateHealthBar(player.GetCurrentHealthBarAmount());
		UpdateHungerBar(player.GetCurrentHungerBarAmount());
		UpdateThirstBar(player.GetCurrentThirstBarAmount());

        // Toggle Pause Menu on Escape Press.
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			TogglePauseMenu();
		}
	}

	public void TogglePauseMenu()
	{
        // Toggles Pause Menu
		pauseMenu.SetActive(!pauseMenu.activeSelf);
		PauseMenu.bIsOn = pauseMenu.activeSelf;

		if(PauseMenu.bIsOn == true) // Pause Menu is on.
		{
            // Make Cursor Visible
			Cursor.visible = true;
            // Confine Cursor to View
			Cursor.lockState = CursorLockMode.Confined;
		}
		else if (PauseMenu.bIsOn == false) // Pause Menu is off.
        {
            // Make Cursor Invisible
			Cursor.visible = false;
            // Lock Cursor to Center of Game Window
			Cursor.lockState = CursorLockMode.Locked;
		}
	}

	void UpdateHealthBar(float a_fAmount)
	{
		HealthFill.fillAmount = a_fAmount; // Sets HealthFill amount equal to amount passed in.
    }

	void UpdateHungerBar(float a_fAmount)
	{
		HungerFill.fillAmount = a_fAmount; // Sets HungerFill amount equal to amount passed in.
    }

	void UpdateThirstBar(float a_fAmount)
	{
		ThirstFill.fillAmount = a_fAmount; // Sets ThirstFill amount equal to amount passed in.
    }


}
