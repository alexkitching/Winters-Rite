//////////////////////////////////////////////////////////////////////////
// File: <WeaponManager.cs>
// Author: <Alex Kitching>
// Date Created: <8/03/17>
// Brief: <Script handling the Weapon Manager.>
/////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour
{
    #region Variables
    [SerializeField]
	private string sWeaponLayerName = "Weapon";

	[SerializeField]
	private Transform weaponSlot;

	[SerializeField]
	private PlayerWeapon[] weapons;

    [SerializeField]
	[SyncVar]
	private int iCurrentWeaponID;

    #endregion

    void Start ()
	{
        // Initialise Weapon Prefabs on Player
        SetupWeaponLayers();
	}


	public PlayerWeapon GetCurrentWeapon() // Returns current weapon
	{
		return weapons[iCurrentWeaponID];
	}

	void SetupWeaponLayers()
	{
        foreach (Transform weapon in weaponSlot) // For every weapon the player has:
        {
            if (isLocalPlayer)
            {
                // Set Weapon Prefab Layer to weapon layer.
                // (Ensure weapon camera only draws local player weapon.)
                Util.SetLayerRecursively(weapon.gameObject, LayerMask.NameToLayer(sWeaponLayerName));
            }
        }
	}

	void EquipWeapon (int a_iWeaponID)
	{
		if(weapons[a_iWeaponID].Obtained & !weapons[a_iWeaponID].Equipped) // If the player has the weapon and it isn't already equiped.
		{
			iCurrentWeaponID = a_iWeaponID; // Set Current Weapon equal to new weapon

			if (isLocalPlayer)
			{
				weapons[iCurrentWeaponID].Equipped = true; // Set Current weapon to equipped
			}

			if(isServer) // Host
			{
                RpcToggleWeapon(a_iWeaponID); // Toggle Weapon.
            }
			else // Host
			{
                CmdToggleWeapon(a_iWeaponID); // Toggle Weapon.
			}
		}
	}

	[ClientRpc]
	void RpcToggleWeapon(int a_iWeaponID) // Toggles weapon prefab on clients
	{
		if(weaponSlot.GetChild(a_iWeaponID).gameObject.activeInHierarchy) // If active
		{
			weaponSlot.GetChild(a_iWeaponID).gameObject.SetActive(false); // Disable
		}
		else if (!weaponSlot.GetChild(a_iWeaponID).gameObject.activeInHierarchy) // If Inactive
		{
			weaponSlot.GetChild(a_iWeaponID).gameObject.SetActive(true); //Enable
		}
	}

	[Command]
	void CmdToggleWeapon(int a_iWeaponID) // Is called on server when player toggles weapon
	{
		RpcToggleWeapon(a_iWeaponID); // Toggles weapon on client
	}
	
	void SwitchWeapon(int a_iWeaponID)
	{
		if (!weapons[a_iWeaponID].Equipped) // If new weapon is not already equipped.
		{
            if(weapons[iCurrentWeaponID].Equipped) // Has Weapon Currently Equipped
            {
                RemoveWeapon(iCurrentWeaponID); // Remove the current weapon.
            }
	  
			EquipWeapon(a_iWeaponID); // Equips new weapon
		}
	}

	void RemoveWeapon(int a_iWeaponID)
	{
		if (!isServer) // Client
		{
			CmdToggleWeapon(a_iWeaponID); // Toggle Weapon.
		}
		else // Host
		{
			RpcToggleWeapon(a_iWeaponID); // Toggle Weapon.
		}

		weapons[iCurrentWeaponID].Equipped = false; // Current weapon is no longer equipped.
	}

	void Update()
	{
        if (!isLocalPlayer) return; // Return if not local palyer

		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			SwitchWeapon(0);
		}

		if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			SwitchWeapon(1);
		}

		if(Input.GetKeyDown(KeyCode.Alpha3))
		{
			SwitchWeapon(2);
		}

		if(Input.GetKeyDown(KeyCode.Alpha4))
		{
			SwitchWeapon(3);
		}
	}
}
